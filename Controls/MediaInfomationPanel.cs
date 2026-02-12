using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//
using ClientLib.STD;

namespace WINFORMS_VLCClient.Controls
{
    public partial class MediaInfomationPanel : UserControl
    {
        private string mediaPath = null!;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string MediaPath
        {
            get => mediaPath;
            set
            {
                this.mediaPath = value;
                this.TBMediaPath.ReadOnly = false;
                this.TBMediaPath.Text = mediaPath;
                this.TBMediaPath.ReadOnly = true;
            }
        }

        private string episodeTitle = null!;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string EpisodeTitle
        {
            get => episodeTitle;
            set
            {
                this.episodeTitle = value;
                this.TBEpisodeTitle.ReadOnly = false;
                this.TBEpisodeTitle.Text = episodeTitle;
                this.TBEpisodeTitle.ReadOnly = true;
            }
        }

        private StandardDefinitions.Timestamp timestamp = null!;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StandardDefinitions.Timestamp Timestamp
        {
            get => timestamp;
            set
            {
                this.timestamp = value;
                this.TBTimestamp.ReadOnly = false;
                this.TBTimestamp.Text = timestamp.GetFormat();
                this.TBTimestamp.ReadOnly = true;
            }
        }

        public MediaInfomationPanel()
        {
            InitializeComponent();

            MediaPath = "";
            EpisodeTitle = "";
            Timestamp = new();
        }
    }
}
