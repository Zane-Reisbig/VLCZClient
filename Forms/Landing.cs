using System.ComponentModel;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using ClientLib.STD;
using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using WINFORMS_VLCClient.Controls;
using WINFORMS_VLCClient.Forms;
using WINFORMS_VLCClient.Lib.MediaInformation;

namespace WINFORMS_VLCClient
{
    public partial class Landing : Form
    {
        static readonly string historyFileName = "history.txt";

        string HistoryPath
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
                if (lastWatched == null)
                    lastWatched = GetLastWatched();

                return lastWatched;
            }
            set
            {
                lastWatched = value;
                lastWatched.WriteToFile(HistoryPath);
            }
        }

        Viewer? videoViewForm;
        Viewer VideoViewForm
        {
            get
            {
                if (videoViewForm == null || videoViewForm.IsDisposed)
                {
                    videoViewForm = new(this);
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
                if (fileDialog == null)
                {
                    fileDialog = new();
                    fileDialog.InitialDirectory = "C://";
                    fileDialog.Filter =
                        "Video Files|*.mp4;*.mov;*.wmv;*.avi;*.mkv;*.flv|All Files (*.*)|*.*";
                }

                return fileDialog;
            }
        }

        public Landing()
        {
            Core.Initialize();
            InitializeComponent();

            FillLastWatched(LastWatched);

            if (LastWatched.FilePath == null)
                BContinueLast.Enabled = false;
        }

        public MediaPlayer MakeMediaPlayer() => new MediaPlayer(VLCLib);

        void CreateNewHistory()
        {
            string contents = "path=None\n";
            contents += "timestamp=None\n";

            File.WriteAllText(HistoryPath, contents);
        }

        MediaInformation GetLastWatched()
        {
            if (!Path.Exists(HistoryPath))
                CreateNewHistory();

            return MediaInformation.ReadFromINI(HistoryPath);
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
    }
}
