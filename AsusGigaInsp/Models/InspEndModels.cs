using AsusGigaInsp.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Text;
using System.Web;

namespace AsusGigaInsp.Models
{
    public class InspEndModels
    {
        public IEnumerable<CombLine> DropDownListLine { get; set; }
        public string LineID { get; set; }
        public string MasterCartonSerial { get; set; }
        public string SrchSerialNo { get; set; }
        public string OldLineID { get; set; }
        public string EntLineID { get; set; }
        public IEnumerable<InspEndSerialList> InspEndSerialLists { get; set; }
        public string LineCompCnt { get; set; } = "0";

        public void SetDropDownListLine()
        {
            // ラインドロップダウンリストを取得
            DropDownList ddList = new DropDownList();
            DropDownListLine = ddList.GetDropDownListLine();
        }

        public void SetInspEndSerialLists()
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            stbSql.Append("SELECT ");
            stbSql.Append("    TSH.SERIAL_ID, ");
            stbSql.Append("    TSH.SO_NO, ");
            stbSql.Append("    TSO.n90N, ");
            stbSql.Append("    TSO.MODEL_NAME, ");
            stbSql.Append("    TSH.SERIAL_NUMBER, ");
            stbSql.Append("    TSH.LINE_ID, ");
            stbSql.Append("    MSS.SERIAL_STATUS_NAME, ");
            stbSql.Append("    TSH.UPDATE_DATE, ");
            stbSql.Append("    USR.USER_NAME ");
            stbSql.Append("FROM ");
            stbSql.Append("    T_SERIAL_STATUS_HISTORY TSH LEFT JOIN T_SO_STATUS TSO ON ");
            stbSql.Append("    TSH.SO_NO = TSO.SO_NO ");
            stbSql.Append("    LEFT JOIN M_SERIAL_STATUS MSS ON ");
            stbSql.Append("    TSH.STATUS = MSS.SERIAL_STATUS_ID ");
            stbSql.Append("    LEFT JOIN M_USER USR ON ");
            stbSql.Append("    USR.ID = TSH.UPDATE_ID ");
            stbSql.Append("WHERE ");
            stbSql.Append("    TSH.STATUS = '4010' AND ");
            stbSql.Append("    CONVERT(VARCHAR(30), TSH.UPDATE_DATE, 112) = CONVERT(VARCHAR(30), GETDATE(), 112) AND ");
            stbSql.Append("    TSH.LINE_ID = '" + LineID + "' ");
            stbSql.Append("ORDER BY ");
            stbSql.Append("    TSH.UPDATE_DATE DESC, ");
            stbSql.Append("    TSH.SERIAL_NUMBER ");

            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

            List<InspEndSerialList> rstRstInspEndSerialList = new List<InspEndSerialList>();

            while (sqlRdr.Read())
            {
                rstRstInspEndSerialList.Add(new InspEndSerialList
                {
                    SerialID = sqlRdr["SERIAL_ID"].ToString(),
                    SONo = sqlRdr["SO_NO"].ToString(),
                    n90N = sqlRdr["n90N"].ToString(),
                    ModelName = sqlRdr["MODEL_NAME"].ToString(),
                    SerialNumber = sqlRdr["SERIAL_NUMBER"].ToString(),
                    LineID = sqlRdr["LINE_ID"].ToString(),
                    StatusName = sqlRdr["SERIAL_STATUS_NAME"].ToString(),
                    StatusUpdateDate = sqlRdr["UPDATE_DATE"].ToString(),
                    UserName = sqlRdr["USER_NAME"].ToString()
                });
            }
            dsnLib.DB_Close();

            InspEndSerialLists = rstRstInspEndSerialList;
        }


