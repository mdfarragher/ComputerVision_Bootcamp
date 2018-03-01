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
    /// A ball in the game.
    /// </summary>
    public class Ball  : GameObject
    {
        /// <summary>
        /// Construct a new ball.
        /// </summary>
        /// <param name="form1">The form owning the ball.</param>
        public Ball(GameForm form1)
        {
            this._form = form1;
            UpdateColor();
            _size = new Size(20, 20);
            UpdateSpeed(new Point());
        }

        /// <summary>
        /// Update the speed of the ball.
        /// </summary>
        /// <param name="newDirection">The new speed vector.</param>
        void UpdateSpeed(Point newDirection)
        {
            if (newDirection.X == 0 &&
              newDirection.Y == 0) // initializing
            {
                while (_velocity.X == 0 && _velocity.Y == 0)
                {
                    _velocity.X = _speedMax -
                            _form._rand.Next(_speedMax * 2);
                    _velocity.Y = _speedMax -
                            _form._rand.Next(_speedMax * 2);
                }
            }
            else
            {
                _velocity.X = newDirection.X;
                _velocity.Y = newDirection.Y;
            }
        }

        /// <summary>
        /// Draw the ball.
        /// </summary>
        /// <param name="e"></param>
        public override void Draw(PaintEventArgs e)
        {
            if (!IsDead)
            {
                var rect = GetRect();
                if (e.ClipRectangle.IntersectsWith(rect))
                {
                    e.Graphics.FillEllipse(_brush, rect);
                }
            }
        }
    }
}
