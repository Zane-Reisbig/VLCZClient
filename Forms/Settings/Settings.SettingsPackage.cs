using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientLib.STD;

namespace WINFORMS_VLCClient.Settings
{
    // To remove the designer
    [DesignerCategory("dummy")]
    public class SettingsDummy { };

    public class SettingsPackage
    {
        public bool showSubtitles;
        public List<string> possibleSubtitleLanguages;
        public List<string> possibleAudioLanguages;
        public List<string> subtitleBlacklist;

        public SettingsPackage()
        {
            showSubtitles = default;
            possibleSubtitleLanguages = [];
            possibleAudioLanguages = [];
            subtitleBlacklist = [];
        }

        public SettingsPackage(
            bool showSubs,
            List<string> possibleSubtitleLanguages,
            List<string> possibleAudioLanguages,
            List<string> subtitleBlacklist
        )
        {
            this.showSubtitles = showSubs;
            this.possibleSubtitleLanguages = possibleSubtitleLanguages;
            this.possibleAudioLanguages = possibleAudioLanguages;
            this.subtitleBlacklist = subtitleBlacklist;
        }

        public SettingsPackage(
            bool showSubs,
            string delimitedPossibleSubtitleLanguages,
            string delimitedPossibleAudioLanguages,
            string delimitedSubtitleBlacklist,
            char delim = ';'
        )
        {
            this.showSubtitles = showSubs;
            this.possibleSubtitleLanguages = [.. delimitedPossibleSubtitleLanguages.Split(delim)];
            this.possibleAudioLanguages = [.. delimitedPossibleAudioLanguages.Split(delim)];
            this.subtitleBlacklist = [.. delimitedSubtitleBlacklist.Split(delim)];
        }

        public static SettingsPackage? ReadFromFile(string path)
        {
            if (!File.Exists(path))
                return null;

            var iniDict = StandardDefinitions.ReadINIString(File.ReadAllText(path));

            iniDict.TryGetValue("showSubtitles", out var showSubtitles);
            iniDict.TryGetValue("possibleSubtitleLanguages", out var possibleSubtitleLanguages);
            iniDict.TryGetValue("possibleAudioLanguages", out var possibleAudioLanguages);
            iniDict.TryGetValue("subtitleBlacklist", out var subtitleBlacklist);

            if (showSubtitles == null)
                throw new Exception($"Failed to get \"showSubtitles\" from path - \"{path}\"");

            if (possibleSubtitleLanguages == null)
                throw new Exception(
                    $"Failed to get \"possibleSubtitleLanguages\" from path - \"{path}\""
                );

            if (possibleAudioLanguages == null)
                throw new Exception(
                    $"Failed to get \"possibleAudioLanguages\" from path - \"{path}\""
                );

            if (subtitleBlacklist == null)
                throw new Exception($"Failed to get \"subtitleBlacklist\" from path - \"{path}\"");

            return new SettingsPackage(
                bool.Parse(showSubtitles),
                possibleSubtitleLanguages,
                possibleAudioLanguages,
                subtitleBlacklist,
                ';'
            );
        }

        public void WriteToFile(string path)
        {
            var _out = new Dictionary<string, string>()
            {
                { "showSubtitles", this.showSubtitles.ToString() },
                {
                    "possibleSubtitleLanguages",
                    string.Join(';', possibleSubtitleLanguages.Select(val => val.ToLower()))
                },
                {
                    "possibleAudioLanguages",
                    string.Join(';', possibleAudioLanguages.Select(val => val.ToLower()))
                },
                {
                    "subtitleBlacklist",
                    string.Join(';', subtitleBlacklist.Select(val => val.ToLower()))
                },
            };

            StandardDefinitions.WriteDictToINIFile(path, _out);
        }
    }
}
