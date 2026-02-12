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
            bool isValid = target.ContainsFocus;
            if (!isValid)
                return false;

            var clientCursorPos = target.PointToClient(Cursor.Position);

            return isValid && target.ClientRectangle.Contains(clientCursorPos);
        }

        public static string PadZero(object to, int maxLength = 2)
        {
            string strRep = to.ToString()!;
            if (strRep.Length > maxLength)
                return strRep;

            string _out = new string('0', maxLength + strRep.Length) + strRep;
            return _out.Substring(_out.Length - maxLength);
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

            public static Timestamp FromString(string source)
            {
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
