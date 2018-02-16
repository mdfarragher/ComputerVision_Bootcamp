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

namespace Bootcamp.CompVis.Traffic
{
    /// <summary>
    /// The TrackerType holds data to track a single car.
    /// </summary>
    struct TrackerType
    {
        /// <summary>
        /// The camshift tracker tracking the car.
        /// </summary>
        public Camshift Tracker;

        /// <summary>
        /// The frame index when the car was first discovered.
        /// </summary>
        public int StartIndex;

        /// <summary>
        /// The index number of the car (1...N)
        /// </summary>
        public int CarNumber;

        public TrackerType(Rectangle rect, int index, int carNumber)
        {
            Tracker = new Camshift(rect);
            Tracker.Smooth = true;
            Tracker.Conservative = false;
            StartIndex = index;
            CarNumber = carNumber;
        }
    }

    /// <summary>
    /// The main application form.
    /// </summary>
    public partial class MainForm : Form
    {
        // switch between camera and video
        private const bool USE_CAMERA = false;

        // the previous video frame
        private Bitmap previousFrame = null;

        // the index of the current frame
        private int frameIndex = 0;

        // the index of the last detected car
        private int carIndex = 0;

        // the list of camshift trackers following cars in the frame
        private List<TrackerType> trackers = new List<TrackerType>();

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
        /// Draw a label on the specified car.
        /// </summary>
        /// <param name="frame">The current video frame.</param>
        /// <param name="rect">The bounding box around the car.</param>
        /// <param name="index">The index number of the car.</param>
        private void DrawCarLabel(Bitmap frame, Rectangle rect, int index)
        {
            using (Graphics g = Graphics.FromImage(frame))
            {
                string name = $"Car {index}";
                Font fnt = new Font("Verdana", 20, GraphicsUnit.Pixel);
                Brush brs = new SolidBrush(Color.Black);
                var stringSize = g.MeasureString(name, fnt);
                var center = rect.Center();
                g.FillRectangle(new SolidBrush(Color.Yellow), center.X, center.Y, stringSize.Width, stringSize.Height);
                g.DrawString(name, fnt, brs, center.X, center.Y);
            }
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

            // start the timer
            timer.Start();
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
        private void videoPlayer_NewFrame(object sender, ref Bitmap frame)
        {
            if (previousFrame != null)
            {
                // find the thresholded euclidian difference between two subsequent frames
                ThresholdedEuclideanDifference threshold = new ThresholdedEuclideanDifference(40);
                threshold.OverlayImage = previousFrame;
                var difference = threshold.Apply(frame);

                // only keep big blobs
                var filter = new BlobsFiltering();
                filter.CoupledSizeFiltering = true;
                filter.MinHeight = 50;
                filter.MinWidth = 50;
                filter.ApplyInPlace(difference);

                // dilate remaining blobs
                var dilate = new BinaryDilation3x3();
                dilate.ApplyInPlace(difference);
                dilate.ApplyInPlace(difference);
                dilate.ApplyInPlace(difference);
                dilate.ApplyInPlace(difference);

                // put this image in the thresholded picturebox
                thresholdedBox.Image = difference.Clone() as Bitmap;

                // use this as a mask for the current frame
                var mask = new ApplyMask(difference);
                var maskedFrame = mask.Apply(frame);

                // put this image in the masked picturebox
                maskedBox.Image = maskedFrame.Clone() as Bitmap;

                // now find all moving blobs
                if (frameIndex % 10 == 0)
                {
                    var counter = new BlobCounter();
                    counter.ProcessImage(difference);

                    // only keep blobs that:
                    //     - do not overlap with known cars
                    //     - do not overlap with other blobs 
                    //     - have crossed the middle of the frame
                    //     - are at least 100 pixels tall
                    var blobs = counter.GetObjectsRectangles();
                    var newBlobs = from r in counter.GetObjectsRectangles()
                                   where !trackers.Any(t => t.Tracker.TrackingObject.Rectangle.IntersectsWith(r))
                                      && !blobs.Any(b => b.IntersectsWith(r) && b != r)
                                      && r.Top >= 240 && r.Bottom <= 480 
                                      && r.Height >= 100
                                   select r;

                    // set up new camshift trackers for each detected blob
                    foreach (var rect in newBlobs)
                    {
                        trackers.Add(new TrackerType(rect, frameIndex, ++carIndex));
                    }
                }

                // now kill all car trackers that have expanded by too much
                trackers.RemoveAll(t => t.Tracker.TrackingObject.Rectangle.Height > 360);

                // and kill all trackers that have lived for 30 frames
                trackers.RemoveAll(t => frameIndex - t.StartIndex > 30);

                // let all remaining trackers process the current frame
                var img = UnmanagedImage.FromManagedImage(maskedFrame);
                trackers
                    .ForEach(t => t.Tracker.ProcessFrame(img));

                // remember this frame for next iteration
                previousFrame.Dispose();
                previousFrame = frame.Clone() as Bitmap;

                // draw labels on all tracked cars
                var outputFrame = frame.Clone() as Bitmap;
                trackers
                    .FindAll(t => !t.Tracker.TrackingObject.IsEmpty)
                    .ForEach(t => DrawCarLabel(outputFrame, t.Tracker.TrackingObject.Rectangle, t.CarNumber));

                // return the processed frame to the video
                frame = outputFrame;
            }

            // or else just remember this frame for next iteration
            else
                previousFrame = frame.Clone() as Bitmap;

            frameIndex++;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            carLabel.Text = $"Number of cars: {carIndex}";
        }
    }
}
