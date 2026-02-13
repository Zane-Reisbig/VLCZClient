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
            TBVolumeBar = new TrackBar();
            ((System.ComponentModel.ISupportInitialize)VVMainView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)TBVolumeBar).BeginInit();
            SuspendLayout();
            // 
            // VVMainView
            // 
            VVMainView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            VVMainView.BackColor = Color.Black;
            VVMainView.Location = new Point(-2, -1);
            VVMainView.MediaPlayer = null;
            VVMainView.Name = "VVMainView";
            VVMainView.Size = new Size(787, 563);
            VVMainView.TabIndex = 0;
            VVMainView.Text = "videoView1";
            // 
            // VPTMainTimeline
            // 
            VPTMainTimeline.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            VPTMainTimeline.BackColor = SystemColors.ActiveCaptionText;
            VPTMainTimeline.ForeColor = SystemColors.Control;
            VPTMainTimeline.Location = new Point(249, 485);
            VPTMainTimeline.MinimumSize = new Size(268, 74);
            VPTMainTimeline.Name = "VPTMainTimeline";
            VPTMainTimeline.Size = new Size(268, 74);
            VPTMainTimeline.TabIndex = 1;
            // 
            // TBVolumeBar
            // 
            TBVolumeBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            TBVolumeBar.AutoSize = false;
            TBVolumeBar.BackColor = SystemColors.ActiveCaptionText;
            TBVolumeBar.Location = new Point(257, 536);
            TBVolumeBar.Maximum = 101;
            TBVolumeBar.Minimum = -1;
            TBVolumeBar.Name = "TBVolumeBar";
            TBVolumeBar.Size = new Size(93, 21);
            TBVolumeBar.TabIndex = 2;
            TBVolumeBar.TickFrequency = 100;
            TBVolumeBar.TickStyle = TickStyle.None;
            // 
            // Viewer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 561);
            Controls.Add(TBVolumeBar);
            Controls.Add(VPTMainTimeline);
            Controls.Add(VVMainView);
            Name = "Viewer";
            Text = "Viewer";
            ((System.ComponentModel.ISupportInitialize)VVMainView).EndInit();
            ((System.ComponentModel.ISupportInitialize)TBVolumeBar).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private LibVLCSharp.WinForms.VideoView VVMainView;
        private Controls.VideoPlaybackTimeline VPTMainTimeline;
        private TrackBar TBVolumeBar;
    }
}