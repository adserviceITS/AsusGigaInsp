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
                    N01NO = sqlRdr["N01_NO"].ToString(),
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
        [DisplayName("N01#")]
        public string N01NO { get; set; }
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

    public class UploadFile
    {
        [Required(ErrorMessage = "ファイルを選択してください。")]
        public HttpPostedFileBase ExcelFile { get; set; }
    }

    public class TestModels
    {
        public void OutPutReport(string StrUpdUID, UploadFile UFUploadFile)
        {
            //　Excel取得用変数
            string[,] SOList = new string[1, 1];
            int IntRowCount = 1;
            int IntColumnCount = 1;

            //------------------------------------------------------
            // Excel取込処理
            //------------------------------------------------------

            //ファイルストリーム作成
            //            FileStream Fs = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),StrPath), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            // Excelを読み取り専用で開く
            using (var WorkBook = new XLWorkbook(UFUploadFile.ExcelFile.InputStream))
            {
                var WorkSheet = WorkBook.Worksheet(1);

                // テーブル作成
                var Table = WorkSheet.RangeUsed().AsTable();

                //　テーブルの行数、列数を取得し、データ格納配列を定義する。
                IntRowCount = Table.RowCount();
                IntColumnCount = Table.ColumnCount();
                SOList = new string[IntRowCount, IntColumnCount];

                // テーブルのデータをセル毎に取得
                for (int RowCounter = 0; RowCounter < IntRowCount; RowCounter++)
                {
                    for (int ColCounter = 0; ColCounter < IntColumnCount; ColCounter++)
                    {
                        SOList[RowCounter, ColCounter] = Table.Row(RowCounter + 1).Cell(ColCounter + 1).Value.ToString();
                    }
                }
            }

            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder strSql = new StringBuilder();

            DateTime DTImportTime = DateTime.Now;
            string StrImportTime = DTImportTime.ToString("yyyyMMddHHmmss");

            //------------------------------------------------------
            // T_SO_STATUSをWK_T_SO_STATUSに保存する。
            //------------------------------------------------------
            // WK_T_SO_STATUSのレコード削除
            strSql.Append("DELETE ");
            strSql.Append("FROM ");
            strSql.Append("    WK_T_SO_STATUS ");

            dsnLib.ExecSQLUpdate(strSql.ToString());
            dsnLib.DB_Close();
            strSql.Clear();

            // WK_T_SO_STATUSにT_SO_STATUSをコピーする
            strSql.Append("INSERT ");
            strSql.Append("INTO WK_T_SO_STATUS ");
            strSql.Append("( ");
            strSql.Append("    SO_NO, ");
            strSql.Append("    n90N, ");
            strSql.Append("    MODEL_NAME, ");
            strSql.Append("    SHIPPING_QUANTITY, ");
            strSql.Append("    EST_ARRIVAL_DATE, ");
            strSql.Append("    PREF_REPORTING_DATE, ");
            strSql.Append("    SI_TEK_EST_ARRIVAL_DATE, ");
            strSql.Append("    EXP_RELEASE_DATE, ");
            strSql.Append("    SCH_RELEASE_DATE, ");
            strSql.Append("    DELIVERY_LOCATION, ");
            strSql.Append("    NG_COUNT, ");
            strSql.Append("    DOA_COUNT, ");
            strSql.Append("    NG_RATE, ");
            strSql.Append("    MEMO, ");
            strSql.Append("    RMA_NO, ");
            strSql.Append("    ACJ_ISSUE_DOA_NO_TO_SI, ");
            strSql.Append("    ASUS_CN_ISSUE_DATE, ");
            strSql.Append("    ADS_REMARK, ");
            strSql.Append("    SO_STATUS_ID, ");
            strSql.Append("    ST_CHANGE_DATE, ");
            strSql.Append("    N01_NO, ");
            strSql.Append("    DEL_FLG, ");
            strSql.Append("    INSERT_DATE, ");
            strSql.Append("    INSERT_ID, ");
            strSql.Append("    UPDATE_DATE, ");
            strSql.Append("    UPDATE_ID ");
            strSql.Append(") ");
            strSql.Append("SELECT");
            strSql.Append("    SO_NO, ");
            strSql.Append("    n90N, ");
            strSql.Append("    MODEL_NAME, ");
            strSql.Append("    SHIPPING_QUANTITY, ");
            strSql.Append("    EST_ARRIVAL_DATE, ");
            strSql.Append("    PREF_REPORTING_DATE, ");
            strSql.Append("    SI_TEK_EST_ARRIVAL_DATE, ");
            strSql.Append("    EXP_RELEASE_DATE, ");
            strSql.Append("    SCH_RELEASE_DATE, ");
            strSql.Append("    DELIVERY_LOCATION, ");
            strSql.Append("    NG_COUNT, ");
            strSql.Append("    DOA_COUNT, ");
            strSql.Append("    NG_RATE, ");
            strSql.Append("    MEMO, ");
            strSql.Append("    RMA_NO, ");
            strSql.Append("    ACJ_ISSUE_DOA_NO_TO_SI, ");
            strSql.Append("    ASUS_CN_ISSUE_DATE, ");
            strSql.Append("    ADS_REMARK, ");
            strSql.Append("    SO_STATUS_ID, ");
            strSql.Append("    ST_CHANGE_DATE, ");
            strSql.Append("    N01_NO, ");
            strSql.Append("    DEL_FLG, ");
            strSql.Append("    INSERT_DATE, ");
            strSql.Append("    INSERT_ID, ");
            strSql.Append("    UPDATE_DATE, ");
            strSql.Append("    UPDATE_ID ");
            strSql.Append("FROM ");
            strSql.Append("    T_SO_STATUS ");

            dsnLib.ExecSQLUpdate(strSql.ToString());
            dsnLib.DB_Close();

            strSql.Clear();

            //------------------------------------------------------
            // ExcelデータをT_SO_LISTに保存する。
            //------------------------------------------------------
            // Excelデータの３行目から順次データをDBに書き込む。
            // (１，２行目はタイトル行のため、読み込まない）
            for (int RowCounter = 2; RowCounter < IntRowCount; RowCounter++)
            {
                strSql.Append("INSERT ");
                strSql.Append("INTO T_SO_LIST ");
                strSql.Append("( ");
                strSql.Append("    SO_ID, ");
                strSql.Append("    SO_NO, ");
                strSql.Append("    n90N, ");
                strSql.Append("    MODEL_NAME, ");
                strSql.Append("    SHIPPING_QUANTITY, ");
                strSql.Append("    EST_ARRIVAL_DATE, ");
                strSql.Append("    PREF_REPORTING_DATE, ");
                strSql.Append("    SI_TEK_EST_ARRIVAL_DATE, ");
                strSql.Append("    EXP_RELEASE_DATE, ");
                strSql.Append("    SCH_RELEASE_DATE, ");
                strSql.Append("    DELIVERY_LOCATION, ");
                strSql.Append("    NG_COUNT, ");
                strSql.Append("    DOA_COUNT, ");
                strSql.Append("    NG_RATE, ");
                strSql.Append("    MEMO, ");
                strSql.Append("    RMA_NO, ");
                strSql.Append("    ACJ_ISSUE_DOA_NO_TO_SI, ");
                strSql.Append("    ASUS_CN_ISSUE_DATE, ");
                strSql.Append("    ADS_REMARK, ");
                strSql.Append("    N01_NO, ");
                strSql.Append("    DEL_FLG, ");
                strSql.Append("    INSERT_DATE, ");
                strSql.Append("    INSERT_ID, ");
                strSql.Append("    UPDATE_DATE, ");
                strSql.Append("    UPDATE_ID ");
                strSql.Append(") ");
                strSql.Append("VALUES ");
                strSql.Append("( ");
                strSql.Append("    '" + StrImportTime + SOList[RowCounter, 0].PadLeft(6, '0') + "', ");
                for (int ColCounter = 1; ColCounter < IntColumnCount; ColCounter++)
                {
                    if (!string.IsNullOrEmpty(SOList[RowCounter, ColCounter]))
                    {
                        if (ColCounter != IntColumnCount)
                        {
                            strSql.Append("    '" + SOList[RowCounter, ColCounter] + "', ");
                        }
                        else
                        {
                            strSql.Append("    '" + SOList[RowCounter, ColCounter] + "' ");
                        }
                    }
                    else
                    {
                        strSql.Append("    null, ");
                    }
                }
                strSql.Append("    '0', ");
                strSql.Append("    '" + DTImportTime + "', ");
                strSql.Append("    '" + StrUpdUID + "', ");
                strSql.Append("    '" + DTImportTime + "', ");
                strSql.Append("    '" + StrUpdUID + "' ");
                strSql.Append(") ");

                dsnLib.ExecSQLUpdate(strSql.ToString());

                dsnLib.DB_Close();

                strSql.Clear();
            }

            //  T_SO_STATUSのレコード区分を初期化する。
            strSql.Append("UPDATE ");
            strSql.Append("    T_SO_STATUS ");
            strSql.Append("SET ");
            strSql.Append("    RECORD_KBN = '0'");

            dsnLib.ExecSQLUpdate(strSql.ToString());

            dsnLib.DB_Close();
            strSql.Clear();

            //------------------------------------------------------
            // ExcelデータをT_SO_STATUSに保存する。
            // 存在して内容に変更があれば更新。
            // 存在しなければ追加。
            //------------------------------------------------------
            for (int RowCounter = 2; RowCounter < IntRowCount; RowCounter++)
            {

                strSql.Append("UPDATE ");
                strSql.Append("    T_SO_STATUS ");
                strSql.Append("SET ");
                strSql.Append("    SO_NO = '" + SOList[RowCounter, 1] + "', ");
                strSql.Append(StrSQLText("n90N", SOList[RowCounter, 2]));
                strSql.Append(StrSQLText("MODEL_NAME", SOList[RowCounter, 3]));
                strSql.Append(StrSQLText("SHIPPING_QUANTITY", SOList[RowCounter, 4]));
                strSql.Append(StrSQLText("EST_ARRIVAL_DATE", SOList[RowCounter, 5]));
                strSql.Append(StrSQLText("PREF_REPORTING_DATE", SOList[RowCounter, 6]));
                strSql.Append(StrSQLText("SI_TEK_EST_ARRIVAL_DATE", SOList[RowCounter, 7]));
                strSql.Append(StrSQLText("EXP_RELEASE_DATE", SOList[RowCounter, 8]));
                strSql.Append(StrSQLText("SCH_RELEASE_DATE", SOList[RowCounter, 9]));
                strSql.Append(StrSQLText("DELIVERY_LOCATION", SOList[RowCounter, 10]));
                strSql.Append(StrSQLText("NG_COUNT", SOList[RowCounter, 11]));
                strSql.Append(StrSQLText("DOA_COUNT", SOList[RowCounter, 12]));
                strSql.Append(StrSQLText("NG_RATE", SOList[RowCounter, 13]));
                strSql.Append(StrSQLText("MEMO", SOList[RowCounter, 14]));
                strSql.Append(StrSQLText("RMA_NO", SOList[RowCounter, 15]));
                strSql.Append(StrSQLText("ACJ_ISSUE_DOA_NO_TO_SI", SOList[RowCounter, 16]));
                strSql.Append(StrSQLText("ASUS_CN_ISSUE_DATE", SOList[RowCounter, 17]));
                strSql.Append(StrSQLText("ADS_REMARK", SOList[RowCounter, 18]));
                strSql.Append(StrSQLText("N01_NO", SOList[RowCounter, 19]));
                strSql.Append("RECORD_KBN = '2', ");
                strSql.Append("DEL_FLG = '0', ");
                strSql.Append("    UPDATE_DATE = '" + DTImportTime + "', ");
                strSql.Append("    UPDATE_ID = '" + StrUpdUID + "' ");
                strSql.Append("WHERE ");
                strSql.Append("    SO_NO = '" + SOList[RowCounter, 1] + "' ");
                strSql.Append("AND ( ");
                strSql.Append("        n90N <> " + StrSQLText("", SOList[RowCounter, 2]) + " ");
                strSql.Append("    OR  MODEL_NAME <> " + StrSQLText("", SOList[RowCounter, 3]) + " ");
                strSql.Append("    OR  SHIPPING_QUANTITY <> " + StrSQLText("", SOList[RowCounter, 4]) + " ");
                strSql.Append("    OR  EST_ARRIVAL_DATE <> " + StrSQLText("", SOList[RowCounter, 5]) + " ");
                strSql.Append("    OR  PREF_REPORTING_DATE <> " + StrSQLText("", SOList[RowCounter, 6]) + " ");
                strSql.Append("    OR  SI_TEK_EST_ARRIVAL_DATE <> " + StrSQLText("", SOList[RowCounter, 7]) + " ");
                strSql.Append("    OR  EXP_RELEASE_DATE <> " + StrSQLText("", SOList[RowCounter, 8]) + " ");
                strSql.Append("    OR  SCH_RELEASE_DATE <> " + StrSQLText("", SOList[RowCounter, 9]) + " ");
                strSql.Append("    OR  DELIVERY_LOCATION <> " + StrSQLText("", SOList[RowCounter, 10]) + " ");
                strSql.Append("    OR  NG_COUNT <> " + StrSQLText("", SOList[RowCounter, 11]) + " ");
                strSql.Append("    OR  DOA_COUNT <> " + StrSQLText("", SOList[RowCounter, 12]) + " ");
                strSql.Append("    OR  NG_RATE <> " + StrSQLText("", SOList[RowCounter, 13]) + " ");
                strSql.Append("    OR  MEMO <> " + StrSQLText("", SOList[RowCounter, 14]) + " ");
                strSql.Append("    OR  RMA_NO <> " + StrSQLText("", SOList[RowCounter, 15]) + " ");
                strSql.Append("    OR  ACJ_ISSUE_DOA_NO_TO_SI <> " + StrSQLText("", SOList[RowCounter, 16]) + " ");
                strSql.Append("    OR  ASUS_CN_ISSUE_DATE <> " + StrSQLText("", SOList[RowCounter, 17]) + " ");
                strSql.Append("    OR  ADS_REMARK <> " + StrSQLText("", SOList[RowCounter, 18]) + " ");
                strSql.Append("    OR  N01_NO <> " + StrSQLText("", SOList[RowCounter, 19]) + " ");
                strSql.Append("    ) ");
                strSql.Append("IF @@ROWCOUNT = 0 ");
                strSql.Append("INSERT ");
                strSql.Append("INTO T_SO_STATUS ");
                strSql.Append("( ");
                strSql.Append("    SO_NO, ");
                strSql.Append("    n90N, ");
                strSql.Append("    MODEL_NAME, ");
                strSql.Append("    SHIPPING_QUANTITY, ");
                strSql.Append("    EST_ARRIVAL_DATE, ");
                strSql.Append("    PREF_REPORTING_DATE, ");
                strSql.Append("    SI_TEK_EST_ARRIVAL_DATE, ");
                strSql.Append("    EXP_RELEASE_DATE, ");
                strSql.Append("    SCH_RELEASE_DATE, ");
                strSql.Append("    DELIVERY_LOCATION, ");
                strSql.Append("    NG_COUNT, ");
                strSql.Append("    DOA_COUNT, ");
                strSql.Append("    NG_RATE, ");
                strSql.Append("    MEMO, ");
                strSql.Append("    RMA_NO, ");
                strSql.Append("    ACJ_ISSUE_DOA_NO_TO_SI, ");
                strSql.Append("    ASUS_CN_ISSUE_DATE, ");
                strSql.Append("    ADS_REMARK, ");
                strSql.Append("    N01_NO, ");
                strSql.Append("    SO_STATUS_ID, ");
                strSql.Append("    RECORD_KBN, ");
                strSql.Append("    DEL_FLG, ");
                strSql.Append("    INSERT_DATE, ");
                strSql.Append("    INSERT_ID, ");
                strSql.Append("    UPDATE_DATE, ");
                strSql.Append("    UPDATE_ID ");
                strSql.Append(") ");
                strSql.Append("SELECT ");
                for (int ColCounter = 1; ColCounter < IntColumnCount; ColCounter++)
                {
                    if (!string.IsNullOrEmpty(SOList[RowCounter, ColCounter]))
                    {
                        if (ColCounter != IntColumnCount)
                        {
                            strSql.Append("    '" + SOList[RowCounter, ColCounter] + "', ");
                        }
                        else
                        {
                            strSql.Append("    '" + SOList[RowCounter, ColCounter] + "' ");
                        }
                    }
                    else
                    {
                        strSql.Append("    null, ");
                    }
                }
                strSql.Append("    '1010', ");
                strSql.Append("    '1', ");
                strSql.Append("    '0', ");
                strSql.Append("    '" + DTImportTime + "', ");
                strSql.Append("    '" + StrUpdUID + "', ");
                strSql.Append("    '" + DTImportTime + "', ");
                strSql.Append("    '" + StrUpdUID + "' ");
                strSql.Append("WHERE NOT EXISTS ");
                strSql.Append("    ( ");
                strSql.Append("        SELECT ");
                strSql.Append("            TOP 1 1 ");
                strSql.Append("        FROM ");
                strSql.Append("            T_SO_STATUS ");
                strSql.Append("        WHERE ");
                strSql.Append("             SO_NO = '" + SOList[RowCounter, 1] + "' ");
                strSql.Append("    ) ");

                dsnLib.ExecSQLUpdate(strSql.ToString());

                dsnLib.DB_Close();

                strSql.Clear();

            }

            strSql.Append("SELECT ");
            strSql.Append("    T_SO_STATUS.SO_NO, ");
            strSql.Append("    T_SO_STATUS.n90N AS NEW_n90N, ");
            strSql.Append("    T_SO_STATUS.MODEL_NAME AS NEW_MODEL_NAME, ");
            strSql.Append("    T_SO_STATUS.SHIPPING_QUANTITY AS NEW_SHIPPING_QUANTITY, ");
            strSql.Append("    T_SO_STATUS.EST_ARRIVAL_DATE AS NEW_EST_ARRIVAL_DATE, ");
            strSql.Append("    T_SO_STATUS.PREF_REPORTING_DATE AS NEW_PREF_REPORTING_DATE, ");
            strSql.Append("    T_SO_STATUS.SI_TEK_EST_ARRIVAL_DATE AS NEW_SI_TEK_EST_ARRIVAL_DATE, ");
            strSql.Append("    T_SO_STATUS.DELIVERY_LOCATION AS NEW_DELIVERY_LOCATION, ");
            strSql.Append("    T_SO_STATUS.N01_NO AS NEW_N01_NO, ");
            strSql.Append("    WK_T_SO_STATUS.n90N AS OLD_n90N, ");
            strSql.Append("    WK_T_SO_STATUS.MODEL_NAME AS OLD_MODEL_NAME, ");
            strSql.Append("    WK_T_SO_STATUS.SHIPPING_QUANTITY AS OLD_SHIPPING_QUANTITY, ");
            strSql.Append("    WK_T_SO_STATUS.EST_ARRIVAL_DATE AS OLD_EST_ARRIVAL_DATE, ");
            strSql.Append("    WK_T_SO_STATUS.PREF_REPORTING_DATE AS OLD_PREF_REPORTING_DATE, ");
            strSql.Append("    WK_T_SO_STATUS.SI_TEK_EST_ARRIVAL_DATE AS OLD_SI_TEK_EST_ARRIVAL_DATE, ");
            strSql.Append("    WK_T_SO_STATUS.DELIVERY_LOCATION AS OLD_DELIVERY_LOCATION, ");
            strSql.Append("    WK_T_SO_STATUS.N01_NO AS OLD_N01_NO ");
            strSql.Append("FROM ");
            strSql.Append("    T_SO_STATUS ");
            strSql.Append("    	INNER JOIN WK_T_SO_STATUS  ");
            strSql.Append("    	    ON T_SO_STATUS.SO_NO = WK_T_SO_STATUS.SO_NO  ");
            strSql.Append("WHERE ");
            strSql.Append("    RECORD_KBN = '2' ");

            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(strSql.ToString());
            List<string> lstSrchComparison = new List<string>();

            while (sqlRdr.Read())
            {
                lstSrchComparison.Add(sqlRdr["SO_NO"].ToString());
                lstSrchComparison.Add(sqlRdr["NEW_n90N"].ToString());
                lstSrchComparison.Add(sqlRdr["NEW_MODEL_NAME"].ToString());
                lstSrchComparison.Add(sqlRdr["NEW_SHIPPING_QUANTITY"].ToString());
                lstSrchComparison.Add(sqlRdr["NEW_EST_ARRIVAL_DATE"].ToString());
                lstSrchComparison.Add(sqlRdr["NEW_PREF_REPORTING_DATE"].ToString());
                lstSrchComparison.Add(sqlRdr["NEW_SI_TEK_EST_ARRIVAL_DATE"].ToString());
                lstSrchComparison.Add(sqlRdr["NEW_DELIVERY_LOCATION"].ToString());
                lstSrchComparison.Add(sqlRdr["NEW_N01_NO"].ToString());
                lstSrchComparison.Add(sqlRdr["OLD_n90N"].ToString());
                lstSrchComparison.Add(sqlRdr["OLD_MODEL_NAME"].ToString());
                lstSrchComparison.Add(sqlRdr["OLD_SHIPPING_QUANTITY"].ToString());
                lstSrchComparison.Add(sqlRdr["NEW_DELIVERY_LOCATION"].ToString());
                lstSrchComparison.Add(sqlRdr["NEW_N01_NO"].ToString());
                lstSrchComparison.Add(sqlRdr["OLD_n90N"].ToString());
                lstSrchComparison.Add(sqlRdr["OLD_MODEL_NAME"].ToString());
                lstSrchComparison.Add(sqlRdr["OLD_SHIPPING_QUANTITY"].ToString());
            }
            string[,] StrColumnChk = new string[1, 1];

            // WK_T_SO_STATUSのレコードを全て読み出す。
            strSql.Append("SELECT ");
            strSql.Append("    SO_NO ");
            strSql.Append("FROM ");
            strSql.Append("    WK_T_SO_STATUS ");



        }

        private string StrSQLText(string StrColumnName, string StrChkVal)

        {
            string StrTEXT = "";

            if (!string.IsNullOrEmpty(StrColumnName))
            {
                if (!string.IsNullOrEmpty(StrChkVal))
                {
                    StrTEXT = "    " + StrColumnName + " = '" + StrChkVal + "', ";
                }
                else
                {
                    StrTEXT = "    " + StrColumnName + " = null, ";
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(StrChkVal))
                {
                    StrTEXT = "    '" + StrChkVal + "' ";
                }
                else
                {
                    StrTEXT = "    null ";
                }
            }
            return StrTEXT;
        }
    }

    public class SrchCopyOrder
    {
        public string SONO { get; set; }
        public string n90N { get; set; }
        public string ModelName { get; set; }
        public string ShippingQuantity { get; set; }
        public string EstArrivalDate { get; set; }
        public string PrefReportingDate { get; set; }
        public string SiTekEstArrivalDate { get; set; }
        public string ExpReleaseDate { get; set; }
        public string SchReleaseDate { get; set; }
        public string DeliveryLocation { get; set; }
        public string N01NO { get; set; }
        public string NGCount { get; set; }
        public string DoaCount { get; set; }
        public string NGRate { get; set; }
        public string Memo { get; set; }
        public string RmaNO { get; set; }
        public string AcjIssueDoaNoToSI { get; set; }
        public string AsusCnIssueDate { get; set; }
        public string ADSRemark { get; set; }
        public string DEL_FLG { get; set; }
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

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? EntEstArrivalDate { get; set; }

        public DateTime? EntPrefReportingDate { get; set; }
        public DateTime? EntSiTekEstArrivalDate { get; set; }

        public string EntDeliveryLocation { get; set; }

        public string EntN01 { get; set; }
        public string CompN01 { get; set; }

        public string AddUser { get; set; }
        public DateTime? AddDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }

        // オーダーの検索結果セット
        public void SetSOListDetails(string strSOID)
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            long lngSOID = long.Parse(strSOID);

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
            stbSql.Append("   T_SO_STATUS.SO_NO = " + lngSOID);

            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

            while (sqlRdr.Read())
            {
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
                AddUser = sqlRdr["USER_NAME1"].ToString();
                AddDate = string.IsNullOrEmpty(sqlRdr["INSERT_DATE"].ToString()) ? (DateTime?)null : DateTime.Parse(sqlRdr["INSERT_DATE"].ToString());
                UpdateUser = sqlRdr["USER_NAME2"].ToString();
                UpdateDate = string.IsNullOrEmpty(sqlRdr["UPDATE_DATE"].ToString()) ? (DateTime?)null : DateTime.Parse(sqlRdr["UPDATE_DATE"].ToString());
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
            stbSql.Append("    SO_NO = N'" + EntSONO + "' ");

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
            stbSql.Append("    N01_NO = N'" + EntN01 + "' ");

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
            strSql.Append("    SHIPPING_QUANTITY = " + EntShippingQuantity + ", ");
            if (!string.IsNullOrEmpty(EntEstArrivalDate.ToString()))
            {
                strSql.Append("    EST_ARRIVAL_DATE = '" + EntEstArrivalDate + "', ");
            }
            else
            {
                strSql.Append("    EST_ARRIVAL_DATE = null, ");
            }
            if (!string.IsNullOrEmpty(EntPrefReportingDate.ToString()))
            {
                strSql.Append("    PREF_REPORTING_DATE = '" + EntPrefReportingDate + "', ");
            }
            else
            {
                strSql.Append("    PREF_REPORTING_DATE = null, ");
            }
            if (!string.IsNullOrEmpty(EntSiTekEstArrivalDate.ToString()))
            {
                strSql.Append("    SI_TEK_EST_ARRIVAL_DATE = '" + EntSiTekEstArrivalDate + "', ");
            }
            else
            {
                strSql.Append("    SI_TEK_EST_ARRIVAL_DATE = null, ");
            }
            strSql.Append("    DELIVERY_LOCATION = N'" + EntDeliveryLocation + "', ");
            strSql.Append("    N01_NO = N'" + EntN01 + "', ");
            strSql.Append("    UPDATE_DATE = GETDATE(), ");
            strSql.Append("    UPDATE_ID = N'" + strUpdUID + "' ");
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
            strSql.Append("    UPDATE_ID = N'" + strUpdUID + "' ");
            strSql.Append("WHERE ");
            strSql.Append("    SO_NO = N'" + CompSONO + "' ");

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
            strSql.Append("    UPDATE_ID = N'" + strUpdUID + "' ");
            strSql.Append("WHERE ");
            strSql.Append("    SO = N'" + CompN01 + "' ");

            dsnLib.ExecSQLUpdate(strSql.ToString());
            dsnLib.DB_Close();
        }

        // オーダー情報の削除処理
        public void DeleteSOList(string strUpdUID)
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder strSql = new StringBuilder();

            strSql.Append("UPDATE T_SO_STATUS ");
            strSql.Append("SET ");
            strSql.Append("    DEL_FLG = N'1', ");
            strSql.Append("    UPDATE_DATE = GETDATE(), ");
            strSql.Append("    UPDATE_ID = N'" + strUpdUID + "' ");
            strSql.Append("WHERE ");
            strSql.Append("    SO_ID = N'" + EntSOID + "' ");

            dsnLib.ExecSQLUpdate(strSql.ToString());
            dsnLib.DB_Close();
        }

        // シリアル情報の削除処理
        public void DeleteSerialList(string strUpdUID)
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder strSql = new StringBuilder();

            strSql.Append("UPDATE T_SERIAL_STATUS ");
            strSql.Append("SET ");
            strSql.Append("    DEL_FLG = N'1', ");
            strSql.Append("    UPDATE_DATE = GETDATE(), ");
            strSql.Append("    UPDATE_ID = N'" + strUpdUID + "' ");
            strSql.Append("WHERE ");
            strSql.Append("    SO = N'" + CompSONO + "' ");

            dsnLib.ExecSQLUpdate(strSql.ToString());
            dsnLib.DB_Close();
        }

    }
}