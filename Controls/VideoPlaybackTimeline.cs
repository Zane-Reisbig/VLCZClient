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
using static ClientLib.STD.StandardDefinitions;

namespace WINFORMS_VLCClient.Controls
{
    public partial class VideoPlaybackTimeline : UserControl
    {
        MediaPlayer? linkedMediaPlayer;
        long TotalTimeMS
        {
            get
            {
                if (linkedMediaPlayer == null)
                    throw new Exception($"No linked media viewer!");

                return linkedMediaPlayer.Length;
            }
        }

        public int movementAmount = 1000;
        public EventHandler<TimelineChangedArgs>? OnTimelineChanged;
        public EventHandler? PauseButtonClicked;
        public EventHandler? MuteButtonClicked;

        public VideoPlaybackTimeline()
        {
            InitializeComponent();

            this.MouseWheel += MouseWheelMoved;
            this.PBVideoTimeline.MouseWheel += MouseWheelMoved;
            this.Disposed += Cleanup;
        }

        public void SetLinkedMediaViewer(MediaPlayer player)
        {
            linkedMediaPlayer = player;
            this.linkedMediaPlayer.TimeChanged += Player_TimeChanged;
        }

        public void UpdateBarPosition(long ms)
        {
            if (linkedMediaPlayer == null)
                return;

            if (TotalTimeMS < ms)
                throw new Exception(
                    $"Total Time: \"{TotalTimeMS}\" is smaller than requested postion: \"{ms}\""
                );

            double percentage = (double)ms / TotalTimeMS * 100;
            PBVideoTimeline.Value = (int)percentage;
            Debug.WriteLine(
                $"Bar Position: \"{percentage}\"\nTotal Time: \"{TotalTimeMS}\"\nRequested Time: \"{ms}\""
            );
        }

        public Timestamp BarPositionToTimeStamp(Point location)
        {
            if (linkedMediaPlayer == null)
                throw new Exception("No linked media player!");

            double percentage = (double)location.X / PBVideoTimeline.Width;
            percentage = Math.Clamp(percentage, 0.0, 1.0);

            long timeMs = (long)(percentage * TotalTimeMS);

            return Timestamp.FromMS(timeMs);
        }

        void SetDisplayedVideoTime(Timestamp to)
        {
            LVideoTime.Text =
                $"{to.GetFormat()} | {Timestamp.FromMS(linkedMediaPlayer?.Length ?? 0).GetFormat()}";
        }

        private void MouseWheelMoved(object? sender, MouseEventArgs e)
        {
            if (linkedMediaPlayer == null)
                return;

            if (!linkedMediaPlayer.IsPlaying)
                return;

            long videoMovementAmount = linkedMediaPlayer.Time;
            if (e.Delta > 0)
                videoMovementAmount += movementAmount;
            else
                videoMovementAmount -= movementAmount;

            videoMovementAmount = long.Clamp(videoMovementAmount, 0, TotalTimeMS);

            OnTimelineChanged?.Invoke(this, new(linkedMediaPlayer.Time, videoMovementAmount));
        }

        private void PauseButton_Animation_MouseDown(object sender, EventArgs e)
        {
            if (sender is not Control control)
                return;

            control.Scale(SizeF.Subtract(control.Size, new SizeF(0.8f, 0.8f)));
        }

        private void PauseButton_MouseUp(object sender, EventArgs e)
        {
            if (sender is not Control control)
                return;

            //control.Scale(SizeF.Add(control.Size, new SizeF(0.2f, 0.2f)));

            if (!LPauseButton.Visible)
            {
                LPlayButton.Visible = false;
                LPlayButton.Enabled = false;
                LPauseButton.Visible = true;
                LPauseButton.Enabled = true;
            }
            else
            {
                LPauseButton.Visible = false;
                LPauseButton.Enabled = false;
                LPlayButton.Visible = true;
                LPlayButton.Enabled = true;
            }

            PauseButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void MuteButton_MouseUp(object sender, EventArgs e)
        {
            if (sender is not Control control)
                return;

            //control.Scale(SizeF.Add(control.Size, new SizeF(0.2f, 0.2f)));

            if (!LMuteButton.Visible)
            {
                LUnMuteButton.Visible = false;
                LUnMuteButton.Enabled = false;
                LMuteButton.Visible = true;
                LMuteButton.Enabled = true;
            }
            else
            {
                LMuteButton.Visible = false;
                LMuteButton.Enabled = false;
                LUnMuteButton.Visible = true;
                LUnMuteButton.Enabled = true;
            }

            MuteButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void Player_TimeChanged(
            object? sender,
            LibVLCSharp.Shared.MediaPlayerTimeChangedEventArgs e
        ) =>
            Invoke(() =>
            {
                UpdateBarPosition(e.Time);
                SetDisplayedVideoTime(Timestamp.FromMS(e.Time));
            });

        private void Cleanup(object? sender, EventArgs e)
        {
            if (this.linkedMediaPlayer == null)
                return;

            this.linkedMediaPlayer.TimeChanged -= Player_TimeChanged;
        }

        private void PBVideoTimeline_MouseClick(object sender, MouseEventArgs e)
        {
            var requestedPosition = BarPositionToTimeStamp(e.Location);
            UpdateBarPosition(requestedPosition.ToMS());

            if (linkedMediaPlayer != null)
                OnTimelineChanged?.Invoke(
                    this,
                    new(linkedMediaPlayer.Time, requestedPosition.ToMS())
                );
        }

        private void PBVideoTimeline_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            var requestedPosition = BarPositionToTimeStamp(e.Location);
            UpdateBarPosition(requestedPosition.ToMS());

            if (linkedMediaPlayer != null)
                OnTimelineChanged?.Invoke(
                    this,
                    new(linkedMediaPlayer.Time, requestedPosition.ToMS())
                );
        }
    }

    public class TimelineChangedArgs(long originalPosition, long requestedPosition) : EventArgs
    {
        public long originalPosition = originalPosition;
        public long requestedPosition = requestedPosition;
    }
}
