using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
//
using ClientLib.STD;
using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using WINFORMS_VLCClient.Controls;
using WINFORMS_VLCClient.Lib.MediaInformation;
using static ClientLib.STD.StandardDefinitions;
//
using Timer = System.Windows.Forms.Timer;

namespace WINFORMS_VLCClient.Forms
{
    public partial class Viewer : Form
    {
        static readonly int POLL_RATE_MS = 900;
        static readonly int SCROLL_SEEK_MS = 500;
        static readonly int ARROW_SEEK_MS = 10 * 1000;
        static readonly int ARROW_VOLUME_CHANGE_PERCENT = 1;

        Landing parent;

        Timer pollTimer;

        Control[] allTheThingsThatNeedToAppearOnHover;
        MediaPlayer? CurrentPlayer => VVMainView.MediaPlayer;

        MediaPlayer GetNewMediaPlayer()
        {
            if (CurrentPlayer != null)
                Cleanup();

            VVMainView.MediaPlayer = parent!.MakeMediaPlayer();
            CurrentPlayer!.TimeChanged += OnTimeChange;
            return CurrentPlayer!;
        }

        public VideoPlaybackTimeline Timeline => this.VPTMainTimeline;

        public Viewer(Landing _parent)
        {
            InitializeComponent();
            this.KeyPreview = true;
            allTheThingsThatNeedToAppearOnHover = [VPTMainTimeline];

            parent = _parent;
            this.FormClosing += (_, _) =>
            {
                pollTimer?.Dispose();
                Cleanup();
            };

            this.MouseEnter += ShowTimeline;
            this.MouseLeave += HideTimeline;

            VVMainView.MouseEnter += ShowTimeline;
            VVMainView.MouseLeave += HideTimeline;

            VPTMainTimeline.MouseDidMove += SeekMediaMouseEvent;
            VPTMainTimeline.ScrollWheelScrolled += ScrollWheelSeek;
            VPTMainTimeline.MuteButtonClicked += (_, _) => CurrentPlayer?.ToggleMute();
            VPTMainTimeline.PauseButtonClicked += (_, _) => CurrentPlayer?.Pause();

            TBVolumeBar.ValueChanged += ChangeVolumeEvent;

            pollTimer = new() { Interval = POLL_RATE_MS };
            //pollTimer.Tick += HideTimeline;
            pollTimer.Tick += (_, _) => WriteTimeStamp();
            pollTimer.Start();
        }

        public void PlayMedia(Media media, Timestamp? startingPosition = null)
        {
            var player = GetNewMediaPlayer();

            player.Play(media);

            if (startingPosition != null)
                player.Time = startingPosition.ToMS();

            if (CurrentPlayer?.Mute ?? false)
                VPTMainTimeline.ShowVolumeIsMuted();
        }

        void SeekMediaMouseEvent(object? sender, MouseEventArgs e)
        {
            if (CurrentPlayer == null)
                return;

            if (e.Button != MouseButtons.Left)
                return;

            var mediaSize = CurrentPlayer.Length;
            if (mediaSize == 0)
                return;

            float requestedPercentage = VPTMainTimeline.ClickPointToBarPercentage(e.Location);
            long requestedTime = (long)(mediaSize * requestedPercentage);
            int barPostion = (int)(100 * requestedPercentage);

            //Debug.WriteLine($"Requested Percentage: {requestedPercentage}");
            //Debug.WriteLine($"Media Size: {mediaSize}");
            //Debug.WriteLine($"Requested Time: {requestedTime}");
            //Debug.WriteLine($"Bar Postion: {barPostion}");
            //Debug.WriteLine($"Have Media: {CurrentPlayer.Media != null}");
            //Debug.WriteLine($"Media Current Time: {CurrentPlayer.Time}");

            if (CurrentPlayer.Time == CurrentPlayer.Length)
            {
                CurrentPlayer.Stop();
                CurrentPlayer.Play();
            }

            CurrentPlayer.Time = requestedTime;
        }

        void ChangeVolumeEvent(object? sender, EventArgs e)
        {
            if (sender is not TrackBar tb || CurrentPlayer == null)
                return;

            var newVol = int.Clamp(tb.Value, 0, 100);
            CurrentPlayer.Volume = newVol;

            if ((newVol == 0 || CurrentPlayer.Mute) && !VPTMainTimeline.IsVolumeMutedShown())
                VPTMainTimeline.ShowVolumeIsMuted();
            else if (VPTMainTimeline.IsVolumeMutedShown() && !CurrentPlayer.Mute)
                VPTMainTimeline.ShowVolumeIsPlaying();
        }

