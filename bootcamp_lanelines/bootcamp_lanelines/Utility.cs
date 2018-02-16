using Accord.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bootcamp.CompVis.LaneLines
{
    /// <summary>
    /// Helper class.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Draw the collection of Hough lines on the given image.
        /// </summary>
        /// <param name="lines">The Hough lines to draw.</param>
        /// <param name="image">The image to draw the lines on.</param>
        /// <param name="color">The color to use.</param>
        /// <param name="width">The line width to use.</param>
        public static void DrawHoughLines(IEnumerable<HoughLine> lines, Bitmap image, Color color, int width)
        {
            using (Graphics g = Graphics.FromImage(image))
            {
                var pen = new Pen(color, width);

                foreach (HoughLine line in lines)
                {
                    // get radius and theta values
                    int r = line.Radius;
                    double t = line.Theta;

                    // check if line is in lower part of the image
                    if (r < 0)
                    {
                        t += 180;
                        r = -r;
                    }

                    // convert degrees to radians
                    t = (t / 180) * Math.PI;

                    // get image centers (all coordinate are measured relative to center)
                    int w2 = image.Width / 2;
                    int h2 = image.Height / 2;

                    double x0 = 0, x1 = 0, y0 = 0, y1 = 0;

                    // calculate line (x,y) coordinate pairs
                    if (line.Theta == 0)
                    {
                        x0 = line.Radius;
                        x1 = line.Radius;
                        y0 = h2;
                        y1 = -h2;
                    }
                    else if (line.Theta > 90)
                    {
                        x0 = -w2; 
                        x1 = 0; // only draw part below center point
                        y0 = (-Math.Cos(t) * x0 + r) / Math.Sin(t);
                        y1 = (-Math.Cos(t) * x1 + r) / Math.Sin(t);
                    }
                    else if (line.Theta < 90)
                    {
                        x0 = 0; // only draw part below center point
                        x1 = w2; 
                        y0 = (-Math.Cos(t) * x0 + r) / Math.Sin(t);
                        y1 = (-Math.Cos(t) * x1 + r) / Math.Sin(t);
                    }

                    // draw line on the image
                    g.DrawLine(
                        pen,
                        new Point((int)x0 + w2, h2 - (int)y0),
                        new Point((int)x1 + w2, h2 - (int)y1));
                }
            }
        }
    }
}
