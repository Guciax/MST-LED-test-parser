using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MST_LED_test_parser
{
    class TestFilesInformation
    {
        public TestFilesInformation(string fileName, DateTime lastModified, List<PcbTestResults> testResults, string modelName, string model12NC, string lotNo, string operatorName)
        {
            FileName = fileName;
            LastModified = lastModified;
            TestResults = testResults;
            Model12NC = model12NC;
            LotNo = lotNo;
            OperatorName = operatorName;
        }

        public string FileName { get; }
        public DateTime LastModified { get; }
        public List<PcbTestResults> TestResults { get; }
        public string Model12NC { get; }
        public string LotNo { get; }
        public string OperatorName { get; }
    }
}
