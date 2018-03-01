using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DlibDotNet;
using DlibDotNet.Extensions;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace Bootcamp.CompVis.Webinar
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// Initialize MainForm.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called when MainForm loads.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            // load the input image
            var bitmap = Bitmap.FromFile("./input.jpg") as Bitmap;

            // load the face to swap
            var bitmapToSwap = Bitmap.FromFile("./swap.jpg") as Bitmap;

            // show images
            pictureBox.Image = bitmap;

            // process image
            var newBitmap = ProcessImage(bitmap, bitmapToSwap);

            // show new image
            pictureBox.Image = newBitmap;
        }

        private Bitmap ProcessImage(Bitmap image, Bitmap newImage)
        {
            // set up Dlib facedetectors and shapedetectors
            using (var fd = FrontalFaceDetector.GetFrontalFaceDetector())
            using (var sp = new ShapePredictor("shape_predictor_68_face_landmarks.dat"))
            {
                // convert images to opencv format
                var selfie = BitmapConverter.ToMat(image);
                var mark = BitmapConverter.ToMat(newImage);

                // convert image to dlib format
                var img = image.ToArray2D<RgbPixel>();

                // find all faces in image
                var faces = fd.Detect(img);

                // get bradley's landmark points
                var bradleyShape = sp.Detect(img, faces[1]);
                var bradleyPoints = (from i in Enumerable.Range(0, (int)bradleyShape.Parts)
                                     let p = bradleyShape.GetPart((uint)i)
                                     select new OpenCvSharp.Point(p.X, p.Y)).ToArray();

                // *** WEBINAR STEP 1: draw landmarks on bradley's face
                // Utility.DrawLandmarks(selfie, bradleyPoints);
                // return BitmapConverter.ToBitmap(selfie);

                // get convex hull of bradley's points
                var hull = Cv2.ConvexHullIndices(bradleyPoints);
                var bradleyHull = from i in hull
                                  select bradleyPoints[i];

                // *** WEBINAR STEP 2a: draw convex hull for bradley
                // Utility.DrawLandmarks(selfie, bradleyHull);
                // Utility.DrawHull(selfie, bradleyHull);
                // return BitmapConverter.ToBitmap(selfie);

                // find landmark points in face to swap
                var imgMark = newImage.ToArray2D<RgbPixel>();
                var faces2 = fd.Detect(imgMark);
                var markShape = sp.Detect(imgMark, faces2[0]);
                var markPoints = (from i in Enumerable.Range(0, (int)markShape.Parts)
                                  let p = markShape.GetPart((uint)i)
                                  select new OpenCvSharp.Point(p.X, p.Y)).ToArray();

                // get convex hull of mark's points
                var hull2 = Cv2.ConvexHullIndices(bradleyPoints);
                var markHull = from i in hull2
                                  select markPoints[i];

                // *** WEBINAR STEP 2b: draw convex hull for mark
                // Utility.DrawLandmarks(mark, markHull);
                // Utility.DrawHull(mark, markHull);
                // return BitmapConverter.ToBitmap(mark);

                // calculate Delaunay triangles
                var triangles = Utility.GetDelaunayTriangles(bradleyHull);

                // *** WEBINAR STEP 3: draw delaunay triangles for bradley
                // Utility.DrawTriangles(selfie, triangles);
                // return BitmapConverter.ToBitmap(selfie);

                // get transformations to warp the new face onto Bradley's face
                var warps = Utility.GetWarps(markHull, bradleyHull, triangles);

                // *** WEBINAR STEP 6: demonstrate triangulation with all landmark points
                // triangles = Utility.GetDelaunayTriangles(bradleyPoints);
                // warps = Utility.GetWarps(markPoints, bradleyPoints, triangles);

                // apply the warps to the new face to prep it for insertion into the main image
                var warpedImg = Utility.ApplyWarps(newImage, image.Width, image.Height, warps);

                // *** WEBINAR STEP 4: warp triangles from mark to bradley
                // return BitmapConverter.ToBitmap(warpedImg);

                // prepare a mask for the warped image
                var mask = new Mat(image.Height, image.Width, MatType.CV_8UC3);
                mask.SetTo(0);
                Cv2.FillConvexPoly(mask, bradleyHull, new Scalar(255, 255, 255), LineTypes.Link8);

                // *** WEBINAR STEP 5a: show blend without seamless clone
                // Cv2.Multiply(selfie, new Scalar(1, 1, 1) - mask, selfie);
                // Cv2.Add(selfie, warpedImg, selfie);
                // return BitmapConverter.ToBitmap(selfie);

                // find the center of the warped face
                var r = Cv2.BoundingRect(bradleyHull);
                var center = new OpenCvSharp.Point(r.Left + r.Width / 2, r.Top + r.Height / 2);

                // blend the warped face into the main image
                var blend = new Mat(selfie.Size(), selfie.Type());
                Cv2.SeamlessClone(warpedImg, selfie, mask, center, blend, SeamlessCloneMethods.NormalClone);

                // *** WEBINAR STEP 5b: show blend with seamless clone
                return BitmapConverter.ToBitmap(blend);
            }
        }
    }
}
