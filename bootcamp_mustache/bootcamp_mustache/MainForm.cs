using Accord.Video.FFMPEG;
using Accord.Video.DirectShow;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Accord.Vision.Detection;
using Accord.Imaging.Filters;
using System.Drawing.Imaging;

namespace Bootcamp.CompVis.Mustache
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// The index of the current frame being processed.
        /// </summary>
        protected int frameCounter = 0;

        /// <summary>
        /// The haar object detector to use for finding faces.
        /// </summary>
        protected HaarObjectDetector faceDetector = null;

        /// <summary>
        /// The haar object detector to use for finding noses.
        /// </summary>
        protected HaarObjectDetector noseDetector = null;

        /// <summary>
        /// The rectangle bounding the face.
        /// </summary>
        protected Rectangle faceRect = Rectangle.Empty;

        /// <summary>
        /// The rectangle bounding the nose.
        /// </summary>
        protected Rectangle noseRect = Rectangle.Empty;

        /// <summary>
        /// The annotation to draw on the face.
        /// </summary>
        protected Bitmap mustache = null;

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
            // activate the camera
            SetCamera();

            // set up a haar object detector to find the face
            var faceCascade = new Accord.Vision.Detection.Cascades.FaceHaarCascade();
            faceDetector = new HaarObjectDetector(faceCascade);
            faceDetector.MinSize = new Size(150, 150);
            faceDetector.ScalingFactor = 1.03f;
            faceDetector.SearchMode = ObjectDetectorSearchMode.Single;
            faceDetector.ScalingMode = ObjectDetectorScalingMode.SmallerToGreater;

            // set up a new haar object detector to find the nose
            var noseCascade = new Accord.Vision.Detection.Cascades.NoseHaarCascade();
            noseDetector = new HaarObjectDetector(noseCascade);
            noseDetector.SearchMode = ObjectDetectorSearchMode.Single;
            noseDetector.ScalingMode = ObjectDetectorScalingMode.SmallerToGreater;

            // load the mustache
            mustache = Bitmap.FromFile(@"./mustache.png") as Bitmap;

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
            // grayscale the image
            var grayscale = new GrayscaleBT709();
            var frame = grayscale.Apply(image);

            // only detect once every 5 frames
            if (frameCounter++ % 5 == 0)
            {
                // scan the image and grab the first face
                var faces = faceDetector.ProcessFrame(frame);
                if (faces.Count() > 0)
                {
                    // copy the face
                    faceRect = faces.First();
                    var face = image.Clone(faces.First(), PixelFormat.DontCare);

                    // search the face for the nose
                    var noses = noseDetector.ProcessFrame(face);
                    if (noses.Count() > 0)
                    {
                        // record new nose location
                        noseRect = noses.First();
                        noseRect.Offset(faceRect.Location);
                    }
                }
            }

            // draw the mustache
            using (Graphics g = Graphics.FromImage(image))
            {
                // we want a mustache three times as wide as the nose
                int mustacheWidth = 2 * noseRect.Width;
                int mustacheHeight = mustacheWidth * mustache.Height / mustache.Width;

                // center the mustache on the bottom of the nose
                int x1 = noseRect.X - (mustacheWidth / 4);
                int x2 = noseRect.X + noseRect.Width + (mustacheWidth / 4);
                int y1 = noseRect.Y + noseRect.Height - (mustacheHeight / 2);
                int y2 = noseRect.Y + noseRect.Height + (mustacheHeight / 2);

                // draw the mustache
                g.DrawImage(mustache, x1, y1, x2 - x1, y2 - y1);
            }

        }
    }
}
