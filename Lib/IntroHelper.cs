using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientLib.STD;
using static ClientLib.STD.StandardDefinitions;

namespace WINFORMS_VLCClient.Lib
{
    public class Intro(
        Timestamp introStart,
        Timestamp introStop,
        Timestamp diff,
        string directoryName
    )
    {
        public const string IntroFileNameSearchString = "intros.txt";
        public bool isMaster;
        public string uri = directoryName;
        public Timestamp introStart = introStart;
        public Timestamp introStop = introStop;
        public Timestamp diff = diff;

        public static Intro? GetIntroFromDirectory(string directory)
        {
            string? introPath = GetIntroPathForDirectory(directory);
            if (!Path.Exists(introPath))
                return null;

            var contents = File.ReadAllText(introPath);
            var dict = ReadINIString(contents);

            return IntroFromDictionary(dict);
        }

        public static string GetIntroPathForDirectory(string directory) =>
            Path.Combine(directory, IntroFileNameSearchString);

        public static Intro IntroFromDictionary(Dictionary<string, string> target)
        {
            if (!target.TryGetValue("introStart", out var introStart_raw))
                throw new Exception($"Failed to get introStart from dictonary!\n{target}");

            if (!target.TryGetValue("introStop", out var introStop_raw))
                throw new Exception($"Failed to get introStop from dictonary!\n{target}");

            if (!target.TryGetValue("diff", out var diff_raw))
                throw new Exception($"Failed to get diff from dictonary!\n{target}");

            if (!target.TryGetValue("uri", out var uri))
                throw new Exception($"Failed to get uri from dictionary!\n{target}");

            return new Intro(
                Timestamp.FromString(introStart_raw),
                Timestamp.FromString(introStop_raw),
                Timestamp.FromString(diff_raw),
                new Uri(uri).LocalPath
            );
        }

        public string ToINIString(bool appendNewLine = true, char delimter = '=')
        {
            var str =
                $"introStart{delimter}{introStart.GetFormat()}\nintroStop{delimter}{introStop.GetFormat()}\ndiff{delimter}{diff.GetFormat()}\nuri{delimter}{uri}";

            return appendNewLine ? str + "\n" : str;
        }
    }
}
