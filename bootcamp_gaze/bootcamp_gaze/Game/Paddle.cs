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
    /// A paddle in the game.
    /// </summary>
    public class Paddle : GameObject 
    {
        /// <summary>
        /// The current speed of the paddle.
        /// </summary>
        public int _speed = 10;

        /// <summary>
        /// 
        /// </summary>
        private Queue<int> gazeHistory = new Queue<int>();

        /// <summary>
        /// Construct a new paddle.
        /// </summary>
        /// <param name="form">The form owning the paddle.</param>
        public Paddle(GameForm form)
        {
            _form = form;
            _brush = Brushes.Blue;
            _pos = new Point(10, _form.Height - 80);
            _size = new Size(150, 10);
        }

        /// <summary>
        /// Draw the paddle.
        /// </summary>
        /// <param name="e"></param>
        public override void Draw(PaintEventArgs e)
        {
            var rect = GetRect();
            if (e.ClipRectangle.IntersectsWith(rect))
            {
                e.Graphics.FillRectangle(_brush, rect);
            }
        }

        /// <summary>
        /// Handle a movement of the gaze point.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void OnGazeMove(object sender, GazeEventArgs e)
        {
            // add the gaze point to the history, keep a queue of 10 values
            gazeHistory.Enqueue(e.Point.X);
            if (gazeHistory.Count > 10)
                gazeHistory.Dequeue();

            // calculate average gaze position
            var average = gazeHistory.Average(x => x);

            // move the paddle so its center aligns with the average location
            _pos.X = (int)average - _size.Width / 2;
        }

    }
}
