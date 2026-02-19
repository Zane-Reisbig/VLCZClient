using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ClientLib.STD;
using LibVLCSharp.Shared;
using LibVLCSharp.Shared.Structures;

namespace WINFORMS_VLCClient.Lib
{
    public static class Subtitles
    {
        public static readonly string[] GoodFileExtensions = [".srt"];

        public static EventHandler? SubtitleAdded;

        static ManualResetEventSlim? GetSubtitlesMutex;
        static ManualResetEventSlim? SetSubtitlesMutex;

        public static TrackDescription[]? GetEmbeddedMediaSubtitles(MediaPlayer? player)
        {
            if (GetSubtitlesMutex != null || player == null)
                return null;

            GetSubtitlesMutex = new();

            TrackDescription[]? tracks = null;
            StandardDefinitions.RunInThreadPool(
                (_) =>
                {
                    tracks = player.SpuDescription;
                    GetSubtitlesMutex.Set();
                },
                "Getting Subtitle Description"
            );

            GetSubtitlesMutex.Wait();
            GetSubtitlesMutex = null;

            return tracks;
        }

        public static bool SetSubtitleTrackByID(MediaPlayer player, int id)
        {
            if (SetSubtitlesMutex != null)
                return false;

            SetSubtitlesMutex = new();

            bool outResult = false;
            StandardDefinitions.RunInThreadPool(
                (_) =>
                {
                    var og = player.Spu;
                    var allSubs = GetEmbeddedMediaSubtitles(player);

                    if (allSubs == null)
                    {
                        Debug.WriteLine($"WARN: No Subtitles returned!");
                        return;
                    }

                    TrackDescription? trackDescription = null;
                    try
                    {
                        if (0 > id)
                            trackDescription = allSubs[^Math.Abs(id)];
                        else
                            trackDescription = allSubs[id];
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Debug.WriteLine($"WARN: {id} out of range {player.SpuCount}");
                        return;
                    }

                    player.SetSpu(trackDescription.Value.Id);
                    Debug.WriteLine($"INFO: Sub Track changed! {og}->{trackDescription.Value.Id}");
                    outResult = true;
                    SetSubtitlesMutex.Set();
                }
            );

            SetSubtitlesMutex.Wait();
            SetSubtitlesMutex = null;

            return outResult;
        }

        public static void Load(MediaPlayer player, string filePath, bool show = true)
        {
            if (!Path.Exists(filePath) || !GoodFileExtensions.Contains(Path.GetExtension(filePath)))
                return;

            StandardDefinitions.RunInThreadPool(
                (_) =>
                {
                    player.AddSlave(
                        MediaSlaveType.Subtitle,
                        filePath.StartsWith("file:///") ? filePath : $"file:///{filePath}",
                        false
                    );

                    player.Pause();
                    Thread.Sleep(500);
                    player.Play();

                    if (show)
                        SetSubtitleTrackByID(player, -1);

                    OnSubtitleAdded(player);
                }
            );
        }

        public static void OnSubtitleAdded(MediaPlayer player) =>
            SubtitleAdded?.Invoke(player, EventArgs.Empty);
    }
}