        void ScrollWheelSeek(object? sender, MouseEventArgs e)
        {
            if (CurrentPlayer == null)
                return;

            if (e.Delta < 0)
                CurrentPlayer.Time = CurrentPlayer.Time + SCROLL_SEEK_MS;
            else
                CurrentPlayer.Time = CurrentPlayer.Time - SCROLL_SEEK_MS;
        }

        void WriteTimeStamp()
        {
            if (CurrentPlayer == null || CurrentPlayer.Media == null)
                return;

            parent.LastWatched = new(
                new Uri(CurrentPlayer.Media.Mrl),
                Timestamp.FromMS(CurrentPlayer.Time)
            );
        }

        void ShowTimeline(object? sender, EventArgs e)
        {
            foreach (var control in allTheThingsThatNeedToAppearOnHover)
            {
                control.Enabled = true;
                control.Visible = true;
            }
        }

        void HideTimeline(object? sender, EventArgs e)
        {
            if (isCursorInForm(this))
                return;

            foreach (var control in allTheThingsThatNeedToAppearOnHover)
            {
                control.Enabled = false;
                control.Visible = false;
            }
        }

        void OnTimeChange(object? sender, MediaPlayerTimeChangedEventArgs e)
        {
            if (
                this.Disposing
                || this.IsDisposed
                || VPTMainTimeline.Disposing
                || VPTMainTimeline.IsDisposed
            )
                return;

            try
            {
                Invoke(() =>
                {
                    var currentTime = Timestamp.FromMS(e.Time);
                    VPTMainTimeline.SetDisplayedVideoTime(
                        currentTime,
                        Timestamp.FromMS(CurrentPlayer?.Length ?? 0)
                    );
                    VPTMainTimeline.SetBarPosition(
                        (int)(100 * e.Time / (CurrentPlayer?.Length ?? 1))
                    );
                });
            }
            catch (ObjectDisposedException)
            {
                Debug.WriteLine("Somehow we've disposed of the object inside the invoke statement");
            }
        }

        Timer? volumeDebouncer;

        void ChangeVolumeDebounced(int to)
        {
            if (volumeDebouncer == null)
            {
                volumeDebouncer = new Timer { Interval = 50 };
                volumeDebouncer.Tick += (_, _) =>
                {
                    if (CurrentPlayer != null)
                    {
                        int newVolume = Math.Max(0, Math.Min(100, CurrentPlayer.Volume + to));
                        TBVolumeBar.Value = newVolume;
                        CurrentPlayer.Volume = newVolume;
                        Debug.WriteLine("Volume Changed!");
                    }

                    volumeDebouncer?.Stop();
                    volumeDebouncer?.Dispose();
                    volumeDebouncer = null;
                };
            }

            volumeDebouncer.Stop();
            volumeDebouncer.Start();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keydata)
        {
            if (CurrentPlayer == null)
                return base.ProcessCmdKey(ref msg, keydata);

            switch (keydata)
            {
                case Keys.Right:
                {
                    CurrentPlayer.Time = CurrentPlayer.Time + ARROW_SEEK_MS;
                    return true;
                }
                case Keys.Left:
                {
                    CurrentPlayer.Time = CurrentPlayer.Time - ARROW_SEEK_MS;
                    return true;
                }
                case Keys.Up:
                    ChangeVolumeDebounced(ARROW_VOLUME_CHANGE_PERCENT);
                    return false;

                case Keys.Down:
                    ChangeVolumeDebounced(-ARROW_VOLUME_CHANGE_PERCENT);
                    return false;
            }

            return base.ProcessCmdKey(ref msg, keydata);
        }

        void Cleanup()
        {
            var player = CurrentPlayer;
            if (player == null)
                return;

            VVMainView.MediaPlayer = null;
            //Insane Workaround because VLC is really quite terrible
            ThreadPool.QueueUserWorkItem(_ =>
            {
                player.TimeChanged -= OnTimeChange;
                player.Stop();
                player.Dispose();
            });
        }
    }
}
