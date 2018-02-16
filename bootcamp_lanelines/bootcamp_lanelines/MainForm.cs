using Accord.Video.FFMPEG;
using Accord.Video.DirectShow;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Accord.Imaging.Filters;
using Accord.Imaging;
using Accord.Statistics.Visualizations;
using Accord;
using System.Drawing.Imaging;

namespace Bootcamp.CompVis.LaneLines
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
        /// Called when videoPlayer receives a new frame. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="image"></param>
        private void videoPlayer_NewFrame(object sender, ref Bitmap image)
        {
            // convert image to grayscale
            var gray = new GrayscaleBT709();
            var frame = gray.Apply(image);

            // threshold image to only keep light pixels
            var threshold = new Threshold(125);
            threshold.ApplyInPlace(frame);

            // blank out everything but the road
            var horizonY = (int)(image.Height * 0.65);
            var fill = new CanvasFill(new Rectangle(0, 0, image.Width, horizonY), Color.Red);
            fill.ApplyInPlace(frame);

            // detect edges
            var edgeDetector = new CannyEdgeDetector();
            edgeDetector.ApplyInPlace(frame);

            // do a hough line transformation, which will search for straight lines in the frame
            var transform = new HoughLineTransformation();
            transform.ProcessImage(frame);
            var rawLines = transform.GetMostIntensiveLines(50);

            // only keep non-horizontal lines that cross the horizon at the vanishing point
            var lines = from l in rawLines
                        let range = new Range(-75, -65) 
                        where range.IsInside(l.Radius)
                            && (l.Theta <= 85 || l.Theta >= 95)
                        select l;

            // show the edge detection view in the bottom left box
            edgeBox.Image = frame;

            // show the lane detection view in the bottom right box
            var laneImg = new Bitmap(image.Width, image.Height);
            Utility.DrawHoughLines(lines, laneImg, Color.White, 1);
            laneBox.Image = laneImg;

            // draw the lanes on the main camera image too
            Utility.DrawHoughLines(lines, image, Color.LightGreen, 2);
        }
    }
}
