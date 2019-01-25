using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MST_LED_test_parser.Data_struct
{
    class DbRecordStruct
    {
        public DbRecordStruct(string serial_no, DateTime inspection_time, int tester_id, int wip_entity_id, string wip_entity_name, string program_id,
            string result, string ng_type, float v, float i, string hi_pot, string light_on, string optical)
        {
            Serial_No = serial_no;
            Inspection_Time = inspection_time;
            Tester_Id = tester_id;
            Wip_Entity_Id = wip_entity_id;
            Wip_Entity_Name = wip_entity_name;
            Program_Id = program_id;
            Result = result;
            Ng_Type = ng_type;
            V = v;
            I = i;
            Hi_Pot = hi_pot;
            Light_On = light_on;
            Optical = optical;
        }

        public string Serial_No { get; }
        public DateTime Inspection_Time { get; }
        public int Tester_Id { get; }
        public int Wip_Entity_Id { get; }
        public string Wip_Entity_Name { get; }
        public string Program_Id { get; }
        public string Result { get; }
        public string Ng_Type { get; }
        public float V { get; }
        public float I { get; }
        public string Hi_Pot { get; }
        public string Light_On { get; }
        public string Optical { get; }
    }
}
