using MST_LED_test_parser.Data_struct;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MST_LED_test_parser
{
    class SqlOperations
    {
        internal static void ParseResults(List<PcbTestResults> resultsToParse)
        {
            throw new NotImplementedException();
        }

        public static bool InsertRecordToMesDb(List<DbRecordStruct> recordToInsertList, RichTextBox console)
        {
            //foreach (var item in recordToInsertList)
            //{
            //    string[] values = new string[] { item.Serial_No, item.Inspection_Time.ToString("HH:mm:ss dd-MM-yyyy"), item.Tester_Id.ToString(), item.Wip_Entity_Id.ToString(), item.Wip_Entity_Name, item.Program_Id, item.Result, item.Ng_Type, item.V.ToString(), item.I.ToString(), item.Hi_Pot, item.Light_On, item.Optical };

            //    Debug.WriteLine(String.Join(" ; ", values));
            //}
            //serial_no, inspection_time, tester_id, wip_entity_id, wip_entity_name, program_id, result, ng_type, v, i, hi_pot, light_on, optical, result_int


            bool result = true;
            using (SqlConnection openCon = new SqlConnection(@"Data Source=MSTMS010;Initial Catalog=MES;User Id=mes;Password=mes;"))
            {
                openCon.Open();
                foreach (var recordToInsert in recordToInsertList)
                {
                    string save = "INSERT into tb_tester_measurements (serial_no, inspection_time, tester_id, wip_entity_id, wip_entity_name, program_id, result, ng_type, v, i, hi_pot, light_on, optical) " +
                                                            "VALUES (@serial_no, @inspection_time, @tester_id, @wip_entity_id, @wip_entity_name, @program_id, @result, @ng_type, @v, @i, @hi_pot, @light_on, @optical)";

                    string[] splittedSerial = recordToInsert.Serial_No.Split('@');
                    foreach (var serial in splittedSerial)
                    {
                        using (SqlCommand querySave = new SqlCommand(save))
                        {
                            querySave.Connection = openCon;
                            querySave.Parameters.Add("@serial_no", SqlDbType.NVarChar).Value = serial;
                            querySave.Parameters.Add("@inspection_time", SqlDbType.NVarChar, 20).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            querySave.Parameters.Add("@tester_id", SqlDbType.Int).Value = recordToInsert.Tester_Id;
                            querySave.Parameters.Add("@wip_entity_id", SqlDbType.Int).Value = recordToInsert.Wip_Entity_Id;
                            querySave.Parameters.Add("@wip_entity_name", SqlDbType.VarChar, 50).Value = recordToInsert.Wip_Entity_Name;
                            querySave.Parameters.Add("@program_id", SqlDbType.Int).Value = recordToInsert.Program_Id;
                            querySave.Parameters.Add("@result", SqlDbType.VarChar, 5).Value = recordToInsert.Result;
                            querySave.Parameters.Add("@ng_type", SqlDbType.VarChar, 100).Value = recordToInsert.Ng_Type;
                            querySave.Parameters.Add("@v", SqlDbType.Float).Value = recordToInsert.V;
                            querySave.Parameters.Add("@i", SqlDbType.Float).Value = recordToInsert.I;
                            querySave.Parameters.Add("@hi_pot", SqlDbType.VarChar, 15).Value = recordToInsert.Hi_Pot;
                            querySave.Parameters.Add("@light_on", SqlDbType.VarChar, 15).Value = recordToInsert.Light_On;
                            querySave.Parameters.Add("@optical", SqlDbType.VarChar, 15).Value = recordToInsert.Optical;

                            bool noError = true;
                            try
                            {
                                querySave.ExecuteNonQuery();
                            }
                            catch (SqlException e)
                            {
                                noError = false;
                                if (e.ErrorCode == -2146232060)
                                {
                                    //duplikat
                                    console.AppendText(System.DateTime.Now.ToString("HH:mm:ss") + "  " + recordToInsert.Serial_No + " pominięty - kod istnieje w bazie." + Environment.NewLine);
                                }
                                else
                                {
                                    console.AppendText(System.DateTime.Now.ToString("HH:mm:ss") + "  " + recordToInsert.Serial_No + " błąd - " + e.ErrorCode + Environment.NewLine + e.Message + Environment.NewLine);
                                    result = false;
                                }
                            }
                            if (noError) console.AppendText(System.DateTime.Now.ToString("HH:mm:ss") + "  " + recordToInsert.Serial_No + " zsynchronizowwany" + Environment.NewLine);
                        }
                    }
                }
            }


            return result;
        }

        public static bool CheckSqlConnection()
        {
            bool result = true;

            DataTable sqlTable = new DataTable();
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = @"Data Source=MSTMS010;Initial Catalog=MES;User Id=mes;Password=mes;";

            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            command.CommandText =
                @"SELECT 1;";

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            try
            {
                adapter.Fill(sqlTable);
            }
            catch
            {
                result = false;
            }
            return result;
        }
    }
}
