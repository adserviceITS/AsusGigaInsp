using AsusGigaInsp.Modules;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace AsusGigaInsp.Models
{
    public class SOListModels
    {
        public IEnumerable<DropDownStatusName> DropDownListStatusName { get; set; }

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
            stbSql.Append("    T_SO_STATUS.SO_NO, ");
            stbSql.Append("    T_SO_STATUS.n90N, ");
            stbSql.Append("    T_SO_STATUS.MODEL_NAME, ");
            stbSql.Append("    T_SO_STATUS.SHIPPING_QUANTITY, ");
            stbSql.Append("    T_SO_STATUS.EST_ARRIVAL_DATE, ");
            stbSql.Append("    T_SO_STATUS.PREF_REPORTING_DATE, ");
            stbSql.Append("    T_SO_STATUS.SI_TEK_EST_ARRIVAL_DATE, ");
            stbSql.Append("    T_SO_STATUS.DELIVERY_LOCATION, ");
            stbSql.Append("    T_SO_STATUS.N01_NO, ");
            stbSql.Append("    T_SO_STATUS.SO_STATUS_ID, ");
            stbSql.Append("    M_SO_STATUS.SO_STATUS_NAME, ");
            stbSql.Append("    T_SO_STATUS.ST_CHANGE_DATE, ");
            stbSql.Append("    IsNull(TBL1.COMPLETE_WORK_UNIT, 0) AS COMPLETE_WORK_UNIT, ");
            stbSql.Append("    IsNull(TBL2.DOA_UNIT, 0) AS DOA_UNIT, ");
            stbSql.Append("    IsNull(TBL3.HOLD_UNIT, 0) AS HOLD_UNIT, ");
            stbSql.Append("    IsNull(TBL1.COMPLETE_WORK_UNIT, 0) - IsNull(TBL2.DOA_UNIT, 0) - IsNull(TBL3.HOLD_UNIT, 0) AS FIXED_SHIPPING_QUANTITY ");
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
            stbSql.Append("            T_SERIAL_STATUS.SERIAL_STATUS_ID = '4010' ");
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
            stbSql.Append("    LEFT JOIN ( ");
            stbSql.Append("        SELECT ");
            stbSql.Append("            T_SERIAL_STATUS.SO_NO, ");
            stbSql.Append("            Count(*) AS HOLD_UNIT ");
            stbSql.Append("        FROM ");
            stbSql.Append("            T_SERIAL_STATUS ");
            stbSql.Append("        WHERE ");
            stbSql.Append("            T_SERIAL_STATUS.INSTRUCTION = '003' ");
            stbSql.Append("        GROUP BY ");
            stbSql.Append("            T_SERIAL_STATUS.SO_NO ");
            stbSql.Append("    ) TBL3 ");
            stbSql.Append("        ON T_SO_STATUS.SO_NO = TBL3.SO_NO ");
            stbSql.Append(stbWhere);

            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

            List<SrchRstOrder> lstSrchRstOrder = new List<SrchRstOrder>();

            while (sqlRdr.Read())
            {
                lstSrchRstOrder.Add(new SrchRstOrder
                {
                    SONO = sqlRdr["SO_NO"].ToString(),
                    n90N = sqlRdr["n90N"].ToString(),
                    ModelName = sqlRdr["MODEL_NAME"].ToString(),
                    ShippingQuantity = sqlRdr["SHIPPING_QUANTITY"].ToString(),
                    FixedShippingQuantity = sqlRdr["FIXED_SHIPPING_QUANTITY"].ToString(),
                    EstArrivalDate = sqlRdr["EST_ARRIVAL_DATE"].ToString(),
                    PrefReportingDate = sqlRdr["PREF_REPORTING_DATE"].ToString(),
                    SiTekEstArrivalDate = sqlRdr["SI_TEK_EST_ARRIVAL_DATE"].ToString(),
                    DeliveryLocation = sqlRdr["DELIVERY_LOCATION"].ToString(),
                    CompleteWorkUnit = sqlRdr["COMPLETE_WORK_UNIT"].ToString(),
                    DOAUnit = sqlRdr["DOA_UNIT"].ToString(),
                    HoldUnit = sqlRdr["HOLD_UNIT"].ToString(),
                    N01NO = sqlRdr["N01_NO"].ToString(),
                    SOStatusID = sqlRdr["SO_STATUS_ID"].ToString(),
                    SOStatusName = sqlRdr["SO_STATUS_NAME"].ToString(),
                    STChangeDate = sqlRdr["ST_CHANGE_DATE"].ToString()
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
                stbWhere.Append("AND T_SO_STATUS.SO_NO = N'" + SrchSONO + "' ");
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
                    stbWhere.Append("AND T_SO_STATUS.SO_STATUS_ID = N'" + SrchStatusID + "' ");
                }

            }
        }
    }

    public class SrchRstOrder
    {
        [DisplayName("SO ID")]
        public string SOID { get; set; }
        [DisplayName("SO#")]
        public string SONO { get; set; }
        [DisplayName("90N")]
        public string n90N { get; set; }
        [DisplayName("Model Name")]
        public string ModelName{ get; set; }
        [DisplayName("出荷予定数")]
        public string ShippingQuantity { get; set; }
        [DisplayName("出荷数")]
        public string FixedShippingQuantity { get; set; }
        [DisplayName("ADS到着予定日")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public string EstArrivalDate { get; set; }
        [DisplayName("レポート提出希望日")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public string PrefReportingDate { get; set; }
        [DisplayName("SI/TEK到着予定日")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public string SiTekEstArrivalDate { get; set; }
        [DisplayName("納品地")]
        public string DeliveryLocation { get; set; }
        [DisplayName("作業完了数")]
        public string CompleteWorkUnit { get; set; }
        [DisplayName("ASUS確認後DOA数")]
        public string DOAUnit { get; set; }
        [DisplayName("保留数")]
        public string HoldUnit { get; set; }
        [DisplayName("N01#")]
        public string N01NO { get; set; }
        [DisplayName("ステータスID")]
        public string SOStatusID { get; set; }
        [DisplayName("ステータス")]
        public string SOStatusName { get; set; }
        [DisplayName("ステータス変更日")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}")]
        public string STChangeDate { get; set; }
    }

    public class DropDownStatusName
    {
        public string SOStatusID { get; set; }
        public string SOStatusName { get; set; }
    }
}