        // マスターカートンシリアルのステータス更新処理
        public void UpdateStatus()
        {
            string[] SerialNOs = MasterCartonSerial.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            StringBuilder stbWhere = new StringBuilder();
            stbWhere.Append("T_SERIAL_STATUS.SERIAL_NUMBER IN ( ");

            for (int i = 0; i < SerialNOs.Length; i++)
            {
                if (i + 1 != SerialNOs.Length)
                {
                    stbWhere.Append("'" + SerialNOs[i] + "', ");
                }
                else
                {
                    stbWhere.Append("'" + SerialNOs[i] + "') ");
                }
            }

            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            string strID = HttpContext.Current.Session["ID"].ToString();

            // シリアルステータス更新
            stbSql.Append("UPDATE T_SERIAL_STATUS ");
            stbSql.Append("SET ");
            stbSql.Append("    T_SERIAL_STATUS.WORKDAY = GETDATE(), ");
            stbSql.Append("    T_SERIAL_STATUS.SERIAL_STATUS_ID = '4010', ");
            stbSql.Append("    T_SERIAL_STATUS.STATUS_UPDATE_DATE = GETDATE(), ");
            stbSql.Append("    T_SERIAL_STATUS.UPDATE_DATE = GETDATE(), ");
            stbSql.Append("    T_SERIAL_STATUS.UPDATE_ID = '" + strID + "' ");
            stbSql.Append("WHERE ");
            stbSql.Append(stbWhere.ToString());

            dsnLib.ExecSQLUpdate(stbSql.ToString());

            stbSql.Clear();

            // シリアルステータス履歴更新
            stbSql.Append("INSERT INTO T_SERIAL_STATUS_HISTORY ");
            stbSql.Append("SELECT ");
            stbSql.Append("    T_SERIAL_STATUS.ID, ");
            stbSql.Append("    T_SERIAL_STATUS.SERIAL_NUMBER, ");
            stbSql.Append("    '" + LineID + "', ");
            stbSql.Append("    T_SERIAL_STATUS.SO_NO, ");
            stbSql.Append("    T_SERIAL_STATUS.SERIAL_STATUS_ID, ");
            stbSql.Append("    GETDATE(), ");
            stbSql.Append("    '" + strID + "', ");
            stbSql.Append("    GETDATE(), ");
            stbSql.Append("    '" + strID + "' ");
            stbSql.Append("FROM T_SERIAL_STATUS ");
            stbSql.Append("WHERE ");
            stbSql.Append(stbWhere.ToString());

            dsnLib.ExecSQLUpdate(stbSql.ToString());

            stbSql.Clear();

            // SOに紐付くシリアルが全て検査完了していた場合はSOステータス更新
            //　検査完了していないシリアルがあるかをチェック
            stbSql.Append("SELECT ");
            stbSql.Append("    * ");
            stbSql.Append("FROM ");
            stbSql.Append("    T_SERIAL_STATUS TSE INNER JOIN ");
            stbSql.Append("    ( SELECT SO_NO FROM T_SERIAL_STATUS ");
            stbSql.Append("      WHERE " + stbWhere.ToString() + " ");
            stbSql.Append("      GROUP BY SO_NO ");
            stbSql.Append("    ) SO_NOS ON ");
            stbSql.Append("    TSE.SO_NO = SO_NOS.SO_NO ");
            stbSql.Append("WHERE TSE.SERIAL_STATUS_ID < '4010' ");

            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

            if (!sqlRdr.HasRows)
            {
                stbSql.Clear();

                // SOステータスを更新
                stbSql.Append("UPDATE T_SO_STATUS ");
                stbSql.Append("SET ");
                stbSql.Append("    T_SO_STATUS.SO_STATUS_ID = '4010', ");
                stbSql.Append("    T_SO_STATUS.ST_CHANGE_DATE = GETDATE(), ");
                stbSql.Append("    T_SO_STATUS.UPDATE_DATE = GETDATE(), ");
                stbSql.Append("    T_SO_STATUS.UPDATE_ID = '" + strID + "' ");
                stbSql.Append("WHERE EXISTS ( ");
                stbSql.Append("    SELECT * FROM T_SERIAL_STATUS ");
                stbSql.Append("    WHERE T_SO_STATUS.SO_NO = T_SERIAL_STATUS.SO_NO AND ");
                stbSql.Append(stbWhere.ToString() + ") ");

                dsnLib.ExecSQLUpdate(stbSql.ToString());

                // SOステータス履歴を更新
                stbSql.Append("INSERT INTO T_SO_STATUS_HISTORY ");
                stbSql.Append("SELECT ");
                stbSql.Append("    T_SO_STATUS_HISTORY.SO_NO, ");
                stbSql.Append("    MAX(T_SO_STATUS_HISTORY.SEQ) + 1, ");
                stbSql.Append("    MAX(T_SO_STATUS_HISTORY.NOW_STATUS), ");
                stbSql.Append("    '4010', ");
                stbSql.Append("    GETDATE(), ");
                stbSql.Append("    '" + strID + "', ");
                stbSql.Append("    GETDATE(), ");
                stbSql.Append("    '" + strID + "' ");
                stbSql.Append("FROM T_SO_STATUS_HISTORY LEFT JOIN T_SERIAL_STATUS ON ");
                stbSql.Append("     T_SO_STATUS_HISTORY.SO_NO = T_SERIAL_STATUS.SO_NO ");
                stbSql.Append("WHERE ");
                stbSql.Append(stbWhere.ToString());
                stbSql.Append("GROUP BY  ");
                stbSql.Append("     T_SO_STATUS_HISTORY.SO_NO  ");

                dsnLib.ExecSQLUpdate(stbSql.ToString());
            }
            stbSql.Clear();
            sqlRdr.Close();
            dsnLib.DB_Close();
        }

