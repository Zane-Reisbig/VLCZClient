using ClientLib.STD;
using WINFORMS_VLCClient.Lib;
using WINFORMS_VLCClient.Lib.Hotkeys;

namespace WINFORMS_VLCClient.Viewer
{
    public partial class Viewer : Form
    {
        int systemHotkeyF9PauseID;
        int systemHotkeyMediaPauseID;

        void HookSystemHotkeys()
        {
            systemHotkeyF9PauseID = new Random().Next(int.MaxValue);
            HotkeyHelper.RegisterHotkey(
                this.Handle,
                systemHotkeyF9PauseID,
                HotkeyHelper.MOD_ALT,
                (uint)Keys.F9
            );

            systemHotkeyMediaPauseID = new Random().Next(int.MaxValue);
            HotkeyHelper.RegisterHotkey(
                this.Handle,
                systemHotkeyMediaPauseID,
                HotkeyHelper.MOD_NONE,
                (uint)Keys.MediaPlayPause
            );
        }

        void ScrollWheelSeek(object? sender, MouseEventArgs e)
        {
            if (CurrentPlayer == null)
                return;

            if (e.Delta < 0)
                SetPlayerTime(CurrentPlayer.Time + SCROLL_SEEK_MS);
            else
                SetPlayerTime(CurrentPlayer.Time - SCROLL_SEEK_MS);
        }

        void VVMainView_Click(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            TogglePausedState();
        }

        void VVMainView_DoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ToggleWindowFullScreenState();
                TogglePausedState();
                return;
            }
            else
                SelectSubtitleTrack();
        }

        void SeekMediaMouseEvent(object? sender, MouseEventArgs e)
        {
            if (CurrentPlayer == null || e.Button != MouseButtons.Left)
                return;

            var mediaSize = CurrentPlayer.Length;
            if (mediaSize == 0)
                return;

            long requestedTime = (long)(
                mediaSize * VPTMainTimeline.ClickPointToBarPercentage(e.Location)
            );

            if (requestedTime >= mediaSize)
            {
                const int REWIND_AFTER_END_MS = 10 * 1000;
                SetPlayerTime(requestedTime - REWIND_AFTER_END_MS);
                SetPausedState(true);
            }
            else
            {
                SetPlayerTime(requestedTime);
            }
        }

        void ShowTimeline(object? sender, EventArgs e) => SetTimelineState(true);

        void HideTimeline(object? sender, EventArgs e) => SetTimelineState(false);

        void SetTimelineState(bool to)
        {
            if (!to && StandardDefinitions.IsCursorInForm(this))
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

        protected override bool ProcessCmdKey(ref Message msg, Keys keydata)
        {
            if (CurrentPlayer == null)
                return base.ProcessCmdKey(ref msg, keydata);

            switch (keydata)
            {
                case Keys.Right:
                    SetPlayerTime(CurrentPlayer.Time + ARROW_SEEK_MS);
                    return true;
                case Keys.Left:
                    SetPlayerTime(CurrentPlayer.Time - ARROW_SEEK_MS);
                    return true;
                case Keys.Up:
                    SetPlayerVolume(TBVolumeBar.Value + VOLUME_ARROW_CHANGE_PERCENT);
                    return true;
                case Keys.Down:
                    SetPlayerVolume(TBVolumeBar.Value - VOLUME_ARROW_CHANGE_PERCENT);
                    return true;
                case Keys.F11:
                    ToggleWindowFullScreenState();
                    return true;
                case Keys.Space:
                    TogglePausedState();
                    return true;
                case Keys.Escape:
                    if (!isFullScreen)
                        return false;

                    SetWindowFullScreenState(false);

                    return true;
            }

            return base.ProcessCmdKey(ref msg, keydata);
        }
    }
}
