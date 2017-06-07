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

    // Snake settings
    private int Length;
    private Direction Direction;
    private List<Point> Body;
    private int StartingLength = 5;

    // Grid settings
    private int GridSize;
    private int BlockSize;
    private int BlockMargin;
    private int Speed;

    // Color settings
    private Color UpColor;
    private Color DownColor;
    private Color SnakeColor;

    private Point UpToken;
    private Point DownToken;

    private Graphics Graphics;

    private bool HasMoved;

    public SnakeVolumeSlider()
    {
      Body = new List<Point>();
      Direction = Direction.Right;

      GridSize = 10;
      BlockSize = 20;
      BlockMargin = 2;
      Speed = 5;

      UpColor = Color.Green;
      DownColor = Color.Red;
      SnakeColor = Color.Blue;

      Width = BlockMargin + GridSize * (BlockSize + BlockMargin);
      Height = BlockMargin + GridSize * (BlockSize + BlockMargin);

      InitializeComponent();

      timer.Interval = 1000 / Speed;

      Start();
    }
    
    public void Step()
    {
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

      // Check collisions

      if (new_head == UpToken)
      {
        VolUp();
        RespawnUpToken();
        Length++;
      }

      if (new_head == DownToken)
      {
        VolDown();
        RespawnDownToken();
        Length++;
      }

      foreach (var point in Body.Skip(1))
      {
        if (point == new_head)
        {
          Mute();
          Reset();
          return;
        }
      }

      Body.Reverse();
      Body.Add(new_head);
      Body.Reverse();

      if (Length == Body.Count)
      {
        Body.RemoveAt(Body.Count - 1);
      }


      Refresh();

    }

    public void Start()
    {
      timer.Start();
    }

    private void Reset()
    {
      Body = new List<Point>();
      Length = StartingLength;
      HasMoved = false;

      Body.Add(new Point(GridSize / 2, GridSize / 2));

      RespawnUpToken();
      RespawnDownToken();
    }

    private void RespawnUpToken()
    {
      Random rand = new Random();
      while (true)
      {
        Point point = new Point(rand.Next(GridSize), rand.Next(GridSize));

        if (point != DownToken && !Body.Contains(point))
        {
          UpToken = point;
          return;
        }
      }
    }

    private void RespawnDownToken()
    {
      Random rand = new Random();
      while (true)
      {
        Point point = new Point(rand.Next(GridSize), rand.Next(GridSize));

        if (point != UpToken && !Body.Contains(point))
        {
          DownToken = point;
          return;
        }
      }
    }

    private void SnakeGame_Load(object sender, EventArgs e)
    {
      Reset();
    }

    private void SnakeGame_Paint(object sender, PaintEventArgs e)
    {
      this.Graphics = e.Graphics;

      Graphics.FillRectangle(Brushes.Black, new Rectangle(0, 0, Width, Height));
      Graphics.DrawRectangle(Pens.DarkGray, new Rectangle(0, 0, Width - 1, Height - 1));

      DrawSquare(UpToken, UpColor);
      DrawSquare(DownToken, DownColor);

      foreach (Point point in Body)
      {
        DrawSquare(point, SnakeColor);
      }
    }

    private void DrawSquare(Point point, Color color)
    {
      Graphics.FillRectangle(new Pen(color).Brush, new Rectangle(BlockMargin + point.X * (BlockMargin + BlockSize), BlockMargin + point.Y * (BlockMargin + BlockSize), BlockSize, BlockSize));
    }

    private void timer_Tick(object sender, EventArgs e)
    {
      Step();
    }

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
