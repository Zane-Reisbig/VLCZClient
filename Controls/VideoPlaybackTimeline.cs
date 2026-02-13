using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibVLCSharp.Shared;
using WINFORMS_VLCClient.Forms;
using WINFORMS_VLCClient.Lib.ComponentCycler;
using static ClientLib.STD.StandardDefinitions;

namespace WINFORMS_VLCClient.Controls
{
    public partial class VideoPlaybackTimeline : UserControl
    {
        public int movementAmount = 1000;
        public EventHandler<MouseEventArgs>? ScrollWheelScrolled;
        public EventHandler<MouseEventArgs>? MouseDidMove;
        public EventHandler? MuteButtonClicked;
        public EventHandler? PauseButtonClicked;

        ComponentCycler PauseCycler;
        ComponentCycler MuteCycler;

        public VideoPlaybackTimeline()
        {
            InitializeComponent();

            PauseCycler = new([LPauseButton, LPlayButton]);
            PauseCycler.ShowAtSlot(0);

            MuteCycler = new([LMuteButton, LUnMuteButton]);
            MuteCycler.ShowAtSlot(0);

            this.MouseWheel += MouseWheelMoved;
            PBVideoTimeline.MouseWheel += MouseWheelMoved;
            PBVideoTimeline.MouseMove += OnMouseDidMove;
        }

        public float ClickPointToBarPercentage(Point location)
        {
            double percentage = (double)location.X / PBVideoTimeline.Width;
            return (float)(Math.Clamp(percentage, 0.0, 1.0));
        }

        public bool IsVolumeMutedShown() => MuteCycler.GetSlot() == 0;

        public void ShowVolumeIsMuted() => MuteCycler.ShowAtSlot(0);

        public void ShowVolumeIsPlaying() => MuteCycler.ShowAtSlot(1);

        public void ShowVideoIsPaused() => MuteCycler.ShowAtSlot(1);

        public void ShowVideoIsPlaying() => PauseCycler.ShowAtSlot(0);

        public void SetDisplayedVideoTime(Timestamp to, Timestamp other) =>
            LVideoTime.Text = $"{to.GetFormat()} | {other.GetFormat()}";

        public void SetBarPosition(int percentage) => PBVideoTimeline.Value = percentage;

        private void OnMouseDidMove(object? sender, MouseEventArgs e) =>
            MouseDidMove?.Invoke(this, e);

        private void MouseWheelMoved(object? sender, MouseEventArgs e) =>
            ScrollWheelScrolled?.Invoke(this, e);

        private void MuteButton_MouseUp(object? sender, MouseEventArgs e)
        {
            MuteCycler.Cycle();
            MuteButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void PauseButton_MouseUp(object? sender, MouseEventArgs e)
        {
            PauseCycler.Cycle();
            PauseButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void VideoPlaybackTimeline_Load(object sender, EventArgs e) { }
    }

    public class TimelineChangedArgs(long originalPosition, long requestedPosition) : EventArgs
    {
        public long originalPosition = originalPosition;
        public long requestedPosition = requestedPosition;
    }
}
