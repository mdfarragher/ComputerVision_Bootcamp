using CppInterop.LandmarkDetector;
using FaceAnalyser_Interop;
using FaceDetectorInterop;
using OpenCVWrappers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using UtilitiesOF;

namespace Bootcamp.CompVis.Emotions
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// A dictionary to map action units to emotions.
        /// </summary>
        public Dictionary<string, List<int>> EmotionList = new Dictionary<string, List<int>>()
        {
            { "Happy",              new List<int>() { 12, 25, 6 } },
            { "Sad",                new List<int>() { 4, 15, 1, 6, 11, 17 } },
            { "Afraid",             new List<int>() { 1, 4, 20, 25, 2, 5, 26 } },
            { "Angry",              new List<int>() { 4, 7, 24, 10, 17, 23 } },
            { "Surprised",          new List<int>() { 1, 2, 25, 26, 5 } },
            { "Disgusted",          new List<int>() { 9, 10, 17, 4, 24 } },

            { "Happy Surprised",    new List<int>() { 1, 2, 12, 25, 5, 26 } },
            { "Happy Disgusted",    new List<int>() { 10, 12, 25, 4, 6, 9 } },

            { "Sad Fearful",        new List<int>() { 1, 4, 20, 25, 2, 5, 6, 15 } },
            { "Sad Angry",          new List<int>() { 4, 15, 6, 7, 11, 17 } },
            { "Sad Surprised",      new List<int>() { 1, 4, 25, 26, 2, 6 } },
            { "Sad Disgusted",      new List<int>() { 4, 10, 1, 6, 9, 11, 15, 17, 25 } },

            { "Afraid Angry",       new List<int>() { 4, 20, 25, 5, 7, 10, 11 } },
            { "Afraid Surprised",   new List<int>() { 1, 2, 5, 20, 25, 4, 10, 11, 26 } },
            { "Afraid Disgusted",   new List<int>() { 1, 4, 10, 20, 25, 2, 5, 6, 9, 15 } },

            { "Angry Surprised",    new List<int>() { 4, 25, 26, 5, 7, 10 } },
            { "Angry Disgusted",    new List<int>() { 4, 10, 17, 7, 9, 24 } },

            { "Disgusted Surprised",new List<int>() { 1, 2, 5, 10, 4, 9, 17, 24 } },
            { "Appalled",           new List<int>() { 4, 10, 6, 9, 17, 24 } },
            { "Hatred",             new List<int>() { 4, 10, 7, 9, 17, 24 } },
            { "Awed",               new List<int>() { 1, 2, 5, 25, 4, 20, 26 } }

         };

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
            var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\images");
            var reader = new SequenceReader(folder, true);

            // process image
            var bitmap = ProcessImage(reader);

            // show new image
            pictureBox.Image = bitmap;
        }

        private Bitmap ProcessImage(SequenceReader reader)
        {
            // set up the face model
            var root = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\");
            var faceModel = new FaceModelParameters(root, false);
            faceModel.optimiseForImages();

            // set up a face detector, a landmark detector, and a face analyser
            var faceDetector = new FaceDetector();
            var landmarkDetector = new CLNF(faceModel);
            var faceAnalyser = new FaceAnalyserManaged(root, true, 0);

            // read the image from the sequence reader
            var frame = new RawImage(reader.GetNextImage());
            var grayFrame = new RawImage(reader.GetCurrentFrameGray());

            // detect faces
            var faces = new List<Rect>();
            var confidences = new List<double>();
            faceDetector.DetectFacesHOG(faces, grayFrame, confidences);

            // detect landmarks
            var image = frame.ToBitmap();
            foreach (var face in faces)
            {
                landmarkDetector.DetectFaceLandmarksInImage(grayFrame, face, faceModel);
                var points = landmarkDetector.CalculateAllLandmarks();

                // calculate action units
                var features = faceAnalyser.PredictStaticAUsAndComputeFeatures(grayFrame, points);

                // find the action units
                var aus = (from au in features.Item2
                           where au.Value > 0
                           orderby au.Key
                           select au.Key);

                // for each emotion, count how many action units match
                var counts = (from e in EmotionList
                              let total = e.Value.Count()
                              let conf = 100.0 * e.Value.Count(au => aus.Contains($"AU{au:00}")) / total
                              select new {
                                  Emotion = e.Key,
                                  Confidence = conf});

                // get the top emotions
                var emotions = (from e in counts
                              orderby e.Confidence descending
                              select $"{e.Emotion} {e.Confidence:0}%").Take(3);

                // draw the emotion on the face
                using (Graphics g = Graphics.FromImage(image))
                {
                    string name = string.Join(Environment.NewLine, emotions);
                    Font fnt = new Font("Verdana", 15, GraphicsUnit.Pixel);
                    Brush brs = new SolidBrush(Color.Black);
                    var bump = 36;
                    System.Drawing.SizeF stringSize = g.MeasureString(name, fnt);
                    g.FillRectangle(new SolidBrush(Color.Yellow), (int)face.X + bump, (int)face.Y, stringSize.Width, stringSize.Height);
                    g.DrawString(name, fnt, brs, (int)face.X + bump, (int)face.Y);
                }
            }
            return image;
        }
    }
}
