namespace WINFORMS_VLCClient
{
    partial class Landing
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            BContinueLast = new Button();
            BWatchNew = new Button();
            label1 = new Label();
            pictureBox1 = new PictureBox();
            MIInformationPanel = new WINFORMS_VLCClient.Controls.MediaInfomationPanel();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // BContinueLast
            // 
            BContinueLast.Location = new Point(48, 12);
            BContinueLast.Name = "BContinueLast";
            BContinueLast.Size = new Size(137, 23);
            BContinueLast.TabIndex = 0;
            BContinueLast.Text = "Continue Watching";
            BContinueLast.UseVisualStyleBackColor = true;
            BContinueLast.Click += BContinueLast_Click;
            // 
            // BWatchNew
            // 
            BWatchNew.Location = new Point(48, 249);
            BWatchNew.Name = "BWatchNew";
            BWatchNew.Size = new Size(137, 23);
            BWatchNew.TabIndex = 0;
            BWatchNew.Text = "Watch Something Else";
            BWatchNew.UseVisualStyleBackColor = true;
            BWatchNew.Click += BWatchNew_Click;
            // 
            // label1
            // 
            label1.BackColor = SystemColors.ActiveCaptionText;
            label1.Location = new Point(19, 237);
            label1.Name = "label1";
            label1.Size = new Size(200, 2);
            label1.TabIndex = 2;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = SystemColors.ControlDark;
            pictureBox1.Location = new Point(12, 41);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(220, 85);
            pictureBox1.TabIndex = 4;
            pictureBox1.TabStop = false;
            // 
            // MIInformationPanel
            // 
            MIInformationPanel.Location = new Point(4, 132);
            MIInformationPanel.Name = "MIInformationPanel";
            MIInformationPanel.Size = new Size(228, 95);
            MIInformationPanel.TabIndex = 5;
            // 
            // Landing
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(241, 278);
            Controls.Add(MIInformationPanel);
            Controls.Add(pictureBox1);
            Controls.Add(label1);
            Controls.Add(BWatchNew);
            Controls.Add(BContinueLast);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Landing";
            Text = "ZPlayer";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button BContinueLast;
        private Button BWatchNew;
        private Label label1;
        private PictureBox pictureBox1;
        private Controls.MediaInfomationPanel MIInformationPanel;
    }
}
