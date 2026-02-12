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
            PBVideoTimeline.MouseClick += PBVideoTimeline_MouseClick;
            PBVideoTimeline.MouseMove += PBVideoTimeline_MouseMove;
            // 
            // LPlayButton
            // 
            LPlayButton.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            LPlayButton.Enabled = false;
            LPlayButton.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LPlayButton.Location = new Point(150, 25);
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
            LPauseButton.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            LPauseButton.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LPauseButton.Location = new Point(150, 25);
            LPauseButton.Margin = new Padding(0);
            LPauseButton.Name = "LPauseButton";
            LPauseButton.Size = new Size(32, 43);
            LPauseButton.TabIndex = 2;
            LPauseButton.Text = "⏸";
            LPauseButton.TextAlign = ContentAlignment.MiddleCenter;
            LPauseButton.MouseUp += PauseButton_MouseUp;
            // 
            // LMuteButton
            // 
            LMuteButton.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            LMuteButton.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LMuteButton.Location = new Point(228, 25);
            LMuteButton.Margin = new Padding(0);
            LMuteButton.Name = "LMuteButton";
            LMuteButton.Size = new Size(32, 43);
            LMuteButton.TabIndex = 2;
            LMuteButton.Text = "🔇";
            LMuteButton.TextAlign = ContentAlignment.MiddleCenter;
            LMuteButton.MouseUp += MuteButton_MouseUp;
            // 
            // LUnMuteButton
            // 
            LUnMuteButton.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            LUnMuteButton.Enabled = false;
            LUnMuteButton.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LUnMuteButton.Location = new Point(228, 25);
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
            // VideoPlaybackTimeline
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaptionText;
            Controls.Add(LVideoTime);
            Controls.Add(PBVideoTimeline);
            Controls.Add(LPlayButton);
            Controls.Add(LPauseButton);
            Controls.Add(LUnMuteButton);
            Controls.Add(LMuteButton);
            ForeColor = SystemColors.Control;
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
    }
}
