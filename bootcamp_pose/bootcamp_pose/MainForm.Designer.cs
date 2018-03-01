namespace Bootcamp.CompVis.Pose
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
            this.label1 = new System.Windows.Forms.Label();
            this.leftRightAngle = new System.Windows.Forms.TrackBar();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.videoPlayer = new Accord.Controls.VideoSourcePlayer();
            this.upDownAngle = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.rotationAngle = new System.Windows.Forms.TrackBar();
            this.label5 = new System.Windows.Forms.Label();
            this.pausedLabel = new System.Windows.Forms.Label();
            this.tvPicture = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.leftRightAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.upDownAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotationAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(68, 1006);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(181, 40);
            this.label1.TabIndex = 2;
            this.label1.Text = "Left/Right:";
            // 
            // leftRightAngle
            // 
            this.leftRightAngle.Enabled = false;
            this.leftRightAngle.Location = new System.Drawing.Point(249, 1006);
            this.leftRightAngle.Maximum = 45;
            this.leftRightAngle.Minimum = -45;
            this.leftRightAngle.Name = "leftRightAngle";
            this.leftRightAngle.Size = new System.Drawing.Size(304, 101);
            this.leftRightAngle.TabIndex = 5;
            this.leftRightAngle.TickFrequency = 25;
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.White;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(669, 937);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(192, 36);
            this.label3.TabIndex = 7;
            this.label3.Text = "Camera Feed";
            // 
            // videoPlayer
            // 
            this.videoPlayer.Location = new System.Drawing.Point(652, 927);
            this.videoPlayer.Name = "videoPlayer";
            this.videoPlayer.Size = new System.Drawing.Size(640, 480);
            this.videoPlayer.TabIndex = 12;
            this.videoPlayer.Text = "videoSourcePlayer1";
            this.videoPlayer.VideoSource = null;
            this.videoPlayer.NewFrameReceived += new Accord.Video.NewFrameEventHandler(this.videoPlayer_NewFrameReceived);
            // 
            // upDownAngle
            // 
            this.upDownAngle.Enabled = false;
            this.upDownAngle.Location = new System.Drawing.Point(249, 1096);
            this.upDownAngle.Maximum = 45;
            this.upDownAngle.Minimum = -45;
            this.upDownAngle.Name = "upDownAngle";
            this.upDownAngle.Size = new System.Drawing.Size(304, 101);
            this.upDownAngle.TabIndex = 14;
            this.upDownAngle.TickFrequency = 25;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(68, 1096);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(175, 40);
            this.label4.TabIndex = 13;
            this.label4.Text = "Up/Down:";
            // 
            // rotationAngle
            // 
            this.rotationAngle.Enabled = false;
            this.rotationAngle.Location = new System.Drawing.Point(249, 1184);
            this.rotationAngle.Maximum = 45;
            this.rotationAngle.Minimum = -45;
            this.rotationAngle.Name = "rotationAngle";
            this.rotationAngle.Size = new System.Drawing.Size(304, 101);
            this.rotationAngle.TabIndex = 16;
            this.rotationAngle.TickFrequency = 25;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(68, 1184);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(161, 40);
            this.label5.TabIndex = 15;
            this.label5.Text = "Rotation:";
            // 
            // pausedLabel
            // 
            this.pausedLabel.AutoSize = true;
            this.pausedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pausedLabel.ForeColor = System.Drawing.Color.Lime;
            this.pausedLabel.Location = new System.Drawing.Point(527, 590);
            this.pausedLabel.Name = "pausedLabel";
            this.pausedLabel.Size = new System.Drawing.Size(249, 63);
            this.pausedLabel.TabIndex = 17;
            this.pausedLabel.Text = "PAUSED";
            this.pausedLabel.Visible = false;
            // 
            // tvPicture
            // 
            this.tvPicture.Location = new System.Drawing.Point(12, 12);
            this.tvPicture.Name = "tvPicture";
            this.tvPicture.Size = new System.Drawing.Size(1280, 905);
            this.tvPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.tvPicture.TabIndex = 18;
            this.tvPicture.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1308, 1420);
            this.Controls.Add(this.pausedLabel);
            this.Controls.Add(this.rotationAngle);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.upDownAngle);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.leftRightAngle);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.videoPlayer);
            this.Controls.Add(this.tvPicture);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Computer Vision Bootcamp";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.leftRightAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.upDownAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotationAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvPicture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar leftRightAngle;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label label3;
        private Accord.Controls.VideoSourcePlayer videoPlayer;
        private System.Windows.Forms.TrackBar upDownAngle;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TrackBar rotationAngle;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label pausedLabel;
        private System.Windows.Forms.PictureBox tvPicture;
    }
}

