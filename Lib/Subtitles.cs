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
    public class Subtitles
    {
        public static readonly string[] GoodFileExtensions = [".srt"];

        public static void Hide(MediaPlayer player) =>
            StandardDefinitions.RunInThreadPool((_) => player.SetSpu(-1));

        public static ManualResetEventSlim? GetSubtitlesMutex;

        public static TrackDescription[]? GetEmbeddedMediaSubtitles(MediaPlayer player)
        {
            if (GetSubtitlesMutex != null)
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

        public static void Load(MediaPlayer player, string filePath)
        {
            if (!Path.Exists(filePath) || !GoodFileExtensions.Contains(Path.GetExtension(filePath)))
                return;

            StandardDefinitions.RunInThreadPool(
                (_) =>
                {
                    player.AddSlave(
                        MediaSlaveType.Subtitle,
                        filePath.StartsWith("file:///") ? filePath : $"file:///{filePath}",
                        true
                    );

                    foreach (var x in player.SpuDescription)
                        Debug.WriteLine($"SubtitleTrack({x.Name}, {x.Id})");
                }
            );
        }
    }
}
