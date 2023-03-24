using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Todo.Core.Util
{
    public static class TodoUtil
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
    }
}
