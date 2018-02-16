using Accord.Video.FFMPEG;
using Accord.Video.DirectShow;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord.Math.Geometry;
using System.Drawing.Imaging;

namespace Bootcamp.CompVis.TrafficSign
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

            // show it
            pictureBox.Image = bitmap;

            // process image
            var newBitmap = ProcessImage(bitmap);

            // show new image
            pictureBox.Image = newBitmap;
        }

        private Bitmap ProcessImage(Bitmap frame)
        {
            // convert the image to grayscale
            var grayConverter = new GrayscaleBT709();
            var grayFrame = grayConverter.Apply(frame);

            // use a sobel edge detector to find color edges
            var edgeDetector = new SobelEdgeDetector();
            var edgeFrame = edgeDetector.Apply(grayFrame);

            // threshold the edges
            var thresholdConverter = new Threshold(200);
            thresholdConverter.ApplyInPlace(edgeFrame);

            // use a blobcounter to find interesting shapes
            var detector = new BlobCounter()
            {
                FilterBlobs = true,
                MinWidth = 25,
                MinHeight = 25
            };
            detector.ProcessImage(edgeFrame);

            // find the circular shape
            var shapeDetector = new SimpleShapeChecker();
            var blobs = detector.GetObjectsInformation();
            var circles =
                from blob in blobs
                let edgePoints = detector.GetBlobsEdgePoints(blob)
                where shapeDetector.CheckShapeType(edgePoints) == ShapeType.Circle
                select blob;

            // show the traffic sign
            if (circles.Count() > 0)
            {
                var circleFrame = frame.Clone(circles.First().Rectangle, PixelFormat.DontCare);
                trafficSignBox.Image = circleFrame;
            }

            // highlight every circle in the image
            using (Graphics g = Graphics.FromImage(frame))
            {
                var rects = detector.GetObjectsRectangles();
                var pen = new Pen(Color.Blue, 4);
                foreach (var circle in circles)
                {
                    g.DrawRectangle(pen, circle.Rectangle);
                }
            }

            // update picture boxes
            thresholdBox.Image = edgeFrame;

            return frame;
        }
    }
}
