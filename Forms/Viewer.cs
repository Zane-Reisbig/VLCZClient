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
using WINFORMS_VLCClient.Lib;
using WINFORMS_VLCClient.Lib.MediaInformation;
using static ClientLib.STD.StandardDefinitions;
//
using Timer = System.Windows.Forms.Timer;

namespace WINFORMS_VLCClient.Forms
{
    public partial class Viewer : Form
    {
        static readonly int POLL_RATE_MS = 900;
        static readonly int ARROW_SEEK_MS = 10 * 1000;
        static readonly int SCROLL_SEEK_MS = 500;
        static readonly int VOLUME_DEBOUNCE_TIME_MS = 500;
        static readonly int ARROW_VOLUME_CHANGE_PERCENT = 1;

        public static readonly Size VIEWER_MINIUM_SIZE = new(800, 600);
        static readonly Size ContainerSizeButtons = new(104, 59);
        static readonly Size ContainerSizeMarker = new(249, 74);
        static readonly Point ContainerLocationButtons = new(189, 485);
        static readonly Point ContainerLocationMarker = new(36, 475);

        readonly Landing parent;
        readonly Timer pollTimer;
        readonly Control[] allTheThingsThatNeedToAppearOnHover;

        public EventHandler? MediaStopped;
        public EventHandler? NextButton;
        public EventHandler? PrevButton;
        public Point lastRegisteredLocation;

        MediaPlayer? CurrentPlayer => VVMainView.MediaPlayer;

        MediaPlayer GetNewMediaPlayer()
        {
            if (CurrentPlayer != null)
                Cleanup();

            VVMainView.MediaPlayer = parent!.MakeMediaPlayer();
            CurrentPlayer!.TimeChanged += OnTimeChange;
            CurrentPlayer!.EndReached += (_, _) =>
            {
                Invoke(() =>
                {
                    if (this.Disposing || this.IsDisposed || !this.IsHandleCreated)
                        return;

                    Debug.WriteLine("End Reached!");
                    MediaStopped?.Invoke(this, EventArgs.Empty);
                });
            };

            CurrentPlayer!.EnableKeyInput = false;
            CurrentPlayer!.EnableMouseInput = false;
            CurrentPlayer!.Volume = TBVolumeBar.Value;
            return CurrentPlayer!;
        }

        public VideoPlaybackTimeline Timeline => this.VPTMainTimeline;
        public string? PlayingMediaParentFolder =>
            CurrentPlayer != null && CurrentPlayer.Media != null
                ? new Uri(Path.GetDirectoryName(CurrentPlayer.Media.Mrl)!).LocalPath
                : null;

