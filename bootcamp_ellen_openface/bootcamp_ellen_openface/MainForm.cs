using CppInterop.LandmarkDetector;
using FaceDetectorInterop;
using OpenCVWrappers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using UtilitiesOF;

namespace Bootcamp.CompVis.Ellen_Openface
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
            var reader = new SequenceReader("images", true);

            // process image
            var bitmap = ProcessImage(reader);

            // show new image
            pictureBox.Image = bitmap;
        }

        private Bitmap ProcessImage(SequenceReader reader)
        {
            // set up the face model
            String root = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\");
            var faceModel = new FaceModelParameters(root, false);
            faceModel.optimiseForImages();

            // set up a face detector and a landmark detector
            var faceDetector = new FaceDetector();
            var landmarkDetector = new CLNF(faceModel);

            // read the image from the sequence reader
            var frame = new RawImage(reader.GetNextImage());
            var grayFrame = new RawImage(reader.GetCurrentFrameGray());

            // detect faces
            var faces = new List<Rect>();
            var confidences = new List<double>();
            faceDetector.DetectFacesHOG(faces, grayFrame, confidences);

            // detect landmarks
            var landmarks = new List<List<Tuple<double, double>>>();
            foreach (var face in faces)
            {
                landmarkDetector.DetectFaceLandmarksInImage(grayFrame, face, faceModel);
                var points = landmarkDetector.CalculateAllLandmarks();
                landmarks.Add(points);
            }

            // draw rectangles and confidence values on image
            var image = frame.ToBitmap();
            using (Graphics g = Graphics.FromImage(image))
            {
                int index = 0;
                var pen = new System.Drawing.Pen(System.Drawing.Color.LightGreen, 4);
                var pen2 = new System.Drawing.Pen(System.Drawing.Color.Red, 4);
                var font = new Font(FontFamily.GenericSansSerif, 30);
                foreach (var face in faces)
                {
                    g.DrawRectangle(pen, (int)face.X, (int)face.Y, (int)face.Width, (int)face.Height);
                    g.DrawString($"{confidences[index]:0.00}", font, Brushes.Black, (int)face.X + 36, (int)face.Y - 36);

                    // draw landmark points
                    foreach (var p in landmarks[index])
                    {
                        g.DrawRectangle(pen2, new Rectangle((int)p.Item1, (int)p.Item2, 4, 4));
                    }
                    index++;
                }
            }

            return image;
        }
    }
}
