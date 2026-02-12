namespace WINFORMS_VLCClient.Forms
{
    partial class Viewer
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
            VVMainView = new LibVLCSharp.WinForms.VideoView();
            VPTMainTimeline = new WINFORMS_VLCClient.Controls.VideoPlaybackTimeline();
            ((System.ComponentModel.ISupportInitialize)VVMainView).BeginInit();
            SuspendLayout();
            // 
            // VVMainView
            // 
            VVMainView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            VVMainView.BackColor = Color.Black;
            VVMainView.Location = new Point(-1, -1);
            VVMainView.MediaPlayer = null;
            VVMainView.Name = "VVMainView";
            VVMainView.Size = new Size(787, 563);
            VVMainView.TabIndex = 0;
            VVMainView.Text = "videoView1";
            // 
            // VPTMainTimeline
            // 
            VPTMainTimeline.BackColor = SystemColors.ActiveCaptionText;
            VPTMainTimeline.ForeColor = SystemColors.Control;
            VPTMainTimeline.Location = new Point(249, 485);
            VPTMainTimeline.Name = "VPTMainTimeline";
            VPTMainTimeline.Size = new Size(268, 64);
            VPTMainTimeline.TabIndex = 1;
            // 
            // Viewer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 561);
            Controls.Add(VPTMainTimeline);
            Controls.Add(VVMainView);
            Name = "Viewer";
            Text = "Viewer";
            ((System.ComponentModel.ISupportInitialize)VVMainView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private LibVLCSharp.WinForms.VideoView VVMainView;
        private Controls.VideoPlaybackTimeline VPTMainTimeline;
    }
}