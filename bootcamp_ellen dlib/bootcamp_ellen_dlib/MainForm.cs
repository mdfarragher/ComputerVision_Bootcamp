using Accord.Video.DirectShow;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq;
using System.Windows.Forms;
using Accord.Vision.Detection;
using System.Drawing.Imaging;
using Accord.Imaging.Filters;
using DlibDotNet;
using DlibDotNet.Extensions;
using Dlib = DlibDotNet.Dlib;
using System.Collections.Generic;

namespace Bootcamp.CompVis.Ellen_Dlib
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
        /// Called when MainForm loads.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            // load the input image
            var bitmap = Bitmap.FromFile("./input.jpg") as Bitmap;

            // show it
            pictureBox.Image = bitmap;

            // process image
            var newBitmap = ProcessImage(bitmap);

            // show new image
            pictureBox.Image = newBitmap;
        }

        private Bitmap ProcessImage(Bitmap image)
        {
            // set up Dlib facedetectors and shapedetectors
            using (var fd = FrontalFaceDetector.GetFrontalFaceDetector())
            using (var sp = new ShapePredictor("shape_predictor_68_face_landmarks.dat"))
            {
                // convert image to dlib format
                var img = image.ToArray2D<RgbPixel>();

                // detect faces
                var faces = fd.Detect(img);

                // detect facial landmarks
                foreach (var rect in faces)
                {
                    // detect facial landmarks
                    var shape = sp.Detect(img, rect);

                    // extract face chip
                    var chip = Dlib.GetFaceChipDetails(shape);
                    var thumbnail = Dlib.ExtractImageChips<RgbPixel>(img, chip);

                    // add picturebox
                    var box = new PictureBox()
                    {
                        Image = thumbnail.ToBitmap<RgbPixel>(),
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Width = 62,
                        Height = 62
                    };
                    imagesPanel.Controls.Add(box);

                    // draw landmarks on main image
                    var lines = Dlib.RenderFaceDetections(new FullObjectDetection[] { shape });
                    foreach (var line in lines)
                        Dlib.DrawRectangle(
                            img,
                            new DlibDotNet.Rectangle(line.Point1),
                            new RgbPixel { Green = 255 },
                            8);
                }
                return img.ToBitmap<RgbPixel>();
            }
        }
    }
}
