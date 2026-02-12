namespace WINFORMS_VLCClient.Controls
{
    partial class MediaInfomationPanel
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
            label1 = new Label();
            TBMediaPath = new TextBox();
            label2 = new Label();
            TBTimestamp = new TextBox();
            label3 = new Label();
            TBEpisodeTitle = new TextBox();
            label4 = new Label();
            label5 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Location = new Point(9, 65);
            label1.Name = "label1";
            label1.Size = new Size(75, 15);
            label1.TabIndex = 0;
            label1.Text = "Path";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // TBMediaPath
            // 
            TBMediaPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            TBMediaPath.Font = new Font("Segoe UI", 12F);
            TBMediaPath.Location = new Point(86, 56);
            TBMediaPath.Name = "TBMediaPath";
            TBMediaPath.ReadOnly = true;
            TBMediaPath.Size = new Size(336, 29);
            TBMediaPath.TabIndex = 1;
            // 
            // label2
            // 
            label2.Location = new Point(9, 10);
            label2.Name = "label2";
            label2.Size = new Size(75, 15);
            label2.TabIndex = 0;
            label2.Text = "Episode Title";
            label2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // TBTimestamp
            // 
            TBTimestamp.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            TBTimestamp.BackColor = SystemColors.WindowFrame;
            TBTimestamp.Font = new Font("Segoe UI", 12F);
            TBTimestamp.ForeColor = SystemColors.Window;
            TBTimestamp.Location = new Point(86, 30);
            TBTimestamp.Name = "TBTimestamp";
            TBTimestamp.ReadOnly = true;
            TBTimestamp.Size = new Size(336, 29);
            TBTimestamp.TabIndex = 1;
            // 
            // label3
            // 
            label3.Location = new Point(9, 38);
            label3.Name = "label3";
            label3.Size = new Size(75, 15);
            label3.TabIndex = 0;
            label3.Text = "Timestamp";
            label3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // TBEpisodeTitle
            // 
            TBEpisodeTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            TBEpisodeTitle.Font = new Font("Segoe UI", 12F);
            TBEpisodeTitle.Location = new Point(86, 3);
            TBEpisodeTitle.Name = "TBEpisodeTitle";
            TBEpisodeTitle.ReadOnly = true;
            TBEpisodeTitle.Size = new Size(336, 29);
            TBEpisodeTitle.TabIndex = 1;
            // 
            // label4
            // 
            label4.BackColor = SystemColors.ActiveCaptionText;
            label4.Location = new Point(15, 31);
            label4.Name = "label4";
            label4.Size = new Size(50, 2);
            label4.TabIndex = 2;
            // 
            // label5
            // 
            label5.BackColor = SystemColors.ActiveCaptionText;
            label5.Location = new Point(15, 59);
            label5.Name = "label5";
            label5.Size = new Size(50, 2);
            label5.TabIndex = 3;
            // 
            // MediaInfomationPanel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(TBEpisodeTitle);
            Controls.Add(TBMediaPath);
            Controls.Add(TBTimestamp);
            Name = "MediaInfomationPanel";
            Size = new Size(425, 88);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private TextBox TBTimestamp;
        private TextBox TBMediaPath;
        private TextBox TBEpisodeTitle;
        private Label label4;
        private Label label5;
    }
}
