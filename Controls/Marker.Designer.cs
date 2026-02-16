namespace WINFORMS_VLCClient.Controls
{
    partial class Marker
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
            BMarkOne = new Button();
            BMarkTwo = new Button();
            LMarkOneTimeStamp = new Label();
            LMarkTwoTimeStamp = new Label();
            label3 = new Label();
            LTotalTime = new Label();
            LExitButton = new Label();
            SuspendLayout();
            // 
            // BMarkOne
            // 
            BMarkOne.BackColor = SystemColors.ControlDark;
            BMarkOne.FlatAppearance.BorderSize = 0;
            BMarkOne.FlatStyle = FlatStyle.Flat;
            BMarkOne.Location = new Point(3, 3);
            BMarkOne.Name = "BMarkOne";
            BMarkOne.Size = new Size(113, 23);
            BMarkOne.TabIndex = 0;
            BMarkOne.Text = "Mark One";
            BMarkOne.UseVisualStyleBackColor = false;
            // 
            // BMarkTwo
            // 
            BMarkTwo.BackColor = SystemColors.ControlDark;
            BMarkTwo.FlatAppearance.BorderSize = 0;
            BMarkTwo.FlatStyle = FlatStyle.Flat;
            BMarkTwo.Location = new Point(122, 3);
            BMarkTwo.Name = "BMarkTwo";
            BMarkTwo.Size = new Size(113, 23);
            BMarkTwo.TabIndex = 0;
            BMarkTwo.Text = "Mark Two";
            BMarkTwo.UseVisualStyleBackColor = false;
            // 
            // LMarkOneTimeStamp
            // 
            LMarkOneTimeStamp.AutoSize = true;
            LMarkOneTimeStamp.ForeColor = SystemColors.Control;
            LMarkOneTimeStamp.Location = new Point(27, 29);
            LMarkOneTimeStamp.Name = "LMarkOneTimeStamp";
            LMarkOneTimeStamp.Size = new Size(64, 15);
            LMarkOneTimeStamp.TabIndex = 1;
            LMarkOneTimeStamp.Text = "00:00:00:00";
            // 
            // LMarkTwoTimeStamp
            // 
            LMarkTwoTimeStamp.AutoSize = true;
            LMarkTwoTimeStamp.ForeColor = SystemColors.Control;
            LMarkTwoTimeStamp.Location = new Point(145, 29);
            LMarkTwoTimeStamp.Name = "LMarkTwoTimeStamp";
            LMarkTwoTimeStamp.Size = new Size(64, 15);
            LMarkTwoTimeStamp.TabIndex = 2;
            LMarkTwoTimeStamp.Text = "00:00:00:00";
            // 
            // label3
            // 
            label3.BackColor = SystemColors.Control;
            label3.ForeColor = SystemColors.ActiveCaptionText;
            label3.Location = new Point(65, 47);
            label3.Name = "label3";
            label3.Size = new Size(100, 2);
            label3.TabIndex = 3;
            // 
            // LTotalTime
            // 
            LTotalTime.AutoSize = true;
            LTotalTime.ForeColor = SystemColors.Control;
            LTotalTime.Location = new Point(38, 54);
            LTotalTime.Name = "LTotalTime";
            LTotalTime.Size = new Size(157, 15);
            LTotalTime.TabIndex = 4;
            LTotalTime.Text = "Total Intro Time - 00:00:00:00";
            // 
            // LExitButton
            // 
            LExitButton.AutoSize = true;
            LExitButton.BackColor = SystemColors.ControlDark;
            LExitButton.ForeColor = SystemColors.ActiveCaptionText;
            LExitButton.Location = new Point(221, 55);
            LExitButton.Name = "LExitButton";
            LExitButton.Size = new Size(14, 15);
            LExitButton.TabIndex = 4;
            LExitButton.Text = "X";
            LExitButton.Click += LExitButton_Click;
            // 
            // Marker
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaptionText;
            Controls.Add(LExitButton);
            Controls.Add(LTotalTime);
            Controls.Add(label3);
            Controls.Add(LMarkTwoTimeStamp);
            Controls.Add(LMarkOneTimeStamp);
            Controls.Add(BMarkTwo);
            Controls.Add(BMarkOne);
            Name = "Marker";
            Size = new Size(240, 73);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BMarkOne;
        private Button BMarkTwo;
        private Label LMarkOneTimeStamp;
        private Label LMarkTwoTimeStamp;
        private Label label3;
        private Label LTotalTime;
        private Label LExitButton;
    }
}
