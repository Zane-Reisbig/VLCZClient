using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using ClientLib.STD;

namespace WINFORMS_VLCClient.Lib.MediaInformation
{
    public enum MediaInformationSerializationType
    {
        INI,
    }

    public class MediaInformation(Uri? path, StandardDefinitions.Timestamp timestamp)
    {
        readonly Uri? filePath = path;
        public Uri? FilePath
        {
            get => filePath;
        }

        string? fileName = null;
        public string FileName
        {
            get
            {
                if (fileName == null)
                    if (filePath != null)
                        if (Path.Exists(filePath.LocalPath))
                            fileName = Path.GetFileNameWithoutExtension(filePath.LocalPath);
                        else
                            fileName = "No Media Found!";
                    else
                        fileName = "No Media Found!";

                return fileName;
            }
        }

        StandardDefinitions.Timestamp timestamp = timestamp;
        public StandardDefinitions.Timestamp Timestamp
        {
            get => timestamp;
            set => timestamp = value;
        }

        string AsINIString()
        {
            string iniString = $"path={filePath}\n";
            iniString += $"timestamp={timestamp.GetFormat()}\n";

            return iniString;
        }

        public void WriteToFile(
            string path,
            MediaInformationSerializationType serializationType =
                MediaInformationSerializationType.INI
        )
        {
            string? output = serializationType switch
            {
                MediaInformationSerializationType.INI => AsINIString(),
                _ => throw new Exception($"Output type: \"{serializationType}\" is unknown!"),
            };
            try
            {
                File.WriteAllText(path, output);
            }
            catch (Exception e)
            {
                Debug.WriteLine(
                    $"Failed to write to \"{path}\"!\nException: \"{e}\"\nMessage: \"{e.Message}\""
                );
            }
        }

        public static MediaInformation ReadFromINI(string _path)
        {
            string[] contents = File.ReadAllLines(_path);

            string? path = null;
            StandardDefinitions.Timestamp? timestamp = null;

            foreach (string line in contents)
            {
                string[] parts = line.Split('=');
                if (parts.Length != 2)
                    throw new Exception(
                        $"Parse error!\nLine: \"{line}\" is not 2 parts when split at \"=\"!"
                    );

                string label = parts[0],
                    value = parts[1];

                switch (label)
                {
                    case ("path"):
                        path = value;
                        break;
                    case ("timestamp"):
                        timestamp = StandardDefinitions.Timestamp.FromString(value);
                        break;
                    default:
                        throw new Exception($"Uknown label!\nLabel: \"{label}\"");
                }
            }

            if (path == null)
                throw new Exception("Failed to parse path!");

            if (timestamp == null)
                throw new Exception("Failed to parse timestamp!");

            try
            {
                return new(new Uri(path), timestamp);
            }
            catch
            {
                return new(null, timestamp);
            }
        }
    }
}
