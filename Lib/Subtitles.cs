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

        static ManualResetEventSlim? GetSubtitlesEvent;
        static ManualResetEventSlim? SetSubtitlesEvent;

        public static TrackDescription[]? GetEmbeddedMediaSubtitles(MediaPlayer? player)
        {
            if (GetSubtitlesEvent != null || player == null)
                return null;

            GetSubtitlesEvent = new();

            TrackDescription[]? tracks = null;
            StandardDefinitions.RunInThreadPool(
                (_) =>
                {
                    tracks = player.SpuDescription;
                    GetSubtitlesEvent.Set();
                },
                "Getting Subtitle Description"
            );

            GetSubtitlesEvent.Wait();
            GetSubtitlesEvent = null;

            return tracks;
        }

        public static void SetByLanguage(
            MediaPlayer player,
            List<string> possibleLanguages,
            List<string>? ignore = null
        )
        {
            var subs = GetEmbeddedMediaSubtitles(player);
            if (subs == null)
                return;

            Debug.WriteLine($"All Tracks: {String.Join(", ", subs.Select(sub => sub.Name))}");

            var _ignore = ignore != null ? ignore.Select(i => i.ToLower()) : [];
            var ourLang = subs.Where(
                    (sub) =>
                    {
                        var good = false;

                        foreach (var langCode in possibleLanguages)
                        {
                            var pred = sub.Name.ToLower();
                            if (pred.Contains(langCode))
                            {
                                good = true;
                                break;
                            }
                        }

                        foreach (var blackListed in _ignore)
                        {
                            var pred = sub.Name.ToLower();
                            if (pred.Contains(blackListed))
                            {
                                good = false;
                                break;
                            }
                        }

                        return good;
                    }
                )
                .ToList();

            if (ourLang == null || ourLang.Count == 0)
                return;

            Debug.WriteLine($"Found Sub Tracks: {String.Join(", ", ourLang)}");

            SetSubtitleTrackByID(player, ourLang[0].Id);
        }

        public static void SetSubtitleTrackByID(MediaPlayer player, int id)
        {
            StandardDefinitions.RunInThreadPool(
                (_) =>
                {
                    var og = player.Spu;
                    player.SetSpu(id);
                    Debug.WriteLine($"INFO: Sub Track changed! {og}->{id}");
                }
            );
        }

        public static bool Load(MediaPlayer player, string filePath, bool show = true)
        {
            if (!Path.Exists(filePath) || !GoodFileExtensions.Contains(Path.GetExtension(filePath)))
                return false;

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

            return true;
        }

        public static void OnSubtitleAdded(MediaPlayer player) =>
            SubtitleAdded?.Invoke(player, EventArgs.Empty);
    }
}
