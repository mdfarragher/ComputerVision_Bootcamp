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
    /// The dot that indicates the current gaze.
    /// </summary>
    public class GazeDot : GameObject
    {
        /// <summary>
        /// Construct a new block.
        /// </summary>
        /// <param name="form">The form owning the block.</param>
        public GazeDot(GameForm form)
        {
            _form = form;
            _size = new Size(4, 4);
            _pos = new Point(form.Width / 2, _form.Height - 80);
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
                    var brush = Brushes.White;
                    e.Graphics.FillRectangle(brush, new Rectangle(_pos, _size));
                }
            }
        }
    }
}
