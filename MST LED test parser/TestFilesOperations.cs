using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using MST_LED_test_parser.Data_struct;

namespace MST_LED_test_parser
{
    class TestFilesOperations
    {
        static Dictionary<string, TestFilesInformation> testFileInfoNamekey = new Dictionary<string, TestFilesInformation>();

        public static List<PcbTestResults> GetPcbTestResultsFromTestFile(FileInfo testFile)
        {
            List<PcbTestResults> result = new List<PcbTestResults>();
            return result;
        }

        internal static bool FileHasNewEntries(FileInfo file)
        {
            return (testFileInfoNamekey[file.Name].LastModified < file.LastWriteTime) ? true : false;
        }

        internal static void CleanUpOldFiles(FileInfo[] listOfResultFiles)
        {
            throw new NotImplementedException();
        }



        internal static ModelSpecStruc GetModelSpecFromFile(string[] resultFile)
        {
            float v_min = 0;
            float v_max = 0;
            float i_min = 0;
            float i_max = 0;

            foreach (var line in resultFile)
            {
                if (line.StartsWith("301"))
                {
                    string[] splittedLine = line.Split(';');
                    v_min = float.Parse(splittedLine[3].Trim(), CultureInfo.InvariantCulture);
                    v_max = float.Parse(splittedLine[4].Trim(), CultureInfo.InvariantCulture);
                }
                if (line.StartsWith("302"))
                {
                    string[] splittedLine = line.Split(';');
                    i_min = float.Parse(splittedLine[3].Trim(), CultureInfo.InvariantCulture);
                    i_max = float.Parse(splittedLine[4].Trim(), CultureInfo.InvariantCulture);
                }
            }
            return new ModelSpecStruc(v_min, v_max, i_min, i_max);
        }

        public static string GetHighPotResultFromTestLine(string[] testLine)
        {
            if (testLine[13].Trim() == "0")
            {
                return "Pass";
            }
            else if (testLine[13].Trim() == "1")
            {
                return "OK";
            }
            else
            {
                return "NG";
            }
        }
        

        internal static string ShortenPcbSerial(string inputId)
        {
            if (!inputId.Contains("_")) return inputId;
            if (inputId.Length <= 50) return inputId;

            string[] split = inputId.Split('_');
            return $"{split[split.Length - 2]}_{split[split.Length - 1]}";
        }


    }
}