        public void InspEndLineChange()
        {
            StringBuilder stbWhere = new StringBuilder();

            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            string strID = HttpContext.Current.Session["ID"].ToString();

            // シリアルステータス履歴更新
            stbSql.Append("UPDATE ");
            stbSql.Append("    T_SERIAL_STATUS_HISTORY ");
            stbSql.Append("SET ");
            stbSql.Append("    LINE_ID = '" + EntLineID + "', ");
            stbSql.Append("    UPDATE_DATE = GETDATE(), ");
            stbSql.Append("    UPDATE_ID = '" + strID + "' ");
            stbSql.Append("WHERE ");
            stbSql.Append("    SERIAL_NUMBER = '" + SrchSerialNo + "' ");
            stbSql.Append("AND STATUS = '4010' ");

            dsnLib.ExecSQLUpdate(stbSql.ToString());

        }

        public void SetLineCompCnt()
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            stbSql.Append("SELECT ");
            stbSql.Append("    COUNT(*) AS CNT ");
            stbSql.Append("FROM ");
            stbSql.Append("    T_SERIAL_STATUS_HISTORY ");
            stbSql.Append("WHERE ");
            stbSql.Append("    STATUS = '4010' AND ");
            stbSql.Append("    LINE_ID = '" + LineID + "' AND ");
            stbSql.Append("    CONVERT(VARCHAR, UPDATE_DATE, 112) = CONVERT(VARCHAR, GETDATE(), 112) ");

            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

            sqlRdr.Read();

            LineCompCnt = sqlRdr["CNT"].ToString();

            sqlRdr.Close();
            dsnLib.DB_Close();
        }

    }

    public class InspEndSerialList
    {
        public string SerialID { get; set; }
        [DisplayName("SO#")]
        public string SONo { get; set; }
        [DisplayName("90N")]
        public string n90N { get; set; }
        [DisplayName("Model Name")]
        public string ModelName { get; set; }
        [DisplayName("シリアル")]
        public string SerialNumber { get; set; }
        [DisplayName("ライン")]
        public string LineID { get; set; }
        [DisplayName("ステータス")]
        public string StatusName { get; set; }
        [DisplayName("ステータス変更日")]
        public string StatusUpdateDate { get; set; }
        [DisplayName("更新者")]
        public string UserName { get; set; }
    }
}
