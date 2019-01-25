using MST_LED_test_parser.Data_struct;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MST_LED_test_parser
{
    class OperationsOnResultDir
    {
        static string resultDirPath = AppSettingsOperations.GetSettings("Result_Folder");
        static string syncedDirPath = Path.Combine(resultDirPath, "Synced");

        public static List<FileInfo> ScanResultDir()
        {
            Dictionary<FileInfo, string[]> result = new Dictionary<FileInfo, string[]>();

            var dir = new DirectoryInfo(resultDirPath);
            return dir.GetFiles().OrderBy(o=>o.LastWriteTime).ToList();

            //foreach (var file in sortedFileList)
            //{
            //    string[] fileContent = File.ReadAllLines(file.FullName);
            //    result.Add(file, fileContent);
            //}

            //return result;
        }

        public static bool SyncTestFile(FileInfo fileToSync, RichTextBox console)
        {
            bool sqlExecuted = true;
            string synchronisedFilePath = Path.Combine(syncedDirPath, fileToSync.Name);
            bool newFile = false;
            string[] resultFile = ReadFileReadOnlyAccess(fileToSync.FullName).ToArray();
            List<string> synchronisedFile = new List<string>();

            if (!File.Exists(synchronisedFilePath))
            {
                if (!Directory.Exists(Path.GetDirectoryName(synchronisedFilePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(synchronisedFilePath));
                }
                //File.Create(syncFilePath);

                foreach (var line in resultFile)
                {
                    if (line.StartsWith("500")) break;
                    synchronisedFile.Add(line);
                }
                File.WriteAllLines(synchronisedFilePath, synchronisedFile);
                console.AppendText(System.DateTime.Now.ToString("HH:mm:ss")+" ------- Nowy plik: " + fileToSync.Name + "-------" + Environment.NewLine);

                newFile = true;
            }

            
            if (!newFile)
            {
                synchronisedFile = ReadFileReadOnlyAccess(synchronisedFilePath);
            }
            

            DateTime lastSaveDate = fileToSync.LastWriteTime;
            double lastSaveCode = GetLastDateCode(resultFile); //co jak zero!!

            if (lastSaveCode > 0)
            {
                ModelSpecStruc modelSpecification = TestFilesOperations.GetModelSpecFromFile(resultFile);

                string nc12 = Get12NCFromResultFile(resultFile);
                List<DbRecordStruct> recordsToSaveList = new List<DbRecordStruct>();

                for (int l = 0; l < resultFile.Length; l++)
                {
                    string[] splittedLine = resultFile[l].Split(';');

                    if (splittedLine[0] != "500")
                    {
                        continue;
                    }
                    else
                    {
                        if (!synchronisedFile.Contains(resultFile[l]))
                        {
                            synchronisedFile.Add(resultFile[l]);
                            double lineTimeCode = double.Parse(splittedLine[5], CultureInfo.InvariantCulture);
                            DateTime testTime = lastSaveDate.AddSeconds(lineTimeCode * (-1));
                            DbRecordStruct newRecord = TestLineToStruct(splittedLine, testTime, modelSpecification);
                            recordsToSaveList.Add(newRecord);
                        }
                        else
                        {
                            Debug.WriteLine("Already exist: " + resultFile[l]);
                        }
                    }
                }

                if (recordsToSaveList.Count > 0)
                {
                    File.WriteAllLines(synchronisedFilePath, synchronisedFile);
                    if (SqlOperations.CheckSqlConnection())
                    {
                        sqlExecuted = SqlOperations.InsertRecordToMesDb(recordsToSaveList, console);
                    }
                    else
                    {
                        console.AppendText(System.DateTime.Now.ToString("HH:mm:ss") + " Brak połączenia z bazą..... "  + Environment.NewLine);
                    }
                }
            }
            return sqlExecuted;
        }

        internal static void DeleteResultFile(FileInfo fileInfo)
        {
            if (File.Exists(Path.Combine(syncedDirPath, fileInfo.Name)))
            {
                try
                {
                    File.Delete(fileInfo.FullName);

                }
                catch {  };
            }
        }

        internal static List<string> ReadFileReadOnlyAccess(string path)
        {
            List<string> result = new List<string>();
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using (var sr = new StreamReader(fs))
            {
                while (!sr.EndOfStream)
                {
                    result.Add(sr.ReadLine());
                }
            }
            return result;
        }

        private static DbRecordStruct TestLineToStruct(string[] testLine, DateTime testDate, ModelSpecStruc specification)
        {
            //Timer – ilość sekund od włączenia systemu(BTS)
            //Fail Code – jest to 5 pozycji(01234) przyjmujące 3 wartości(1 - ok, 2 - nok, 0 - nie testowane)
            //- poz 0 – napięcie
            //- poz 1 – prąd
            // - poz 2 – napiecie na odczepie
            //  - poz 3 – napiecie zwarciowe(106 %)
            //-poz 4 – prąd zwarciowy
            //My używamy tylko dwóch pierwszych pozycji oznaczających kolejno czy napięcie i prąd na panelu jest ok.
            //HV – podobnie jak wyżej 1 - ok 2 - nok 0 - nie testowany(gdy nie przejdzie testu funkcyjnego)

            /*0 header; 
             *1 product_counter; 
             *2 Nr zlecenia; 
             *3 Operator; 
             *4 Produkt_id; 
             *5 timer; 
             *6 fail_code; 
             *7 nom_volt; 
             *8 nom_curr; 
             *9 tap_volt; 
             *10 short_volt; 
             *11 short_curr; 
             *12 Ploss; 
             *13 HV;
             */
            string serial_no = TestFilesOperations.ShortenPcbSerial( testLine[4].Trim());
            int tester_id = int.Parse(AppSettingsOperations.GetSettings("tester_id"));
            int wip_entity_id = 0;
            string wip_entity_name = testLine[2];
            string program_id = "0";
            float v = float.Parse(testLine[7].Trim(), CultureInfo.InvariantCulture);
            float i = float.Parse(testLine[8], CultureInfo.InvariantCulture);
            string high_pot = TestFilesOperations.GetHighPotResultFromTestLine(testLine);
            string light_on = "Manual";
            string optical = "Pass";
            string ng_type = "";
            string result = "NG";

            bool voltageResult = (v >= specification.V_Min & v <= specification.V_Max);
            bool currentResult = (i >= specification.I_Min & i <= specification.I_Max);
            bool highPotResult = true;
            if (high_pot == "NG") highPotResult = false;

            if (voltageResult & currentResult & highPotResult) result = "OK";
            else
            {
                if (!voltageResult) ng_type += "V_";
                if (!currentResult) ng_type += "I_";
                if (!highPotResult) ng_type += "HV_";
            }
            return new DbRecordStruct(serial_no, testDate, tester_id, wip_entity_id, wip_entity_name, program_id, result, ng_type, v, i, high_pot, light_on, optical);
        }

        private static string Get12NCFromResultFile(string[] resultFile)
        {
            foreach (var line in resultFile)
            {
                if (line.StartsWith("100"))
                {
                    string[] splittedLine = line.Split(';');
                    return splittedLine[2].Replace("\"","");
                }
            }
            return "";
        }

        private static double GetLastDateCode(string[] resultFile)
        {
            foreach (var line in resultFile.Reverse())
            {
                if (line.Trim() == "") continue;
                string[] lineSplitted = line.Split(';');
                if (lineSplitted[0] != "500") continue;

                double result = 0;
                string codeString = lineSplitted[5].Trim();
                //if (!double.TryParse(codeString, CultureInfo.InvariantCulture, out result)) continue;

                return double.Parse(codeString, CultureInfo.InvariantCulture);
            }

            return 0;
        }
    }
}
