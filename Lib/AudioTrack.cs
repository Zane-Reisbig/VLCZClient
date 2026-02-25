using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ClientLib.STD;
using LibVLCSharp.Shared;

namespace WINFORMS_VLCClient.Lib
{
    public class AudioTrack
    {
        public string lang;
        public int id;

        public AudioTrack(string name, int id)
        {
            this.lang = name;
            this.id = id;
        }

        static ManualResetEventSlim? tracksEvent = null;

        public static List<AudioTrack> GetAllAudioTracks(Media media)
        {
            if (tracksEvent != null)
                return [];

            tracksEvent = new();

            List<AudioTrack>? tracks = null;

            StandardDefinitions.RunInThreadPool(_ =>
            {
                tracks = media
                    .Tracks.ToList()
                    .Where(t => t.TrackType == TrackType.Audio)
                    .Select(t => new AudioTrack(t.Language ?? "Unknown", t.Id))
                    .ToList();

                tracksEvent.Set();
            });

            tracksEvent.Wait();

            if (tracks == null)
                throw new Exception("Failed to get tracks from media!");

            tracksEvent = null;
            return tracks;
        }

        public static void SetByLanguage(MediaPlayer player, List<string> languages)
        {
            if (player.Media == null)
                return;

            var allTracks = GetAllAudioTracks(player.Media);
            if (allTracks == null)
                return;

            var ourTracks = allTracks
                .Where(track => languages.Contains(track.lang.ToLower()))
                .ToList();
            if (ourTracks == null || ourTracks.Count == 0)
                return;

            SetAudioTrack(player, ourTracks[0].id);
        }

        public static void SetAudioTrack(MediaPlayer player, int trackID)
        {
            StandardDefinitions.RunInThreadPool(_ =>
            {
                if (player.SetAudioTrack(trackID))
                {
                    Debug.WriteLine($"INFO: Set Audio Track to id: {trackID}");
                    return;
                }

                Debug.WriteLine($"WARN: Failed to set Audio Track to id: {trackID}");
            });
        }
    }
}
