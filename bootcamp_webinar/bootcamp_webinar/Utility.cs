using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dlib = DlibDotNet.Dlib;

namespace Bootcamp.CompVis.Webinar
{
    #region Datatypes

    /// <summary>
    /// This structure stores a single Delaunay triangle.
    /// </summary>
    public struct Triangle
    {
        /// <summary>
        /// The first triangle corner. 
        /// </summary>
        public Point P1;

        /// <summary>
        /// The second triangle corner. 
        /// </summary>
        public Point P2;

        /// <summary>
        /// The third triangle corner. 
        /// </summary>
        public Point P3;

        /// <summary>
        /// Construct a new Triangle instance.
        /// </summary>
        /// <param name="p1">The first triangle corner.</param>
        /// <param name="p2">The second triangle corner.</param>
        /// <param name="p3">The third triangle corner.</param>
        public Triangle(Point p1, Point p2, Point p3)
        {
            P1 = p1;
            P2 = p2;
            P3 = p3;
        }

        /// <summary>
        /// Convert the triangle to an array of Point2f structures.
        /// </summary>
        /// <returns>An array of Point2f structures corresponding to each triangle corner</returns>
        public Point2f[] ToPoint2f()
        {
            return new Point2f[]
            {
                new Point2f(P1.X, P1.Y),
                new Point2f(P2.X, P2.Y),
                new Point2f(P3.X, P3.Y)
            };
        }
    }

    /// <summary>
    /// This structure stores a single warp from one triangle to another.
    /// </summary>
    public struct Warp
    {
        /// <summary>
        /// The source triangle to warp from.
        /// </summary>
        public Triangle Source;

        /// <summary>
        /// The destination triangle to warp to.
        /// </summary>
        public Triangle Destination;

        /// <summary>
        /// Construct a new Warp instance.
        /// </summary>
        /// <param name="source">The source triangle to warp from.</param>
        /// <param name="destination">The destination triangle to warp to.</param>
        public Warp(Triangle source, Triangle destination)
        {
            Source = source;
            Destination = destination;
        }
    }

    #endregion

    /// <summary>
    /// An utility class with helper methods. 
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Perform Delaunay triangulation on a given set of landmark points.
        /// </summary>
        /// <param name="points">The landmark points to use for triangulation.</param>
        /// <returns>A list of Triangle structures that each refer to a single triangle of landmark points.</returns>
        public static IEnumerable<Triangle> GetDelaunayTriangles(IEnumerable<Point> points)
        {
            var result = new List<Triangle>();

            // calculate the bounding box around the points
            var rect = Cv2.BoundingRect(points);
            rect.Inflate(10, 10);

            // the Subdiv2D class handles Delaunay triangulation
            // first we add all points, and then start triangulation
            Vec6f[] triangles;
            using (var subdiv = new Subdiv2D(rect))
            {
                foreach (var p in points)
                {
                    var cv_p = new OpenCvSharp.Point2f(p.X, p.Y);
                    subdiv.Insert(cv_p);
                }
                triangles = subdiv.GetTriangleList();
            }

            // return result as an enumeration of triangle structs
            return from t in triangles
                   let p1 = new Point(t[0], t[1])
                   let p2 = new Point(t[2], t[3])
                   let p3 = new Point(t[4], t[5])
                   where rect.Contains(p1) && rect.Contains(p2) && rect.Contains(p3)
                   select new Triangle(p1, p2, p3);
        }

        /// <summary>
        /// Calculate the warps between the source and destination landmark points
        /// </summary>
        /// <param name="sourcePoints">The landmark points in the source image</param>
        /// <param name="destPoints">The landmark points in the destination image</param>
        /// <param name="destTriangles">The Delaunay triangles in the destination image</param>
        /// <returns>An enumeration of Warp structs that describe how to warp the source image to the destination image</returns>
        public static IEnumerable<Warp> GetWarps(IEnumerable<Point> sourcePoints, IEnumerable<Point> destPoints, IEnumerable<Triangle> destTriangles)
        {
            // build lists of source and destination landmark points
            var sourceList = sourcePoints.ToList();
            var destList = destPoints.ToList();

            // find all three triangle points in the list of destination landmark points
            var indices = from t in destTriangles
                          let p1 = destPoints.First(p => Math.Abs(p.X - t.P1.X) < 1 && Math.Abs(p.Y - t.P1.Y) < 1)
                          let p2 = destPoints.First(p => Math.Abs(p.X - t.P2.X) < 1 && Math.Abs(p.Y - t.P2.Y) < 1)
                          let p3 = destPoints.First(p => Math.Abs(p.X - t.P3.X) < 1 && Math.Abs(p.Y - t.P3.Y) < 1)
                          select new {
                              X1 = destList.IndexOf(p1),
                              X2 = destList.IndexOf(p2),
                              X3 = destList.IndexOf(p3)
                          };

            // return enumeration of warps from source to destination triangles
            return from x in indices
                   select new Warp(
                       new Triangle(sourceList[x.X1], sourceList[x.X2], sourceList[x.X3]),
                       new Triangle(destList[x.X1], destList[x.X2], destList[x.X3]));
        }

