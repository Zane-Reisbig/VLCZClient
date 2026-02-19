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
                    vlcLib = new LibVLC("--vout=direct3d11", "--direct3d11-hw-blending");

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
                    videoViewForm.MediaStopped += (_, _) => ChangeMedia(forward: true);
                    videoViewForm.NextButton += (_, _) => ChangeMedia(forward: true);
                    videoViewForm.PrevButton += (_, _) => ChangeMedia(forward: false);
                    videoViewForm.FormClosed += (_, _) =>
                    {
                        FillLastWatched(LastWatched);
                        BContinueLast.Enabled = true;
                        BWatchNew.Enabled = true;
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
            StandardDefinitions.WriteDictToINIFile<string, string>(
                HistoryPath,
                new() { { "path", "None" }, { "timestamp", "None" } }
            );

        static MediaInformation GetLastWatched()
        {
            if (!Path.Exists(HistoryPath))
                CreateNewHistory();

            return MediaInformation.ReadFromINI(HistoryPath);
        }

        public Landing()
        {
            Core.Initialize();
            InitializeComponent();

            FillLastWatched(LastWatched);

            if (LastWatched.FilePath == null)
                BContinueLast.Enabled = false;

#if DEBUG
            this.Text = "[DEBUG]";
#endif
        }

        public MediaPlayer MakeMediaPlayer() => new(VLCLib);

        public void ChangeMedia(bool forward)
        {
            if (LastWatched.FilePath == null)
                return;

            var nextEpisode = forward
                ? EpisodeHelper.GetNextFileAlphOrder(LastWatched.FilePath.LocalPath)
                : EpisodeHelper.GetPrevFileAlphOrder(LastWatched.FilePath.LocalPath);

            if (nextEpisode == LastWatched.FilePath.LocalPath || nextEpisode == null)
                return;

            this.LastWatched = new(new(nextEpisode), Timestamp.FromMS(0));

            VideoViewForm.Close();
            VideoViewForm.PlayMedia(new Media(VLCLib, nextEpisode!));
            VideoViewForm.Show();
        }

        void FillLastWatched(MediaInformation source)
        {
            MIInformationPanel.EpisodeTitle = source.FileName;
            MIInformationPanel.Timestamp = source.Timestamp;
            MIInformationPanel.MediaPath = Path.GetDirectoryName(source.FilePath?.LocalPath) ?? "";
        }

        void BWatchNew_Click(object? sender, EventArgs e)
        {
            BWatchNew.Enabled = false;
            BContinueLast.Enabled = false;

            var res = FileDialog.ShowDialog();
            if (res != DialogResult.OK)
            {
                BWatchNew.Enabled = true;
                return;
            }

            var filePath = FileDialog.FileNames[0];
            VideoViewForm.PlayMedia(new Media(VLCLib, filePath));

            if (!VideoViewForm.Visible)
                VideoViewForm.Show();
        }

        void BContinueLast_Click(object? sender, EventArgs e)
        {
            BContinueLast.Enabled = false;
            BWatchNew.Enabled = false;

            if (LastWatched.FilePath == null)
            {
                BContinueLast.Enabled = true;
                return;
            }

            VideoViewForm.PlayMedia(new Media(VLCLib, LastWatched.FilePath), LastWatched.Timestamp);

            if (!VideoViewForm.Visible)
                VideoViewForm.Show();
        }

        void RestoreLocationHook(object? sender, EventArgs e)
        {
            if (sender is not Viewer.Viewer view)
                return;

            if (VideoViewFormRestorePosition != null)
                VideoViewForm.SetLocation((Point)VideoViewFormRestorePosition);
            // Have to do this to get the VideoViewForm out of its null state
            //  for the first launch
            else
                VideoViewForm.SetLocation(VideoViewForm.Location);

            view.Shown -= RestoreLocationHook;
        }
    }
}
