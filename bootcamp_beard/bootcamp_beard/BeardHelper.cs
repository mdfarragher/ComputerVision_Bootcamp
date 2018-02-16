using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace Bootcamp.CompVis.Beard
{
    public static class BeardHelper
    {
        /// <summary>
        /// Get the subset of landmark points needed for beard swapping.
        /// </summary>
        /// <param name="shape">The ShapePredictor output</param>
        /// <returns>A subset of landmark points suitable for beard swapping</returns>
        public static Point[] GetLandmarkPoints(DlibDotNet.FullObjectDetection shape)
        {
            return (from i in new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 31, 32, 33, 34, 35, 55, 56, 57, 58, 59 }
                              let p = shape.GetPart((uint)i)
                              select new OpenCvSharp.Point(p.X, p.Y)).ToArray();
        }

        /// <summary>
        /// Get the hardcoded list of beard landmark points.
        /// </summary>
        /// <returns>A Point[] array of beard landmark points</returns>
        public static Point[] GetBeardPoints()
        {
            return new Point[]
            {
                new Point(26, 127),
                new Point(37, 269),
                new Point(67, 387),
                new Point(103, 450),
                new Point(130, 500),
                new Point(163, 535),
                new Point(200, 566),
                new Point(258, 580),
                new Point(323, 554),
                new Point(367, 516),
                new Point(401, 484),
                new Point(421, 443),
                new Point(455, 378),
                new Point(472, 286),
                new Point(487, 110),
                new Point(170, 326),
                new Point(206, 325),
                new Point(250, 344),
                new Point(295, 331),
                new Point(325, 323),
                new Point(320, 426),
                new Point(297, 441),
                new Point(267, 441),
                new Point(233, 435),
                new Point(191, 423)
            };
        }

        /// <summary>
        /// Split a bitmap into an alpha mask and an RGB part.
        /// </summary>
        /// <param name="sourceImage">The bitmap to split.</param>
        /// <param name="alphaMask">The alpha mask of the image.</param>
        /// <param name="rgbImage">The RGB part of the image.</param>
        public static void SplitChannels(Mat sourceImage, out Mat alphaMask, out Mat rgbImage)
        {
            // extract the alpha mask from the source image
            var rgbAchannels = sourceImage.Split();
            alphaMask = new Mat(sourceImage.Size(), MatType.CV_8UC3);
            Cv2.Merge(new Mat[] { rgbAchannels[3], rgbAchannels[3], rgbAchannels[3] }, alphaMask);

            // build an RGB beard image
            rgbImage = new Mat(sourceImage.Size(), MatType.CV_8UC3);
            Cv2.Merge(new Mat[] { rgbAchannels[0], rgbAchannels[1], rgbAchannels[2] }, rgbImage);
        }

        /// <summary>
        /// Blend a source image onto a destination image using a mask.
        /// </summary>
        /// <param name="sourceImage">The source image to mask.</param>
        /// <param name="mask">The mask to use.</param>
        /// <param name="destinationImage">The destination image to mask onto.</param>
        /// <returns></returns>
        public static Mat Blend(Mat sourceImage, Mat mask, Mat destinationImage)
        {
            // convert the source image to float
            sourceImage.ConvertTo(sourceImage, MatType.CV_32FC3);

            // convert the mask to float and normalize to 0..1 range
            mask.ConvertTo(mask, MatType.CV_32FC3, 1.0 / 255.0);

            // multiply destination imagfe with 1-mask
            destinationImage.ConvertTo(destinationImage, MatType.CV_32FC3);
            Cv2.Multiply(new Scalar(1.0, 1.0, 1.0) - mask, destinationImage, destinationImage);

            // multiply source image with mask
            Cv2.Multiply(mask, sourceImage, sourceImage);

            // add these two together and convert back to int
            Cv2.Add(sourceImage, destinationImage, destinationImage);
            destinationImage.ConvertTo(destinationImage, MatType.CV_8U);

            return destinationImage;
        }
    }
}
