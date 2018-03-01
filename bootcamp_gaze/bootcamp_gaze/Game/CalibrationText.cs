using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bootcamp.CompVis.Gaze.Game
{
    /// <summary>
    /// The calibration text that appears before the game starts.
    /// </summary>
    public class CalibrationText : GameObject
    {
        /// <summary>
        /// Construct a new calibration text.
        /// </summary>
        /// <param name="form">The form owning the text.</param>
        public CalibrationText(GameForm form)
        {
            _form = form;
        }

        /// <summary>
        /// Draw the block.
        /// </summary>
        /// <param name="e">The paint event arguments.</param>
        public override void Draw(PaintEventArgs e)
        {
            if (!IsDead)
            {
                var rect = GetRect();
                if (rect.IsEmpty || e.ClipRectangle.IntersectsWith(rect))
                {
                    var text = "Calibrating.... Please look at the circle";
                    Font fnt = new Font("Verdana", 30, GraphicsUnit.Pixel);
                    Brush brs = new SolidBrush(Color.Black);
                    var x = _form.Width / 2 - 300;
                    var y = _form.Height / 2 + 100;
                    System.Drawing.SizeF stringSize = e.Graphics.MeasureString(text, fnt);
                    e.Graphics.FillRectangle(new SolidBrush(Color.Yellow), x, y, stringSize.Width, stringSize.Height);
                    e.Graphics.DrawString(text, fnt, brs, x, y);

                    // update position and size
                    _pos.X = x;
                    _pos.Y = y;
                    _size.Width = (int)stringSize.Width;
                    _size.Height = (int)stringSize.Height;
                }
            }
        }
    }
}
