namespace WINFORMS_VLCClient.Controls
{
    partial class VideoPlaybackTimeline
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            PBVideoTimeline = new ProgressBar();
            LPlayButton = new Label();
            LPauseButton = new Label();
            LMuteButton = new Label();
            LUnMuteButton = new Label();
            LVideoTime = new Label();
            LNextEpisode = new Label();
            LPreviousEpisode = new Label();
            SuspendLayout();
            // 
            // PBVideoTimeline
            // 
            PBVideoTimeline.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            PBVideoTimeline.ForeColor = Color.LightSalmon;
            PBVideoTimeline.Location = new Point(3, 3);
            PBVideoTimeline.Name = "PBVideoTimeline";
            PBVideoTimeline.Size = new Size(262, 28);
            PBVideoTimeline.TabIndex = 0;
            // 
            // LPlayButton
            // 
            LPlayButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            LPlayButton.Enabled = false;
            LPlayButton.Font = new Font("Segoe UI", 18F);
            LPlayButton.Location = new Point(233, 33);
            LPlayButton.MaximumSize = new Size(32, 40);
            LPlayButton.Name = "LPlayButton";
            LPlayButton.Size = new Size(32, 40);
            LPlayButton.TabIndex = 1;
            LPlayButton.Text = "▶";
            LPlayButton.TextAlign = ContentAlignment.MiddleCenter;
            LPlayButton.Visible = false;
            LPlayButton.MouseUp += PauseButton_MouseUp;
            // 
            // LPauseButton
            // 
            LPauseButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            LPauseButton.Font = new Font("Segoe UI", 18F);
            LPauseButton.Location = new Point(233, 33);
            LPauseButton.Margin = new Padding(0);
            LPauseButton.MaximumSize = new Size(32, 43);
            LPauseButton.Name = "LPauseButton";
            LPauseButton.Size = new Size(32, 43);
            LPauseButton.TabIndex = 2;
            LPauseButton.Text = "⏸";
            LPauseButton.TextAlign = ContentAlignment.MiddleCenter;
            LPauseButton.MouseUp += PauseButton_MouseUp;
            // 
            // LMuteButton
            // 
            LMuteButton.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            LMuteButton.Font = new Font("Segoe UI", 15F);
            LMuteButton.Location = new Point(106, 39);
            LMuteButton.Margin = new Padding(0);
            LMuteButton.MaximumSize = new Size(32, 43);
            LMuteButton.Name = "LMuteButton";
            LMuteButton.Size = new Size(32, 43);
            LMuteButton.TabIndex = 2;
            LMuteButton.Text = "🔇";
            LMuteButton.TextAlign = ContentAlignment.MiddleCenter;
            LMuteButton.MouseUp += MuteButton_MouseUp;
            // 
            // LUnMuteButton
            // 
            LUnMuteButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            LUnMuteButton.Enabled = false;
            LUnMuteButton.Font = new Font("Segoe UI", 15F);
            LUnMuteButton.Location = new Point(106, 39);
            LUnMuteButton.Margin = new Padding(0);
            LUnMuteButton.Name = "LUnMuteButton";
            LUnMuteButton.Size = new Size(32, 43);
            LUnMuteButton.TabIndex = 3;
            LUnMuteButton.Text = "🔊";
            LUnMuteButton.TextAlign = ContentAlignment.MiddleCenter;
            LUnMuteButton.Visible = false;
            LUnMuteButton.MouseUp += MuteButton_MouseUp;
            // 
            // LVideoTime
            // 
            LVideoTime.AutoSize = true;
            LVideoTime.Location = new Point(6, 34);
            LVideoTime.Name = "LVideoTime";
            LVideoTime.Size = new Size(100, 15);
            LVideoTime.TabIndex = 4;
            LVideoTime.Text = "00:00:00 | 00:00:00";
            // 
            // LNextEpisode
            // 
            LNextEpisode.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            LNextEpisode.Font = new Font("Segoe UI", 18F);
            LNextEpisode.Location = new Point(196, 34);
            LNextEpisode.MaximumSize = new Size(32, 40);
            LNextEpisode.Name = "LNextEpisode";
            LNextEpisode.Size = new Size(32, 40);
            LNextEpisode.TabIndex = 1;
            LNextEpisode.Text = "⏭";
            LNextEpisode.TextAlign = ContentAlignment.MiddleCenter;
            LNextEpisode.Click += LNextEpisode_Click;
            // 
            // LPreviousEpisode
            // 
            LPreviousEpisode.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            LPreviousEpisode.Font = new Font("Segoe UI", 18F);
            LPreviousEpisode.Location = new Point(164, 34);
            LPreviousEpisode.MaximumSize = new Size(32, 40);
            LPreviousEpisode.Name = "LPreviousEpisode";
            LPreviousEpisode.Size = new Size(32, 40);
            LPreviousEpisode.TabIndex = 1;
            LPreviousEpisode.Text = "⏮";
            LPreviousEpisode.TextAlign = ContentAlignment.MiddleCenter;
            LPreviousEpisode.Click += LPreviousEpisode_Click;
            // 
            // VideoPlaybackTimeline
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaptionText;
            Controls.Add(LVideoTime);
            Controls.Add(PBVideoTimeline);
            Controls.Add(LPreviousEpisode);
            Controls.Add(LNextEpisode);
            Controls.Add(LPlayButton);
            Controls.Add(LPauseButton);
            Controls.Add(LMuteButton);
            Controls.Add(LUnMuteButton);
            ForeColor = SystemColors.Control;
            MinimumSize = new Size(268, 74);
            Name = "VideoPlaybackTimeline";
            Size = new Size(268, 74);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ProgressBar PBVideoTimeline;
        private Label LPlayButton;
        private Label LPauseButton;
        private Label LMuteButton;
        private Label LUnMuteButton;
        private Label LVideoTime;
        private Label LNextEpisode;
        private Label LPreviousEpisode;
    }
}
