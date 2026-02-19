using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WINFORMS_VLCClient.Controls;
using static ClientLib.STD.StandardDefinitions;

namespace WINFORMS_VLCClient.Viewer
{
    public partial class Viewer
    {
        void HookControls()
        {
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
        }
    }
}
