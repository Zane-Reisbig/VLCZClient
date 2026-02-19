using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

        public static bool IsCursorInControl(Control target)
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
            return _out[^maxLength..];
        }

        public static void RunInThreadPool(WaitCallback callable, string? debugLabel = null)
        {
            Debug.WriteLineIf(debugLabel != null, $"INFO: Running Pooled Job: {debugLabel}");
            ThreadPool.QueueUserWorkItem(
                (input) =>
                {
                    callable(input);
                    Debug.WriteLineIf(debugLabel != null, $"INFO: \"{debugLabel}\" DONE!");
                }
            );
        }

        public static void RunSafeInvoke(Control target, Action callable, string? debugLabel = null)
        {
            if (target.Disposing || target.IsDisposed || !target.IsHandleCreated)
            {
                Debug.Write($"WARN: Invoke Job \"{debugLabel}\" FAILED!");
                return;
            }

            Debug.WriteLineIf(debugLabel != null, $"INFO: Running Invoke Job: {debugLabel}");

            try
            {
                target.Invoke(() =>
                {
                    callable();
                    Debug.WriteLineIf(debugLabel != null, $"INFO: \"{debugLabel}\" DONE!");
                });
            }
            catch (ObjectDisposedException)
            {
                Debug.WriteLine("WARN: WinForms Object was disposed mid-Invoke!\nMEMORY MAY LEAK!");
            }
            catch (NullReferenceException)
            {
                Debug.WriteLine("WARN: Player was disposed mid-Invoke!\nMEMORY MAY LEAK!");
            }
        }

        public static Dictionary<string, string> ReadINIString(string source, char delimiter = '=')
        {
            var lines = source.Split("\n");

            Dictionary<string, string> _out = [];
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
                if (!int.TryParse(hmsf[0], out int hours))
                    throw new Exception($"Failed to parse hours!\nFound \"{hmsf[0]}\"");

                if (!int.TryParse(hmsf[1], out int minute))
                    throw new Exception($"Failed to parse minutes!\nFound \"{hmsf[1]}\"");

                if (!int.TryParse(hmsf[2], out int second))
                    throw new Exception($"Failed to parse second!\nFound \"{hmsf[2]}\"");

                if (!int.TryParse(hmsf[3], out int frames))
                    throw new Exception($"Failed to parse frames!\nFound \"{hmsf[3]}\"");

                return new Timestamp(hours, minute, second, frames);
            }

            public int hours = hours;
            public int minute = minute;
            public int second = second;
            public int frames = frames;

            public string GetFormat(TimestampFormat format = TimestampFormat.FFMPEG)
            {
                return format switch
                {
                    TimestampFormat.FFMPEG =>
                        $"{PadZero(hours)}:{PadZero(minute)}:{PadZero(second)}:{PadZero(frames)}",
                    _ => throw new Exception($"Format \"{format}\" not recognized!"),
                };
            }

            public long ToMS() => (long)((hours * 3600 + minute * 60 + second) * 1000 + frames);
        }
    }
}
