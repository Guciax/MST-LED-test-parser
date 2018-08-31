using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

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

        private static void FileInfoToTestFileInformation(FileInfo file, ref Dictionary<FileInfo, TestFilesInformation> result)
        {

            Dictionary<string, string> pcbFromFile = new Dictionary<string, string>();

            string modelName = "";
            string model12NC = "";
            string lot = "";
            string oper = "";

            string[] fileLines = File.ReadAllLines(file.FullName);

            
            foreach (var line in fileLines)
            {
                string[] splittedLine = line.Split(';');
                

                switch (line.Substring(0, 3))
                {

                    case "100":
                        modelName = line.Split(';')[1];
                        model12NC = line.Split(';')[2];
                        break;
                    case "500":
                        lot = splittedLine[2];
                        oper = splittedLine[3];
                        break;

                }

                if (splittedLine.Length > 10 & splittedLine[0] != "500") break;
            }
                TestFilesInformation newFileInfo = new TestFilesInformation(file.FullName, file.LastWriteTime, new List<PcbTestResults>(),modelName,model12NC,lot,oper);
                
                DateTime lastModificationTime = file.LastWriteTime;
                double firstTimer = 0;


            foreach (var line in fileLines.Reverse())
            {
                if (firstTimer == 0)
                {
                    string[] splittedLine = line.Split(';');
                    string pcbSerial = ShortenPcbSerial(splittedLine[4]);
                    DateTime inspectionTime;
                    if (firstTimer == 0)
                    {
                        inspectionTime = file.LastWriteTime;
                        firstTimer = double.Parse(splittedLine[5]);
                    }
                    else
                    {
                        double currentTimer = double.Parse(splittedLine[5]);
                        inspectionTime = file.LastWriteTime.AddSeconds((firstTimer - currentTimer) * -1);
                    }

                    double voltage = double.Parse(splittedLine[7]);
                    double current = double.Parse(splittedLine[8]);
                    string hiPotResult = "";

                    if (splittedLine[12] == "0")
                    {
                        hiPotResult = "Pass";
                    }
                    else if (splittedLine[12] == "1")
                    {
                        hiPotResult = "OK";
                    }
                    else
                    {
                        hiPotResult = "NG";
                    }

                    string funcResult = "OK";
                    if (splittedLine[5].Contains("2"))
                    {
                        funcResult = "NG";
                    }


                    PcbTestResults newtest = new PcbTestResults(pcbSerial, inspectionTime, voltage, current, funcResult, hiPotResult, false);
                    newFileInfo.TestResults.Add(newtest);
                }
                break;

            }

            
        }
        

        private static string ShortenPcbSerial(string inputId)
        {
            if (!inputId.Contains("_")) return inputId;

            string[] split = inputId.Split('_');
            return $"{split[split.Length - 2]}_{split[split.Length - 1]}";
        }






        private static void SaveTestFileInfoDictToFile(Dictionary<string, TestFilesInformation> dict)
        {
            using (StreamWriter writetext = new StreamWriter("parsedPCBs.txt"))
            {
                foreach (var key in dict)
                {
                    foreach (var pcb in key.Value.ParsedPcbId)
                    {
                        writetext.WriteLine("{0}\t{1}", key.Value.FileName, pcb);
                    }
                    
                }
            }
        }
    }
}
