namespace Bootcamp.CompVis.Eyes
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
            this.eyeBox = new Accord.Controls.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureButton = new System.Windows.Forms.Button();
            this.eyeState = new System.Windows.Forms.TrackBar();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.thumbnailBox = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.eyeBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.eyeState)).BeginInit();
            this.SuspendLayout();
            // 
            // videoPlayer
            // 
            this.videoPlayer.Location = new System.Drawing.Point(12, 12);
            this.videoPlayer.Name = "videoPlayer";
            this.videoPlayer.Size = new System.Drawing.Size(1280, 900);
            this.videoPlayer.TabIndex = 0;
            this.videoPlayer.Text = "videoSourcePlayer1";
            this.videoPlayer.VideoSource = null;
            this.videoPlayer.NewFrame += new Accord.Controls.VideoSourcePlayer.NewFrameHandler(this.videoPlayer_NewFrame);
            // 
            // eyeBox
            // 
            this.eyeBox.Image = null;
            this.eyeBox.Location = new System.Drawing.Point(662, 918);
            this.eyeBox.Name = "eyeBox";
            this.eyeBox.Size = new System.Drawing.Size(630, 300);
            this.eyeBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.eyeBox.TabIndex = 1;
            this.eyeBox.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(34, 961);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(177, 40);
            this.label1.TabIndex = 2;
            this.label1.Text = "Eye state:";
            // 
            // pictureButton
            // 
            this.pictureButton.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.pictureButton.FlatAppearance.BorderSize = 2;
            this.pictureButton.Location = new System.Drawing.Point(233, 1095);
            this.pictureButton.Name = "pictureButton";
            this.pictureButton.Size = new System.Drawing.Size(356, 86);
            this.pictureButton.TabIndex = 4;
            this.pictureButton.Text = "Take Picture";
            this.pictureButton.UseVisualStyleBackColor = true;
            this.pictureButton.Click += new System.EventHandler(this.pictureButton_Click);
            // 
            // eyeState
            // 
            this.eyeState.Enabled = false;
            this.eyeState.Location = new System.Drawing.Point(233, 961);
            this.eyeState.Maximum = 100;
            this.eyeState.Name = "eyeState";
            this.eyeState.Size = new System.Drawing.Size(356, 101);
            this.eyeState.TabIndex = 5;
            this.eyeState.TickFrequency = 25;
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(21, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(192, 36);
            this.label2.TabIndex = 6;
            this.label2.Text = "Camera Feed";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.White;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(669, 927);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(199, 36);
            this.label3.TabIndex = 7;
            this.label3.Text = "Eye Detection";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(216, 1017);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 29);
            this.label4.TabIndex = 8;
            this.label4.Text = "closed";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(521, 1017);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(93, 29);
            this.label5.TabIndex = 9;
            this.label5.Text = "bulging";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(372, 1017);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(87, 29);
            this.label6.TabIndex = 10;
            this.label6.Text = "normal";
            // 
            // thumbnailBox
            // 
            this.thumbnailBox.Location = new System.Drawing.Point(1298, 11);
            this.thumbnailBox.Name = "thumbnailBox";
            this.thumbnailBox.Size = new System.Drawing.Size(320, 1206);
            this.thumbnailBox.TabIndex = 11;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1628, 1229);
            this.Controls.Add(this.thumbnailBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.eyeState);
            this.Controls.Add(this.pictureButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.eyeBox);
            this.Controls.Add(this.videoPlayer);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Computer Vision Bootcamp";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.eyeBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.eyeState)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Accord.Controls.VideoSourcePlayer videoPlayer;
        private Accord.Controls.PictureBox eyeBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button pictureButton;
        private System.Windows.Forms.TrackBar eyeState;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.FlowLayoutPanel thumbnailBox;
    }
}

