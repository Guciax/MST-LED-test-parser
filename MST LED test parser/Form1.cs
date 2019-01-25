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
        bool sqlOnline = true;
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
            List<FileInfo> listOfResultFiles = OperationsOnResultDir.ScanResultDir();
            for (int i = 0; i < listOfResultFiles.Count; i++) 
            {
                bool sqlExecuted = OperationsOnResultDir.SyncTestFile(listOfResultFiles[i], richTextBox1);
                if (i == listOfResultFiles.Count - 1) break;
                if (sqlExecuted) OperationsOnResultDir.DeleteResultFile(listOfResultFiles[i]);
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.ScrollToCaret();
            if (richTextBox1.Lines.Length > 100) 
            {
                richTextBox1.Select(0, richTextBox1.GetFirstCharIndexFromLine(1));
                richTextBox1.SelectedText = "";
            }
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<Data_struct.DbRecordStruct> list = new List<Data_struct.DbRecordStruct>();
            Data_struct.DbRecordStruct ada = new Data_struct.DbRecordStruct("raz@dwa", DateTime.Now, 6, 6, "testtest", "0", "OK", "OK", 20, 20, "OK", "OK", "OK");
            list.Add(ada);
            SqlOperations.InsertRecordToMesDb(list, richTextBox1);
        }
    }
}
