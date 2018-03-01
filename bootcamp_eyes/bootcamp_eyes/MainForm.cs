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
using Accord.Imaging.Filters;

namespace Bootcamp.CompVis.Eyes
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
        /// The current state of the eye: 100 = fully open, 0 = fully closed.
        /// </summary>
        protected int eyeStateValue = 0;

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
        /// Set the video player to read from the built-in camera.
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
            SetCamera();

            // initialize face detector
            faceDetector = FrontalFaceDetector.GetFrontalFaceDetector();

            // initialize shape predictor
            shapePredictor = new ShapePredictor("shape_predictor_68_face_landmarks.dat");

            // start the player
            videoPlayer.Start();
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
        /// Detect the eye state from the landmark points.
        /// </summary>
        /// <param name="frame">The current video frame.</param>
        /// <param name="shape">The current landmark points.</param>
        private void DetectEyeState(System.Drawing.Bitmap frame, FullObjectDetection shape)
        {
            // get all landmark points of the left eye
            var leftEye = from i in Enumerable.Range(36, 6)
                            let p = shape.GetPart((uint)i)
                            select new OpenCvSharp.Point(p.X, p.Y);

            // get all landmark points of the right eye
            var rightEye = from i in Enumerable.Range(42, 6)
                            let p = shape.GetPart((uint)i)
                            select new OpenCvSharp.Point(p.X, p.Y);

            // draw the eye areas into a new image
            using (var mask = new Mat(new Size(frame.Width, frame.Height), MatType.CV_8UC1))
            {
                mask.SetTo(0);
                Cv2.FillConvexPoly(mask, leftEye, Scalar.White);
                Cv2.FillConvexPoly(mask, rightEye, Scalar.White);

                // calculate surface area of both eyes
                int area = Cv2.CountNonZero(mask);

                // the maximum possible area is 40% of the surface area of both eyeballs
                int r1 = (shape.GetPart(39).X - shape.GetPart(36).X)/2;
                int r2 = (shape.GetPart(45).X - shape.GetPart(42).X)/2;
                double normalizedArea = 0.4 * Math.PI * r1 * r1 + 0.4 * Math.PI * r2 * r2;

                // calculate fractional area and normalize on a 0-100 scale
                var value = (int) (100 * area / normalizedArea - 20);
                eyeStateValue = value >= 0 && value <= 100 ? value : 0;

                // calculate bounding box around eyes
                var rect = Cv2.BoundingRect(Enumerable.Union(leftEye, rightEye));
                rect.Inflate(30, 30);

                // copy the eye image to the picturebox
                var maskImg = BitmapConverter.ToBitmap(mask.Clone(rect));
                eyeBox.Image = maskImg;
            }
        }

        /// <summary>
        /// Called when videoPlayer receives a new frame. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="image"></param>
        private void videoPlayer_NewFrame(object sender, ref System.Drawing.Bitmap image)
        {
            // conver frame to grayscale
            var grayscale = new GrayscaleBT709();
            var grayImage = grayscale.Apply(image);

            // convert image to dlib format
            var img = grayImage.ToArray2D<RgbPixel>();

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

            // detect eye state
            DetectEyeState(image, shape);

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
            // update eye state slider
            eyeState.Value = eyeStateValue;

            // enable camera button if the eye state is around 50
            pictureButton.Enabled = (eyeStateValue > 25 && eyeStateValue < 75);
        }

        private void pictureButton_Click(object sender, EventArgs e)
        {
            // add current video frame to thumbnails box
            var box = new PictureBox()
            {
                Image = videoPlayer.GetCurrentVideoFrame(),
                SizeMode = PictureBoxSizeMode.Zoom,
                Width = 160,
                Height = 100
            };
            thumbnailBox.Controls.Add(box);
        }
    }
}
