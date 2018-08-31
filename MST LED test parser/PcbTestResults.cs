using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MST_LED_test_parser
{
    class PcbTestResults
    {
        public PcbTestResults( string pcbSerial, DateTime inspectionTime, double voltage, double current, string funcResult, string hiPotResult, bool parsedToDb)
        {

            PcbSerial = pcbSerial;
            InspectionTime = inspectionTime;
            Voltage = voltage;
            FuncResult = funcResult;
            HiPotResult = hiPotResult;
            ParsedToDb = parsedToDb;
        }


        public string PcbSerial { get; }
        public DateTime InspectionTime { get; }
        public double Voltage { get; }
        public string FuncResult { get; }
        public string HiPotResult { get; }
        public bool ParsedToDb { get; }
    }
}
