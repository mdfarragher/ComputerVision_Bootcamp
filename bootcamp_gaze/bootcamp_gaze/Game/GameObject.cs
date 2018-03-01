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
    /// The base class for all ganme objects.
    /// </summary>
    public abstract class GameObject
    {
        /// <summary>
        /// The brush to use when drawing the game object.
        /// </summary>
        public Brush _brush;

        /// <summary>
        /// The current location of the game object.
        /// </summary>
        public Point _pos;

        /// <summary>
        /// The size of the game object.
        /// </summary>
        public Size _size;

        /// <summary>
        /// The form that owns the game object.
        /// </summary>
        public GameForm _form;

        /// <summary>
        /// The current velocity vector of the game object.
        /// </summary>
        public Point _velocity;

        /// <summary>
        /// The maximum speed of the game object.
        /// </summary>
        public int _speedMax = 20;

        /// <summary>
        /// A flag indicating if the game object is dead.
        /// </summary>
        public bool IsDead;

        /// <summary>
        /// The current color of the game object.
        /// </summary>
        static int curColor = 0xffffff;

        /// <summary>
        /// Draw the game object.
        /// </summary>
        /// <param name="e">The paint event arguments.</param>
        public abstract void Draw(PaintEventArgs e);

        /// <summary>
        /// Get the rectangle bounding the game object.
        /// </summary>
        /// <returns></returns>
        public Rectangle GetRect()
        {
            return new Rectangle(
              _pos.X,
              _pos.Y,
              _size.Width + 1,
              _size.Height + 1);
        }

        /// <summary>
        /// Update the color of the game object.
        /// </summary>
        public void UpdateColor()
        {
            var newColor = Color.FromArgb(
                            (byte)(0xff), 
                            (byte)(curColor & 0xff), 
                            (byte)((curColor >> 4) & 0xff),
                            (byte)((curColor >> 8) & 0xff) 
                            );
            _brush = new SolidBrush(newColor);
            curColor -= 100; 
        }
    }
}
