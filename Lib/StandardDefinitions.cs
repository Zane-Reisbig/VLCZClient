using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib.STD
{
    public static class StandardDefinitions
    {
        public enum TimestampFormat
        {
            FFMPEG,
        }

        public static bool isCursorInForm(Form target)
        {
            if (target.IsDisposed || target.Disposing)
                return false;

            var clientCursorPos = target.PointToClient(Cursor.Position);
            return target.ClientRectangle.Contains(clientCursorPos);
        }

        public static string PadZero(object to, int maxLength = 2)
        {
            string strRep = to.ToString()!;
            if (strRep.Length > maxLength)
                return strRep;

            string _out = new string('0', maxLength + strRep.Length) + strRep;
            return _out.Substring(_out.Length - maxLength);
        }

        public static Dictionary<string, string> ReadINIString(string source, char delimiter = '=')
        {
            var lines = source.Split("\n");

            Dictionary<string, string> _out = new();
            foreach (string line in lines)
            {
                if (line == "")
                    continue;

                var pairs = line.Split(delimiter);
                if (pairs.Length != 2)
                    throw new Exception($"Malformed K/V Pair!\nOffending: \"{line}\"");

                _out[pairs[0]] = pairs[1];
            }

            return _out;
        }

        public static void WriteDictToINIFile<K, V>(
            string path,
            Dictionary<K, V> source,
            char delimter = '='
        )
            where K : notnull
        {
            string fileContents = "";
            foreach (var key in source.Keys)
                fileContents += $"{key}{delimter}{source[key]}\n";

            File.WriteAllText(path, fileContents);
        }

        public class Timestamp(int hours = 0, int minute = 0, int second = 0, int frames = 0)
        {
            public static Timestamp FromMS(long ms)
            {
                int totalSeconds = (int)ms / 1000;
                int hours = totalSeconds / 3600;
                int minutes = (totalSeconds % 3600) / 60;
                int seconds = totalSeconds % 60;
                return new Timestamp(hours, minutes, seconds, 0);
            }

            public static Timestamp Diff(Timestamp? source, Timestamp? other)
            {
                if (source == null || other == null)
                    return new Timestamp();

                int diffFrames = source.frames - other.frames;
                int diffSeconds = source.second - other.second;
                int diffMinutes = source.minute - other.minute;
                int diffHours = source.hours - other.hours;
                //
                // csharpier-ignore-start
                if (diffFrames < 0) { diffFrames += 60; diffSeconds--; }
                if (diffSeconds < 0) { diffSeconds += 60; diffMinutes--; }
                if (diffMinutes < 0) { diffMinutes += 60; diffHours--; }
                // csharpier-ignore-end
                //

                return new Timestamp(diffHours, diffMinutes, diffSeconds, diffFrames);
            }

            public static Timestamp FromString(string? source)
            {
                if (source == null)
                    throw new Exception("Cannot make timestamp from null!");

                if (source == "None")
                    return new Timestamp();

                string[] hmsf = source.Split(':');
                int hours;
                if (!int.TryParse(hmsf[0], out hours))
                    throw new Exception($"Failed to parse hours!\nFound \"{hmsf[0]}\"");

                int minute;
                if (!int.TryParse(hmsf[1], out minute))
                    throw new Exception($"Failed to parse minutes!\nFound \"{hmsf[1]}\"");

                int second;
                if (!int.TryParse(hmsf[2], out second))
                    throw new Exception($"Failed to parse second!\nFound \"{hmsf[2]}\"");

                int frames;
                if (!int.TryParse(hmsf[3], out frames))
                    throw new Exception($"Failed to parse frames!\nFound \"{hmsf[3]}\"");

                return new Timestamp(hours, minute, second, frames);
            }

            public int hours = hours;
            public int minute = minute;
            public int second = second;
            public int frames = frames;

            public string GetFormat(TimestampFormat format = TimestampFormat.FFMPEG)
            {
                switch (format)
                {
                    case TimestampFormat.FFMPEG:
                        return $"{PadZero(hours)}:{PadZero(minute)}:{PadZero(second)}:{PadZero(frames)}";
                    default:
                        throw new Exception($"Format \"{format.ToString()}\" not recognized!");
                }
            }

            public long ToMS() => (long)((hours * 3600 + minute * 60 + second) * 1000 + frames);
        }
    }
}
