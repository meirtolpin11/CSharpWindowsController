namespace Meir_Tolpin_Project.Stream
{
    partial class PicStream
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
            this.streamPic = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.streamPic)).BeginInit();
            this.SuspendLayout();
            // 
            // streamPic
            // 
            this.streamPic.Location = new System.Drawing.Point(12, 12);
            this.streamPic.Name = "streamPic";
            this.streamPic.Size = new System.Drawing.Size(853, 409);
            this.streamPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.streamPic.TabIndex = 1;
            this.streamPic.TabStop = false;
            this.streamPic.Click += new System.EventHandler(this.streamPic_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // PicStream
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(869, 425);
            this.Controls.Add(this.streamPic);
            this.Name = "PicStream";
            this.Text = "PicStream";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PicStream_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PicStream_FormClosed);
            this.Load += new System.EventHandler(this.PicStream_Load);
            ((System.ComponentModel.ISupportInitialize)(this.streamPic)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.PictureBox streamPic;
    }
}