using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibVLCSharp.Shared;
using WINFORMS_VLCClient.Lib;
using static ClientLib.STD.StandardDefinitions;

namespace WINFORMS_VLCClient.Viewer
{
    public partial class Viewer
    {
        public void PlayMedia(Media media, Timestamp? startingPosition = null)
        {
            var player = GetNewMediaPlayer();
            RunInThreadPool(
                (_) =>
                {
                    // Have to do it this way to normalize the [media.Mrl]'s "URL-iffied" path characters
                    var fileName = Path.GetFileName(new Uri(media.Mrl).LocalPath);
#if DEBUG
                    RunSafeInvoke(this, () => this.Text = $"[DEBUG] - {fileName}");
#else
                    RunSafeInvoke(this, () => this.Text = $"Viewer - {fileName}");
#endif
                    player.Play(media);

                    if (startingPosition != null)
                        player.Time = startingPosition.ToMS();

                    // Auto link subtitles here if the name matches
                }
            );

            PollingTick();
        }

        MediaPlayer GetNewMediaPlayer()
        {
            if (CurrentPlayer != null)
                CleanupPlayer();

            VVMainView.MediaPlayer = parent!.MakeMediaPlayer();
            CurrentPlayer!.EnableKeyInput = false;
            CurrentPlayer.EnableMouseInput = false;

            CurrentPlayer.TimeChanged += OnTimeChange;
            CurrentPlayer.EndReached += (_, _) =>
                RunSafeInvoke(this, () => MediaStopped?.Invoke(this, EventArgs.Empty));

            if (
                (CurrentPlayer.Volume == 0 || CurrentPlayer.Mute)
                && TBVolumeBar.Value != 0
                && !mediaPlayerHasBeenCreated
            )
            {
                Debug.WriteLine("Set Volume to default");
                SetPlayerVolume(VOLUME_DEFAULT_PERCENTAGE);
                TBVolumeBar.Value = VOLUME_DEFAULT_PERCENTAGE;
            }
            else
                TBVolumeBar.Value = CurrentPlayer.Volume;

            mediaPlayerHasBeenCreated = true;
            return CurrentPlayer!;
        }

        void SetPlayerVolume(int to, bool fromVolumeBar = false)
        {
            RunInThreadPool(
                (_) =>
                {
                    if (CurrentPlayer == null)
                        return;

                    int newVolume = Math.Max(0, Math.Min(100, CurrentPlayer.Volume + to));

                    RunSafeInvoke(
                        this,
                        () =>
                        {
                            if (
                                (newVolume == 0 || CurrentPlayer.Mute)
                                && !VPTMainTimeline.IsVolumeMutedShown()
                            )
                                VPTMainTimeline.ShowVolumeIsMuted();
                            else if (VPTMainTimeline.IsVolumeMutedShown() && !CurrentPlayer.Mute)
                                VPTMainTimeline.ShowVolumeIsPlaying();

                            if (!fromVolumeBar)
                                TBVolumeBar.Value = to;
                        },
                        "Volume Invoke Ran!"
                    );

                    CurrentPlayer.Volume = to;
                },
                $"Volume Change: {CurrentPlayer?.Volume}->{to}"
            );
        }

        void SetPlayerTime(long to) =>
            RunInThreadPool(
                (_) =>
                {
                    if (CurrentPlayer == null)
                        return;

                    CurrentPlayer.Time = to;
                }
            );

        void TogglePausedState()
        {
            if (CurrentPlayer == null)
                return;

            SetPausedState(!CurrentPlayer.IsPlaying);
        }

        void SetPausedState(bool to) =>
            RunInThreadPool(
                (_) =>
                {
                    if (CurrentPlayer == null || to == CurrentPlayer.IsPlaying)
                        return;

                    CurrentPlayer.Pause();

                    RunSafeInvoke(
                        VPTMainTimeline,
                        () =>
                        {
                            if (VPTMainTimeline.IsVideoPausedShown())
                                VPTMainTimeline.ShowVideoIsPlaying();
                            else
                                VPTMainTimeline.ShowVideoIsPaused();
                        }
                    );
                }
            );
    }
}
