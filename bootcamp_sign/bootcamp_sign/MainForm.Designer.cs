namespace Bootcamp.CompVis.TrafficSign
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
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.thresholdBox = new System.Windows.Forms.PictureBox();
            this.trafficSignBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.thresholdBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trafficSignBox)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(12, 12);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(1200, 700);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            // 
            // thresholdBox
            // 
            this.thresholdBox.Location = new System.Drawing.Point(12, 718);
            this.thresholdBox.Name = "thresholdBox";
            this.thresholdBox.Size = new System.Drawing.Size(600, 350);
            this.thresholdBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.thresholdBox.TabIndex = 1;
            this.thresholdBox.TabStop = false;
            // 
            // trafficSignBox
            // 
            this.trafficSignBox.BackColor = System.Drawing.Color.Black;
            this.trafficSignBox.Location = new System.Drawing.Point(618, 718);
            this.trafficSignBox.Name = "trafficSignBox";
            this.trafficSignBox.Size = new System.Drawing.Size(592, 350);
            this.trafficSignBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.trafficSignBox.TabIndex = 2;
            this.trafficSignBox.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1222, 1080);
            this.Controls.Add(this.trafficSignBox);
            this.Controls.Add(this.thresholdBox);
            this.Controls.Add(this.pictureBox);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Computer Vision Bootcamp";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.thresholdBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trafficSignBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.PictureBox thresholdBox;
        private System.Windows.Forms.PictureBox trafficSignBox;
    }
}

