using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ClientLib.STD.StandardDefinitions;

namespace WINFORMS_VLCClient.Controls
{
    public enum MarkButton
    {
        One,
        Two,
    }

    public partial class Marker : UserControl
    {
        [DefaultValue(true)]
        public bool RejectNegative { get; set; } = true;
        public bool? WasCanceled { get; private set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Timestamp? MarkOne
        {
            get => _markOne;
            set
            {
                _markOne = value;

                if (value != null)
                    LMarkOneTimeStamp.Text = value.GetFormat();
                else
                    LMarkOneTimeStamp.Text = "00:00:00:00";
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Timestamp? MarkTwo
        {
            get => _markTwo;
            set
            {
                _markTwo = value;

                if (value != null)
                    LMarkTwoTimeStamp.Text = value.GetFormat();
                else
                    LMarkTwoTimeStamp.Text = "00:00:00:00";
            }
        }

        public void SetTotalTextTime(string to) => LTotalTime.Text = $"Total Intro Time - {to}";

        public EventHandler? ExitButtonClicked;
        public EventHandler? ConfirmButtonClicked;
        public EventHandler? MarkOneButtonClicked;
        public EventHandler? MarkTwoButtonClicked;
        public EventHandler<MarkerEventArgs>? MarkConfirmed;
        public Func<Timestamp?> TimestampGetter = () => null;

        void ComputeAndSetTotalTimeText() =>
            SetTotalTextTime(Timestamp.Diff(MarkTwo!, MarkOne!).GetFormat());

        Timestamp? _markOne;
        Timestamp? _markTwo;

        public Marker()
        {
            InitializeComponent();
            ResetState();
            BMarkOne.Click += ButtonClicked;
            BMarkTwo.Click += ButtonClicked;
        }

        public void SetButtonState(MarkButton button, bool enabled)
        {
            switch (button)
            {
                case (MarkButton.One):
                    BMarkOne.Enabled = enabled;
                    break;

                case (MarkButton.Two):
                    BMarkTwo.Enabled = enabled;
                    break;
            }
        }

        public void ResetState()
        {
            WasCanceled = null;
            MarkOne = null;
            MarkTwo = null;
            LTotalTime.Text = "Total Intro Time - 00:00:00:00";
            SetButtonState(MarkButton.One, true);
            SetButtonState(MarkButton.Two, false);
        }

        void ButtonClicked(object? sender, EventArgs e)
        {
            switch (sender == BMarkOne)
            {
                case (true):
                    MarkOneButtonClicked?.Invoke(this, EventArgs.Empty);
                    MarkOne = TimestampGetter();
                    SetButtonState(MarkButton.One, false);
                    SetButtonState(MarkButton.Two, true);
                    break;

                default:
                    MarkTwoButtonClicked?.Invoke(this, EventArgs.Empty);
                    MarkTwo = TimestampGetter();
                    SetButtonState(MarkButton.Two, false);
                    var time = Timestamp.Diff(MarkTwo!, MarkOne!);

                    if (time.ToMS() <= 0 && RejectNegative)
                    {
                        this.ResetState();
                        return;
                    }

                    ComputeAndSetTotalTimeText();
                    WasCanceled = false;
                    MarkConfirmed?.Invoke(this, new(MarkOne!, MarkTwo!, time));

                    break;
            }
        }

        private void LExitButton_Click(object sender, EventArgs e)
        {
            WasCanceled = true;
            ExitButtonClicked?.Invoke(this, EventArgs.Empty);
        }
    }

    public class MarkerEventArgs(Timestamp start, Timestamp end, Timestamp diff) : EventArgs()
    {
        public Timestamp one = start;
        public Timestamp two = end;
        public Timestamp diff = diff;
    }
}
