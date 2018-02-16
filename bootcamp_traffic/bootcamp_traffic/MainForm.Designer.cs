namespace Bootcamp.CompVis.Traffic
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            this.videoPlayer = new Accord.Controls.VideoSourcePlayer();
            this.thresholdedBox = new System.Windows.Forms.PictureBox();
            this.maskedBox = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.carLabel = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.thresholdedBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maskedBox)).BeginInit();
            this.SuspendLayout();
            // 
            // videoPlayer
            // 
            this.videoPlayer.Location = new System.Drawing.Point(14, 12);
            this.videoPlayer.Name = "videoPlayer";
            this.videoPlayer.Size = new System.Drawing.Size(1280, 720);
            this.videoPlayer.TabIndex = 0;
            this.videoPlayer.Text = "videoSourcePlayer1";
            this.videoPlayer.VideoSource = null;
            this.videoPlayer.NewFrame += new Accord.Controls.VideoSourcePlayer.NewFrameHandler(this.videoPlayer_NewFrame);
            // 
            // thresholdedBox
            // 
            this.thresholdedBox.Location = new System.Drawing.Point(12, 755);
            this.thresholdedBox.Name = "thresholdedBox";
            this.thresholdedBox.Size = new System.Drawing.Size(640, 360);
            this.thresholdedBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.thresholdedBox.TabIndex = 1;
            this.thresholdedBox.TabStop = false;
            // 
            // maskedBox
            // 
            this.maskedBox.Location = new System.Drawing.Point(652, 755);
            this.maskedBox.Name = "maskedBox";
            this.maskedBox.Size = new System.Drawing.Size(640, 360);
            this.maskedBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.maskedBox.TabIndex = 2;
            this.maskedBox.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(27, 772);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(227, 29);
            this.label1.TabIndex = 3;
            this.label1.Text = "Thresholded Frame";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(671, 772);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(174, 29);
            this.label2.TabIndex = 4;
            this.label2.Text = "Masked Frame";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.White;
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(27, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(141, 29);
            this.label3.TabIndex = 5;
            this.label3.Text = "Input Frame";
            // 
            // carLabel
            // 
            this.carLabel.AutoSize = true;
            this.carLabel.BackColor = System.Drawing.Color.White;
            this.carLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.carLabel.ForeColor = System.Drawing.Color.Black;
            this.carLabel.Location = new System.Drawing.Point(462, 642);
            this.carLabel.Name = "carLabel";
            this.carLabel.Size = new System.Drawing.Size(395, 55);
            this.carLabel.TabIndex = 6;
            this.carLabel.Text = "Number of cars: -";
            // 
            // timer
            // 
            this.timer.Interval = 500;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1306, 1156);
            this.Controls.Add(this.carLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.maskedBox);
            this.Controls.Add(this.thresholdedBox);
            this.Controls.Add(this.videoPlayer);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Computer Vision Bootcamp";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.thresholdedBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maskedBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Accord.Controls.VideoSourcePlayer videoPlayer;
        private System.Windows.Forms.PictureBox thresholdedBox;
        private System.Windows.Forms.PictureBox maskedBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label carLabel;
        private System.Windows.Forms.Timer timer;
    }
}