        /// <summary>
        /// Apply the given warps to a specified image and return the warped result.
        /// </summary>
        /// <param name="sourceImage">The source image to warp</param>
        /// <param name="width">The width of the destination image</param>
        /// <param name="height">The height of the destination image</param>
        /// <param name="warps">The warps to apply</param>
        /// <returns>The warped image</returns>
        public static Mat ApplyWarps(System.Drawing.Bitmap sourceImage, int width, int height, IEnumerable<Warp> warps)
        {
            // set up opencv images for the replacement image and the output
            var source = BitmapConverter.ToMat(sourceImage);
            var destination = new Mat(height, width, MatType.CV_8UC3);
            destination.SetTo(0);

            // process all warps
            foreach (var warp in warps)
            {
                var t1 = warp.Source.ToPoint2f();
                var t2 = warp.Destination.ToPoint2f();

                // get bounding rects around source and destination triangles
                var r1 = Cv2.BoundingRect(t1);
                var r2 = Cv2.BoundingRect(t2);

                // crop the input image to r1
                Mat img1Cropped = new Mat(r1.Size, source.Type());
                new Mat(source, r1).CopyTo(img1Cropped);

                // adjust triangles to local coordinates within their bounding box
                for (int i=0; i<t1.Length; i++)
                {
                    t1[i].X -= r1.Left;
                    t1[i].Y -= r1.Top;
                    t2[i].X -= r2.Left;
                    t2[i].Y -= r2.Top;
                }

                // get the transformation matrix to warp t1 to t2
                var transform = Cv2.GetAffineTransform(t1, t2);

                // warp triangle
                var img2Cropped = new Mat(r2.Height, r2.Width, img1Cropped.Type());
                Cv2.WarpAffine(img1Cropped, img2Cropped, transform, img2Cropped.Size());

                // create a mask in the shape of the t2 triangle
                var hull = from p in t2 select new Point(p.X, p.Y);
                var mask = new Mat(r2.Height, r2.Width, MatType.CV_8UC3);
                mask.SetTo(0);
                Cv2.FillConvexPoly(mask, hull, new Scalar(1, 1, 1), LineTypes.Link8, 0);

                // alpha-blend the t2 triangle - this sets all pixels outside the triangle to zero
                Cv2.Multiply(img2Cropped, mask, img2Cropped);

                // cut the t2 triangle out of the destination image
                var target = new Mat(destination, r2);
                Cv2.Multiply(target, new Scalar(1, 1, 1) - mask, target);

                // insert the t2 triangle into the destination image
                Cv2.Add(target, img2Cropped, target);
            }

            // return the destination image
            return destination;
        }

        /// <summary>
        /// Draw landmark points on the given image.
        /// </summary>
        /// <param name="image">The image to draw on.</param>
        /// <param name="points">The landmark points to render.</param>
        public static void DrawLandmarks(Mat image, IEnumerable<Point> points)
        {
            foreach (var point in points)
                Cv2.Rectangle(
                    image,
                    new Rect(point.X, point.Y, 8, 8), 
                    Scalar.LightGreen, 
                    8);
        }

        /// <summary>
        /// Draw the convex hull around the face.
        /// </summary>
        /// <param name="image">The image to draw on.</param>
        /// <param name="points">The hull points to draw.</param>
        public static void DrawHull(Mat image, IEnumerable<Point> points)
        {
            var array = points.ToArray();
            for (int i=0; i < points.Count() - 1; i++)
            {
                Cv2.Line(
                    image,
                    new Point(array[i].X, array[i].Y),
                    new Point(array[i+1].X, array[i+1].Y),
                    Scalar.Red,
                    4);
            }
            Cv2.Line(
                image,
                new Point(array[array.Length-1].X, array[array.Length-1].Y),
                new Point(array[0].X, array[0].Y),
                Scalar.Red,
                4);
        }

        public static void DrawTriangles(Mat image, IEnumerable<Triangle> triangles)
        {
            foreach (var triangle in triangles)
            {
                Cv2.Line(
                    image,
                    new Point(triangle.P1.X, triangle.P1.Y),
                    new Point(triangle.P2.X, triangle.P2.Y),
                    Scalar.Blue,
                    4);
                Cv2.Line(
                    image,
                    new Point(triangle.P2.X, triangle.P2.Y),
                    new Point(triangle.P3.X, triangle.P3.Y),
                    Scalar.Blue,
                    4);
                Cv2.Line(
                    image,
                    new Point(triangle.P3.X, triangle.P3.Y),
                    new Point(triangle.P1.X, triangle.P1.Y),
                    Scalar.Blue,
                    4);
            }
        }
    }
}
