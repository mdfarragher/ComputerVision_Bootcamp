using Accord.Video.DirectShow;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DlibDotNet;
using DlibDotNet.Extensions;
using Dlib = DlibDotNet.Dlib;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing;

namespace Bootcamp.CompVis.Pose
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// The face detector to use.
        /// </summary>
        protected FrontalFaceDetector faceDetector = null;

        /// <summary>
        /// The shape predictor to use to detect landmarks
        /// </summary>
        protected ShapePredictor shapePredictor = null;

        /// <summary>
        /// The most recent detected face.
        /// </summary>
        protected DlibDotNet.Rectangle currentFace = default(DlibDotNet.Rectangle);

        /// <summary>
        /// The current head rotation Euler matrix.
        /// </summary>
        protected MatOfDouble headRotation = null;

        /// <summary>
        /// The index of the current video frame.
        /// </summary>
        protected int frameIndex = 0;

        /// <summary>
        /// Initialize MainForm.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Set the camera player to read from the built-in camera.
        /// </summary>
        private void SetCamera()
        {
            // use the first video input device
            var deviceName = (from d in new FilterInfoCollection(FilterCategory.VideoInputDevice)
                              select d).FirstOrDefault();
            var captureDevice = new VideoCaptureDevice(deviceName.MonikerString);

            // switch to 640x480 resolution
            captureDevice.VideoResolution = (from r in captureDevice.VideoCapabilities
                                             where r.FrameSize.Width == 640
                                             select r).First();

            videoPlayer.VideoSource = captureDevice;
        }

        /// <summary>
        /// Called when MainForm loads.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            // initialize video player
            SetCamera();

            // load the tv picture
            tvPicture.Image = Bitmap.FromFile(@"input.jpg");

            // initialize face detector
            faceDetector = FrontalFaceDetector.GetFrontalFaceDetector();

            // initialize shape predictor
            shapePredictor = new ShapePredictor("shape_predictor_68_face_landmarks.dat");

            // start the players
            videoPlayer.Start();
            // videoPlayer.Start();
        }

        /// <summary>
        /// Called when MainForm is closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            videoPlayer.Stop();

            // dispose dlib resources
            if (faceDetector != null)
            {
                faceDetector.Dispose();
                faceDetector = null;
            }
            if (shapePredictor != null)
            {
                shapePredictor.Dispose();
                shapePredictor = null;
            }
        }

        /// <summary>
        /// Detect the orientation of the head in the current video frame.
        /// </summary>
        /// <param name="image">The current video frame.</param>
        /// <param name="shape">The landmark points.</param>
        private void DetectHeadPose(System.Drawing.Bitmap image, FullObjectDetection shape)
        {
            // build the 3d face model
            var model = Utility.GetFaceModel();

            // build the landmark point list
            var landmarks = new MatOfPoint2d(1, 6,
                (from i in new int[] { 30, 8, 36, 45, 48, 54 }
                let p = shape.GetPart((uint)i)
                select new OpenCvSharp.Point2d(p.X, p.Y)).ToArray());

            // build the camera matrix
            var cameraMatrix = Utility.GetCameraMatrix(image.Width, image.Height);

            // build the coefficient matrix
            var coeffs = new MatOfDouble(4, 1);
            coeffs.SetTo(0);

            // find head rotation and translation
            Mat rotation = new MatOfDouble();
            Mat translation = new MatOfDouble();
            Cv2.SolvePnP(model, landmarks, cameraMatrix, coeffs, rotation, translation);

            // find and store euler angles
            var euler = Utility.GetEulerMatrix(rotation);
            headRotation = euler;

            // create a new model point in front of the nose, and project it into 2d
            var poseModel = new MatOfPoint3d(1, 1, new Point3d(0, 0, 1000));
            var poseProjection = new MatOfPoint2d();
            Cv2.ProjectPoints(poseModel, rotation, translation, cameraMatrix, coeffs, poseProjection);

            // draw the 6 landmark points
            using (Graphics g = Graphics.FromImage(image))
            {
                foreach (var i in new int[] { 30, 8, 36, 45, 48, 54 })
                {
                    var point = shape.GetPart((uint)i);
                    g.FillRectangle(Brushes.LightGreen, point.X - 5, point.Y - 5, 10, 10);
                }

                // draw a line from the tip of the nose pointing in the direction of head pose
                var landmark = landmarks.At<Point2d>(0);  
                var p = poseProjection.At<Point2d>(0);    
                var pen = new Pen(Brushes.LightGreen, 4);
                g.DrawLine(pen, (int)landmark.X, (int)landmark.Y, (int)p.X, (int)p.Y);
            }
        }

        /// <summary>
        /// Called when videoPlayer receives a new frame. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="image"></param>
        private void videoPlayer_NewFrameReceived(object sender, Accord.Video.NewFrameEventArgs eventArgs)
        {
            // convert image to dlib format
            var img = eventArgs.Frame.ToArray2D<RgbPixel>();

            // detect face every 4 frames
            if (frameIndex % 4 == 0)
            {
                var faces = faceDetector.Detect(img);
                if (faces.Length > 0)
                    currentFace = faces.First();
            }

            // abort if we don't have a face at this point
            if (currentFace == default(DlibDotNet.Rectangle))
                return;

            // detect facial landmarks
            var shape = shapePredictor.Detect(img, currentFace);

            // detect head pose
            if (shape.Parts == 68)
                DetectHeadPose(eventArgs.Frame, shape);

            // update frame counter
            frameIndex++;
        }

        /// <summary>
        /// Timer event to update the user interface.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            if (headRotation == null)
                return;

            // calculate head pose in degrees
            var leftRight = 180 * headRotation.At<double>(0, 2) / Math.PI;
            var upDown = 180 * headRotation.At<double>(0, 1) / Math.PI;
            var rotation = 180 * headRotation.At<double>(0, 0) / Math.PI;

            // looking straight ahead wraps at -180/180, so make the range smooth
            upDown = Math.Sign(upDown) * 180 - upDown;

            // update sliders
            if (leftRight >= -45 && leftRight <= 45)
                leftRightAngle.Value = (int)leftRight;
            if (upDown >= -45 && upDown <= 45)
                upDownAngle.Value = (int)upDown;
            if (rotation >= -45 && rotation <= 45)
                rotationAngle.Value = (int)rotation;

            // pause the video player if we're not looking at it
            // the left/right angle must be in the -25..25 range
            // the up/down angle must be in the -10..10 range
            bool facingForward = (
                leftRight >= -25 && leftRight <= 25 &&
                upDown >= -10 && upDown <= 10);

            // start and stop the video player and show the paused label
            pausedLabel.Visible = !facingForward;
            /*
            if (facingForward && !videoPlayer.IsRunning)
                videoPlayer.Start();
            else if (!facingForward && videoPlayer.IsRunning)
                videoPlayer.Stop();
                */
        }
    }
}
