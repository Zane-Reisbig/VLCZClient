using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WINFORMS_VLCClient.Settings
{
    public partial class SettingsForm : Form
    {
        Landing parent;
        SettingsPackage initalState;

        public SettingsForm(Landing parent)
        {
            InitializeComponent();
            this.parent = parent;

            this.initalState = parent.Settings;
            SetFormStateFromPackage(initalState);
        }

        public void SetFormStateFromPackage(SettingsPackage package)
        {
            this.TBAudioDefaultLanguage.Text = String.Join(';', package.possibleAudioLanguages);
            this.TBSubtitleDefaultLanguage.Text = String.Join(
                ';',
                package.possibleSubtitleLanguages
            );
            this.TBSubtitleBlacklist.Text = String.Join(";", package.subtitleBlacklist);
            this.CBUseSubsIfAvailable.Checked = package.showSubtitles;
        }

        public SettingsPackage GetPackageFromFormState() =>
            new SettingsPackage(
                this.CBUseSubsIfAvailable.Checked,
                this.TBAudioDefaultLanguage.Text,
                this.TBSubtitleDefaultLanguage.Text,
                this.TBSubtitleBlacklist.Text,
                ';'
            );

        void Close_Click(object sender, EventArgs e)
        {
            parent.Settings = GetPackageFromFormState();
            this.Close();
        }

        void Reset_Click(object sender, EventArgs e) => SetFormStateFromPackage(this.initalState);
    }
}
