using Accord.Video.FFMPEG;
using Accord.Video.DirectShow;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Accord.Imaging;
using Accord.Imaging.Filters;
using System.Collections.Generic;
using Accord.Vision.Tracking;
using Accord;

namespace Bootcamp.CompVis.Mario
{
    /// <summary>
    /// The main application form.
    /// </summary>
    public partial class MainForm : Form
    {
        // switch between camera and video
        private const bool USE_CAMERA = false;

        // the previous video frame
        private Camshift tracker = new Camshift()
        {
            Conservative = false,
            Smooth = false,
            Mode = CamshiftMode.RGB
        };

        // the index of the current frame
        private int frameIndex = 0;

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

            // switch to 1280x720 resolution
            captureDevice.VideoResolution = (from r in captureDevice.VideoCapabilities
                                             where r.FrameSize.Width == 1280
                                             select r).First();

            videoPlayer.VideoSource = captureDevice;
        }

        /// <summary>
        /// Set the video player to read a video file from disk.
        /// </summary>
        /// <param name="fileName"></param>
        private void SetVideo(string fileName)
        {
            var source = new Accord.Video.FFMPEG.VideoFileSource(fileName);
            videoPlayer.VideoSource = source;
        }

        /// <summary>
        /// Called when MainForm loads.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            if (USE_CAMERA)
                SetCamera();
            else
                SetVideo("./input.mp4");

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
        /// Draw the tracking reticle.
        /// </summary>
        /// <param name="frame">The frame to draw the reticle in.</param>
        /// <param name="rect">The location and size of the reticle.</param>
        private void DrawReticle(Bitmap frame, Rectangle rect)
        {
            using (Graphics g = Graphics.FromImage(frame))
            {
                // box
                var pen = new Pen(Color.White);
                pen.Width = 4f;
                pen.DashCap = System.Drawing.Drawing2D.DashCap.Flat;
                pen.DashPattern = new float[] { rect.Width / 25f, rect.Height / 25f };
                g.DrawLine(pen, rect.X, rect.Y, rect.X + rect.Width, rect.Y);
                g.DrawLine(pen, rect.X, rect.Y, rect.X, rect.Y + rect.Height);
                g.DrawLine(pen, rect.X + rect.Width, rect.Y, rect.X + rect.Width, rect.Y + rect.Height);
                g.DrawLine(pen, rect.X, rect.Y + rect.Height, rect.X + rect.Width, rect.Y + rect.Height);

                // crosshair lines
                pen = new Pen(Color.Yellow);
                pen.Width = 4f;
                pen.DashCap = System.Drawing.Drawing2D.DashCap.Flat;
                var size = 10;
                g.DrawLine(pen, rect.X, rect.Y + rect.Height / 2, rect.X + size, rect.Y + rect.Height / 2);
                g.DrawLine(pen, rect.X + rect.Width, rect.Y + rect.Height / 2, rect.X + rect.Width - size, rect.Y + rect.Height / 2);
                g.DrawLine(pen, rect.X + rect.Width / 2, rect.Y, rect.X + rect.Width / 2, rect.Y + size);
                g.DrawLine(pen, rect.X + rect.Width / 2, rect.Y + rect.Height, rect.X + rect.Width / 2, rect.Y + rect.Height - size);

                // corners
                pen = new Pen(Color.Yellow);
                pen.Width = 8f;
                pen.DashCap = System.Drawing.Drawing2D.DashCap.Round;
                size = 10;
                g.DrawLine(pen, rect.X - 2, rect.Y, rect.X + size, rect.Y);
                g.DrawLine(pen, rect.X, rect.Y - 2, rect.X, rect.Y + size);
                g.DrawLine(pen, rect.X + rect.Width - size, rect.Y, rect.X + rect.Width + 2, rect.Y);
                g.DrawLine(pen, rect.X + rect.Width, rect.Y - 2, rect.X + rect.Width, rect.Y + size);
                g.DrawLine(pen, rect.X - 2, rect.Y + rect.Height, rect.X + size, rect.Y + rect.Height);
                g.DrawLine(pen, rect.X, rect.Y + rect.Height + 2, rect.X, rect.Y + rect.Height - size);
                g.DrawLine(pen, rect.X + rect.Width - size, rect.Y + rect.Height, rect.X + rect.Width + 2, rect.Y + rect.Height);
                g.DrawLine(pen, rect.X + rect.Width, rect.Y + rect.Height + 2, rect.X + rect.Width, rect.Y + rect.Height - size);

                // name
                string name = "Mario";
                Font fnt = new Font("Verdana", 30, GraphicsUnit.Pixel);
                Brush brs = new SolidBrush(Color.Black);
                var bump = -50;
                System.Drawing.SizeF stringSize = g.MeasureString(name, fnt);
                g.FillRectangle(new SolidBrush(Color.Yellow), rect.X, rect.Y + bump, stringSize.Width, stringSize.Height);
                g.DrawString(name, fnt, brs, rect.X , rect.Y + bump);
            }
        }

        /// <summary>
        /// Called when videoPlayer receives a new frame. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="image"></param>
        private void videoPlayer_NewFrame(object sender, ref Bitmap frame)
        {
            // use an rgb filter to remove everything but mario's outfit
            var filter = new ColorFiltering(
                new IntRange(220, 240),
                new IntRange(200, 220),
                new IntRange(160, 180)
                );
            var filteredFrame = filter.Apply(frame);

            // convert the image to grayscale
            var gray = new GrayscaleBT709();
            var trackingFrame = gray.Apply(filteredFrame);

            // use a threshold to make the image black & white
            var threshold = new Threshold(1);
            threshold.ApplyInPlace(trackingFrame);

            // apply agressive dilation
            var dilation = new BinaryDilation3x3();
            dilation.ApplyInPlace(trackingFrame);
            dilation.ApplyInPlace(trackingFrame);
            dilation.ApplyInPlace(trackingFrame);

            // use image to mask the original frame
            var mask = new ApplyMask(trackingFrame);
            var maskedFrame = mask.Apply(frame);

            // is the camshift tracker still tracking mario?
            //     - the tracking object cannot be empty
            //     - the tracking rectangle has to be small
            bool isTracking = tracker.TrackingObject?.IsEmpty == false
                && tracker.TrackingObject?.Rectangle.Width <= 50;

            if (!isTracking)
            {
                // no - so do a blob search
                var blobSearch = new BlobCounter();
                blobSearch.FilterBlobs = true;
                blobSearch.MinHeight = 25;
                blobSearch.MinWidth = 25;
                blobSearch.ProcessImage(trackingFrame);
                if (blobSearch.ObjectsCount > 0)
                {
                    // assume the first blog we find is mario
                    var rectangles = blobSearch.GetObjectsRectangles();
                    var rect = rectangles.First();

                    // use camshift to track mario from now on
                    var img = UnmanagedImage.FromManagedImage(maskedFrame);
                    tracker.SearchWindow = rect;
                    tracker.ProcessFrame(img);
                }
            }

            else
            {
                // track mario using camshift
                var img = UnmanagedImage.FromManagedImage(maskedFrame);
                tracker.ProcessFrame(img);

                // draw the reticle where mario is
                var rect = tracker.TrackingObject.Rectangle;
                rect.Inflate(30, 30);
                DrawReticle(frame, rect);
            }

            // update pictureboxes
            thresholdedBox.Image = trackingFrame;
            maskedBox.Image = maskedFrame;

            frameIndex++;
        }
    }
}
