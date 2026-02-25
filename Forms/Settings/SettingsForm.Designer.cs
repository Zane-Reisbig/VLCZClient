namespace WINFORMS_VLCClient.Settings
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            label2 = new Label();
            CBUseSubsIfAvailable = new CheckBox();
            TBSubtitleDefaultLanguage = new TextBox();
            panel1 = new Panel();
            BClose = new Button();
            BReset = new Button();
            label1 = new Label();
            label3 = new Label();
            panel2 = new Panel();
            TBAudioDefaultLanguage = new TextBox();
            label4 = new Label();
            TBSubtitleBlacklist = new TextBox();
            label5 = new Label();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(3, 28);
            label2.Name = "label2";
            label2.Size = new Size(143, 15);
            label2.TabIndex = 0;
            label2.Text = "Subtitle Default Language";
            // 
            // CBUseSubsIfAvailable
            // 
            CBUseSubsIfAvailable.AutoSize = true;
            CBUseSubsIfAvailable.Location = new Point(2, 7);
            CBUseSubsIfAvailable.Name = "CBUseSubsIfAvailable";
            CBUseSubsIfAvailable.RightToLeft = RightToLeft.Yes;
            CBUseSubsIfAvailable.Size = new Size(145, 19);
            CBUseSubsIfAvailable.TabIndex = 1;
            CBUseSubsIfAvailable.Text = "-  Use Subs if Available";
            CBUseSubsIfAvailable.UseVisualStyleBackColor = true;
            // 
            // TBSubtitleDefaultLanguage
            // 
            TBSubtitleDefaultLanguage.Location = new Point(8, 46);
            TBSubtitleDefaultLanguage.Name = "TBSubtitleDefaultLanguage";
            TBSubtitleDefaultLanguage.Size = new Size(207, 23);
            TBSubtitleDefaultLanguage.TabIndex = 2;
            // 
            // panel1
            // 
            panel1.BorderStyle = BorderStyle.Fixed3D;
            panel1.Controls.Add(TBSubtitleBlacklist);
            panel1.Controls.Add(label5);
            panel1.Controls.Add(CBUseSubsIfAvailable);
            panel1.Controls.Add(TBSubtitleDefaultLanguage);
            panel1.Controls.Add(label2);
            panel1.Location = new Point(8, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(226, 120);
            panel1.TabIndex = 3;
            // 
            // BClose
            // 
            BClose.Location = new Point(8, 248);
            BClose.Name = "BClose";
            BClose.Size = new Size(118, 23);
            BClose.TabIndex = 4;
            BClose.Text = "Save and Close";
            BClose.UseVisualStyleBackColor = true;
            BClose.Click += Close_Click;
            // 
            // BReset
            // 
            BReset.Location = new Point(147, 248);
            BReset.Name = "BReset";
            BReset.Size = new Size(87, 23);
            BReset.TabIndex = 5;
            BReset.Text = "Reset";
            BReset.UseVisualStyleBackColor = true;
            BReset.Click += Reset_Click;
            // 
            // label1
            // 
            label1.BackColor = Color.Black;
            label1.Location = new Point(136, 250);
            label1.Name = "label1";
            label1.Size = new Size(2, 20);
            label1.TabIndex = 6;
            // 
            // label3
            // 
            label3.BackColor = Color.Black;
            label3.Location = new Point(6, 239);
            label3.Name = "label3";
            label3.Size = new Size(230, 2);
            label3.TabIndex = 6;
            // 
            // panel2
            // 
            panel2.BorderStyle = BorderStyle.Fixed3D;
            panel2.Controls.Add(TBAudioDefaultLanguage);
            panel2.Controls.Add(label4);
            panel2.Location = new Point(8, 138);
            panel2.Name = "panel2";
            panel2.Size = new Size(226, 63);
            panel2.TabIndex = 7;
            // 
            // TBAudioDefaultLanguage
            // 
            TBAudioDefaultLanguage.Location = new Point(8, 26);
            TBAudioDefaultLanguage.Name = "TBAudioDefaultLanguage";
            TBAudioDefaultLanguage.Size = new Size(207, 23);
            TBAudioDefaultLanguage.TabIndex = 2;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(4, 8);
            label4.Name = "label4";
            label4.Size = new Size(135, 15);
            label4.TabIndex = 0;
            label4.Text = "Audio Default Language";
            // 
            // TBSubtitleBlacklist
            // 
            TBSubtitleBlacklist.Location = new Point(8, 90);
            TBSubtitleBlacklist.Name = "TBSubtitleBlacklist";
            TBSubtitleBlacklist.Size = new Size(207, 23);
            TBSubtitleBlacklist.TabIndex = 4;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(3, 72);
            label5.Name = "label5";
            label5.Size = new Size(142, 15);
            label5.TabIndex = 3;
            label5.Text = "Subtitle Keyword Blacklist";
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(241, 278);
            Controls.Add(panel2);
            Controls.Add(label3);
            Controls.Add(label1);
            Controls.Add(BReset);
            Controls.Add(BClose);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "SettingsForm";
            Text = "Settings";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label label2;
        private CheckBox CBUseSubsIfAvailable;
        private TextBox TBSubtitleDefaultLanguage;
        private Panel panel1;
        private Button BClose;
        private Button BReset;
        private Label label1;
        private Label label3;
        private Panel panel2;
        private TextBox TBAudioDefaultLanguage;
        private Label label4;
        private TextBox TBSubtitleBlacklist;
        private Label label5;
    }
}