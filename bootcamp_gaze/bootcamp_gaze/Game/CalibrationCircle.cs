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
    /// The calibration circle that appears before the game starts.
    /// </summary>
    public class CalibrationCircle : GameObject
    {
        /// <summary>
        /// Construct a new block.
        /// </summary>
        /// <param name="form">The form owning the block.</param>
        public CalibrationCircle(GameForm form)
        {
            _form = form;
            _size = new Size(100, 100);
            _pos = new Point(form.Width / 2 - 50, form.Height / 2 - 50);
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
                if (e.ClipRectangle.IntersectsWith(rect))
                {
                    var pen = Pens.LightBlue;
                    e.Graphics.DrawEllipse(pen, new Rectangle(_pos, _size));
                }
            }
        }
    }
}
