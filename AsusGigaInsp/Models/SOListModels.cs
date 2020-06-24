using AsusGigaInsp.Modules;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Text;
using System;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;

namespace AsusGigaInsp.Models
{
    public class SOListModels
    {
        public IEnumerable<DropDownStatusName> DropDownListStatusName { get; set; }

        public string SrchMode { get; set; }

        public string SrchSONO { get; set; }
        public string Srch90N { get; set; }
        public string SrchModelName { get; set; }
        public string SrchN01NO { get; set; }
        public string SrchEstArrivalDate_S { get; set; }
        public string SrchEstArrivalDate_E { get; set; }
        public string SrchPrefReportingDate_S { get; set; }
        public string SrchPrefReportingDate_E { get; set; }
        public string SrchSiTekEstArrivalDate_S { get; set; }
        public string SrchSiTekEstArrivalDate_E { get; set; }
        public string SrchStatusID { get; set; }

        public IEnumerable<SrchRstOrder> SrchRstOrderList { get; set; }
        private StringBuilder stbWhere = new StringBuilder();

        public string SelectOrderID { get; set; }

        public void SetDropDownListStatusName()
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            stbSql.Append("SELECT ");
            stbSql.Append("    M_SO_STATUS.SO_STATUS_ID, ");
            stbSql.Append("    M_SO_STATUS.SO_STATUS_NAME ");
            stbSql.Append("FROM ");
            stbSql.Append("    M_SO_STATUS ");
            stbSql.Append("WHERE ");
            stbSql.Append("    M_SO_STATUS.DEL_FLG = '0' ");
            stbSql.Append("ORDER BY ");
            stbSql.Append("    M_SO_STATUS.SO_STATUS_ID ");

            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

            List<DropDownStatusName> lstDropDownStatusName = new List<DropDownStatusName>();

            while (sqlRdr.Read())
            {
                lstDropDownStatusName.Add(new DropDownStatusName
                {
                    SOStatusID = sqlRdr["SO_STATUS_ID"].ToString(),
                    SOStatusName = sqlRdr["SO_STATUS_NAME"].ToString()
                });
            }
            dsnLib.DB_Close();

            DropDownListStatusName = lstDropDownStatusName;
        }

