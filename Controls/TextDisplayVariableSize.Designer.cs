namespace WINFORMS_VLCClient.Forms
{
    partial class TextDisplayVariableSize
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
            LTextDisplay = new Label();
            SuspendLayout();
            // 
            // LTextDisplay
            // 
            LTextDisplay.Dock = DockStyle.Fill;
            LTextDisplay.Font = new Font("Segoe UI", 46F);
            LTextDisplay.Location = new Point(0, 0);
            LTextDisplay.Margin = new Padding(0);
            LTextDisplay.Name = "LTextDisplay";
            LTextDisplay.Size = new Size(600, 90);
            LTextDisplay.TabIndex = 0;
            LTextDisplay.Text = "<YOUR TEXT HERE>";
            LTextDisplay.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // TextDisplayVariableSize
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            Controls.Add(LTextDisplay);
            ForeColor = Color.White;
            Name = "TextDisplayVariableSize";
            Size = new Size(600, 90);
            ResumeLayout(false);
        }

        #endregion

        private Label LTextDisplay;
    }
}
