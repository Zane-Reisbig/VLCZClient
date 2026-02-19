using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientLib.STD;
using LibVLCSharp.Shared.Structures;
using WINFORMS_VLCClient.Controls;
using WINFORMS_VLCClient.Lib;
using static ClientLib.STD.StandardDefinitions;

namespace WINFORMS_VLCClient.Viewer
{
    // Have to do this so this file is no longer a designer file
    [System.ComponentModel.DesignerCategory("dummy")]
    internal class DummyControls();

    public partial class Viewer
    {
        void HookControls()
        {
            VVMainView.MouseEnter += ShowTimeline;
            VVMainView.MouseLeave += HideTimeline;

            VPTMainTimeline.MouseDidMove += SeekMediaMouseEvent;
            VPTMainTimeline.MouseEnter += ShowTimeline;
            VPTMainTimeline.ScrollWheelScrolled += ScrollWheelSeek;
            VPTMainTimeline.MuteButtonClicked += OnMuteButtonClicked;
            VPTMainTimeline.PauseButtonClicked += OnPauseButtonClicked;
            VPTMainTimeline.NextButtonClicked += OnNextButtonClicked;
            VPTMainTimeline.PreviousButtonClicked += OnPreviousButtonClicked;

            TBVolumeBar.ValueChanged += OnVolumeBarValueChanged;

            TSMISubtitle.MouseHover += ShowSubtitleBox;
            Subtitles.SubtitleAdded += PopulateSubtitlesInMenuHook;

            MKRIntroSkip.MarkConfirmed += ConfirmIntro;
            MKRIntroSkip.ExitButtonClicked += HideIntroSkipMaker;
            MKRIntroSkip.TimestampGetter = GetCurrentTimestamp;
            MKRIntroSkip.Hide();

            BRecordIntro.Click += ShowIntroSkipMaker;
            BSkipIntro.Click += DoSkipIntro;
            PSkipIntroButtonContainer.Show();
        }

        void UnhookControls()
        {
            VVMainView.MouseEnter -= ShowTimeline;
            VVMainView.MouseLeave -= HideTimeline;

            VPTMainTimeline.MouseDidMove -= SeekMediaMouseEvent;
            VPTMainTimeline.MouseEnter -= ShowTimeline;
            VPTMainTimeline.ScrollWheelScrolled -= ScrollWheelSeek;
            VPTMainTimeline.MuteButtonClicked -= OnMuteButtonClicked;
            VPTMainTimeline.PauseButtonClicked -= OnPauseButtonClicked;
            VPTMainTimeline.NextButtonClicked -= OnNextButtonClicked;
            VPTMainTimeline.PreviousButtonClicked -= OnPreviousButtonClicked;

            TBVolumeBar.ValueChanged -= OnVolumeBarValueChanged;

            TSMISubtitle.MouseHover -= ShowSubtitleBox;
            Subtitles.SubtitleAdded -= PopulateSubtitlesInMenuHook;

            ToolStripMenuItem last = (ToolStripMenuItem)TSMISubtitle.DropDownItems[^1];
            foreach (ToolStripMenuItem item in TSMISubtitle.DropDownItems)
            {
                if (item == last)
                    continue;

                item.MouseUp -= EnableSubtitle;
            }
            last.MouseUp -= SelectSubtitleTrackHook;

            MKRIntroSkip.MarkConfirmed -= ConfirmIntro;
            MKRIntroSkip.ExitButtonClicked -= HideIntroSkipMaker;
            MKRIntroSkip.TimestampGetter = GetCurrentTimestamp;

            BRecordIntro.Click -= ShowIntroSkipMaker;
            BSkipIntro.Click -= DoSkipIntro;
        }

        private void ShowSubtitleBox(object? sender, EventArgs e) => TSMISubtitle.ShowDropDown();

        void PopulateSubtitlesInMenuHook(object? sender, EventArgs e) =>
            RunSafeInvoke(this, PopulateSubtitlesInMenu);

        void PopulateSubtitlesInMenu()
        {
            if (CurrentPlayer == null)
                return;

            TSMISubtitle.DropDownItems.Clear();

            var haveSubtitles = Subtitles.GetEmbeddedMediaSubtitles(CurrentPlayer);
            if (haveSubtitles == null)
            {
                Debug.WriteLine("WARN: No subtitles found on media!");
                return;
            }

            List<ToolStripMenuItem> subtitles = [];
            foreach (var subtitle in haveSubtitles)
            {
                ToolStripMenuItem item = new()
                {
                    Text = $"[{subtitle.Id}] - {subtitle.Name}",
                    Tag = subtitle.Id - 1,
                };
                item.MouseUp += EnableSubtitle;

                subtitles.Add(item);
            }

            ToolStripMenuItem customSub = new() { Text = $"[-99] - Custom", Tag = -99 };
            customSub.MouseUp += SelectSubtitleTrackHook;
            subtitles.Add(customSub);

            TSMISubtitle.DropDownItems.AddRange([.. subtitles]);
        }

        void SelectSubtitleTrackHook(object? sender, EventArgs e) => SelectSubtitleTrack();

        void SelectSubtitleTrack()
        {
            if (CurrentPlayer == null)
                return;

            if (FileDialog.ShowDialog() != DialogResult.OK)
                return;

            var file = FileDialog.FileNames[0];
            Subtitles.Load(CurrentPlayer, file);
        }

        void EnableSubtitle(object? sender, MouseEventArgs e)
        {
            if (sender is not ToolStripMenuItem menuItem || CurrentPlayer == null)
                return;

            var subId = (int)menuItem.Tag!;
            Subtitles.SetSubtitleTrackByID(CurrentPlayer, subId);
        }

        void ShowTimeline(object? sender, EventArgs e) => SetTimelineState(true);

        void HideTimeline(object? sender, EventArgs e) => SetTimelineState(false);

        void SetTimelineState(bool to)
        {
            if (!to && StandardDefinitions.IsCursorInControl(this))
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

        private void OnMuteButtonClicked(object? sender, EventArgs e)
        {
            CurrentPlayer?.ToggleMute();
        }

        private void OnPauseButtonClicked(object? sender, EventArgs e)
        {
            CurrentPlayer?.Pause();
        }

        private void OnNextButtonClicked(object? sender, EventArgs e)
        {
            NextButton?.Invoke(this, EventArgs.Empty);
        }

        private void OnPreviousButtonClicked(object? sender, EventArgs e)
        {
            PrevButton?.Invoke(this, EventArgs.Empty);
        }

        private void OnVolumeBarValueChanged(object? sender, EventArgs e)
        {
            SetPlayerVolume(TBVolumeBar.Value, fromVolumeBar: true);
        }

        private Timestamp? GetCurrentTimestamp()
        {
            return CurrentPlayer != null ? Timestamp.FromMS(CurrentPlayer.Time) : null;
        }
    }
}
