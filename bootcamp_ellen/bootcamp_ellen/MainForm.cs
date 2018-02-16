using Accord.Video.FFMPEG;
using Accord.Video.DirectShow;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Accord.Vision.Detection;
using System.Drawing.Imaging;
using Accord.Imaging.Filters;

namespace Bootcamp.CompVis.Ellen
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

        private Bitmap ProcessImage(Bitmap image)
        {
            // convert to grayscale
            var gray = new GrayscaleBT709();
            var frame = gray.Apply(image);

            // boost contrast
            var contrast = new ContrastCorrection();
            contrast.ApplyInPlace(frame);

            // set up a haar object detector to find faces
            var cascade = new Accord.Vision.Detection.Cascades.FaceHaarCascade();
            var detector = new HaarObjectDetector(cascade);
            detector.MinSize = new Size(150, 150);
            detector.ScalingFactor = 1.03f;
            detector.SearchMode = ObjectDetectorSearchMode.Average;
            detector.ScalingMode = ObjectDetectorScalingMode.SmallerToGreater;

            // scan the image
            var faces = detector.ProcessFrame(frame);

            // create thumbnails for each face
            var resizer = new ResizeNearestNeighbor(128, 128);
            foreach (var rect in faces)
            {
                // zoom out a bit
                var r = rect;
                r.Inflate(50, 50);
                r.X = r.X < 0 ? 0 : r.X;
                r.Y = r.Y < 0 ? 0 : r.Y;

                // copy the face into a new picturebox 
                using (var face = image.Clone(r, PixelFormat.Format24bppRgb))
                {
                    var smallFace = resizer.Apply(face);
                    var thumbnail = new PictureBox()
                    {
                        Image = smallFace,
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Width = 62,
                        Height = 62
                    };
                    imagesPanel.Controls.Add(thumbnail);
                }
            }

            // highlight all detected faces in the main image
            using (Graphics g = Graphics.FromImage(image))
            {
                var pen = new Pen(Color.LightGreen, 4);
                foreach (var face in faces)
                {
                    g.DrawRectangle(pen, face);
                }
            }

            return image;
        }
    }
}
