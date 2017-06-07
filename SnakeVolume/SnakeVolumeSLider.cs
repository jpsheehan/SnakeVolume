using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace SnakeVolume
{
  public enum Direction
  {
    Up,
    Left,
    Down,
    Right
  }

  public partial class SnakeVolumeSlider : UserControl
  {
    #region The Volume Changing Code

    // All code in this region is courtesy of StackOverflow user Paedow
    // https://stackoverflow.com/questions/13139181/how-to-programmatically-set-the-system-volume

    private const int APPCOMMAND_VOLUME_MUTE = 0x80000;
    private const int APPCOMMAND_VOLUME_UP = 0xA0000;
    private const int APPCOMMAND_VOLUME_DOWN = 0x90000;
    private const int WM_APPCOMMAND = 0x319;

    [DllImport("user32.dll")]
    public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg,
        IntPtr wParam, IntPtr lParam);

    private void Mute()
    {
      SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle,
          (IntPtr)APPCOMMAND_VOLUME_MUTE);
    }

    private void VolDown()
    {
      SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle,
          (IntPtr)APPCOMMAND_VOLUME_DOWN);
    }

    private void VolUp()
    {
      SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle,
          (IntPtr)APPCOMMAND_VOLUME_UP);
    }
    #endregion

    #region Constant Values
    
    // Snake settings
    public static readonly int StartingLength = 5;

    // Grid settings
    public static readonly int GridSize = 20;
    public static readonly int BlockSize = 10;
    public static readonly int BlockMargin = 1;

    // Color settings
    public static readonly Color UpColor = Color.Green;
    public static readonly Color DownColor = Color.Red;
    public static readonly Color SnakeColor = Color.Blue;

    #endregion

    #region Snake Variables and Properties
    private int Length;
    private Direction Direction = Direction.Right;
    private List<Point> Body;
    private const int Speed = 10;

    private Point UpToken;
    private Point DownToken;

    private Graphics Graphics;

    private bool HasMoved;

    // for optimised drawing
    private bool HasUpTokenMoved;
    private bool HasDownTokenMoved;
    #endregion

    public SnakeVolumeSlider()
    {
      Reset();

      BackColor = Color.Black;

      Width = BlockMargin + GridSize * (BlockSize + BlockMargin);
      Height = BlockMargin + GridSize * (BlockSize + BlockMargin);

      InitializeComponent();

      timer.Interval = 1000 / Speed;

      timer.Start();
    }

    /// <summary>
    /// To be run when the snake eats itself. Resets the playfield.
    /// </summary>
    private void Reset()
    {
      Body = new List<Point>();
      Length = StartingLength;
      HasMoved = false;

      Body.Add(new Point(GridSize / 2, GridSize / 2));

      RespawnUpToken();
      RespawnDownToken();
    }

    /// <summary>
    /// Respawns the token to a valid location.
    /// </summary>
    private void RespawnUpToken()
    {
      Random rand = new Random();
      while (true)
      {
        Point point = new Point(rand.Next(GridSize), rand.Next(GridSize));

        if (point != DownToken && !Body.Contains(point))
        {
          UpToken = point;
          HasUpTokenMoved = true;
          return;
        }
      }
    }

    /// <summary>
    /// Respawns the token to a valid location.
    /// </summary>
    private void RespawnDownToken()
    {
      Random rand = new Random();
      while (true)
      {
        Point point = new Point(rand.Next(GridSize), rand.Next(GridSize));

        if (point != UpToken && !Body.Contains(point))
        {
          DownToken = point;
          HasDownTokenMoved = true;
          return;
        }
      }
    }

    /// <summary>
    /// Draws a square according to the constants.
    /// </summary>
    /// <param name="point"></param>
    /// <param name="color"></param>
    private void DrawSquare(Point point, Color color)
    {
      Graphics.FillRectangle(new Pen(color).Brush, new Rectangle(BlockMargin + point.X * (BlockMargin + BlockSize), BlockMargin + point.Y * (BlockMargin + BlockSize), BlockSize, BlockSize));
    }

    /// <summary>
    /// Initializes the snake and the tokens. Begins the timer.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SnakeGame_Load(object sender, EventArgs e)
    {
      Reset();
    }

    /// <summary>
    /// The redraw event for the control. Draws all the graphics.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SnakeGame_Paint(object sender, PaintEventArgs e)
    {
      this.Graphics = e.Graphics;

      Graphics.DrawRectangle(Pens.DarkGray, new Rectangle(0, 0, Width - 1, Height - 1));

      // draw the new uptoken if required
      if (HasUpTokenMoved)
        DrawSquare(UpToken, UpColor);

      // draw the new downtoken if required
      if (HasDownTokenMoved)
        DrawSquare(DownToken, DownColor);

      // draw the new head
      foreach (Point body in Body)
        DrawSquare(body, SnakeColor);
    }

    /// <summary>
    /// Performed according to the speed constant. Moves the snake, checks for collisions and grows the snake if required.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void timer_Tick(object sender, EventArgs e)
    {
      #region Move the snake
      int new_x = Body[0].X,
        new_y = Body[0].Y;

      switch (Direction)
      {
        case Direction.Up:
          new_y--;
          break;
        case Direction.Down:
          new_y++;
          break;
        case Direction.Right:
          new_x++;
          break;
        case Direction.Left:
          new_x--;
          break;
      }

      if (new_x < 0)
        new_x = GridSize - 1;
      if (new_x > GridSize - 1)
        new_x = 0;
      if (new_y < 0)
        new_y = GridSize - 1;
      if (new_y > GridSize - 1)
        new_y = 0;

      Point new_head = new Point(new_x, new_y);

      HasMoved = false;
      #endregion

      #region Check for collisions

      // has collided with an UpToken?
      if (new_head == UpToken)
      {
        VolUp();
        RespawnUpToken();
        Length++;
      }

      // has collided with a DownToken?
      if (new_head == DownToken)
      {
        VolDown();
        RespawnDownToken();
        Length++;
      }

      // has collided with itself?
      foreach (var point in Body.Skip(1))
      {
        if (point == new_head)
        {
          Mute();
          Reset();
          return; // return because we don't want to increase the length after this
        }
      }
      #endregion

      // Add the new_head and possibly remove the tail
      Body.Reverse();
      Body.Add(new_head);
      Body.Reverse();

      if (Length == Body.Count)
      {
        Body.RemoveAt(Body.Count - 1);
      }

      // Redraw the control
      Refresh();
    }

    /// <summary>
    /// Moves the snake depending on which arrow key is pressed. Doesn't allow the user to turn 180 degrees on itself. Exits the application if the Enter or Escape keys are pressed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SnakeGame_KeyUp(object sender, KeyEventArgs e)
    {
      if (HasMoved)
        return;

      switch (e.KeyCode)
      {
        case Keys.Up:
          if (Direction != Direction.Down)
          {
            HasMoved = true;
            Direction = Direction.Up;
          }
          break;
        case Keys.Down:
          if (Direction != Direction.Up)
          {
            HasMoved = true;
            Direction = Direction.Down;
          }
          break;
        case Keys.Left:
          if (Direction != Direction.Right)
          {
            HasMoved = true;
            Direction = Direction.Left;
          }
          break;
        case Keys.Right:
          if (Direction != Direction.Left)
          {
            HasMoved = true;
            Direction = Direction.Right;
          }
          break;
        case Keys.Enter:
        case Keys.Escape:
          Application.Exit();
          break;
      }
    }
  }
}
