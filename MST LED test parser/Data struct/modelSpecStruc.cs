using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MST_LED_test_parser.Data_struct
{
    public class ModelSpecStruc
    {
        public ModelSpecStruc(float v_min, float v_max, float i_min, float i_max)
        {
            V_Min = v_min;
            V_Max = v_max;
            I_Min = i_min;
            I_Max = i_max;
        }

        public float V_Min { get; }
        public float V_Max { get; }
        public float I_Min { get; }
        public float I_Max { get; }
    }
}