        public void SetSrchRstOrderList()
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            stbSql.Append("SELECT ");
            stbSql.Append("    T_SO_STATUS.SO_ID, ");
            stbSql.Append("    T_SO_STATUS.SO_NO, ");
            stbSql.Append("    T_SO_STATUS.n90N, ");
            stbSql.Append("    T_SO_STATUS.MODEL_NAME, ");
            stbSql.Append("    T_SO_STATUS.SHIPPING_QUANTITY, ");
            stbSql.Append("    T_SO_STATUS.EST_ARRIVAL_DATE, ");
            stbSql.Append("    T_SO_STATUS.PREF_REPORTING_DATE, ");
            stbSql.Append("    T_SO_STATUS.SI_TEK_EST_ARRIVAL_DATE, ");
            stbSql.Append("    T_SO_STATUS.DELIVERY_LOCATION, ");
            stbSql.Append("    T_SO_STATUS.N01_NO, ");
            stbSql.Append("    M_SO_STATUS.SO_STATUS_NAME, ");
            stbSql.Append("    T_SO_STATUS.ST_CHANGE_DATE, ");
            stbSql.Append("    IsNull(TBL1.COMPLETE_WORK_UNIT, 0) AS COMPLETE_WORK_UNIT, ");
            stbSql.Append("    IsNull(TBL2.DOA_UNIT, 0) AS DOA_UNIT, ");
            stbSql.Append("    IsNull(TBL1.COMPLETE_WORK_UNIT, 0) - IsNull(TBL2.DOA_UNIT, 0) AS FIXED_SHIPPING_QUANTITY ");
            stbSql.Append("FROM ");
            stbSql.Append("    T_SO_STATUS ");
            stbSql.Append("    INNER JOIN M_SO_STATUS ");
            stbSql.Append("        ON T_SO_STATUS.SO_STATUS_ID = M_SO_STATUS.SO_STATUS_ID ");
            stbSql.Append("    LEFT JOIN ( ");
            stbSql.Append("        SELECT ");
            stbSql.Append("            T_SERIAL_STATUS.SO_NO, ");
            stbSql.Append("            Count(*) AS COMPLETE_WORK_UNIT ");
            stbSql.Append("        FROM ");
            stbSql.Append("            T_SERIAL_STATUS ");
            stbSql.Append("        WHERE ");
            stbSql.Append("            T_SERIAL_STATUS.STATUS = '4010' ");
            stbSql.Append("        GROUP BY ");
            stbSql.Append("            T_SERIAL_STATUS.SO_NO ");
            stbSql.Append("    ) TBL1 ");
            stbSql.Append("        ON T_SO_STATUS.SO_NO = TBL1.SO_NO ");
            stbSql.Append("    LEFT JOIN ( ");
            stbSql.Append("        SELECT ");
            stbSql.Append("            T_SERIAL_STATUS.SO_NO, ");
            stbSql.Append("            Count(*) AS DOA_UNIT ");
            stbSql.Append("        FROM ");
            stbSql.Append("            T_SERIAL_STATUS ");
            stbSql.Append("        WHERE ");
            stbSql.Append("            T_SERIAL_STATUS.INSTRUCTION = '001' ");
            stbSql.Append("        GROUP BY ");
            stbSql.Append("            T_SERIAL_STATUS.SO_NO ");
            stbSql.Append("    ) TBL2 ");
            stbSql.Append("        ON T_SO_STATUS.SO_NO = TBL2.SO_NO ");
            stbSql.Append(stbWhere);
            Debug.WriteLine(stbSql.ToString());

            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

            List<SrchRstOrder> lstSrchRstOrder = new List<SrchRstOrder>();

            while (sqlRdr.Read())
            {
                lstSrchRstOrder.Add(new SrchRstOrder
                {
                    SOID = long.Parse(sqlRdr["SO_ID"].ToString()),
                    SONO = sqlRdr["SO_NO"].ToString(),
                    n90N = sqlRdr["n90N"].ToString(),
                    ModelName = sqlRdr["MODEL_NAME"].ToString(),
                    ShippingQuantity = int.Parse(sqlRdr["SHIPPING_QUANTITY"].ToString()),
                    FixedShippingQuantity = int.Parse(sqlRdr["FIXED_SHIPPING_QUANTITY"].ToString()),
                    EstArrivanDate = string.IsNullOrEmpty(sqlRdr["EST_ARRIVAL_DATE"].ToString()) ? (DateTime?)null : DateTime.Parse(sqlRdr["EST_ARRIVAL_DATE"].ToString()),
                    PrefReportingDate = string.IsNullOrEmpty(sqlRdr["PREF_REPORTING_DATE"].ToString()) ? (DateTime?)null : DateTime.Parse(sqlRdr["PREF_REPORTING_DATE"].ToString()),
                    SiTekEstArrivalDate = string.IsNullOrEmpty(sqlRdr["SI_TEK_EST_ARRIVAL_DATE"].ToString()) ? (DateTime?)null : DateTime.Parse(sqlRdr["SI_TEK_EST_ARRIVAL_DATE"].ToString()),
                    DeliveryLocation = sqlRdr["DELIVERY_LOCATION"].ToString(),
                    CompleteWorkUnit = int.Parse(sqlRdr["COMPLETE_WORK_UNIT"].ToString()),
                    DOAUnit = int.Parse(sqlRdr["DOA_UNIT"].ToString()),
                    N01NO = sqlRdr["N01_NO"].ToString(),
                    SOStatusName = sqlRdr["SO_STATUS_NAME"].ToString(),
                    STChangeDate = string.IsNullOrEmpty(sqlRdr["ST_CHANGE_DATE"].ToString()) ? (DateTime?)null : DateTime.Parse(sqlRdr["ST_CHANGE_DATE"].ToString())
                });
            }
            dsnLib.DB_Close();

