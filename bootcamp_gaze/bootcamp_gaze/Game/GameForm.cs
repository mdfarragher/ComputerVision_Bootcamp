using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Bootcamp.CompVis.Gaze.Game
{
    /// <summary>
    /// The delegate for reading current gaze values.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void OnGazeMoveDelegate(object sender, GazeEventArgs e);


    /// <summary>
    /// The game form.
    /// </summary>
    public partial class GameForm : Form
    {
        // private members
        Paddle _paddle;
        Ball[] _balls;
        CalibrationCircle _circle;
        CalibrationText _text;
        GazeDot _dot;
        Point gazeOffset = new Point();
        const int _NumBalls = 2; 
        int _NumBallsAlive;
        int _nLevels;
        int _Score = 0;
        Block[,] _blocks;
        int _nBlocksX = 8;
        int _nBlocksY = 3;
        int _numBlocksAlive;
        int _frameIndex = 0;
        Timer _timer;
        public Random _rand = new Random(1);

        // events
        public event OnGazeMoveDelegate OnGazeMove;

        /// <summary>
        /// Form constructor.
        /// </summary>
        public GameForm()
        {
            InitializeComponent();

            // tweak the form style
            SetStyle(ControlStyles.AllPaintingInWmPaint |
               ControlStyles.OptimizedDoubleBuffer |
               ControlStyles.DoubleBuffer |
                ControlStyles.UserPaint
               , true);

            // resize the game board
            Width = 1000;
            Height = 800;

            // create paddle and balls
            BackColor = Color.Black;
            _paddle = new Paddle(this);
            _balls = new Ball[_NumBalls];
            _NumBallsAlive = _NumBalls;
            for (int i = 0; i < _NumBalls; i++)
            {
                _balls[i] = new Ball(this)
                {
                    _pos = new Point(
                      100 + i * 20,
                      200 + i * 20)
                };
            }

            // create blocks
            CreateBlocks();

            // create the calibration objects
            _circle = new CalibrationCircle(this);
            _text = new CalibrationText(this);

            // create the gaze dot
            _dot = new GazeDot(this);

            // set up a timer to update the screen
            _timer = new Timer();
            _timer.Interval = 30; 
            _timer.Tick += tmr_Tick;
            _timer.Enabled = true;
        }

        /// <summary>
        /// Create the blocks that populate the game field.
        /// </summary>
        private void CreateBlocks()
        {
            _numBlocksAlive = _nBlocksX * _nBlocksY;
            _blocks = new Block[_nBlocksX, _nBlocksY];
            for (int i = 0; i < _nBlocksX; i++)
            {
                for (int j = 0; j < _nBlocksY; j++)
                {
                    _blocks[i, j] = new Block(this, i, j);
                    Invalidate(_blocks[i, j].GetRect());
                }
            }
        }

        /// <summary>
        /// Update the game board.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tmr_Tick(object sender, EventArgs e)
        {
            // get the current gaze point
            var gea = new GazeEventArgs();
            OnGazeMove?.Invoke(this, gea);

            // perform calibration for one second
            var interval = ((Timer)sender).Interval;
            if (_frameIndex < 1000/interval)
            {
                gazeOffset.X -= (_dot._pos.X - this.Width / 2);
            }

            // after the calibration period, kill the circle and text and start the game
            else
            {
                if (!_circle.IsDead)
                {
                    _circle.IsDead = true;
                    _text.IsDead = true;
                    Invalidate(_circle.GetRect());
                    Invalidate(_text.GetRect());
                }
            }

            // apply offset to gaze point
            gea.Point.X += gazeOffset.X;

            // move the paddle
            Invalidate(_paddle.GetRect());
            _paddle.OnGazeMove(this, gea);
            Invalidate(_paddle.GetRect());

            // move the gaze dot
            Invalidate(_dot.GetRect());
            _dot._pos.X = gea.Point.X;
            Invalidate(_dot.GetRect());

            Point res;
            for (int iBall = 0; iBall < _NumBalls; iBall++)
            {
                // check if the ball is alive
                var ball = _balls[iBall];
                if (!ball.IsDead)
                {
                    // detect collisions with the wall
                    res = DetectCollision(ball, ClientRectangle, fShouldBeInside: true);
                    if (res.X == 0 && res.Y == 0) 
                    {
                        // detect colissions with the paddle
                        res = DetectCollision(ball, _paddle.GetRect(), fShouldBeInside: false);
                        if (res.X == 0 && res.Y == 0) 
                        {
                            // all blocks gone?
                            if (_numBlocksAlive == 0)
                            {
                                // increment level
                                _nLevels++;

                                // build more blocks
                                _nBlocksY++;

                                // bring all balls back to life
                                int nBallsToBringBackToLife = 0;
                                for (int i = 0; i < _NumBalls; i++)
                                {
                                    if (_balls[i].IsDead)
                                    {
                                        _balls[i].IsDead = false;
                                        _balls[i]._pos.X = 100 + 10 * i;
                                        _balls[i]._pos.Y = 100 + 10 * i;
                                        _NumBallsAlive++;
                                        Invalidate(_balls[i].GetRect());
                                        if (++nBallsToBringBackToLife == 3)
                                        {
                                            break;
                                        }
                                    }
                                }

                                // recreate the blocks
                                CreateBlocks();
                            }

                            // some blocks remaining
                            else
                            {
                                // detect collision with any block
                                for (int i = 0; i < _nBlocksX; i++)
                                {
                                    for (int j = 0; j < _nBlocksY; j++)
                                    {
                                        if (!_blocks[i, j].IsDead)
                                        {
                                            res = DetectCollision(ball, _blocks[i, j].GetRect(), fShouldBeInside: false);
                                            if (res.X != 0 || res.Y != 0)
                                            {
                                                // kill the block
                                                _blocks[i, j].IsDead = true;
                                                _numBlocksAlive--;
                                                _Score++;
                                                Invalidate(_blocks[i, j].GetRect());
                                            }
                                        }
                                    }
                                }

                            }
                        }

                        // ball hit the paddle
                        else
                        {
                            // bounce handled elsewhere
                        }
                    }

                    // ball hit the wall
                    else
                    { 
                        // if the ball hit the bottom wall, it's dead
                        if (res.Y == -1)
                        {
                            ball.IsDead = true;
                            Invalidate(ball.GetRect());
                            _NumBallsAlive--;
                        }
                    }

                    // update the ball position
                    Invalidate(ball.GetRect());
                    ball._pos.X += ball._velocity.X;
                    ball._pos.Y += ball._velocity.Y;
                    Invalidate(ball.GetRect());
                }
            }

            // stop the timer if all balls are dead
            if (_NumBallsAlive == 0)
            {
                this._timer.Enabled = false;
            }

            // update the frame counter
            _frameIndex++;

            // update game status
            this.Text = string.Format(
              "Score = {0}  # Levels = {1} # of balls left = {2}",
              _Score,
              _nLevels,
              _NumBallsAlive);
        }

        /// <summary>
        /// Detect collision between the ball and another game object.
        /// </summary>
        /// <param name="ball">The ball to check.</param>
        /// <param name="rectTest">The rectangle bounding a game object.</param>
        /// <param name="fShouldBeInside">Indicates thatthe ball should be inside the rectangle.</param>
        /// <returns>A point instance indicating collisions along the x- or y-axis.</returns>
        Point DetectCollision(Ball ball, Rectangle rectTest, bool fShouldBeInside)
        {
            var res = new Point();

            int nCollides = 0;
            if (fShouldBeInside)
            {
                nCollides = 1;
            }
            else
            {
                var rect = ball.GetRect();
                rect = new Rectangle(
                    new Point(
                        rect.Left + ball._velocity.X,
                        rect.Top + ball._velocity.Y),
                    ball._size);
                if (rect.IntersectsWith(rectTest))
                {
                    nCollides = 1;
                }
            }

            if (ball._velocity.X != 0)
            {
                if (ball._velocity.X > 0)
                {
                    var edgeX = rectTest.Left;
                    if (fShouldBeInside)
                    {
                        edgeX = rectTest.Left + rectTest.Width;
                    }
                    if (ball._pos.X < edgeX &&
                        ball._pos.X +
                        ball._size.Width +
                        ball._velocity.X >= edgeX)
                    {
                        res.X = -nCollides;
                    }
                }
                else
                {
                    var edgeX = rectTest.Left + rectTest.Width;
                    if (fShouldBeInside)
                    {
                        edgeX = rectTest.Left;
                    }
                    if (ball._pos.X > edgeX &&
                        ball._pos.X +
                        ball._velocity.X <= edgeX)
                    {
                        res.X = nCollides;
                    }
                }
            }
            if (ball._velocity.Y != 0)
            {
                if (ball._velocity.Y > 0)
                {
                    var edgeY = rectTest.Top;
                    if (fShouldBeInside)
                    {
                        edgeY = rectTest.Top + rectTest.Height;
                    }
                    if (ball._pos.Y < edgeY &&
                        ball._pos.Y +
                        ball._size.Height +
                        ball._velocity.Y >= edgeY)
                    {
                        res.Y = -nCollides;
                    }
                }
                else
                {
                    var edgeY = rectTest.Top + rectTest.Height;
                    if (fShouldBeInside)
                    {
                        edgeY = rectTest.Top;
                    }
                    if (ball._pos.Y > edgeY &&
                        ball._pos.Y +
                        ball._velocity.Y <= edgeY)
                    {
                        res.Y = nCollides;
                    }
                }
            }
            if (res.X != 0) // if it collides in x direction
            {
                ball._velocity.X =
                    res.X * (1 + _rand.Next(ball._speedMax));
            }
            if (res.Y != 0)
            {
                ball._velocity.Y =
                    res.Y * (1 + _rand.Next(ball._speedMax));
            }

            return res;

        }

        /// <summary>
        /// Paint the form.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            //draw the paddle.
            _paddle.Draw(e);

            // draw the balls
            for (int i = 0; i < _NumBalls; i++)
            {
                _balls[i].Draw(e);
            }

            // draw the blocks
            foreach (var blk in _blocks)
            {
                blk.Draw(e);
            }

            // draw the calibration objects
            _circle.Draw(e);
            _text.Draw(e);

            // draw the gaze dot
            _dot.Draw(e);
        }
    }
}
