using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibVLCSharp.Shared;
using WINFORMS_VLCClient.Lib;
using static ClientLib.STD.StandardDefinitions;
using AudioTrack = WINFORMS_VLCClient.Lib.AudioTrack;

namespace WINFORMS_VLCClient.Viewer
{
    // Have to do this so this file is no longer a designer file
    [System.ComponentModel.DesignerCategory("dummy")]
    internal class Dummy_VLCPlayer();

    public partial class Viewer
    {
        void SettingsExecutor(object? sender, EventArgs e)
        {
            if (parent.Settings.showSubtitles)
                Subtitles.SetByLanguage(
                    CurrentPlayer!,
                    parent.Settings.possibleSubtitleLanguages,
                    ignore: parent.Settings.subtitleBlacklist
                );

            if (parent.Settings.possibleAudioLanguages.Count != 0)
                AudioTrack.SetByLanguage(CurrentPlayer!, parent.Settings.possibleAudioLanguages);

            PopulateLanguagesInMenu();
            CurrentPlayer!.MediaChanged -= SettingsExecutor;
        }

        public void PlayMedia(Media media, Timestamp? startingPosition = null)
        {
            var player = GetNewMediaPlayer();
            player.ESAdded += SettingsExecutor;

            RunInThreadPool(
                (_) =>
                {
                    // Have to do it this way to normalize the [media.Mrl]'s "URL-iffied" path characters
                    var uri = new Uri(media.Mrl).LocalPath;
                    if (!File.Exists(uri))
                    {
                        MessageBox.Show(
                            $"File does not exist!\nPath: \"{uri}\"",
                            "Error",
                            MessageBoxButtons.OK
                        );
                        RunSafeInvoke(this, this.Close);
                        return;
                    }

                    var fileName = Path.GetFileName(uri);
                    player.Play(media);

                    if (startingPosition != null)
                        player.Time = startingPosition.ToMS();

                    RunSafeInvoke(
                        this,
                        () =>
                        {
#if DEBUG
                            this.Text = $"[DEBUG] - {fileName}";
#else
                            this.Text = $"Viewer - {fileName}";
#endif
                        }
                    );
                }
            );

            PollingTick();
        }

        MediaPlayer GetNewMediaPlayer()
        {
            if (CurrentPlayer != null)
            {
                CleanupPlayer();
                VVMainView.MediaPlayer = null;
            }

            VVMainView.MediaPlayer = parent!.MakeMediaPlayer();
            CurrentPlayer!.EnableKeyInput = false;
            CurrentPlayer.EnableMouseInput = false;

            CurrentPlayer.TimeChanged += OnTimeChange;
            CurrentPlayer.EndReached += OnEndReachedHook;

            PopulateSubtitlesInMenu();

            if (
                (CurrentPlayer.Volume == 0 || CurrentPlayer.Mute)
                && TBVolumeBar.Value != 0
                && !mediaPlayerHasBeenCreated
            )
            {
                SetPlayerVolume(VOLUME_DEFAULT_PERCENTAGE);
                TBVolumeBar.Value = VOLUME_DEFAULT_PERCENTAGE;
                Debug.WriteLine($"INFO: Set Volume to default ({VOLUME_DEFAULT_PERCENTAGE})");
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

                    int newVolume = Math.Max(0, Math.Min(100, to));

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
                                TBVolumeBar.Value = newVolume;
                        },
                        "Volume Invoke Ran!"
                    );

                    CurrentPlayer.Volume = newVolume;
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
                            if (to)
                                VPTMainTimeline.ShowVideoIsPlaying();
                            else
                                VPTMainTimeline.ShowVideoIsPaused();
                        },
                        "Switching Paused State"
                    );
                }
            );

        void OnEndReachedHook(object? sender, EventArgs e) =>
            RunSafeInvoke(this, () => MediaStopped?.Invoke(this, EventArgs.Empty));

        void CleanupPlayer()
        {
            var player = CurrentPlayer;
            if (player == null)
                return;

            VVMainView.MediaPlayer = null;

            RunInThreadPool(_ =>
            {
                void PlayerStopped(object? sender, EventArgs e)
                {
                    player.Dispose();
                    player.Stopped -= PlayerStopped;
                }

                player.TimeChanged -= OnTimeChange;
                player.EndReached -= OnEndReachedHook;
                player.Stop();
                player.Stopped += PlayerStopped;
            });
        }
    }
}
