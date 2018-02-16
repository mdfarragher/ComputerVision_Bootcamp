using Accord.Video.FFMPEG;
using Accord.Video.DirectShow;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DlibDotNet;
using DlibDotNet.Extensions;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace Bootcamp.CompVis.Beard
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// The face detector to use.
        /// </summary>
        protected FrontalFaceDetector faceDetector = null;

        /// <summary>
        /// The shape predictor to use. 
        /// </summary>
        protected ShapePredictor shapePredictor = null;

        /// <summary>
        /// The rectangle where a face was last detected.
        /// </summary>
        protected DlibDotNet.Rectangle faceRect = default(DlibDotNet.Rectangle);

        /// <summary>
        /// The beard image to use. 
        /// </summary>
        protected Bitmap beardImage = null;

        /// <summary>
        /// The index number of the current frame.
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

            // resize player
            videoPlayer.Width = captureDevice.VideoResolution.FrameSize.Width;
            videoPlayer.Height = captureDevice.VideoResolution.FrameSize.Height;

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

            // load the beard image
            beardImage = Bitmap.FromFile(@".\beard.png") as Bitmap;

            // set up our face detector and shape predictor
            faceDetector = FrontalFaceDetector.GetFrontalFaceDetector();
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
        }

        /// <summary>
        /// Called when videoPlayer receives a new frame. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="image"></param>
        private void videoPlayer_NewFrame(object sender, ref Bitmap image)
        {
            // convert image to dlib format
            var img = image.ToArray2D<RgbPixel>();

            // find the face
            // note that we only detect faces every 4 frames
            if (faceRect == default(DlibDotNet.Rectangle) || frameIndex++ % 4 == 0)
            {
                var faces = faceDetector.Detect(img);
                faceRect = faces.FirstOrDefault();
            }

            // abort if we found no face
            if (faceRect == default(DlibDotNet.Rectangle))
                return;

            // find face landmark points
            var shape = shapePredictor.Detect(img, faceRect);
            var landmarkPoints = BeardHelper.GetLandmarkPoints(shape);

            // find beard landmark points
            var beardPoints = BeardHelper.GetBeardPoints();

            // calculate Delaunay triangles
            var triangles = Utility.GetDelaunayTriangles(landmarkPoints);

            // get transformations to warp the beard onto the face
            var warps = Utility.GetWarps(beardPoints, landmarkPoints, triangles);

            // split the beard image into an alpha mask and an RGB part
            var beard = BitmapConverter.ToMat(beardImage);
            BeardHelper.SplitChannels(beard, out var beardMask, out var beardRgb);

            // warp the beard RGB image
            var warpedBeard = Utility.ApplyWarps(BitmapConverter.ToBitmap(beardRgb), image.Width, image.Height, warps);

            // warp the beard alpha mask
            var warpedBeardMask = Utility.ApplyWarps(BitmapConverter.ToBitmap(beardMask), image.Width, image.Height, warps);

            // blend the beard onto the camera frame by using the mask
            var frame = BitmapConverter.ToMat(image);
            var result = BeardHelper.Blend(warpedBeard, warpedBeardMask, frame);

            // return result
            image = BitmapConverter.ToBitmap(result);
        }
    }
}
