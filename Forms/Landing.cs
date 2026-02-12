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
                    videoViewForm = new(this);

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

            lastWatched = GetLastWatched();
            FillLastWatched(lastWatched);

#if DEBUG
            BContinueLast_Click(BContinueLast, null);
            VideoViewForm.Timeline.MuteButtonClicked?.Invoke(null, null);
#endif
        }

        public MediaPlayer RequestMediaPlayer(VideoView outputsource)
        {
            var player = new MediaPlayer(VLCLib)
            {
                EnableKeyInput = false,
                EnableMouseInput = false,
            };
            outputsource.MediaPlayer = player;

            return player;
        }

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
            MIInformationPanel.MediaPath = Path.GetDirectoryName(source.FilePath.LocalPath) ?? "";
        }

        void PlayMedia(Uri media, StandardDefinitions.Timestamp? startingPos = null)
        {
            if (!VideoViewForm.Visible)
                VideoViewForm.Show(this);

            VideoViewForm.PlayMedia(new Media(VLCLib, media), startingPos);
        }

        void BWatchNew_Click(object sender, EventArgs e)
        {
            var _sender = (Control)sender;
            _sender.Enabled = false;

            var dialogRes = FileDialog.ShowDialog(this);
            if (!dialogRes.HasFlag(DialogResult.OK))
            {
                _sender.Enabled = true;
                return;
            }

            PlayMedia(new Uri(FileDialog.FileNames[0]));
            _sender.Enabled = true;
        }

        void BContinueLast_Click(object sender, EventArgs e)
        {
            var _sender = (Control)sender;
            _sender.Enabled = false;
            PlayMedia(LastWatched.FilePath, LastWatched.Timestamp);
            _sender.Enabled = true;
        }
    }
}
