using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AWEngine.Util
{
    public static class AWUtil
    {
        public static string NormalizeFileName(string fileName)
        {
            var invalidCharacters = Path.GetInvalidFileNameChars();
            for (int i=0; i<fileName.Length; i++)
            {
                // TEST: more efficient to just replace that index?
                if (invalidCharacters.Contains(fileName[i]))
                    fileName = fileName.Replace(fileName[i], '_');
            }
            return fileName;
        }

        public static IEnumerable<DirectoryInfo> FindAllSubFolders(string path)
        {
            var di = new DirectoryInfo(path);
            foreach (var dir in di.GetDirectories())
                yield return di;
        }

        /// <summary>
        /// Given a "folder/mask", return all files in the folder matching the mask. E.g. "SomeFolder1/SomeFolder2/*.json"
        /// </summary>
        /// <param name="pathAndSearchPattern">the mask should only be in the filename. E.g. no "SomeFolder*/foo.json"</param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// TEST: drp040223 - untested
        public static IEnumerable<FileInfo> FindAllFiles(string pathAndSearchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var folder = Path.GetDirectoryName(pathAndSearchPattern);
            var mask = Path.GetFileName(pathAndSearchPattern);
            return FindAllFiles(folder, mask, searchOption);
        }

        /// <summary>
        /// Return all files in the given folder matching the given mask.
        /// </summary>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IEnumerable<FileInfo> FindAllFiles(string absoluteFolder, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var di = new DirectoryInfo(absoluteFolder);
            return FindAllFiles(di, searchPattern, searchOption);
        }

        /// <summary>
        /// Return all files in the given folder matching the given mask.
        /// </summary>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IEnumerable<FileInfo> FindAllFiles(DirectoryInfo di, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (!di.Exists)
                yield break;
            foreach (var fileInfo in di.GetFiles(searchPattern, searchOption))
                yield return fileInfo;
        }
    }
}
