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
using WINFORMS_VLCClient.Lib.FullScreen;
using WINFORMS_VLCClient.Lib.Hotkeys;
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
        static readonly int VOLUME_DEFAULT_PERCENTAGE = 70;
        static readonly int VOLUME_ARROW_CHANGE_PERCENT = 1;

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

        MediaPlayer? CurrentPlayer => VVMainView.MediaPlayer;

        int systemHotkeyF9PauseID;
        int systemHotkeyMediaPauseID;
        bool mediaPlayerHasBeenCreated = false;
        bool hasBeenRepositioned = false;
        bool isFullScreen = false;

        MediaPlayer GetNewMediaPlayer()
        {
            if (CurrentPlayer != null)
                CleanupPlayer();

            VVMainView.MediaPlayer = parent!.MakeMediaPlayer();
            CurrentPlayer!.EnableKeyInput = false;
            CurrentPlayer.EnableMouseInput = false;

            CurrentPlayer.TimeChanged += OnTimeChange;
            CurrentPlayer.EndReached += (_, _) =>
                RunSafeInvoke(this, () => MediaStopped?.Invoke(this, EventArgs.Empty));

            if (
                (CurrentPlayer.Volume == 0 || CurrentPlayer.Mute)
                && TBVolumeBar.Value != 0
                && !mediaPlayerHasBeenCreated
            )
            {
                Debug.WriteLine("Set Volume to default");
                SetPlayerVolume(VOLUME_DEFAULT_PERCENTAGE);
                TBVolumeBar.Value = VOLUME_DEFAULT_PERCENTAGE;
            }
            else
                TBVolumeBar.Value = CurrentPlayer.Volume;

            mediaPlayerHasBeenCreated = true;
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

            systemHotkeyF9PauseID = new Random().Next(int.MaxValue);
            HotkeyHelper.RegisterHotkey(
                this.Handle,
                systemHotkeyF9PauseID,
                HotkeyHelper.MOD_ALT,
                (uint)Keys.F9
            );

            systemHotkeyMediaPauseID = new Random().Next(int.MaxValue);
            HotkeyHelper.RegisterHotkey(
                this.Handle,
                systemHotkeyMediaPauseID,
                HotkeyHelper.MOD_NONE,
                (uint)Keys.MediaPlayPause
            );

            this.MinimumSize = VIEWER_MINIUM_SIZE;
            this.KeyPreview = true;
            this.Disposed += OnDispose;
            this.FormClosing += (_, _) =>
            {
                pollTimer?.Stop();
                pollTimer?.Dispose();
                CleanupPlayer();
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

            TBVolumeBar.ValueChanged += (_, _) =>
                SetPlayerVolume(TBVolumeBar.Value, fromVolumeBar: true);

            MKRIntroSkip.MarkConfirmed += ConfirmIntro;
            MKRIntroSkip.ExitButtonClicked += HideIntroSkipMaker;
            MKRIntroSkip.TimestampGetter = () =>
                CurrentPlayer != null ? Timestamp.FromMS(CurrentPlayer.Time) : null;
            MKRIntroSkip.Hide();

            BRecordIntro.Click += ShowIntroSkipMaker;
            BSkipIntro.Click += DoSkipIntro;
            PSkipIntroButtonContainer.Show();

            HideIntroSkipMaker(null, null!);

            pollTimer = new() { Interval = POLL_RATE_MS };
            pollTimer.Tick += (_, _) => PollingTick();
            pollTimer.Start();

#if DEBUG
            this.Text = "Viewer - [DEBUG]";
#endif
        }

        public void PlayMedia(Media media, Timestamp? startingPosition = null)
        {
            var player = GetNewMediaPlayer();
            RunInThreadPool(
                (_) =>
                {
                    // Have to do it this way to normalize the [media.Mrl]'s "URL-iffied" path characters
                    var fileName = Path.GetFileName(new Uri(media.Mrl).LocalPath);
#if DEBUG
                    RunSafeInvoke(this, () => this.Text = $"[DEBUG] - {fileName}");
#else
                    RunSafeInvoke(this, () => this.Text = $"Viewer - {fileName}");
#endif

                    player.Play(media);

                    if (startingPosition != null)
                        player.Time = startingPosition.ToMS();
                }
            );

            PollingTick();
        }

        public void SetLocation(Point position)
        {
            this.Location = position;
            hasBeenRepositioned = true;
        }

        void PollingTick()
        {
            if (CurrentPlayer == null || CurrentPlayer.Media == null)
                return;

            parent.LastWatched = new(
                new Uri(CurrentPlayer.Media.Mrl),
                Timestamp.FromMS(CurrentPlayer.Time)
            );

            // Do it this way to avoid recording when the window pops up as a new window,
            //  since we cant set the actual restore position.
            if (hasBeenRepositioned)
                parent.VideoViewFormRestorePosition = this.Location;

            SetTimelineState(false);
        }

        Intro? GetIntroForCurrentMedia()
        {
            if (CurrentPlayer == null || CurrentPlayer.Media == null)
                return null;

            return Intro.GetIntroFromDirectory(PlayingMediaParentFolder!);
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

            if (requestedTime >= mediaSize)
            {
                const int REWIND_AFTER_END_MS = 10 * 1000;
                SetPlayerTime(requestedTime - REWIND_AFTER_END_MS);
                SetPausedState(true);
            }
            else
            {
                SetPlayerTime(requestedTime);
            }
        }

        void ScrollWheelSeek(object? sender, MouseEventArgs e)
        {
            if (CurrentPlayer == null)
                return;

            if (e.Delta < 0)
                SetPlayerTime(CurrentPlayer.Time + SCROLL_SEEK_MS);
            else
                SetPlayerTime(CurrentPlayer.Time - SCROLL_SEEK_MS);
        }

        void DoSkipIntro(object? sender, EventArgs e)
        {
            string? mPath = PlayingMediaParentFolder;
            if (mPath == null || CurrentPlayer == null || !CurrentPlayer.IsPlaying)
                return;

            var intro = Intro.GetIntroFromDirectory(mPath);
            if (intro == null)
                return;

            SetPlayerTime(CurrentPlayer.Time + intro.diff.ToMS());
        }

        void ConfirmIntro(object? sender, MarkerEventArgs e)
        {
            string? mPath = PlayingMediaParentFolder;
            if (mPath == null)
                return;

            var haveIntro = GetIntroForCurrentMedia();
            if (haveIntro != null)
            {
                if (
                    MessageBox.Show(
                        $"Do you really want to overwrite the old intro time?\nOld intro: {haveIntro.diff.GetFormat()}\nNew intro: {e.diff.GetFormat()}",
                        "Confirm Overwrite",
                        MessageBoxButtons.YesNo
                    ) != DialogResult.Yes
                )
                    return;
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
            if (!to && IsCursorInForm(this))
                return;

            foreach (var control in allTheThingsThatNeedToAppearOnHover)
            {
                if (control.Enabled != to)
                    control.Enabled = to;

                if (control.Visible != to)
                    control.Visible = to;
            }
        }

        void ShowIntroSkipMaker(object? sender, EventArgs e)
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

        void HideIntroSkipMaker(object? sender, EventArgs e)
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
                VPTMainTimeline.Disposing
                || VPTMainTimeline.IsDisposed
                || CurrentPlayer == null
                || CurrentPlayer.Length == 0
            )
                return;

            RunSafeInvoke(
                this,
                () =>
                {
                    var currentTime = Timestamp.FromMS(e.Time);
                    long? len = CurrentPlayer.Length;

                    VPTMainTimeline.SetDisplayedVideoTime(currentTime, Timestamp.FromMS((long)len));
                    VPTMainTimeline.SetBarPosition((int)((100 * e.Time) / (long)len));
                }
            );
        }

        void VVMainView_Click(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            TogglePausedState();
        }

        void VVMainView_DoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            ToggleWindowFullScreenState();
            TogglePausedState();
        }

        void TogglePausedState()
        {
            if (CurrentPlayer == null)
                return;

            SetPausedState(!CurrentPlayer.IsPlaying);
        }

        void SetPausedState(bool to) =>
            RunInThreadPool(
                (_) =>
                {
                    if (CurrentPlayer == null || to == CurrentPlayer.IsPlaying)
                        return;

                    CurrentPlayer.Pause();

                    RunSafeInvoke(
                        VPTMainTimeline,
                        () =>
                        {
                            if (VPTMainTimeline.IsVideoPausedShown())
                                VPTMainTimeline.ShowVideoIsPlaying();
                            else
                                VPTMainTimeline.ShowVideoIsPaused();
                        }
                    );
                }
            );

        void SetPlayerVolume(int to, bool fromVolumeBar = false)
        {
            RunInThreadPool(
                (_) =>
                {
                    if (CurrentPlayer == null)
                        return;

                    int newVolume = Math.Max(0, Math.Min(100, CurrentPlayer.Volume + to));

                    RunSafeInvoke(
                        this,
                        () =>
                        {
                            if (
                                (newVolume == 0 || CurrentPlayer.Mute)
                                && !VPTMainTimeline.IsVolumeMutedShown()
                            )
                                VPTMainTimeline.ShowVolumeIsMuted();
                            else if (VPTMainTimeline.IsVolumeMutedShown() && !CurrentPlayer.Mute)
                                VPTMainTimeline.ShowVolumeIsPlaying();

                            if (!fromVolumeBar)
                                TBVolumeBar.Value = to;
                        },
                        "Volume Invoke Ran!"
                    );

                    CurrentPlayer.Volume = to;
                },
                $"Volume Change: {CurrentPlayer?.Volume}->{to}"
            );
        }

        void SetPlayerTime(long to) =>
            RunInThreadPool(
                (_) =>
                {
                    if (CurrentPlayer == null)
                        return;

                    CurrentPlayer.Time = to;
                }
            );

        void ToggleWindowFullScreenState() => SetWindowFullScreenState(!isFullScreen);

        void SetWindowFullScreenState(bool to)
        {
            if (to)
                FullscreenHelper.FullScreenWindow(this.Handle);
            else
                FullscreenHelper.LeaveFullscreen(this.Handle);

            isFullScreen = to;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == HotkeyHelper.WM_HOTKEY)
                TogglePausedState();

            base.WndProc(ref m);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keydata)
        {
            if (CurrentPlayer == null)
                return base.ProcessCmdKey(ref msg, keydata);

            switch (keydata)
            {
                case Keys.Right:
                    SetPlayerTime(CurrentPlayer.Time + ARROW_SEEK_MS);
                    return true;
                case Keys.Left:
                    SetPlayerTime(CurrentPlayer.Time - ARROW_SEEK_MS);
                    return true;
                case Keys.Up:
                    SetPlayerVolume(TBVolumeBar.Value + VOLUME_ARROW_CHANGE_PERCENT);
                    return true;
                case Keys.Down:
                    SetPlayerVolume(TBVolumeBar.Value - VOLUME_ARROW_CHANGE_PERCENT);
                    return true;
                case Keys.F11:
                    ToggleWindowFullScreenState();
                    return true;
                case Keys.Space:
                    TogglePausedState();
                    return true;
                case Keys.Escape:
                    if (!isFullScreen)
                        return false;

                    SetWindowFullScreenState(false);

                    return true;
            }

            return base.ProcessCmdKey(ref msg, keydata);
        }

        void OnDispose(object? sender, EventArgs e)
        {
            HotkeyHelper.UnregisterHotkey(this.Handle, systemHotkeyF9PauseID);
            HotkeyHelper.UnregisterHotkey(this.Handle, systemHotkeyMediaPauseID);
        }

        void CleanupPlayer()
        {
            var player = CurrentPlayer;
            if (player == null)
                return;

            VVMainView.MediaPlayer = null;
            RunInThreadPool(_ =>
            {
                player.TimeChanged -= OnTimeChange;
                player.Stop();
                player.Dispose();
            });
        }
    }
}
