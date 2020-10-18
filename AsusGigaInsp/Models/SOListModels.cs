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
        public string SrchDateError { get; set; }
        public string DispInsertDate { get; set; }

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

        public void SetInsertDate()
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            stbSql.Append("SELECT ");
            stbSql.Append("    MIN(INSERT_DATE) AS INSERT_DATE ");
            stbSql.Append("FROM ");
            stbSql.Append("    T_SO_CHANGE_CONTROL ");

            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

            while (sqlRdr.Read())
            {
                DispInsertDate = sqlRdr["INSERT_DATE"].ToString();
            }

            dsnLib.DB_Close();
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
            stbSql.Append("    T_SO_STATUS.CAP, ");
            stbSql.Append("    T_SO_STATUS.DELIVERY_LOCATION, ");
            stbSql.Append("    T_SO_STATUS.N01_NO, ");
            stbSql.Append("    T_SO_STATUS.SO_STATUS_ID, ");
            stbSql.Append("    M_SO_STATUS.SO_STATUS_NAME, ");
            stbSql.Append("    T_SO_STATUS.ST_CHANGE_DATE, ");
            stbSql.Append("    T_SO_STATUS.RECORD_KBN, ");
            stbSql.Append("    T_SO_STATUS.HOLD_FLG, ");
            stbSql.Append("    IsNull(TBL4.INPUT_UNIT, 0) AS INPUT_UNIT, ");
            stbSql.Append("    IsNull(TBL1.COMPLETE_WORK_UNIT, 0) AS COMPLETE_WORK_UNIT, ");
            stbSql.Append("    IsNull(TBL2.DOA_UNIT, 0) AS DOA_UNIT, ");
            stbSql.Append("    IsNull(TBL3.HOLD_UNIT, 0) AS HOLD_UNIT, ");
            stbSql.Append("    IsNull(TBL1.COMPLETE_WORK_UNIT, 0) - IsNull(TBL2.DOA_UNIT, 0) - IsNull(TBL3.HOLD_UNIT, 0) AS FIXED_SHIPPING_QUANTITY, ");
            stbSql.Append("    IsNull(T_SO_CHANGE_CONTROL.CHG_n90N_FLG, '0') AS CHG_n90N_FLG,  ");
            stbSql.Append("    IsNull(T_SO_CHANGE_CONTROL.CHG_MODEL_NAME_FLG, '0') AS CHG_MODEL_NAME_FLG, ");
            stbSql.Append("    IsNull(T_SO_CHANGE_CONTROL.CHG_SHIPPING_QUANTITY_FLG, '0') AS CHG_SHIPPING_QUANTITY_FLG, ");
            stbSql.Append("    IsNull(T_SO_CHANGE_CONTROL.CHG_EST_ARRIVAL_DATE_FLG, '0') AS CHG_EST_ARRIVAL_DATE_FLG, ");
            stbSql.Append("    IsNull(T_SO_CHANGE_CONTROL.CHG_PREF_REPORTING_DATE_FLG, '0') AS CHG_PREF_REPORTING_DATE_FLG, ");
            stbSql.Append("    IsNull(T_SO_CHANGE_CONTROL.CHG_SI_TEK_EST_ARRIVAL_DATE_FLG, '0') AS CHG_SI_TEK_EST_ARRIVAL_DATE_FLG, ");
            stbSql.Append("    IsNull(T_SO_CHANGE_CONTROL.CHG_CAP_FLG, '0') AS CHG_CAP_FLG, ");
            stbSql.Append("    IsNull(T_SO_CHANGE_CONTROL.CHG_DELIVERY_LOCATION_FLG, '0') AS CHG_DELIVERY_LOCATION_FLG, ");
            stbSql.Append("    IsNull(T_SO_CHANGE_CONTROL.CHG_N01_NO_FLG, '0') AS CHG_N01_NO_FLG, ");
            stbSql.Append("    T_SO_CHANGE_CONTROL.EST_ARRIVAL_DATE_WARNING_FLG, ");
            stbSql.Append("    T_SO_CHANGE_CONTROL.PREF_REPORTING_DATE_WARNING_FLG, ");
            stbSql.Append("    T_SO_CHANGE_CONTROL.SI_TEK_EST_ARRIVAL_DATE_WARNING_FLG, ");
            stbSql.Append("    T_SO_CHANGE_CONTROL.INSERT_DATE ");
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
            stbSql.Append("            T_SERIAL_STATUS.SERIAL_STATUS_ID >= '4010' ");
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
            stbSql.Append("    LEFT JOIN ( ");
            stbSql.Append("        SELECT ");
            stbSql.Append("            T_SERIAL_STATUS.SO_NO, ");
            stbSql.Append("            Count(*) AS INPUT_UNIT ");
            stbSql.Append("        FROM ");
            stbSql.Append("            T_SERIAL_STATUS ");
            stbSql.Append("        WHERE ");
            stbSql.Append("            T_SERIAL_STATUS.SERIAL_STATUS_ID >= '3010' ");
            stbSql.Append("        GROUP BY ");
            stbSql.Append("            T_SERIAL_STATUS.SO_NO ");
            stbSql.Append("    ) TBL4 ");
            stbSql.Append("        ON T_SO_STATUS.SO_NO = TBL4.SO_NO ");
            stbSql.Append("    LEFT JOIN T_SO_CHANGE_CONTROL ");
            stbSql.Append("        ON T_SO_STATUS.SO_NO = T_SO_CHANGE_CONTROL.SO_NO ");
            stbSql.Append("    LEFT JOIN M_DESTINATION ");
            stbSql.Append("        ON T_SO_STATUS.DELIVERY_LOCATION = M_DESTINATION.DESTINATION_NAME ");
            stbSql.Append(stbWhere);
            stbSql.Append("ORDER BY ");
            //stbSql.Append("    T_SO_STATUS, ");
            stbSql.Append("    HOLD_FLG, ");
            stbSql.Append("    PREF_REPORTING_DATE DESC, ");
            stbSql.Append("    SI_TEK_EST_ARRIVAL_DATE DESC, ");
            stbSql.Append("    SORT_ORDER, ");
            stbSql.Append("    SO_NO ");

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
                    Cap = sqlRdr["CAP"].ToString(),
                    HoldFlg = (bool)sqlRdr["HOLD_FLG"],
                    InputUnit = sqlRdr["INPUT_UNIT"].ToString(),
                    CompleteWorkUnit = sqlRdr["COMPLETE_WORK_UNIT"].ToString(),
                    DOAUnit = sqlRdr["DOA_UNIT"].ToString(),
                    HoldUnit = sqlRdr["HOLD_UNIT"].ToString(),
                    N01NO = sqlRdr["N01_NO"].ToString(),
                    SOStatusID = sqlRdr["SO_STATUS_ID"].ToString(),
                    SOStatusName = sqlRdr["SO_STATUS_NAME"].ToString(),
                    STChangeDate = sqlRdr["ST_CHANGE_DATE"].ToString(),
                    RecordKBN = sqlRdr["RECORD_KBN"].ToString(),
                    CHGn90NFLG = sqlRdr["CHG_n90N_FLG"].ToString(),
                    CHGModelNameFLG = sqlRdr["CHG_MODEL_NAME_FLG"].ToString(),
                    CHGShippingQuantityFLG = sqlRdr["CHG_SHIPPING_QUANTITY_FLG"].ToString(),
                    CHGEstArrivalDateFLG = sqlRdr["CHG_EST_ARRIVAL_DATE_FLG"].ToString(),
                    CHGPrefReportingDateFLG = sqlRdr["CHG_PREF_REPORTING_DATE_FLG"].ToString(),
                    CHGSiTekEstArrivalDateFLG = sqlRdr["CHG_SI_TEK_EST_ARRIVAL_DATE_FLG"].ToString(),
                    CHGNCapFLG = sqlRdr["CHG_CAP_FLG"].ToString(),
                    CHGDeliveryLocationFLG = sqlRdr["CHG_DELIVERY_LOCATION_FLG"].ToString(),
                    CHGN01NoFLG = sqlRdr["CHG_N01_NO_FLG"].ToString(),
                    EstArrivalDateWarningFLG = sqlRdr["EST_ARRIVAL_DATE_WARNING_FLG"].ToString(),
                    PrefReportingDateWarningFLG = sqlRdr["PREF_REPORTING_DATE_WARNING_FLG"].ToString(),
                    SiTekEstArrivalDateWarningFLG = sqlRdr["SI_TEK_EST_ARRIVAL_DATE_WARNING_FLG"].ToString(),
                    InsertDate = sqlRdr["INSERT_DATE"].ToString()
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
                    stbWhere.Append("AND LEFT(T_SO_STATUS.EST_ARRIVAL_DATE, 10) >= '" + SrchEstArrivalDate_S + "' ");
                }

                if (!string.IsNullOrEmpty(SrchEstArrivalDate_E))
                {
                    stbWhere.Append("AND LEFT (T_SO_STATUS.EST_ARRIVAL_DATE, 10) <= '" + SrchEstArrivalDate_E + "' ");
                }

                if (!string.IsNullOrEmpty(SrchPrefReportingDate_S))
                {
                    stbWhere.Append("AND LEFT(T_SO_STATUS.PREF_REPORTING_DATE, 10) >= '" + SrchPrefReportingDate_S + "' ");
                }

                if (!string.IsNullOrEmpty(SrchPrefReportingDate_E))
                {
                    stbWhere.Append("AND LEFT(T_SO_STATUS.PREF_REPORTING_DATE, 10) <= '" + SrchPrefReportingDate_E + "' ");
                }

                if (!string.IsNullOrEmpty(SrchSiTekEstArrivalDate_S))
                {
                    stbWhere.Append("AND LEFT(T_SO_STATUS.SI_TEK_EST_ARRIVAL_DATE, 10) >= '" + SrchSiTekEstArrivalDate_S + "' ");
                }

                if (!string.IsNullOrEmpty(SrchSiTekEstArrivalDate_E))
                {
                    stbWhere.Append("AND LEFT(T_SO_STATUS.SI_TEK_EST_ARRIVAL_DATE, 10) <= '" + SrchSiTekEstArrivalDate_E + "' ");
                }

                if (!string.IsNullOrEmpty(SrchStatusID))
                {
                    stbWhere.Append("AND T_SO_STATUS.SO_STATUS_ID = N'" + SrchStatusID + "' ");
                }

                if (!string.IsNullOrEmpty(SrchDateError))
                {
                    if (SrchDateError == "0")
                    {
                        stbWhere.Append("AND (T_SO_CHANGE_CONTROL.EST_ARRIVAL_DATE_WARNING_FLG = N'0' AND T_SO_CHANGE_CONTROL.PREF_REPORTING_DATE_WARNING_FLG = N'0' AND T_SO_CHANGE_CONTROL.SI_TEK_EST_ARRIVAL_DATE_WARNING_FLG = N'0') ");
                    }
                    else
                    {
                        stbWhere.Append("AND (T_SO_CHANGE_CONTROL.EST_ARRIVAL_DATE_WARNING_FLG = N'1' OR T_SO_CHANGE_CONTROL.PREF_REPORTING_DATE_WARNING_FLG = N'1' OR T_SO_CHANGE_CONTROL.SI_TEK_EST_ARRIVAL_DATE_WARNING_FLG = N'1') ");
                    }
                }

            }
        }
    }

    public class SrchRstOrder
    {
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

        // 2020/9/28 Add K.Kikuchi
        [DisplayName("キャップ有無")]
        public string Cap { get; set; }
        [DisplayName("保留")]
        public bool HoldFlg { get; set; }
        // add End

        [DisplayName("投入数")]
        public string InputUnit { get; set; }
        [DisplayName("作業完了数")]
        public string CompleteWorkUnit { get; set; }
        [DisplayName("ASUS確認後DOA数")]
        public string DOAUnit { get; set; }
        [DisplayName("保留数")]
        public string HoldUnit { get; set; }
        [DisplayName("N01#")]
        public string N01NO { get; set; }
        [DisplayName("ステータス")]
        public string SOStatusName { get; set; }
        [DisplayName("ステータス変更日")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}")]
        public string STChangeDate { get; set; }
        public string SOStatusID { get; set; }
        public string RecordKBN { get; set; }
        public string CHGn90NFLG { get; set; }
        public string CHGModelNameFLG { get; set; }
        public string CHGShippingQuantityFLG { get; set; }
        public string CHGEstArrivalDateFLG { get; set; }
        public string CHGPrefReportingDateFLG { get; set; }
        public string CHGSiTekEstArrivalDateFLG { get; set; }
        public string CHGNCapFLG { get; set; }
        public string CHGDeliveryLocationFLG { get; set; }
        public string CHGN01NoFLG { get; set; }
        public string EstArrivalDateWarningFLG { get; set; }
        public string PrefReportingDateWarningFLG { get; set; }
        public string SiTekEstArrivalDateWarningFLG { get; set; }
        public string InsertDate { get; set; }
    }

    public class DropDownStatusName
    {
        public string SOStatusID { get; set; }
        public string SOStatusName { get; set; }
    }
}
