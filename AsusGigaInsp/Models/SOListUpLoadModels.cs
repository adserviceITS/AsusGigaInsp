using AsusGigaInsp.Modules;
using System.Data.SqlClient;
using System.Text;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using ClosedXML.Excel;
using System.Data;

namespace AsusGigaInsp.Models
{
    public class UploadFile
    {
        [Required(ErrorMessage = "ファイルを選択してください。")]
        public HttpPostedFileBase ExcelFile { get; set; }
    }

    public class SOListUpLoadModels
    {
        public string[,] SOList = new string[1, 1];
        public UploadFile UFUploadFile;

        public void GetSOExcelData()
        {
            //　Excel取得用変数
            int IntRowCount = 1;
            int IntColumnCount = 1;

            //------------------------------------------------------
            // Excel取込処理
            //------------------------------------------------------
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
                        SOList[RowCounter, ColCounter] = Table.Row(RowCounter + 1).Cell(1).Value.ToString();

                        if (!string.IsNullOrEmpty(Table.Row(RowCounter + 1).Cell(2).Value.ToString()))
                        {
                            // Q列とR列に変なデータが入ってるので明示的に日付に変換している
                            if ((RowCounter > 1 && ColCounter == 16) || (RowCounter > 1 && ColCounter == 17))
                            {
                                if (!string.IsNullOrEmpty(Table.Row(RowCounter + 1).Cell(ColCounter + 1).Value.ToString()))
                                {
                                    DateTime DTDateValue;
                                    if (DateTime.TryParse(Table.Row(RowCounter + 1).Cell(ColCounter + 1).Value.ToString(), out DTDateValue))
                                    {
                                        SOList[RowCounter, ColCounter] = DTDateValue.ToString();
                                    }
                                    else
                                    {
                                        DateTime DTVal = DateTime.FromOADate(Double.Parse(Table.Row(RowCounter + 1).Cell(ColCounter + 1).Value.ToString()));
                                        SOList[RowCounter, ColCounter] = DTVal.ToString();
                                    }
                                }
                            }
                            else
                            {
                                SOList[RowCounter, ColCounter] = Table.Row(RowCounter + 1).Cell(ColCounter + 1).Value.ToString();
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        //------------------------------------------------------
        // ExcelデータをT_SO_LISTに保存する。
        //------------------------------------------------------
        public void InsertSOList(DateTime DTImportTime, string StrUpdUID)
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();
            string StrImportTime = DTImportTime.ToString("yyyyMMddHHmmss");

            // Excelデータの３行目から順次データをDBに書き込む。
            // (１，２行目はタイトル行のため、読み込まない）
            int IntRowCount = SOList.GetLength(0);
            int IntColumnCount = SOList.GetLength(1);

            for (int RowCounter = 2; RowCounter < IntRowCount; RowCounter++)
            {
                stbSql.Append("INSERT ");
                stbSql.Append("INTO T_SO_LIST ");
                stbSql.Append("( ");
                stbSql.Append("    SO_ID, ");
                stbSql.Append("    SO_NO, ");
                stbSql.Append("    n90N, ");
                stbSql.Append("    MODEL_NAME, ");
                stbSql.Append("    SHIPPING_QUANTITY, ");
                stbSql.Append("    EST_ARRIVAL_DATE, ");
                stbSql.Append("    PREF_REPORTING_DATE, ");
                stbSql.Append("    SI_TEK_EST_ARRIVAL_DATE, ");
                stbSql.Append("    EXP_RELEASE_DATE, ");
                stbSql.Append("    SCH_RELEASE_DATE, ");
                stbSql.Append("    DELIVERY_LOCATION, ");
                stbSql.Append("    NG_COUNT, ");
                stbSql.Append("    DOA_COUNT, ");
                stbSql.Append("    NG_RATE, ");
                stbSql.Append("    MEMO, ");
                stbSql.Append("    RMA_NO, ");
                stbSql.Append("    ACJ_ISSUE_DOA_NO_TO_SI, ");
                stbSql.Append("    ASUS_CN_ISSUE_DATE, ");
                stbSql.Append("    ADS_REMARK, ");
                stbSql.Append("    N01_NO, ");
                stbSql.Append("    DEL_FLG, ");
                stbSql.Append("    INSERT_DATE, ");
                stbSql.Append("    INSERT_ID, ");
                stbSql.Append("    UPDATE_DATE, ");
                stbSql.Append("    UPDATE_ID ");
                stbSql.Append(") ");
                stbSql.Append("VALUES ");
                stbSql.Append("( ");
                stbSql.Append("    '" + StrImportTime + SOList[RowCounter, 0].PadLeft(6, '0') + "', ");
                for (int ColCounter = 1; ColCounter < IntColumnCount; ColCounter++)
                {
                    if (!string.IsNullOrEmpty(SOList[RowCounter, ColCounter]))
                    {
                        if (ColCounter != IntColumnCount)
                        {
                            stbSql.Append("    '" + SOList[RowCounter, ColCounter] + "', ");
                        }
                        else
                        {
                            stbSql.Append("    '" + SOList[RowCounter, ColCounter] + "' ");
                        }
                    }
                    else
                    {
                        stbSql.Append("    null, ");
                    }
                }
                stbSql.Append("    '0', ");
                stbSql.Append("    '" + DTImportTime + "', ");
                stbSql.Append("    '" + StrUpdUID + "', ");
                stbSql.Append("    '" + DTImportTime + "', ");
                stbSql.Append("    '" + StrUpdUID + "' ");
                stbSql.Append(") ");

                dsnLib.ExecSQLUpdate(stbSql.ToString());

                dsnLib.DB_Close();

                stbSql.Clear();
            }
        }

        //------------------------------------------------------
        // ExcelデータをT_SO_STATUSに保存する。
        //------------------------------------------------------
        public void UpsertSOStatus(DateTime DTImportTime, string StrUpdUID)
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            // Excelデータの３行目から順次データをDBに書き込む。
            // (１，２行目はタイトル行のため、読み込まない）
            int IntRowCount = SOList.GetLength(0);
            int IntColumnCount = SOList.GetLength(1);

            //------------------------------------------------------
            // T_SO_STATUSをWK_T_SO_STATUSに保存する。
            //------------------------------------------------------
            // WK_T_SO_STATUSのレコード削除
            stbSql.Append("DELETE ");
            stbSql.Append("FROM ");
            stbSql.Append("    WK_T_SO_STATUS ");

            dsnLib.ExecSQLUpdate(stbSql.ToString());
            dsnLib.DB_Close();
            stbSql.Clear();

            // WK_T_SO_STATUSにT_SO_STATUSをコピーする
            stbSql.Append("INSERT ");
            stbSql.Append("INTO WK_T_SO_STATUS ");
            stbSql.Append("( ");
            stbSql.Append("    SO_NO, ");
            stbSql.Append("    n90N, ");
            stbSql.Append("    MODEL_NAME, ");
            stbSql.Append("    SHIPPING_QUANTITY, ");
            stbSql.Append("    EST_ARRIVAL_DATE, ");
            stbSql.Append("    PREF_REPORTING_DATE, ");
            stbSql.Append("    SI_TEK_EST_ARRIVAL_DATE, ");
            stbSql.Append("    EXP_RELEASE_DATE, ");
            stbSql.Append("    SCH_RELEASE_DATE, ");
            stbSql.Append("    DELIVERY_LOCATION, ");
            stbSql.Append("    NG_COUNT, ");
            stbSql.Append("    DOA_COUNT, ");
            stbSql.Append("    NG_RATE, ");
            stbSql.Append("    MEMO, ");
            stbSql.Append("    RMA_NO, ");
            stbSql.Append("    ACJ_ISSUE_DOA_NO_TO_SI, ");
            stbSql.Append("    ASUS_CN_ISSUE_DATE, ");
            stbSql.Append("    ADS_REMARK, ");
            stbSql.Append("    SO_STATUS_ID, ");
            stbSql.Append("    ST_CHANGE_DATE, ");
            stbSql.Append("    N01_NO, ");
            stbSql.Append("    DEL_FLG, ");
            stbSql.Append("    INSERT_DATE, ");
            stbSql.Append("    INSERT_ID, ");
            stbSql.Append("    UPDATE_DATE, ");
            stbSql.Append("    UPDATE_ID ");
            stbSql.Append(") ");
            stbSql.Append("SELECT");
            stbSql.Append("    SO_NO, ");
            stbSql.Append("    n90N, ");
            stbSql.Append("    MODEL_NAME, ");
            stbSql.Append("    SHIPPING_QUANTITY, ");
            stbSql.Append("    EST_ARRIVAL_DATE, ");
            stbSql.Append("    PREF_REPORTING_DATE, ");
            stbSql.Append("    SI_TEK_EST_ARRIVAL_DATE, ");
            stbSql.Append("    EXP_RELEASE_DATE, ");
            stbSql.Append("    SCH_RELEASE_DATE, ");
            stbSql.Append("    DELIVERY_LOCATION, ");
            stbSql.Append("    NG_COUNT, ");
            stbSql.Append("    DOA_COUNT, ");
            stbSql.Append("    NG_RATE, ");
            stbSql.Append("    MEMO, ");
            stbSql.Append("    RMA_NO, ");
            stbSql.Append("    ACJ_ISSUE_DOA_NO_TO_SI, ");
            stbSql.Append("    ASUS_CN_ISSUE_DATE, ");
            stbSql.Append("    ADS_REMARK, ");
            stbSql.Append("    SO_STATUS_ID, ");
            stbSql.Append("    ST_CHANGE_DATE, ");
            stbSql.Append("    N01_NO, ");
            stbSql.Append("    DEL_FLG, ");
            stbSql.Append("    INSERT_DATE, ");
            stbSql.Append("    INSERT_ID, ");
            stbSql.Append("    UPDATE_DATE, ");
            stbSql.Append("    UPDATE_ID ");
            stbSql.Append("FROM ");
            stbSql.Append("    T_SO_STATUS ");

            dsnLib.ExecSQLUpdate(stbSql.ToString());
            dsnLib.DB_Close();

            stbSql.Clear();

            // T_SO_STATUSのレコード区分を初期化する。
            stbSql.Append("UPDATE ");
            stbSql.Append("    T_SO_STATUS ");
            stbSql.Append("SET ");
            stbSql.Append("    RECORD_KBN = '0'");

            dsnLib.ExecSQLUpdate(stbSql.ToString());

            dsnLib.DB_Close();
            stbSql.Clear();

            for (int RowCounter = 2; RowCounter < IntRowCount; RowCounter++)
            {
                //------------------------------------------------------
                // ExcelデータをT_SO_STATUSに保存する。
                // 存在して内容に変更があれば更新。
                // 存在しなければ追加。
                //------------------------------------------------------
                stbSql.Append("UPDATE ");
                stbSql.Append("    T_SO_STATUS ");
                stbSql.Append("SET ");
                stbSql.Append("    SO_NO = '" + SOList[RowCounter, 1] + "', ");
                stbSql.Append(StrSQLText("n90N", SOList[RowCounter, 2]));
                stbSql.Append(StrSQLText("MODEL_NAME", SOList[RowCounter, 3]));
                stbSql.Append(StrSQLText("SHIPPING_QUANTITY", SOList[RowCounter, 4]));
                stbSql.Append(StrSQLText("EST_ARRIVAL_DATE", SOList[RowCounter, 5]));
                stbSql.Append(StrSQLText("PREF_REPORTING_DATE", SOList[RowCounter, 6]));
                stbSql.Append(StrSQLText("SI_TEK_EST_ARRIVAL_DATE", SOList[RowCounter, 7]));
                stbSql.Append(StrSQLText("EXP_RELEASE_DATE", SOList[RowCounter, 8]));
                stbSql.Append(StrSQLText("SCH_RELEASE_DATE", SOList[RowCounter, 9]));
                stbSql.Append(StrSQLText("DELIVERY_LOCATION", SOList[RowCounter, 10]));
                stbSql.Append(StrSQLText("NG_COUNT", SOList[RowCounter, 11]));
                stbSql.Append(StrSQLText("DOA_COUNT", SOList[RowCounter, 12]));
                stbSql.Append(StrSQLText("NG_RATE", SOList[RowCounter, 13]));
                stbSql.Append(StrSQLText("MEMO", SOList[RowCounter, 14]));
                stbSql.Append(StrSQLText("RMA_NO", SOList[RowCounter, 15]));
                stbSql.Append(StrSQLText("ACJ_ISSUE_DOA_NO_TO_SI", SOList[RowCounter, 16]));
                stbSql.Append(StrSQLText("ASUS_CN_ISSUE_DATE", SOList[RowCounter, 17]));
                stbSql.Append(StrSQLText("ADS_REMARK", SOList[RowCounter, 18]));
                stbSql.Append(StrSQLText("N01_NO", SOList[RowCounter, 19]));
                stbSql.Append("RECORD_KBN = '2', "); //---　レコード区分（更新あり）
                stbSql.Append("    UPDATE_DATE = '" + DTImportTime + "', ");
                stbSql.Append("    UPDATE_ID = '" + StrUpdUID + "' ");
                stbSql.Append("WHERE ");
                stbSql.Append("    SO_NO = '" + SOList[RowCounter, 1] + "' ");
                stbSql.Append("AND ( ");
                stbSql.Append("        n90N <> " + StrSQLText("", SOList[RowCounter, 2]) + " ");
                stbSql.Append("    OR  MODEL_NAME <> " + StrSQLText("", SOList[RowCounter, 3]) + " ");
                stbSql.Append("    OR  SHIPPING_QUANTITY <> " + StrSQLText("", SOList[RowCounter, 4]) + " ");
                stbSql.Append("    OR  EST_ARRIVAL_DATE <> " + StrSQLText("", SOList[RowCounter, 5]) + " ");
                stbSql.Append("    OR  PREF_REPORTING_DATE <> " + StrSQLText("", SOList[RowCounter, 6]) + " ");
                stbSql.Append("    OR  SI_TEK_EST_ARRIVAL_DATE <> " + StrSQLText("", SOList[RowCounter, 7]) + " ");
                stbSql.Append("    OR  EXP_RELEASE_DATE <> " + StrSQLText("", SOList[RowCounter, 8]) + " ");
                stbSql.Append("    OR  SCH_RELEASE_DATE <> " + StrSQLText("", SOList[RowCounter, 9]) + " ");
                stbSql.Append("    OR  DELIVERY_LOCATION <> " + StrSQLText("", SOList[RowCounter, 10]) + " ");
                stbSql.Append("    OR  NG_COUNT <> " + StrSQLText("", SOList[RowCounter, 11]) + " ");
                stbSql.Append("    OR  DOA_COUNT <> " + StrSQLText("", SOList[RowCounter, 12]) + " ");
                stbSql.Append("    OR  NG_RATE <> " + StrSQLText("", SOList[RowCounter, 13]) + " ");
                stbSql.Append("    OR  MEMO <> " + StrSQLText("", SOList[RowCounter, 14]) + " ");
                stbSql.Append("    OR  RMA_NO <> " + StrSQLText("", SOList[RowCounter, 15]) + " ");
                stbSql.Append("    OR  ACJ_ISSUE_DOA_NO_TO_SI <> " + StrSQLText("", SOList[RowCounter, 16]) + " ");
                stbSql.Append("    OR  ASUS_CN_ISSUE_DATE <> " + StrSQLText("", SOList[RowCounter, 17]) + " ");
                stbSql.Append("    OR  ADS_REMARK <> " + StrSQLText("", SOList[RowCounter, 18]) + " ");
                stbSql.Append("    OR  N01_NO <> " + StrSQLText("", SOList[RowCounter, 19]) + " ");
                stbSql.Append("    ) ");
                stbSql.Append("IF @@ROWCOUNT = 0 ");
                stbSql.Append("INSERT ");
                stbSql.Append("INTO T_SO_STATUS ");
                stbSql.Append("( ");
                stbSql.Append("    SO_NO, ");
                stbSql.Append("    n90N, ");
                stbSql.Append("    MODEL_NAME, ");
                stbSql.Append("    SHIPPING_QUANTITY, ");
                stbSql.Append("    EST_ARRIVAL_DATE, ");
                stbSql.Append("    PREF_REPORTING_DATE, ");
                stbSql.Append("    SI_TEK_EST_ARRIVAL_DATE, ");
                stbSql.Append("    EXP_RELEASE_DATE, ");
                stbSql.Append("    SCH_RELEASE_DATE, ");
                stbSql.Append("    DELIVERY_LOCATION, ");
                stbSql.Append("    NG_COUNT, ");
                stbSql.Append("    DOA_COUNT, ");
                stbSql.Append("    NG_RATE, ");
                stbSql.Append("    MEMO, ");
                stbSql.Append("    RMA_NO, ");
                stbSql.Append("    ACJ_ISSUE_DOA_NO_TO_SI, ");
                stbSql.Append("    ASUS_CN_ISSUE_DATE, ");
                stbSql.Append("    ADS_REMARK, ");
                stbSql.Append("    N01_NO, ");
                stbSql.Append("    SO_STATUS_ID, ");
                stbSql.Append("    RECORD_KBN, ");
                stbSql.Append("    DEL_FLG, ");
                stbSql.Append("    INSERT_DATE, ");
                stbSql.Append("    INSERT_ID, ");
                stbSql.Append("    UPDATE_DATE, ");
                stbSql.Append("    UPDATE_ID ");
                stbSql.Append(") ");
                stbSql.Append("SELECT ");
                for (int ColCounter = 1; ColCounter < IntColumnCount; ColCounter++)
                {
                    if (!string.IsNullOrEmpty(SOList[RowCounter, ColCounter]))
                    {
                        if (ColCounter != IntColumnCount)
                        {
                            stbSql.Append("    '" + SOList[RowCounter, ColCounter] + "', ");
                        }
                        else
                        {
                            stbSql.Append("    '" + SOList[RowCounter, ColCounter] + "' ");
                        }
                    }
                    else
                    {
                        stbSql.Append("    null, ");
                    }
                }
                stbSql.Append("    '1010', ");
                stbSql.Append("    '1', ");　//---　レコード区分（新規）
                stbSql.Append("    '0', ");
                stbSql.Append("    '" + DTImportTime + "', ");
                stbSql.Append("    '" + StrUpdUID + "', ");
                stbSql.Append("    '" + DTImportTime + "', ");
                stbSql.Append("    '" + StrUpdUID + "' ");
                stbSql.Append("WHERE NOT EXISTS ");
                stbSql.Append("    ( ");
                stbSql.Append("        SELECT ");
                stbSql.Append("            TOP 1 1 ");
                stbSql.Append("        FROM ");
                stbSql.Append("            T_SO_STATUS ");
                stbSql.Append("        WHERE ");
                stbSql.Append("             SO_NO = '" + SOList[RowCounter, 1] + "' ");
                stbSql.Append("    ) ");

                dsnLib.ExecSQLUpdate(stbSql.ToString());

                dsnLib.DB_Close();

                stbSql.Clear();
            }

            stbSql.Append("SELECT ");
            stbSql.Append("    T_SO_STATUS.SO_NO, ");
            stbSql.Append("    T_SO_STATUS.n90N AS NEW_n90N, ");
            stbSql.Append("    T_SO_STATUS.MODEL_NAME AS NEW_MODEL_NAME, ");
            stbSql.Append("    T_SO_STATUS.SHIPPING_QUANTITY AS NEW_SHIPPING_QUANTITY, ");
            stbSql.Append("    T_SO_STATUS.EST_ARRIVAL_DATE AS NEW_EST_ARRIVAL_DATE, ");
            stbSql.Append("    T_SO_STATUS.PREF_REPORTING_DATE AS NEW_PREF_REPORTING_DATE, ");
            stbSql.Append("    T_SO_STATUS.SI_TEK_EST_ARRIVAL_DATE AS NEW_SI_TEK_EST_ARRIVAL_DATE, ");
            stbSql.Append("    T_SO_STATUS.DELIVERY_LOCATION AS NEW_DELIVERY_LOCATION, ");
            stbSql.Append("    T_SO_STATUS.N01_NO AS NEW_N01_NO, ");
            stbSql.Append("    WK_T_SO_STATUS.n90N AS OLD_n90N, ");
            stbSql.Append("    WK_T_SO_STATUS.MODEL_NAME AS OLD_MODEL_NAME, ");
            stbSql.Append("    WK_T_SO_STATUS.SHIPPING_QUANTITY AS OLD_SHIPPING_QUANTITY, ");
            stbSql.Append("    WK_T_SO_STATUS.EST_ARRIVAL_DATE AS OLD_EST_ARRIVAL_DATE, ");
            stbSql.Append("    WK_T_SO_STATUS.PREF_REPORTING_DATE AS OLD_PREF_REPORTING_DATE, ");
            stbSql.Append("    WK_T_SO_STATUS.SI_TEK_EST_ARRIVAL_DATE AS OLD_SI_TEK_EST_ARRIVAL_DATE, ");
            stbSql.Append("    WK_T_SO_STATUS.DELIVERY_LOCATION AS OLD_DELIVERY_LOCATION, ");
            stbSql.Append("    WK_T_SO_STATUS.N01_NO AS OLD_N01_NO ");
            stbSql.Append("FROM ");
            stbSql.Append("    T_SO_STATUS ");
            stbSql.Append("    	INNER JOIN WK_T_SO_STATUS  ");
            stbSql.Append("    	    ON T_SO_STATUS.SO_NO = WK_T_SO_STATUS.SO_NO  ");
            stbSql.Append("WHERE ");
            stbSql.Append("    RECORD_KBN = '2' ");

            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

            DataSet DSDataSet = new DataSet();
            DataTable DTDataTable = new DataTable();

            // カラム名の追加
            DTDataTable.Columns.Add("SO_NO");
            DTDataTable.Columns.Add("CHG_n90N_FLG");
            DTDataTable.Columns.Add("CHG_MODEL_NAME_FLG");
            DTDataTable.Columns.Add("CHG_SHIPPING_QUANTITY_FLG");
            DTDataTable.Columns.Add("CHG_EST_ARRIVAL_DATE_FLG");
            DTDataTable.Columns.Add("CHG_PREF_REPORTING_DATE_FLG");
            DTDataTable.Columns.Add("CHG_SI_TEK_EST_ARRIVAL_DATE_FLG");
            DTDataTable.Columns.Add("CHG_DELIVERY_LOCATION_FLG");
            DTDataTable.Columns.Add("CHG_N01_NO_FLG");

            while (sqlRdr.Read())
            {
                // DataRowクラスを使ってデータを追加
                DataRow DRDataRow = DTDataTable.NewRow();

                DRDataRow["SONO"] = sqlRdr["NEW_n90N"].ToString();

                if (sqlRdr["NEW_n90N"].ToString() != sqlRdr["OLD_n90N"].ToString())
                {
                    DRDataRow["CHG_n90N_FLG"] = "1";
                }
                else
                {
                    DRDataRow["CHG_n90N_FLG"] = "0";
                }

                if (sqlRdr["NEW_MODEL_NAME"].ToString() != sqlRdr["OLD_MODEL_NAME"].ToString())
                {
                    DRDataRow["CHG_MODEL_NAME_FLG"] = "1";
                }
                else
                {
                    DRDataRow["CHG_MODEL_NAME_FLG"] = "0";
                }

                if (sqlRdr["NEW_SHIPPING_QUANTITY"].ToString() != sqlRdr["OLD_SHIPPING_QUANTITY"].ToString())
                {
                    DRDataRow["CHG_SHIPPING_QUANTITY_FLG"] = "1";
                }
                else
                {
                    DRDataRow["CHG_SHIPPING_QUANTITY_FLG"] = "0";
                }

                if (sqlRdr["NEW_EST_ARRIVAL_DATE"].ToString() != sqlRdr["OLD_EST_ARRIVAL_DATE"].ToString())
                {
                    DRDataRow["CHG_EST_ARRIVAL_DATE_FLG"] = "1";
                }
                else
                {
                    DRDataRow["CHG_EST_ARRIVAL_DATE_FLG"] = "0";
                }

                if (sqlRdr["NEW_PREF_REPORTING_DATE"].ToString() != sqlRdr["OLD_PREF_REPORTING_DATE"].ToString())
                {
                    DRDataRow["CHG_PREF_REPORTING_DATE_FLG"] = "1";
                }
                else
                {
                    DRDataRow["CHG_PREF_REPORTING_DATE_FLG"] = "0";
                }

                if (sqlRdr["NEW_SI_TEK_EST_ARRIVAL_DATE"].ToString() != sqlRdr["OLD_SI_TEK_EST_ARRIVAL_DATE"].ToString())
                {
                    DRDataRow["CHG_SI_TEK_EST_ARRIVAL_DATE_FLG"] = "1";
                }
                else
                {
                    DRDataRow["CHG_SI_TEK_EST_ARRIVAL_DATE_FLG"] = "0";
                }

                if (sqlRdr["NEW_DELIVERY_LOCATION"].ToString() != sqlRdr["OLD_DELIVERY_LOCATION"].ToString())
                {
                    DRDataRow["CHG_DELIVERY_LOCATION_FLG"] = "1";
                }
                else
                {
                    DRDataRow["CHG_DELIVERY_LOCATION_FLG"] = "0";
                }

                if (sqlRdr["NEW_N01_NO"].ToString() != sqlRdr["OLD_N01_NO"].ToString())
                {
                    DRDataRow["CHG_N01_NO_FLG"] = "1";
                }
                else
                {
                    DRDataRow["CHG_N01_NO_FLG"] = "0";
                }

                DTDataTable.Rows.Add(DRDataRow);
            }



        }

        //------------------------------------------------------
        // ExcelデータをT_SO_LIST_HISTORYに保存する。
        //------------------------------------------------------
        public void InsertSOStatusHistory(DateTime DTImportTime, string StrUpdUID)
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            // Excelデータの３行目から順次データをDBに書き込む。
            // (１，２行目はタイトル行のため、読み込まない）
            int IntRowCount = SOList.GetLength(0);
            int IntColumnCount = SOList.GetLength(1);
            //------------------------------------------------------
            // ExcelデータをT_SO_STATUS_HYSTORYに保存する。
            // 存在しなければ追加。
            //------------------------------------------------------
            for (int RowCounter = 2; RowCounter < IntRowCount; RowCounter++)
            {

                stbSql.Append("INSERT ");
                stbSql.Append("INTO T_SO_STATUS_HYSTORY ");
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
                stbSql.Append("    '" + SOList[RowCounter, 1] + "', ");
                stbSql.Append("    1, ");
                stbSql.Append("    '0', ");
                stbSql.Append("    '1010', ");
                stbSql.Append("    '" + DTImportTime + "', ");
                stbSql.Append("    '" + StrUpdUID + "', ");
                stbSql.Append("    '" + DTImportTime + "', ");
                stbSql.Append("    '" + StrUpdUID + "' ");
                stbSql.Append("WHERE NOT EXISTS ");
                stbSql.Append("    ( ");
                stbSql.Append("        SELECT ");
                stbSql.Append("            TOP 1 1 ");
                stbSql.Append("        FROM ");
                stbSql.Append("            T_SO_STATUS_HYSTORY ");
                stbSql.Append("        WHERE ");
                stbSql.Append("             SO_NO = '" + SOList[RowCounter, 1] + "' ");
                stbSql.Append("    ) ");

                dsnLib.ExecSQLUpdate(stbSql.ToString());

                dsnLib.DB_Close();

                stbSql.Clear();
            }
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

    public class SrchRstOrderReport
    {
        public int RecNum { get; set; }
        public string SerialNumber { get; set; }
        public string Digit15 { get; set; }
        public string Digit8 { get; set; }
        public string Digit9 { get; set; }
        public string Digit11 { get; set; }
        public string Digit12 { get; set; }
        public string SerialDigitVarification { get; set; }
        public string NGFLG { get; set; }
        public string NGReason { get; set; }
        public DateTime? WorkDay { get; set; }
        public string Instruction { get; set; }
        public string RmaType { get; set; }
        public string DeliveryLocation { get; set; }
        public string Other { get; set; }
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
}