        public Viewer(Landing _parent)
        {
            InitializeComponent();
            parent = _parent;

            this.MinimumSize = VIEWER_MINIUM_SIZE;
            this.KeyPreview = true;
            this.FormClosing += (_, _) =>
            {
                pollTimer?.Stop();
                pollTimer?.Dispose();
                Cleanup();
            };

            allTheThingsThatNeedToAppearOnHover =
            [
                VPTMainTimeline,
                TBVolumeBar,
                PSkipIntroFullContainer,
            ];

            VVMainView.MouseEnter += ShowTimeline;
            VVMainView.MouseLeave += HideTimeline;

            VPTMainTimeline.MouseDidMove += SeekMediaMouseEvent;
            VPTMainTimeline.MouseEnter += ShowTimeline;
            VPTMainTimeline.ScrollWheelScrolled += ScrollWheelSeek;
            VPTMainTimeline.MuteButtonClicked += (_, _) => CurrentPlayer?.ToggleMute();
            VPTMainTimeline.PauseButtonClicked += (_, _) => CurrentPlayer?.Pause();
            VPTMainTimeline.NextButtonClicked += (_, _) =>
                NextButton?.Invoke(this, EventArgs.Empty);
            VPTMainTimeline.PreviousButtonClicked += (_, _) =>
                PrevButton?.Invoke(this, EventArgs.Empty);

            TBVolumeBar.ValueChanged += ChangeVolumeEvent;

            BRecordIntro.Click += DoMarkSkipIntro;
            BSkipIntro.Click += DoSkipIntro;

            MKRIntroSkip.TimestampGetter = () =>
                CurrentPlayer != null ? Timestamp.FromMS(CurrentPlayer.Time) : null;

            MKRIntroSkip.MarkConfirmed += ConfirmIntro;

            MKRIntroSkip.ExitButtonClicked += CleanupDoMarkSkipIntro;

            PSkipIntroButtonContainer.Show();
            MKRIntroSkip.Hide();
            CleanupDoMarkSkipIntro(null, null);

            pollTimer = new() { Interval = POLL_RATE_MS };
            pollTimer.Tick += (_, _) => PollingTick();
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
            if (CurrentPlayer == null || e.Button != MouseButtons.Left)
                return;

            var mediaSize = CurrentPlayer.Length;
            if (mediaSize == 0)
                return;

            long requestedTime = (long)(
                mediaSize * VPTMainTimeline.ClickPointToBarPercentage(e.Location)
            );

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

        void PollingTick()
        {
            if (CurrentPlayer == null || CurrentPlayer.Media == null)
                return;

            parent.LastWatched = new(
                new Uri(CurrentPlayer.Media.Mrl),
                Timestamp.FromMS(CurrentPlayer.Time)
            );

            this.lastRegisteredLocation = this.Location;

            SetTimelineState(false);
        }

        Intro? GetIntroForCurrentMedia()
        {
            if (CurrentPlayer == null || CurrentPlayer.Media == null)
                return null;

            return Intro.GetIntroFromDirectory(PlayingMediaParentFolder!);
        }

        void DoSkipIntro(object? sender, EventArgs e)
        {
            string? mPath = PlayingMediaParentFolder;
            if (mPath == null || CurrentPlayer == null || !CurrentPlayer.IsPlaying)
                return;

            var intro = Intro.GetIntroFromDirectory(mPath);
            if (intro == null)
                return;

            CurrentPlayer.Time = CurrentPlayer.Time + intro.diff.ToMS();
        }

        void ConfirmIntro(object? sender, MarkerEventArgs e)
        {
            string? mPath = PlayingMediaParentFolder;
            if (mPath == null)
                return;

            var haveIntro = GetIntroForCurrentMedia();
            if (haveIntro != null)
            {
                // Confirm Overwrite here
            }

            File.WriteAllText(
                path: Intro.GetIntroPathForDirectory(mPath)!,
                new Intro(
                    e.one,
                    e.two,
                    e.diff,
                    directoryName: CurrentPlayer!.Media!.Mrl
                ).ToINIString()
            );
        }

        void ShowTimeline(object? sender, EventArgs e) => SetTimelineState(true);

        void HideTimeline(object? sender, EventArgs e) => SetTimelineState(false);

        void SetTimelineState(bool to)
        {
            if (!to && isCursorInForm(this))
                return;

            foreach (var control in allTheThingsThatNeedToAppearOnHover)
            {
                if (control.Enabled != to)
                    control.Enabled = to;

                if (control.Visible != to)
                    control.Visible = to;
            }
        }

        void DoMarkSkipIntro(object? sender, EventArgs e)
        {
            if (PSkipIntroButtonContainer.Visible)
                PSkipIntroButtonContainer.Hide();

            PSkipIntroFullContainer.Size = ContainerSizeMarker;
            PSkipIntroFullContainer.Location = ContainerLocationMarker;

            MKRIntroSkip.Show();

            if (PlayingMediaParentFolder != null)
            {
                Intro? haveIntro = Intro.GetIntroFromDirectory(PlayingMediaParentFolder);
                if (haveIntro != null)
                    MKRIntroSkip.SetTotalTextTime(haveIntro.diff.GetFormat());
            }
        }

        void CleanupDoMarkSkipIntro(object? sender, EventArgs e)
        {
            if (MKRIntroSkip.Visible)
                MKRIntroSkip.Hide();

            PSkipIntroFullContainer.Size = ContainerSizeButtons;
            PSkipIntroFullContainer.Location = ContainerLocationButtons;

            PSkipIntroButtonContainer.Show();
            MKRIntroSkip.ResetState();
        }

        void OnTimeChange(object? sender, MediaPlayerTimeChangedEventArgs e)
        {
            if (
                this.Disposing
                || this.IsDisposed
                || VPTMainTimeline.Disposing
                || VPTMainTimeline.IsDisposed
                || CurrentPlayer == null
                || CurrentPlayer.Length == 0
                || !this.IsHandleCreated
            )
                return;

            try
            {
                Invoke(() =>
                {
                    var currentTime = Timestamp.FromMS(e.Time);
                    VPTMainTimeline.SetDisplayedVideoTime(
                        currentTime,
                        Timestamp.FromMS(CurrentPlayer.Length)
                    );
                    VPTMainTimeline.SetBarPosition((int)(100 * e.Time / (CurrentPlayer.Length)));
                });
            }
            catch (ObjectDisposedException)
            {
                Debug.WriteLine("Object was disposed mid-Invoke!");
                Debug.WriteLine(e);
            }
        }

        Timer? volumeDebouncer;

        void ChangeVolumeDebounced(int to)
        {
            if (volumeDebouncer == null)
            {
                volumeDebouncer = new Timer { Interval = VOLUME_DEBOUNCE_TIME_MS };
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

                    if (CurrentPlayer.Volume == 100)
                        return true;

                    return false;

                case Keys.Down:
                    ChangeVolumeDebounced(-ARROW_VOLUME_CHANGE_PERCENT);

                    if (CurrentPlayer.Volume == 0)
                        return true;

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

        private void PSkipIntroFullContainer_Paint(object sender, PaintEventArgs e) { }

        private void VVMainView_Click(object sender, EventArgs e) { }
    }
}
