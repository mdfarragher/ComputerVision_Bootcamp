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
        public Dictionary<string, List<string>> EmotionList = new Dictionary<string, List<string>>()
        {
            { "Happy",              new List<string>() { "AU06", "AU12" } },
            { "Sad",                new List<string>() { "AU04", "AU15" } },
            { "Appalled",           new List<string>() { "AU04", "AU10" } },
            { "Disgusted",          new List<string>() { "AU09", "AU10", "AU17" } },
            { "Afraid",             new List<string>() { "AU01", "AU04", "AU20", "AU25" } },

            { "Afraid Surprised",   new List<string>() { "AU01", "AU02", "AU05", "AU20", "AU25" } },
            { "Afraid Disgusted",   new List<string>() { "AU01", "AU04", "AU10", "AU20", "AU25" } },

            { "Surprised",          new List<string>() { "AU01", "AU02", "AU25", "AU26" } },

            { "Angry",              new List<string>() { "AU04", "AU07", "AU24" } },
            { "Angry Surprised",    new List<string>() { "AU04", "AU25", "AU26" } },
            { "Angry Disgusted",    new List<string>() { "AU04", "AU10", "AU17" } },
            { "Happy Surprised",    new List<string>() { "AU01", "AU02", "AU12" } },
            { "Happy Disgusted",    new List<string>() { "AU10", "AU12", "AU25" } },
            { "Afraid Angry",       new List<string>() { "AU04", "AU20", "AU25" } },

            { "Sad Fearful",        new List<string>() { "AU01", "AU04", "AU20", "AU25" } },
            { "Sad Surprised",      new List<string>() { "AU01", "AU04", "AU25", "AU26"} },
            { "Disgusted Surprised",new List<string>() { "AU01", "AU02", "AU05", "AU10" } },
            { "Awed",               new List<string>() { "AU01", "AU02", "AU05", "AU25" } }

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
                var features = faceAnalyser.PredictStaticAUsAndComputeFeatures(frame, points);

                // find the action units
                var aus = (from au in features.Item2
                           where au.Value == 1
                           orderby au.Key
                           select au.Key);

                // find the emotion that matches these action units
                var emotion = (from e in EmotionList
                               where e.Value.All(a => aus.Contains(a))
                               select e.Key).FirstOrDefault();

                // draw the emotion on the face
                using (Graphics g = Graphics.FromImage(image))
                {
                    string name = emotion;
                    // name += $"{Environment.NewLine}{string.Join(Environment.NewLine, aus)}";
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
