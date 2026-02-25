using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WINFORMS_VLCClient.Lib
{
    public static class EpisodeHelper
    {
        public static string? GetNextFileAlphOrder(string currentFilePath) =>
            GetRelativeFile(currentFilePath, 1);

        public static string? GetPrevFileAlphOrder(string currentFilePath) =>
            GetRelativeFile(currentFilePath, -1);

        static string? GetRelativeFile(string currentFilePath, int relativeIndex)
        {
            Uri path = new(currentFilePath);
            if (!Path.Exists(path.LocalPath))
            {
                Debug.WriteLine(
                    $"WARN: Directory or Path does not exist!\nPath:\"{currentFilePath}\""
                );
                return null;
            }

            string targetExtension = Path.GetExtension(path.LocalPath);
            List<string> files =
            [
                .. Directory
                    .GetFiles(Directory.GetParent(path.LocalPath)!.FullName)
                    .OrderBy(f => f)
                    .Where(f => Path.GetExtension(f) == targetExtension),
            ];
            int index = files.IndexOf(currentFilePath);

            if (index == -1)
                return null;

            int newIndex = index + relativeIndex;
            if (newIndex < 0 || newIndex >= files.Count)
                return currentFilePath;

            return files[newIndex];
        }
    }
}
