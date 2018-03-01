using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bootcamp.CompVis.Gaze.Game
{
    /// <summary>
    /// The gaze event arguments for gaze events.
    /// </summary>
    public class GazeEventArgs : EventArgs
    {
        /// <summary>
        /// The current gaze point on the screen.
        /// </summary>
        public Point Point = default(Point);

        /// <summary>
        /// Construct a new GazeEventArgs instance.
        /// </summary>
        public GazeEventArgs()
        {
            this.Point = new Point();
        }

        /// <summary>
        /// Construct a new GazeEventArgs instance.
        /// </summary>
        /// <param name="point">The current gaze point.</param>
        public GazeEventArgs(Point point)
        {
            this.Point = point;
        }
    }
}
