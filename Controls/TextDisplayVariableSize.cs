using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WINFORMS_VLCClient.Viewer
{
    public enum TextDisplaySize
    {
        Small,
        Medium,
        Large,
    }

    public partial class TextDisplayVariableSize : UserControl
    {
        static Dictionary<TextDisplaySize, Size> Sizes = new()
        {
            { TextDisplaySize.Small, new(600, 90) },
            { TextDisplaySize.Medium, new(1000, 90) },
            { TextDisplaySize.Large, new(1500, 90) },
        };

        [Category("Appearance")]
        [Browsable(true)]
        [Description("The Text Displayed.")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public string TextDisplayContent
        {
            get => LTextDisplay.Text;
            set => LTextDisplay.Text = value;
        }

        public TextDisplayVariableSize()
        {
            InitializeComponent();
        }

        public void OnResizeHook(object? sender, EventArgs e)
        {
            if (sender is not Control userControl)
                return;

            var newWidth = userControl.Size.Width;

            if (newWidth > Sizes[TextDisplaySize.Large].Width)
            {
                SetSize(TextDisplaySize.Large);
                return;
            }

            if (newWidth > Sizes[TextDisplaySize.Medium].Width)
            {
                SetSize(TextDisplaySize.Medium);
                return;
            }

            if (newWidth > Sizes[TextDisplaySize.Small].Width)
            {
                SetSize(TextDisplaySize.Small);
                return;
            }
        }

        public void SetSize(TextDisplaySize size) => this.Size = Sizes[size];
    }
}