            SrchRstOrderList = lstSrchRstOrder;
        }

        public void SetWhere()

        {
            stbWhere.Append("WHERE ");
            stbWhere.Append("     ((T_SO_STATUS.DEL_FLG = '0') AND (M_SO_STATUS.DEL_FLG = '0')) ");

            if (!string.IsNullOrEmpty(SrchSONO))
            {
                stbWhere.Append("AND T_ORDER.SO_NO = N'" + SrchSONO + "' ");
            }
            else
            {
                if (!string.IsNullOrEmpty(Srch90N))
                {
                    stbWhere.Append("AND T_SO_STATUS.n90N LIKE N'%" + Srch90N + "%' ");
                }

                if (!string.IsNullOrEmpty(SrchModelName))
                {
                    stbWhere.Append("AND T_SO_STATUS.MODEL_NAME Like N'%" + SrchModelName + "%' ");
                }

                if (!string.IsNullOrEmpty(SrchN01NO))
                {
                    stbWhere.Append("AND T_SO_STATUS.N01_NO = N'" + SrchN01NO + "' ");
                }

                if (!string.IsNullOrEmpty(SrchEstArrivalDate_S))
                {
                    stbWhere.Append("AND T_SO_STATUS.EST_ARRIVAL_DATE >= '" + SrchEstArrivalDate_S + "' ");
                }

                if (!string.IsNullOrEmpty(SrchEstArrivalDate_E))
                {
                    stbWhere.Append("AND T_SO_STATUS.EST_ARRIVAL_DATE <= '" + SrchEstArrivalDate_E + "' ");
                }

                if (!string.IsNullOrEmpty(SrchPrefReportingDate_S))
                {
                    stbWhere.Append("AND T_SO_STATUS.PREF_REPORTING_DATE >= '" + SrchPrefReportingDate_S + "' ");
                }

                if (!string.IsNullOrEmpty(SrchPrefReportingDate_E))
                {
                    stbWhere.Append("AND T_SO_STATUS.PREF_REPORTING_DATE <= '" + SrchPrefReportingDate_E + "' ");
                }

                if (!string.IsNullOrEmpty(SrchSiTekEstArrivalDate_S))
                {
                    stbWhere.Append("AND T_SO_STATUS.SI_TEK_EST_ARRIVAL_DATE >= '" + SrchSiTekEstArrivalDate_S + "' ");
                }

                if (!string.IsNullOrEmpty(SrchSiTekEstArrivalDate_E))
                {
                    stbWhere.Append("AND T_SO_STATUS.SI_TEK_EST_ARRIVAL_DATE <= '" + SrchSiTekEstArrivalDate_E + "' ");
                }

                if (!string.IsNullOrEmpty(SrchStatusID))
                {
                    stbWhere.Append("AND T_SO_STATUS.SO_STATUS_ID = N'%" + SrchStatusID + "%' ");
                }

            }
        }
    }

    public class SrchRstOrder
    {
        [DisplayName("SO ID")]
        public long SOID { get; set; }
        [DisplayName("SO#")]
        public string SONO { get; set; }
        [DisplayName("90N")]
        public string n90N { get; set; }
        [DisplayName("Model Name")]
        public string ModelName{ get; set; }
        [DisplayName("出荷予定数")]
        public int ShippingQuantity { get; set; }
        [DisplayName("出荷数")]
        public int FixedShippingQuantity { get; set; }
        [DisplayName("ADS到着予定日")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? EstArrivanDate { get; set; }
        [DisplayName("レポート提出希望日")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? PrefReportingDate { get; set; }
        [DisplayName("SI/TEK到着予定日")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? SiTekEstArrivalDate { get; set; }
        [DisplayName("納品地")]
        public string DeliveryLocation { get; set; }
        [DisplayName("作業完了数")]
        public int CompleteWorkUnit { get; set; }
        [DisplayName("ASUS確認後DOA数")]
        public int DOAUnit { get; set; }
        [DisplayName("N01#")]
        public string N01NO { get; set; }
        [DisplayName("ステータス")]
        public string SOStatusName { get; set; }
        [DisplayName("ステータス変更日")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? STChangeDate { get; set; }
    }

    public class DropDownStatusName
    {
        public string SOStatusID { get; set; }
        public string SOStatusName { get; set; }
    }

    public class SOListUpdateModels
    {
        public string EntMode { get; set; }
        public long EntSOID { get; set; }

        [Required(ErrorMessage = "SO#は必須入力です")]
        [RegularExpression(@"[0-9]+", ErrorMessage = "SO#は半角数字で入力してください。")]
        public string EntSONO { get; set; }
        public string CompSONO { get; set; }

        public string Ent90N { get; set; }
        public string EntModelName { get; set; }

        [RegularExpression(@"[0-9]+", ErrorMessage = "出荷予定数は半角数字で入力してください。")]
        public int EntShippingQuantity{ get; set; }

        public DateTime? EntEstArrivalDate { get; set; }
        public DateTime? EntPrefReportingDate { get; set; }
        public DateTime? EntSiTekEstArrivalDate { get; set; }

        public string EntDeliveryLocation { get; set; }

        public string EntN01 { get; set; }
        public string CompN01 { get; set; }

        //取引先検索時の検索結果セット
        public void SetSOListDetails(string strSOID)
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            long lngSOID = long.Parse(strSOID);

            stbSql.Append("SELECT ");
            stbSql.Append("    * ");
            stbSql.Append("FROM ");
            stbSql.Append("    T_SO_STATUS ");
            stbSql.Append("WHERE ");
            stbSql.Append("   SO_ID = " + lngSOID);

            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

        while (sqlRdr.Read())
            {
                EntSOID = long.Parse(sqlRdr["SO_ID"].ToString());
                EntSONO = sqlRdr["SO_NO"].ToString();
                CompSONO = sqlRdr["SO_NO"].ToString();
                Ent90N = sqlRdr["n90N"].ToString();
                EntModelName = sqlRdr["MODEL_NAME"].ToString();
                EntShippingQuantity = int.Parse(sqlRdr["SHIPPING_QUANTITY"].ToString());
                EntEstArrivalDate = string.IsNullOrEmpty(sqlRdr["EST_ARRIVAL_DATE"].ToString()) ? (DateTime?)null : DateTime.Parse(sqlRdr["EST_ARRIVAL_DATE"].ToString());
                EntPrefReportingDate = string.IsNullOrEmpty(sqlRdr["PREF_REPORTING_DATE"].ToString()) ? (DateTime?)null : DateTime.Parse(sqlRdr["PREF_REPORTING_DATE"].ToString());
                EntSiTekEstArrivalDate = string.IsNullOrEmpty(sqlRdr["SI_TEK_EST_ARRIVAL_DATE"].ToString()) ? (DateTime?)null : DateTime.Parse(sqlRdr["SI_TEK_EST_ARRIVAL_DATE"].ToString());
                EntDeliveryLocation = sqlRdr["DELIVERY_LOCATION"].ToString();
                EntN01 = sqlRdr["N01_NO"].ToString();
                CompN01 = sqlRdr["N01_NO"].ToString();
            }
            dsnLib.DB_Close();
        }

        // 重複SO#の確認処理
        public Boolean ChkSONO()
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            Boolean blExist = false;

            stbSql.Append("SELECT ");
            stbSql.Append("    * ");
            stbSql.Append("FROM ");
            stbSql.Append("    T_SO_STATUS ");
            stbSql.Append("WHERE ");
            stbSql.Append("    SO_NO = '" + EntSONO + "' ");

            Debug.WriteLine(stbSql.ToString());
            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

            if (sqlRdr.HasRows)
            {
                blExist = true;
            }

            dsnLib.DB_Close();

            if (blExist)
            {
                // 当該SO#が既に存在している（登録NG）
                return false;
            }
            else
            {
                // 当該SO#なし（登録OK）
                return true;
            }
        }

        // 重複N01#の確認処理
        public Boolean ChkN01()
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            Boolean blExist = false;

            stbSql.Append("SELECT ");
            stbSql.Append("    * ");
            stbSql.Append("FROM ");
            stbSql.Append("    T_SO_STATUS ");
            stbSql.Append("WHERE ");
            stbSql.Append("    N01_NO = '" + EntN01 + "' ");

            Debug.WriteLine(stbSql.ToString());
            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

            if (sqlRdr.HasRows)
            {
                blExist = true;
            }

            dsnLib.DB_Close();

            if (blExist)
            {
                // 当該N01#が既に存在している（登録NG）
                return false;
            }
            else
            {
                // 当該N01#なし（登録OK）
                return true;
            }
        }

        // オーダー情報の更新処理
        public void UpdateSOList(string strUpdUID)
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder strSql = new StringBuilder();

            strSql.Append("UPDATE T_SO_STATUS ");
            strSql.Append("SET ");
            strSql.Append("    SO_NO = N'" + EntSONO + "', ");
            strSql.Append("    n90N = '" + Ent90N + "', ");
            strSql.Append("    MODEL_NAME = N'" + EntModelName + "', ");
            strSql.Append("    SHIPPING_QUANTITY = N'" + EntShippingQuantity + "', ");
            strSql.Append("    EST_ARRIVAL_DATE = '" + EntEstArrivalDate + "', ");
            strSql.Append("    PREF_REPORTING_DATE = N'" + EntPrefReportingDate + "', ");
            strSql.Append("    SI_TEK_EST_ARRIVAL_DATE = '" + EntSiTekEstArrivalDate + "', ");
            strSql.Append("    DELIVERY_LOCATION = N'" + EntDeliveryLocation + "', ");
            strSql.Append("    N01_NO = N'" + EntN01 + "', ");
            strSql.Append("    UPDATE_DATE = GETDATE(), ");
            strSql.Append("    UPDATE_UID = '" + strUpdUID + "' ");
            strSql.Append("WHERE ");
            strSql.Append("    SO_ID = '" + EntSOID + "' ");

            dsnLib.ExecSQLUpdate(strSql.ToString());
            dsnLib.DB_Close();
        }

        // シリアル情報のSO#更新処理
        public void UpdateSONO(string strUpdUID)
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder strSql = new StringBuilder();

            strSql.Append("UPDATE T_SERIAL_STATUS ");
            strSql.Append("SET ");
            strSql.Append("    SO_NO = N'" + EntSONO + "', ");
            strSql.Append("    UPDATE_DATE = GETDATE(), ");
            strSql.Append("    UPDATE_ID = '" + strUpdUID + "' ");
            strSql.Append("WHERE ");
            strSql.Append("    SO_NO = '" + CompSONO + "' ");

            dsnLib.ExecSQLUpdate(strSql.ToString());
            dsnLib.DB_Close();
        }

        // シリアル情報のN01#更新処理
        public void UpdateN01(string strUpdUID)
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder strSql = new StringBuilder();

            strSql.Append("UPDATE T_SERIAL_STATUS ");
            strSql.Append("SET ");
            strSql.Append("    SO = N'" + EntN01 + "', ");
            strSql.Append("    UPDATE_DATE = GETDATE(), ");
            strSql.Append("    UPDATE_ID = '" + strUpdUID + "' ");
            strSql.Append("WHERE ");
            strSql.Append("    SO = '" + CompN01 + "' ");

            dsnLib.ExecSQLUpdate(strSql.ToString());
            dsnLib.DB_Close();
        }
    }
}