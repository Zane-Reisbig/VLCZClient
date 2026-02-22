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
        static readonly int REWIND_AFTER_END_MS = 10 * 1000;
        static readonly int SCROLL_SEEK_MS = 500;
        static readonly int VOLUME_DEFAULT_PERCENTAGE = 70;
        static readonly int VOLUME_ARROW_CHANGE_PERCENT = 1;
        static readonly int VISUAL_ERROR_TIMEOUT_SMALL = 1000;

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
            this.FormClosing += CleanupViewer;

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

            // needed to avoid recording when the window pops up as a new window,
            //  since we cant set the initial creation position.
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
                if (
                    MessageBox.Show(
                        $"Do you really want to overwrite the old intro time?\nOld intro: {haveIntro.diff.GetFormat()}\nNew intro: {e.diff.GetFormat()}",
                        "Confirm Overwrite",
                        MessageBoxButtons.YesNo
                    ) != DialogResult.Yes
                )
                    return;

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

        void OnTimeChange(object? sender, MediaPlayerTimeChangedEventArgs e) =>
            RunSafeInvoke(
                this,
                () =>
                {
                    if (
                        VPTMainTimeline.Disposing
                        || VPTMainTimeline.IsDisposed
                        || CurrentPlayer == null
                        || CurrentPlayer.Length == 0
                    )
                        return;

                    long len = CurrentPlayer.Length;
                    VPTMainTimeline.SetDisplayedVideoTime(
                        Timestamp.FromMS(e.Time),
                        Timestamp.FromMS(len)
                    );
                    VPTMainTimeline.SetBarPosition((int)((100 * e.Time) / len));
                }
            );

        void ToggleWindowFullScreenState() => SetWindowFullScreenState(!isFullScreen);

        void SetWindowFullScreenState(bool to)
        {
            if (to && !isFullScreen)
                FullscreenHelper.FullScreenWindow(this.Handle);

            if (!to && isFullScreen)
                FullscreenHelper.LeaveFullscreen(this.Handle);

            isFullScreen = to;
        }

        private void VVMainView_DragDrop(object sender, DragEventArgs e)
        {
            if (
                e.Data == null
                || e.Data.GetData(DataFormats.FileDrop) is not string[] file
                || file.Length == 0
            )
                return;

            if (LoadSubtitleFromFile(file[0]))
                // Successful return early
                return;

            Cursor = Cursors.No;
            RunInThreadPool(
                (_) =>
                {
                    Thread.Sleep(VISUAL_ERROR_TIMEOUT_SMALL);
                    RunSafeInvoke(this, () => Cursor = Cursors.Default);
                }
            );
            Debug.WriteLine($"WARN: Failed to set subtitles to: \"{file[0]}\"");
        }

        private void VVMainView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data == null || !e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            e.Effect = DragDropEffects.Link;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == HotkeyHelper.WM_HOTKEY)
                TogglePausedState();

            base.WndProc(ref m);
        }

        void CleanupViewer(object? sender, EventArgs e)
        {
            HotkeyHelper.UnregisterHotkey(this.Handle, systemHotkeyF9PauseID);
            HotkeyHelper.UnregisterHotkey(this.Handle, systemHotkeyMediaPauseID);

            RunSafeInvoke(
                this,
                () =>
                {
                    this.FormClosing -= CleanupViewer;

                    pollTimer?.Stop();
                    pollTimer?.Dispose();
                    fileDialog?.Dispose();

                    UnhookControls();
                    CleanupPlayer();
                }
            );
        }
    }
}
