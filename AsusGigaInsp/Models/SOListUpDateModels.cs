using AsusGigaInsp.Modules;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Text;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using ClosedXML.Excel;

namespace AsusGigaInsp.Models
{
    public class SOListUpdateModels
    {
        public IEnumerable<DropDownNewStatusName> DropDownListNewStatusName { get; set; }
        public class DropDownNewStatusName
        {
            public string SOStatusID { get; set; }
            public string SOStatusName { get; set; }
        }
        public string EntStatusID { get; set; }
        public string CompStatusID { get; set; }

        public string EntSONO { get; set; }

        public string Ent90N { get; set; }
        public string EntModelName { get; set; }

        public int EntShippingQuantity{ get; set; }

        public string EntEstArrivalDate { get; set; }

        public string EntPrefReportingDate { get; set; }
        public string EntSiTekEstArrivalDate { get; set; }

        public string EntDeliveryLocation { get; set; }

        public string EntN01 { get; set; }

        public string AddUser { get; set; }
        public string AddDate { get; set; }
        public string UpdateUser { get; set; }
        public string UpdateDate { get; set; }
        public string STChangeUser { get; set; }
        public string STChangeDate { get; set; }

        // ステータスコンボボックスに次ステータスをセットする。
        public void SetDropDownListNewStatusName(string strSONO)
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            stbSql.Append("SELECT ");
            stbSql.Append("    TBL1.SO_STATUS_ID, ");
            stbSql.Append("    M_SO_STATUS.SO_STATUS_NAME ");
            stbSql.Append("FROM ");
            stbSql.Append("    M_SO_STATUS, ");
            stbSql.Append("    ( ");
            stbSql.Append("        SELECT");
            stbSql.Append("            T_SO_STATUS.SO_STATUS_ID ");
            stbSql.Append("        FROM ");
            stbSql.Append("            T_SO_STATUS ");
            stbSql.Append("        WHERE ");
            stbSql.Append("            T_SO_STATUS.SO_NO = '" + strSONO + "' ");
            stbSql.Append("    ) TBL1 ");
            stbSql.Append("WHERE ");
            stbSql.Append("    M_SO_STATUS.SO_STATUS_ID = TBL1.SO_STATUS_ID ");
            stbSql.Append("    AND M_SO_STATUS.DEL_FLG = '0' ");
            stbSql.Append("UNION ");
            stbSql.Append("SELECT ");
            stbSql.Append("    TBL2.STATUS_ID, ");
            stbSql.Append("    M_SO_STATUS.SO_STATUS_NAME ");
            stbSql.Append("FROM ");
            stbSql.Append("    M_SO_STATUS, ");
            stbSql.Append("    ( ");
            stbSql.Append("        SELECT");
            stbSql.Append("            M_SO_STATUS_SELECT.NEXT_STATUS_ID AS STATUS_ID ");
            stbSql.Append("        FROM ");
            stbSql.Append("            M_SO_STATUS_SELECT, ");
            stbSql.Append("            ( ");
            stbSql.Append("                SELECT ");
            stbSql.Append("                    T_SO_STATUS.SO_STATUS_ID ");
            stbSql.Append("                FROM ");
            stbSql.Append("                    T_SO_STATUS ");
            stbSql.Append("                WHERE ");
            stbSql.Append("                    T_SO_STATUS.SO_NO = '" + strSONO + "' ");
            stbSql.Append("            ) TBL1 ");
            stbSql.Append("        WHERE ");
            stbSql.Append("            M_SO_STATUS_SELECT.STATUS_ID = TBL1.SO_STATUS_ID ");
            stbSql.Append("        AND M_SO_STATUS_SELECT.DEL_FLG = '0' ");
            stbSql.Append("        AND M_SO_STATUS_SELECT.USE_FLG = '1' ");
            stbSql.Append("    ) TBL2 ");
            stbSql.Append("WHERE ");
            stbSql.Append("    M_SO_STATUS.SO_STATUS_ID = TBL2.STATUS_ID ");

            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

            List<DropDownNewStatusName> lstDropDownNewStatusName = new List<DropDownNewStatusName>();

            while (sqlRdr.Read())
            {
                lstDropDownNewStatusName.Add(new DropDownNewStatusName
                {
                    SOStatusID = sqlRdr["SO_STATUS_ID"].ToString(),
                    SOStatusName = sqlRdr["SO_STATUS_NAME"].ToString()
                });
            }
            dsnLib.DB_Close();

            DropDownListNewStatusName = lstDropDownNewStatusName;
        }

