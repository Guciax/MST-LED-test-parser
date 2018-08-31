using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MST_LED_test_parser
{
    class OperationsOnResultDir
    {
        static string resultPath = AppSettingsOperations.GetSettings("Result_Folder");
        public static Dictionary<FileInfo[], TestFilesInformation> ScanResultDir()
        {
            Dictionary<FileInfo[], TestFilesInformation> result = new Dictionary<FileInfo[], TestFilesInformation>();

            var dir = new DirectoryInfo(resultPath);
            List<FileInfo> sortedFileList = dir.GetFiles().OrderByDescending(o=>o.LastWriteTime).ToList();

            foreach (var file in sortedFileList)
            {

            }

            return result;
        }
    }
}
