using Bootcamp.CompVis.Gaze.Game;
using CppInterop.LandmarkDetector;
using FaceDetectorInterop;
using GazeAnalyser_Interop;
using OpenCVWrappers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using UtilitiesOF;

namespace Bootcamp.CompVis.Gaze
{
    /// <summary>
    /// The main program.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The game form.
        /// </summary>
        static GameForm form = null;

        /// <summary>
        /// The face model to use. 
        /// </summary>
        static FaceModelParameters faceModel = null;

        /// <summary>
        /// The sequence reader to read webcam images with. 
        /// </summary>
        static SequenceReader reader = null;

        /// <summary>
        /// The face detector to use.
        /// </summary>
        static FaceDetector faceDetector = null;

        /// <summary>
        /// The landmark detector to use. 
        /// </summary>
        static CLNF landmarkDetector = null;

        /// <summary>
        /// The gaze analyser to use. 
        /// </summary>
        static GazeAnalyserManaged gazeAnalyser = null;

        /// <summary>
        /// The last known position of the face. 
        /// </summary>
        static Rect currentFace = new Rect();

        /// <summary>
        /// The number of the current camera frame.
        /// </summary>
        static int frameIndex = 0;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread] 
        static void Main()
        {
            // initialize application
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // set up the game form
            form = new Game.GameForm();
            form.OnGazeMove += new Game.OnGazeMoveDelegate(OnGazeMove);

            // grab the first camera
            var baseFolder = AppDomain.CurrentDomain.BaseDirectory;
            var camera = SequenceReader.GetCameras(baseFolder).First();

            // set up a sequence reader
            reader = new SequenceReader(camera.Item1, 640, 400);

            // set up the face model
            String root = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\");
            faceModel = new FaceModelParameters(root, false);
            faceModel.optimiseForVideo();

            // set up a face detector, a landmark detector, and a gaze analyser
            faceDetector = new FaceDetector();
            landmarkDetector = new CLNF(faceModel);
            gazeAnalyser = new GazeAnalyserManaged();

            // run the game
            Application.Run(form);
        }

        /// <summary>
        /// Calculate the current gaze point.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void OnGazeMove(object sender, GazeEventArgs e)
        {
            // read the image from the sequence reader
            var frame = new RawImage(reader.GetNextImage());
            var grayFrame = new RawImage(reader.GetCurrentFrameGray());

            // only detect faces every 5 frames
            if (currentFace == default(Rect) || frameIndex % 5 == 0)
            {
                // detect faces
                var faces = new List<System.Windows.Rect>();
                var confidences = new List<double>();
                faceDetector.DetectFacesHOG(faces, grayFrame, confidences);

                // abort if we couldn't find a face
                if (currentFace == default(Rect) && faces.Count() == 0)
                    return;

                currentFace = faces.First();
            }

            // detect landmarks
            var success = landmarkDetector.DetectLandmarksInVideo(grayFrame, faceModel);

            // calculate gaze angle
            gazeAnalyser.AddNextFrame(landmarkDetector, success, reader.GetFx(), reader.GetFy(), reader.GetCx(), reader.GetCy());
            var gaze = gazeAnalyser.GetGazeAngle();

            // convert gaze to a point coordinate
            e.Point.X = (int)(5000 * Math.Tan(-gaze.Item1) + form.Width / 2);
            e.Point.Y = form.Height / 2;

            // update frame counter
            frameIndex++;
        }
    }
}
