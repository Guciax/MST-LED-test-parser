using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MST_LED_test_parser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (AppSettingsOperations.GetSettings("Result_Folder")=="")
            {
                AppSettingsOperations.AddOrUpdateAppSettings("Result_Folder", @"C:\Results");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Dictionary<FileInfo[], TestFilesInformation> listOfResultFiles = OperationsOnResultDir.ScanResultDir();

            foreach (var file in listOfResultFiles)
            {
                if (TestFilesOperations.FileHasNewEntries(file))
                {
                    List<PcbTestResults> resultsToParse = TestFilesOperations.GetPcbTestResultsFromTestFile(file);
                    SqlOperations.ParseResults(resultsToParse);
                }
            }

            TestFilesOperations.CleanUpOldFiles(listOfResultFiles);
        }
    }
}
