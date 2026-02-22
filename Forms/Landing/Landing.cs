using System.ComponentModel;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using ClientLib.STD;
using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using WINFORMS_VLCClient.Controls;
using WINFORMS_VLCClient.Lib;
using WINFORMS_VLCClient.Lib.MediaInformation;
using WINFORMS_VLCClient.Viewer;
using static ClientLib.STD.StandardDefinitions;

namespace WINFORMS_VLCClient
{
    public partial class Landing : Form
    {
        static readonly string[] libVLCParams =
        [
            "--vout=direct3d11",
            "--direct3d11-hw-blending",
            "--network-caching=10",
            "--live-caching=10",
            "--disc-caching=10",
        ];

        static readonly string historyFileName = "history.txt";

        static string HistoryPath
        {
            get => Path.Combine(Environment.CurrentDirectory, historyFileName);
        }

        LibVLC? vlcLib;
        LibVLC VLCLib
        {
            get
            {
                if (vlcLib == null)
                    vlcLib = new LibVLC(libVLCParams);

                return vlcLib;
            }
        }

        MediaInformation? lastWatched;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MediaInformation LastWatched
        {
            get
            {
                lastWatched ??= GetLastWatched();
                return lastWatched;
            }
            set
            {
                lastWatched = value;
                lastWatched.WriteToFile(HistoryPath);
            }
        }

        Point? videoViewFormRestorePosition = null;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Point? VideoViewFormRestorePosition
        {
            private get => videoViewFormRestorePosition;
            set => videoViewFormRestorePosition = value;
        }

        Viewer.Viewer? videoViewForm;
        Viewer.Viewer VideoViewForm
        {
            get
            {
                if (videoViewForm == null || videoViewForm.IsDisposed)
                {
                    videoViewForm = new(this);
                    videoViewForm.Shown += RestoreLocationHook;
                    videoViewForm.MediaStopped += (_, _) => NextPreviousEpisode(next: true);
                    videoViewForm.NextButton += (_, _) => NextPreviousEpisode(next: true);
                    videoViewForm.PrevButton += (_, _) => NextPreviousEpisode(next: false);
                    videoViewForm.FormClosed += (_, _) =>
                    {
                        FillLastWatched(LastWatched);
                        EnableButtons();
                    };
                }

                return videoViewForm;
            }
        }

        OpenFileDialog? fileDialog;
        OpenFileDialog FileDialog
        {
            get
            {
                fileDialog ??= new()
                {
                    InitialDirectory = "C://",
                    Filter = "Video Files|*.mp4;*.mov;*.wmv;*.avi;*.mkv;*.flv|All Files (*.*)|*.*",
                };

                return fileDialog;
            }
        }

        static void CreateNewHistory() =>
            WriteDictToINIFile<string, string>(
                HistoryPath,
                new() { { "path", "None" }, { "timestamp", "None" } }
            );

        static MediaInformation GetLastWatched()
        {
            if (!Path.Exists(HistoryPath))
                CreateNewHistory();

            return MediaInformation.ReadFromINI(HistoryPath);
        }

        public Landing(string? playPath)
        {
            Core.Initialize();
            InitializeComponent();

            FillLastWatched(LastWatched);

            if (LastWatched.FilePath == null)
                BContinueLast.Enabled = false;

            if (playPath != null)
                PlayMediaFromString(playPath);

#if DEBUG
            this.Text = "[DEBUG]";
#endif
        }

        public MediaPlayer MakeMediaPlayer() => new(VLCLib);

        public void NextPreviousEpisode(bool next)
        {
            if (LastWatched.FilePath == null)
                return;

            var nextEpisode = next
                ? EpisodeHelper.GetNextFileAlphOrder(LastWatched.FilePath.LocalPath)
                : EpisodeHelper.GetPrevFileAlphOrder(LastWatched.FilePath.LocalPath);

            if (nextEpisode == LastWatched.FilePath.LocalPath || nextEpisode == null)
                return;

            this.LastWatched = new(new(nextEpisode), Timestamp.FromMS(0));

            VideoViewForm.Close();
            PlayMediaFromString(nextEpisode!);
            VideoViewForm.Show();
        }

        void FillLastWatched(MediaInformation source)
        {
            MIInformationPanel.EpisodeTitle = source.FileName;
            MIInformationPanel.Timestamp = source.Timestamp;
            MIInformationPanel.MediaPath = Path.GetDirectoryName(source.FilePath?.LocalPath) ?? "";
        }

        void DisableButtons()
        {
            BWatchNew.Enabled = false;
            BContinueLast.Enabled = false;
        }

        void EnableButtons()
        {
            BWatchNew.Enabled = true;
            BContinueLast.Enabled = true;
        }

        void PlayMediaFromString(string path, Timestamp? timestamp = null)
        {
            DisableButtons();
            VideoViewForm.PlayMedia(new Media(VLCLib, path), timestamp);

            if (!VideoViewForm.Visible)
                VideoViewForm.Show();
        }

        void BWatchNew_Click(object? sender, EventArgs e)
        {
            DisableButtons();

            var res = FileDialog.ShowDialog();
            if (res != DialogResult.OK)
            {
                BWatchNew.Enabled = true;
                return;
            }

            var filePath = FileDialog.FileNames[0];
            PlayMediaFromString(filePath, LastWatched.Timestamp);
        }

        void BContinueLast_Click(object? sender, EventArgs e)
        {
            DisableButtons();

            if (LastWatched.FilePath == null)
            {
                BContinueLast.Enabled = true;
                return;
            }

            PlayMediaFromString(LastWatched.FilePath.LocalPath, LastWatched.Timestamp);
        }

        void RestoreLocationHook(object? sender, EventArgs e)
        {
            if (sender is not Viewer.Viewer view)
                return;

            if (VideoViewFormRestorePosition != null)
                VideoViewForm.SetLocation((Point)VideoViewFormRestorePosition);
            else
                VideoViewFormRestorePosition = VideoViewForm.Location;

            view.Shown -= RestoreLocationHook;
        }
    }
}