        // オーダーの検索結果セット
        public void SetSOListDetails(string strSONO)
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            stbSql.Append("SELECT ");
            stbSql.Append("    T_SO_STATUS.*, ");
            stbSql.Append("    M_USER1.USER_NAME AS USER_NAME1, ");
            stbSql.Append("    M_USER2.USER_NAME AS USER_NAME2 ");
            stbSql.Append("FROM ");
            stbSql.Append("    T_SO_STATUS ");
            stbSql.Append("    LEFT JOIN M_USER M_USER1 ");
            stbSql.Append("        ON T_SO_STATUS.INSERT_ID = M_USER1.ID ");
            stbSql.Append("    LEFT JOIN M_USER M_USER2 ");
            stbSql.Append("        ON T_SO_STATUS.UPDATE_ID = M_USER2.ID ");
            stbSql.Append("WHERE ");
            stbSql.Append("   T_SO_STATUS.SO_NO = " + strSONO);

            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

            while (sqlRdr.Read())
            {
                EntSONO = sqlRdr["SO_NO"].ToString();
                EntStatusID = sqlRdr["SO_STATUS_ID"].ToString();
                CompStatusID = sqlRdr["SO_STATUS_ID"].ToString();
                Ent90N = sqlRdr["n90N"].ToString();
                EntModelName = sqlRdr["MODEL_NAME"].ToString();
                EntShippingQuantity = int.Parse(sqlRdr["SHIPPING_QUANTITY"].ToString());
                EntEstArrivalDate = sqlRdr["EST_ARRIVAL_DATE"].ToString();
                EntPrefReportingDate = sqlRdr["PREF_REPORTING_DATE"].ToString();
                EntSiTekEstArrivalDate = sqlRdr["SI_TEK_EST_ARRIVAL_DATE"].ToString();
                EntDeliveryLocation = sqlRdr["DELIVERY_LOCATION"].ToString();
                EntN01 = sqlRdr["N01_NO"].ToString();
                AddUser = sqlRdr["USER_NAME1"].ToString();
                AddDate = sqlRdr["INSERT_DATE"].ToString();
                UpdateUser = sqlRdr["USER_NAME2"].ToString();
                UpdateDate = sqlRdr["UPDATE_DATE"].ToString();
                STChangeUser = sqlRdr["USER_NAME2"].ToString();
                STChangeDate = sqlRdr["UPDATE_DATE"].ToString();
            }
            dsnLib.DB_Close();
        }

        // オーダー情報の更新処理
        public void UpdateSOList(string strUpdUID, DateTime DTNow, string strStatusID, string strSONO)
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            stbSql.Append("UPDATE T_SO_STATUS ");
            stbSql.Append("SET ");
            stbSql.Append("    SO_STATUS_ID = N'" + strStatusID + "', ");
            stbSql.Append("    ST_CHANGE_DATE = '" + DTNow + "', ");
            stbSql.Append("    UPDATE_DATE = '" + DTNow + "', ");
            stbSql.Append("    UPDATE_ID = N'" + strUpdUID + "' ");
            stbSql.Append("WHERE ");
            stbSql.Append("    SO_NO = N'" + strSONO + "' ");

            dsnLib.ExecSQLUpdate(stbSql.ToString());
            dsnLib.DB_Close();
        }

        // オーダー情報履歴にレコード追加
        public void UpdateSOListHistory(string strUpdUID, DateTime DTNow, string strMaeStatusID, string strAtoStatusID, string strSONO)
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            stbSql.Append("INSERT ");
            stbSql.Append("INTO T_SO_STATUS_HISTORY ");
            stbSql.Append("( ");
            stbSql.Append("    SO_NO, ");
            stbSql.Append("    SEQ, ");
            stbSql.Append("    BEFORE_STATUS, ");
            stbSql.Append("    NOW_STATUS, ");
            stbSql.Append("    INSERT_DATE, ");
            stbSql.Append("    INSERT_ID, ");
            stbSql.Append("    UPDATE_DATE,");
            stbSql.Append("    UPDATE_ID ");
            stbSql.Append(") ");
            stbSql.Append("SELECT ");
            stbSql.Append("    SO_NO, ");
            stbSql.Append("    MAX(SEQ) + 1, ");
            stbSql.Append("    '" + strMaeStatusID + "', ");
            stbSql.Append("    '" + strAtoStatusID + "', ");
            stbSql.Append("    '" + DTNow + "', ");
            stbSql.Append("    '" + strUpdUID + "', ");
            stbSql.Append("    '" + DTNow + "', ");
            stbSql.Append("    '" + strUpdUID + "' ");
            stbSql.Append("FROM ");
            stbSql.Append("    T_SO_STATUS_HISTORY ");
            stbSql.Append("WHERE ");
            stbSql.Append("    SO_NO = N'" + strSONO + "' ");
            stbSql.Append("GROUP BY ");
            stbSql.Append("    SO_NO ");

            dsnLib.ExecSQLUpdate(stbSql.ToString());
            dsnLib.DB_Close();
        }

        // シリアル情報のステータス更新処理
        public void UpdateSerialList(string strUpdUID, DateTime DTNow, string strStatusID, string strSONO)
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            stbSql.Append("UPDATE T_SERIAL_STATUS ");
            stbSql.Append("SET ");
            stbSql.Append("    SERIAL_STATUS_ID = N'" + strStatusID + "', ");
            stbSql.Append("    STATUS_UPDATE_DATE = '" + DTNow + "', ");
            stbSql.Append("    UPDATE_DATE = '" + DTNow + "', ");
            stbSql.Append("    UPDATE_ID = N'" + strUpdUID + "' ");
            stbSql.Append("WHERE ");
            stbSql.Append("    SO_NO = N'" + strSONO + "' ");

            dsnLib.ExecSQLUpdate(stbSql.ToString());
            dsnLib.DB_Close();
        }

        // シリアルステータス履歴にレコード追加
        public void UpdateSerialListHistory(string strUpdUID, DateTime DTNow, string strStatusID, string strSONO)
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            stbSql.Append("INSERT ");
            stbSql.Append("INTO T_SERIAL_STATUS_HISTORY ");
            stbSql.Append("( ");
            stbSql.Append("    SERIAL_ID, ");
            stbSql.Append("    SERIAL_NUMBER, ");
            stbSql.Append("    LINE_ID, ");
            stbSql.Append("    SO_NO, ");
            stbSql.Append("    STATUS, ");
            stbSql.Append("    INSERT_DATE, ");
            stbSql.Append("    INSERT_ID, ");
            stbSql.Append("    UPDATE_DATE, ");
            stbSql.Append("    UPDATE_ID ");
            stbSql.Append(") ");
            stbSql.Append("SELECT ");
            stbSql.Append("    ID, ");
            stbSql.Append("    SERIAL_NUMBER, ");
            stbSql.Append("    'ZZZ', ");
            stbSql.Append("    SO_NO, ");
            stbSql.Append("    '" + strStatusID + "', ");
            stbSql.Append("    '" + DTNow + "', ");
            stbSql.Append("    '" + strUpdUID + "', ");
            stbSql.Append("    '" + DTNow + "', ");
            stbSql.Append("    '" + strUpdUID + "' ");
            stbSql.Append("FROM ");
            stbSql.Append("    T_SERIAL_STATUS ");
            stbSql.Append("WHERE ");
            stbSql.Append("    SO_NO = '" + strSONO + "' ");

            dsnLib.ExecSQLUpdate(stbSql.ToString());
            dsnLib.DB_Close();
        }

        public string NowStatus(string StrSONO)
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            stbSql.Append("SELECT" );
            stbSql.Append("    SO_STATUS_ID ");
            stbSql.Append("FROM ");
            stbSql.Append("    T_SO_STATUS ");
            stbSql.Append("WHERE ");
            stbSql.Append("    SO_NO = N'" + StrSONO + "' ");
            stbSql.Append("AND DEL_FLG = '0' ");

            string StrStatusID = "";
            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

            while (sqlRdr.Read())
            {
                StrStatusID = sqlRdr["SO_STATUS_ID"].ToString();
            }
            dsnLib.DB_Close();

            return StrStatusID;
        }

        // オーダー情報の削除処理
        public void DeleteSOList(string strUpdUID)
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            stbSql.Append("UPDATE T_SO_STATUS ");
            stbSql.Append("SET ");
            stbSql.Append("    DEL_FLG = N'1', ");
            stbSql.Append("    UPDATE_DATE = GETDATE(), ");
            stbSql.Append("    UPDATE_ID = N'" + strUpdUID + "' ");
            stbSql.Append("WHERE ");
            stbSql.Append("    SO_NO = N'" + EntSONO + "' ");

            dsnLib.ExecSQLUpdate(stbSql.ToString());
            dsnLib.DB_Close();
        }

        // シリアル情報の削除処理
        public void DeleteSerialList(string strUpdUID)
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            stbSql.Append("UPDATE T_SERIAL_STATUS ");
            stbSql.Append("SET ");
            stbSql.Append("    DEL_FLG = N'1', ");
            stbSql.Append("    UPDATE_DATE = GETDATE(), ");
            stbSql.Append("    UPDATE_ID = N'" + strUpdUID + "' ");
            stbSql.Append("WHERE ");
            stbSql.Append("    SO = N'" + EntSONO + "' ");

            dsnLib.ExecSQLUpdate(stbSql.ToString());
            dsnLib.DB_Close();
        }
    }
}