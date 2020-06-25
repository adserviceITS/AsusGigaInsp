using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using AsusGigaInsp.Modules;

namespace AsusGigaInsp.Models
{
    public class SerialEditModels
    {
        public string SelectEditSerialID;

        public IEnumerable<CombInstruction> DropDownListInstruction { get; set; }

        public string SONo { get; set; }
        public string SerialNumber { get; set; }
        public bool NGFlg { get; set; }
        public string NGReason { get; set; }
        public string Instruction { get; set; }
        public string DescriptionAds { get; set; }

        public string CondSONo { get; set; }
        public string CondMasterCartonSerial { get; set; }
        public string CondLineID { get; set; }
        public string CondSerialNumber { get; set; }
        public string Cond90N { get; set; }
        public string CondModelName { get; set; }
        public string CondWorkDayFrom { get; set; }
        public string CondWorkDayTo { get; set; }
        public string CondInstruction { get; set; }
        public bool CondNGFlg { get; set; }

        public void SetDropDownListInstruction()
        {
            // ASUS指示ドロップダウンリストを取得
            DropDownList ddList = new DropDownList();
            DropDownListInstruction = ddList.GetDropDownListInstruction();
        }

        public void SetRstSerialInfo()
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            stbSql.Append("SELECT ");
            stbSql.Append("    TSE.ID, ");
            stbSql.Append("    TSE.SO_NO, ");
            stbSql.Append("    TSE.SERIAL_NUMBER, ");
            stbSql.Append("    TSE.NG_FLG, ");
            stbSql.Append("    TSE.NG_REASON, ");
            stbSql.Append("    TSE.WORKDAY, ");
            stbSql.Append("    TSE.INSTRUCTION, ");
            stbSql.Append("    TSE.DESCRIPTION_ADS ");
            stbSql.Append("FROM ");
            stbSql.Append("    T_SERIAL_STATUS TSE ");
            stbSql.Append("WHERE");
            stbSql.Append("    TSE.ID = '" + SelectEditSerialID + "' ");

            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());
            string strNGFlg = "";
            
            while (sqlRdr.Read())
            {
                SONo = sqlRdr["SO_NO"].ToString();
                SerialNumber = sqlRdr["SERIAL_NUMBER"].ToString();

                strNGFlg = sqlRdr["NG_FLG"].ToString();
                if (strNGFlg == "0")
                {
                    NGFlg = false;
                } else
                {
                    NGFlg = true;
                }

                NGReason = sqlRdr["NG_REASON"].ToString();
                Instruction = sqlRdr["INSTRUCTION"].ToString();
                DescriptionAds = sqlRdr["DESCRIPTION_ADS"].ToString();
            }
            dsnLib.DB_Close();
        }
    }
}