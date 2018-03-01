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
    /// A block in the game. 
    /// </summary>
    public class Block : GameObject
    {
        // private members
        int _marginX = 50;
        int _marginY = 70;

        /// <summary>
        /// Construct a new block.
        /// </summary>
        /// <param name="form">The form owning the block.</param>
        /// <param name="i">The horizontal grid position of the block.</param>
        /// <param name="j">The vertical grid position of the block.</param>
        public Block(GameForm form, int i, int j)
        {
            _form = form;
            _size = new Size(70, 20);
            _pos = new Point(
              (_marginX + _size.Width) * i,
              40 + (_marginY + _size.Height) * j + 20
              );
            UpdateColor();
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
                    e.Graphics.FillRectangle(_brush, rect);
                }
            }
        }
    }
}
