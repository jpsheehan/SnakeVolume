namespace SnakeVolume
{
  partial class FormSnakeVolume
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSnakeVolume));
      this.snakeGame1 = new SnakeVolume.SnakeVolumeSlider();
      this.SuspendLayout();
      // 
      // snakeGame1
      // 
      this.snakeGame1.Location = new System.Drawing.Point(12, 12);
      this.snakeGame1.Name = "snakeGame1";
      this.snakeGame1.Size = new System.Drawing.Size(222, 222);
      this.snakeGame1.TabIndex = 0;
      // 
      // FormSnakeVolume
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.Black;
      this.ClientSize = new System.Drawing.Size(248, 248);
      this.Controls.Add(this.snakeGame1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "FormSnakeVolume";
      this.ShowInTaskbar = false;
      this.Text = "Snake Volume Slider";
      this.ResumeLayout(false);

    }

    #endregion

    private SnakeVolumeSlider snakeGame1;
  }
}

