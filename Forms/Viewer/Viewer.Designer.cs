namespace WINFORMS_VLCClient.Viewer
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
            components = new System.ComponentModel.Container();
            VVMainView = new LibVLCSharp.WinForms.VideoView();
            VPTMainTimeline = new WINFORMS_VLCClient.Controls.VideoPlaybackTimeline();
            TBVolumeBar = new TrackBar();
            BSkipIntro = new Button();
            BRecordIntro = new Button();
            MKRIntroSkip = new WINFORMS_VLCClient.Controls.Marker();
            PSkipIntroButtonContainer = new Panel();
            PSkipIntroFullContainer = new Panel();
            CMSLeftClickMenu = new ContextMenuStrip(components);
            TSMISubtitle = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)VVMainView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)TBVolumeBar).BeginInit();
            PSkipIntroButtonContainer.SuspendLayout();
            PSkipIntroFullContainer.SuspendLayout();
            CMSLeftClickMenu.SuspendLayout();
            SuspendLayout();
            // 
            // VVMainView
            // 
            VVMainView.AllowDrop = true;
            VVMainView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            VVMainView.BackColor = Color.Black;
            VVMainView.Location = new Point(-1, -1);
            VVMainView.MediaPlayer = null;
            VVMainView.Name = "VVMainView";
            VVMainView.Size = new Size(787, 562);
            VVMainView.TabIndex = 0;
            VVMainView.Text = "videoView1";
            VVMainView.DragDrop += VVMainView_DragDrop;
            VVMainView.DragEnter += VVMainView_DragEnter;
            VVMainView.MouseDoubleClick += VVMainView_DoubleClick;
            VVMainView.MouseUp += VVMainView_Click;
            // 
            // VPTMainTimeline
            // 
            VPTMainTimeline.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            VPTMainTimeline.BackColor = SystemColors.ActiveCaptionText;
            VPTMainTimeline.ForeColor = SystemColors.Control;
            VPTMainTimeline.Location = new Point(301, 475);
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
            TBVolumeBar.Location = new Point(309, 528);
            TBVolumeBar.Maximum = 101;
            TBVolumeBar.Minimum = -1;
            TBVolumeBar.Name = "TBVolumeBar";
            TBVolumeBar.Size = new Size(93, 21);
            TBVolumeBar.TabIndex = 2;
            TBVolumeBar.TabStop = false;
            TBVolumeBar.TickFrequency = 100;
            TBVolumeBar.TickStyle = TickStyle.None;
            // 
            // BSkipIntro
            // 
            BSkipIntro.BackColor = SystemColors.ControlDark;
            BSkipIntro.FlatAppearance.BorderSize = 0;
            BSkipIntro.FlatStyle = FlatStyle.Flat;
            BSkipIntro.ForeColor = SystemColors.ControlText;
            BSkipIntro.Location = new Point(0, 1);
            BSkipIntro.Name = "BSkipIntro";
            BSkipIntro.Size = new Size(99, 23);
            BSkipIntro.TabIndex = 3;
            BSkipIntro.Text = "Skip Intro";
            BSkipIntro.UseVisualStyleBackColor = false;
            // 
            // BRecordIntro
            // 
            BRecordIntro.BackColor = SystemColors.ControlDark;
            BRecordIntro.FlatAppearance.BorderSize = 0;
            BRecordIntro.FlatStyle = FlatStyle.Flat;
            BRecordIntro.ForeColor = SystemColors.ControlText;
            BRecordIntro.Location = new Point(0, 30);
            BRecordIntro.Name = "BRecordIntro";
            BRecordIntro.Size = new Size(99, 23);
            BRecordIntro.TabIndex = 4;
            BRecordIntro.Text = "Set New Intro";
            BRecordIntro.UseVisualStyleBackColor = false;
            // 
            // MKRIntroSkip
            // 
            MKRIntroSkip.BackColor = SystemColors.ActiveCaptionText;
            MKRIntroSkip.Location = new Point(5, 0);
            MKRIntroSkip.Name = "MKRIntroSkip";
            MKRIntroSkip.Size = new Size(240, 73);
            MKRIntroSkip.TabIndex = 5;
            // 
            // PSkipIntroButtonContainer
            // 
            PSkipIntroButtonContainer.BackColor = SystemColors.ActiveCaptionText;
            PSkipIntroButtonContainer.Controls.Add(BSkipIntro);
            PSkipIntroButtonContainer.Controls.Add(BRecordIntro);
            PSkipIntroButtonContainer.ForeColor = Color.Transparent;
            PSkipIntroButtonContainer.Location = new Point(1, 2);
            PSkipIntroButtonContainer.Name = "PSkipIntroButtonContainer";
            PSkipIntroButtonContainer.Size = new Size(102, 56);
            PSkipIntroButtonContainer.TabIndex = 6;
            PSkipIntroButtonContainer.Visible = false;
            // 
            // PSkipIntroFullContainer
            // 
            PSkipIntroFullContainer.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            PSkipIntroFullContainer.BackColor = SystemColors.ActiveCaptionText;
            PSkipIntroFullContainer.Controls.Add(MKRIntroSkip);
            PSkipIntroFullContainer.Controls.Add(PSkipIntroButtonContainer);
            PSkipIntroFullContainer.ForeColor = Color.Transparent;
            PSkipIntroFullContainer.Location = new Point(28, 475);
            PSkipIntroFullContainer.Name = "PSkipIntroFullContainer";
            PSkipIntroFullContainer.Size = new Size(249, 74);
            PSkipIntroFullContainer.TabIndex = 6;
            // 
            // CMSLeftClickMenu
            // 
            CMSLeftClickMenu.Items.AddRange(new ToolStripItem[] { TSMISubtitle });
            CMSLeftClickMenu.Name = "CMSLeftClickMenu";
            CMSLeftClickMenu.Size = new Size(120, 26);
            CMSLeftClickMenu.Tag = "Subtitles";
            // 
            // TSMISubtitle
            // 
            TSMISubtitle.Name = "TSMISubtitle";
            TSMISubtitle.Size = new Size(119, 22);
            TSMISubtitle.Text = "Subtitles";
            // 
            // Viewer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(784, 561);
            Controls.Add(PSkipIntroFullContainer);
            Controls.Add(TBVolumeBar);
            Controls.Add(VPTMainTimeline);
            Controls.Add(VVMainView);
            DoubleBuffered = true;
            Name = "Viewer";
            Text = "Viewer";
            ((System.ComponentModel.ISupportInitialize)VVMainView).EndInit();
            ((System.ComponentModel.ISupportInitialize)TBVolumeBar).EndInit();
            PSkipIntroButtonContainer.ResumeLayout(false);
            PSkipIntroFullContainer.ResumeLayout(false);
            CMSLeftClickMenu.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private LibVLCSharp.WinForms.VideoView VVMainView;
        private Controls.VideoPlaybackTimeline VPTMainTimeline;
        private TrackBar TBVolumeBar;
        private Button BSkipIntro;
        private Button BRecordIntro;
        private Controls.Marker MKRIntroSkip;
        private Panel PSkipIntroButtonContainer;
        private Panel PSkipIntroFullContainer;
        private ContextMenuStrip CMSLeftClickMenu;
        private ToolStripMenuItem TSMISubtitle;
    }
}