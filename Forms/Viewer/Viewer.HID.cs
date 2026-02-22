using ClientLib.STD;
using WINFORMS_VLCClient.Lib;
using WINFORMS_VLCClient.Lib.Hotkeys;

namespace WINFORMS_VLCClient.Viewer
{
    // Have to do this so this file is no longer a designer file
    [System.ComponentModel.DesignerCategory("dummy")]
    internal class Dummy();

    public partial class Viewer : Form
    {
        int systemHotkeyF9PauseID;
        int systemHotkeyMediaPauseID;

        void HookSystemHotkeys()
        {
            if (systemHotkeyF9PauseID != default)
                HotkeyHelper.UnregisterHotkey(this.Handle, systemHotkeyF9PauseID);

            systemHotkeyF9PauseID = new Random().Next(int.MaxValue);
            HotkeyHelper.RegisterHotkey(
                this.Handle,
                systemHotkeyF9PauseID,
                HotkeyHelper.MOD_ALT,
                (uint)Keys.F9
            );

            if (systemHotkeyMediaPauseID != default)
                HotkeyHelper.UnregisterHotkey(this.Handle, systemHotkeyMediaPauseID);

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
            if (e.Button == MouseButtons.Left)
                TogglePausedState();

            if (e.Button == MouseButtons.Right)
            {
                if (CMSLeftClickMenu.Visible)
                    CMSLeftClickMenu.Hide();

                if (TSMISubtitle.DropDownItems.Count == 1)
                    PopulateSubtitlesInMenu();

                var client = VVMainView.PointToScreen(new Point(e.X, e.Y));
                CMSLeftClickMenu.Show(client.X, client.Y);
            }
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
                LoadSubtitleFromFile();
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
                SetPlayerTime(requestedTime - REWIND_AFTER_END_MS);
                SetPausedState(true);
            }
            else
            {
                SetPlayerTime(requestedTime);
            }
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
