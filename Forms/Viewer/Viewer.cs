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

namespace WINFORMS_VLCClient.Viewer
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

        OpenFileDialog? fileDialog;
        OpenFileDialog FileDialog
        {
            get
            {
                fileDialog ??= new()
                {
                    InitialDirectory = "C://",
                    Filter = "Subtitle Files|*.srt;|All Files (*.*)|*.*",
                };

                return fileDialog;
            }
        }

        bool mediaPlayerHasBeenCreated = false;
        bool hasBeenRepositioned = false;
        bool isFullScreen = false;

        public string? PlayingMediaParentFolder =>
            CurrentPlayer != null && CurrentPlayer.Media != null
                ? new Uri(Path.GetDirectoryName(CurrentPlayer.Media.Mrl)!).LocalPath
                : null;

        public Viewer(Landing _parent)
        {
            InitializeComponent();
            HookSystemHotkeys();
            parent = _parent;
            allTheThingsThatNeedToAppearOnHover =
            [
                VPTMainTimeline,
                TBVolumeBar,
                PSkipIntroFullContainer,
            ];

            this.MinimumSize = VIEWER_MINIUM_SIZE;
            this.KeyPreview = true;
            this.Disposed += OnDispose;
            this.FormClosing += (_, _) =>
            {
                pollTimer?.Stop();
                pollTimer?.Dispose();
                CleanupPlayer();
            };

            HookControls();
            HideIntroSkipMaker(null, null!);

            pollTimer = new() { Interval = POLL_RATE_MS };
            pollTimer.Tick += (_, _) => PollingTick();
            pollTimer.Start();

#if DEBUG
            this.Text = "Viewer - [DEBUG]";
#endif
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

        void SelectSubtitleTrack()
        {
            if (CurrentPlayer == null)
                return;

            if (FileDialog.ShowDialog() != DialogResult.OK)
                return;

            var file = FileDialog.FileNames[0];
            Subtitles.Load(CurrentPlayer, file);
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
