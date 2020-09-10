using AsusGigaInsp.Modules;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace AsusGigaInsp.Models
{
    public class SerialEditModels
    {
        public IEnumerable<CombInstruction> DropDownListInstruction { get; set; }

        public string SerialID { get; set; }
        public string SONo { get; set; }
        public string SerialNumber { get; set; }
        public bool NGFlg { get; set; }
        public string NGReason { get; set; }
        public string Instruction { get; set; }
        public bool SidewaysFlg { get; set; }
        public string DescriptionAds { get; set; }
        public string INSERT_DATE { get; set; }
        public string INSERT_NAME { get; set; }
        public string UPDATE_DATE { get; set; }
        public string UPDATE_NAME { get; set; }

        public void SetDropDownListInstruction()
        {
            // ASUS指示ドロップダウンリストを取得
            DropDownList ddList = new DropDownList();
            DropDownListInstruction = ddList.GetDropDownListInstruction();
        }

        public void SetSerialInfo ()
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            stbSql.Append("SELECT ");
            stbSql.Append("    TSE.*, ");
            stbSql.Append("    USER_INSERT.USER_NAME INSERT_NAME, ");
            stbSql.Append("    USER_UPDATE.USER_NAME UPDATE_NAME ");
            stbSql.Append("FROM ");
            stbSql.Append("    T_SERIAL_STATUS TSE LEFT JOIN M_USER USER_INSERT ON ");
            stbSql.Append("    TSE.INSERT_ID = USER_INSERT.ID ");
            stbSql.Append("     LEFT JOIN M_USER USER_UPDATE ON ");
            stbSql.Append("    TSE.UPDATE_ID = USER_UPDATE.ID ");
            stbSql.Append("WHERE ");
            stbSql.Append("    TSE.ID = '" + SerialID + "' ");

            //stbSql.Append(stbWhere);
            //Debug.WriteLine(stbSql.ToString());

            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

            while (sqlRdr.Read())
            {
                SerialID = sqlRdr["ID"].ToString();
                SONo = sqlRdr["SO_NO"].ToString();
                SerialNumber = sqlRdr["SERIAL_NUMBER"].ToString();
                if (sqlRdr["NG_FLG"].ToString() == "1")
                {
                    NGFlg = true;
                } else
                {
                    NGFlg = false;
                }
                NGReason = sqlRdr["NG_REASON"].ToString();
                Instruction = sqlRdr["INSTRUCTION"].ToString();
                if (sqlRdr["SIDEWAYS_FLG"].ToString() == "1")
                {
                    SidewaysFlg = true;
                }
                else
                {
                    SidewaysFlg = false;
                }
                DescriptionAds = sqlRdr["DESCRIPTION_ADS"].ToString();
                INSERT_DATE = sqlRdr["INSERT_DATE"].ToString();
                INSERT_NAME = sqlRdr["INSERT_NAME"].ToString();
                UPDATE_DATE = sqlRdr["UPDATE_DATE"].ToString();
                UPDATE_NAME = sqlRdr["UPDATE_NAME"].ToString();

            }
            dsnLib.DB_Close();
        }

        public void UpdateSerialInfo()
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            string strUID = HttpContext.Current.Session["ID"].ToString();

            // NGFlgの変換
            string strNGFlg = NGFlg ? "1" : "0";

            // SidewaysFlgの変換
            string strSidewaysFlg = SidewaysFlg ? "1" : "0";

            // シリアルステータス更新
            stbSql.Append("UPDATE T_SERIAL_STATUS ");
            stbSql.Append("SET ");
            stbSql.Append("    T_SERIAL_STATUS.SO_NO = '" + SONo +"', ");
            stbSql.Append("    T_SERIAL_STATUS.SERIAL_NUMBER = '" + SerialNumber + "', ");
            stbSql.Append("    T_SERIAL_STATUS.NG_FLG = '" + strNGFlg + "', ");
            stbSql.Append("    T_SERIAL_STATUS.NG_REASON = '" + NGReason + "', ");
            stbSql.Append("    T_SERIAL_STATUS.INSTRUCTION = '" + Instruction + "', ");
            stbSql.Append("    T_SERIAL_STATUS.SIDEWAYS_FLG = '" + strSidewaysFlg + "', ");
            stbSql.Append("    T_SERIAL_STATUS.DESCRIPTION_ADS = '" + DescriptionAds + "', ");
            stbSql.Append("    T_SERIAL_STATUS.UPDATE_DATE = GETDATE(), ");
            stbSql.Append("    T_SERIAL_STATUS.UPDATE_ID = '" + strUID + "' ");
            stbSql.Append("WHERE ");
            stbSql.Append("    T_SERIAL_STATUS.ID = '" + SerialID + "' ");

            dsnLib.ExecSQLUpdate(stbSql.ToString());

            stbSql.Clear();
            dsnLib.DB_Close();
        }

        public void UpdateDelFlg()
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            string strUID = HttpContext.Current.Session["ID"].ToString();

            // シリアルステータス更新 ⇒ 物理削除に変更
            //stbSql.Append("UPDATE T_SERIAL_STATUS ");
            //stbSql.Append("SET ");
            //stbSql.Append("    T_SERIAL_STATUS.DEL_FLG = '1', ");
            //stbSql.Append("    T_SERIAL_STATUS.UPDATE_DATE = GETDATE(), ");
            //stbSql.Append("    T_SERIAL_STATUS.UPDATE_ID = '" + strUID + "' ");
            //stbSql.Append("WHERE ");
            //stbSql.Append("    T_SERIAL_STATUS.ID = '" + SerialID + "' ");

            stbSql.Append("DELETE FROM T_SERIAL_LIST ");
            stbSql.Append("WHERE ");
            stbSql.Append("   SERIAL_NUMBER = '" + SerialNumber + "' ");

            dsnLib.ExecSQLUpdate(stbSql.ToString());

            stbSql.Append("DELETE FROM T_SERIAL_STATUS ");
            stbSql.Append("WHERE ");
            stbSql.Append("   SERIAL_NUMBER = '" + SerialNumber + "' ");

            dsnLib.ExecSQLUpdate(stbSql.ToString());

            stbSql.Append("DELETE FROM T_SERIAL_STATUS_HISTORY ");
            stbSql.Append("WHERE ");
            stbSql.Append("   SERIAL_NUMBER = '" + SerialNumber + "' ");

            dsnLib.ExecSQLUpdate(stbSql.ToString());

            stbSql.Clear();
            dsnLib.DB_Close();
        }
    }
}
