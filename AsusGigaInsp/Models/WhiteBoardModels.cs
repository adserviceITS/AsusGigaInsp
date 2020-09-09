using AsusGigaInsp.Modules;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System;
using System.Linq;
using DataTable = System.Data.DataTable;
using ClosedXML.Excel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace AsusGigaInsp.Models
{

    //---------------------------------------------------------------------------//
    //                      　　 ホワイトボード出力処理                          //
    //---------------------------------------------------------------------------//
    public class WhiteBoardModels
    {
        public int IntRstInput { get; set; }
        public int IntRstComplete { get; set; }

        public int IntRstAccumulationBeforeArrival { get; set; }
        public int IntRstAccumulationBeforeStart { get; set; }
        public int IntRstAccumulationWorking { get; set; }
        public int IntRstAccumulationComplete { get; set; }
        public int IntRstAccumulationReadyToShip { get; set; }
        public int IntRstAccumulationTotal { get; set; }

        public void SetSrchRstWhiteBoard()
        {
            IntRstInput = 0;
            IntRstComplete = 0;
            IntRstAccumulationBeforeArrival = 0;
            IntRstAccumulationBeforeStart = 0;
            IntRstAccumulationWorking = 0;
            IntRstAccumulationComplete = 0;
            IntRstAccumulationReadyToShip = 0;
            IntRstAccumulationTotal = 0;

            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder strSql = new StringBuilder();

            //------------------------------------------------------
            // 本日の作業実績（完了）を取得する。
            //------------------------------------------------------
            strSql.Append("SELECT ");
            strSql.Append("    STATUS, ");
            strSql.Append("    COUNT(STATUS) AS COUNT ");
            strSql.Append("FROM ");
            strSql.Append("    ( ");
            strSql.Append("        SELECT ");
            strSql.Append("            STATUS ");
            strSql.Append("        FROM ");
            strSql.Append("            T_SERIAL_STATUS_HISTORY ");
            strSql.Append("        WHERE ");
            strSql.Append("            UPDATE_DATE >= '" + DateTime.Today + "' ");
            strSql.Append("            AND (STATUS = '3010' OR STATUS = '4010') ");
            strSql.Append("        GROUP BY ");
            strSql.Append("            SERIAL_NUMBER, ");
            strSql.Append("            STATUS ");
            strSql.Append("    ) TBL ");
            strSql.Append("GROUP BY ");
            strSql.Append("    STATUS ");

            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(strSql.ToString());

            // ステータスごとの件数を各項目にセット
            while (sqlRdr.Read())
            {
                if (sqlRdr["STATUS"].ToString() == "3010")
                {
                    IntRstInput = int.Parse(sqlRdr["COUNT"].ToString());

                }
                else
                {
                    IntRstComplete = int.Parse(sqlRdr["COUNT"].ToString());
                }
            }

            dsnLib.DB_Close();
            strSql.Clear();

            //------------------------------------------------------
            // 累積の作業実績（仕掛り）を取得する。
            //------------------------------------------------------
            strSql.Append("SELECT ");
            strSql.Append("    SERIAL_STATUS_ID, ");
            strSql.Append("    COUNT(SERIAL_STATUS_ID) AS COUNT ");
            strSql.Append("FROM ");
            strSql.Append("    T_SERIAL_STATUS ");
            strSql.Append("WHERE ");
            strSql.Append("    DEL_FLG = '0' ");
            strSql.Append("GROUP BY ");
            strSql.Append("    SERIAL_STATUS_ID ");

            sqlRdr = dsnLib.ExecSQLRead(strSql.ToString());

            while (sqlRdr.Read())
            {
                switch (sqlRdr["SERIAL_STATUS_ID"].ToString())
                {
                    case "1010":
                        IntRstAccumulationBeforeArrival = int.Parse(sqlRdr["COUNT"].ToString());
                        break;
                    case "2010":
                        IntRstAccumulationBeforeStart = int.Parse(sqlRdr["COUNT"].ToString());
                        break;
                    case "3010":
                        IntRstAccumulationWorking = int.Parse(sqlRdr["COUNT"].ToString());
                        break;
                    case "4010":
                        IntRstAccumulationComplete = int.Parse(sqlRdr["COUNT"].ToString());
                        break;
                    case "6010":
                        IntRstAccumulationReadyToShip = int.Parse(sqlRdr["COUNT"].ToString());
                        break;
                    default:
                        break;
                }
            }
            dsnLib.DB_Close();
            strSql.Clear();

            // 拠点内総仕掛の算出
            IntRstAccumulationTotal = IntRstAccumulationBeforeStart + IntRstAccumulationWorking + IntRstAccumulationComplete + IntRstAccumulationReadyToShip;

        }
    }

    //---------------------------------------------------------------------------//
    //                             作業計画取込処理                              //
    //---------------------------------------------------------------------------//
    // 取込ファイル取得用クラス
    public class PlanUploadFileModels
    {
        [Required(ErrorMessage = "ファイルを選択してください。")]
        public HttpPostedFileBase PlanDataExcelFile { get; set; }
    }

    // 取込処理
    public class PlanDataUpLoadModels
    {
        public void UpLoadPlanData(string StrUpdUID, PlanUploadFileModels UFUploadFile)
        {
            //　Excel取得用変数
            string[,] PlanData = new string[1, 1];
            int IntRowCount = 0;
            int IntColumnCount = 0;

            //------------------------------------------------------
            // Excel取込処理
            //------------------------------------------------------

            using (var WorkBook = new XLWorkbook(UFUploadFile.PlanDataExcelFile.InputStream))
            {
                var WorkSheet = WorkBook.Worksheet("取り込み用");

                // テーブル作成
                var Table = WorkSheet.RangeUsed().AsTable();

                //　テーブルの行数、列数を取得し、データ格納配列を定義する。
                IntRowCount = Table.RowCount();
                IntColumnCount = Table.ColumnCount();
                PlanData = new string[IntRowCount, IntColumnCount];

                // テーブルのデータをセル毎に取得
                for (int RowCounter = 0; RowCounter < IntRowCount; RowCounter++)
                {
                    for (int ColCounter = 0; ColCounter < IntColumnCount; ColCounter++)
                    {
                        PlanData[RowCounter, ColCounter] = Table.Row(RowCounter + 1).Cell(ColCounter + 1).Value.ToString();
                    }
                }
            }

            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder strSql = new StringBuilder();

            DateTime DTImportTime = DateTime.Now;

            //------------------------------------------------------
            // 既存のT_PLANSのレコードを削除する
            //------------------------------------------------------
            strSql.Append("DELETE ");
            strSql.Append("FROM ");
            strSql.Append("    T_PLANS ");

            dsnLib.ExecSQLUpdate(strSql.ToString());
            dsnLib.DB_Close();
            strSql.Clear();

            //------------------------------------------------------
            // ExcelデータをT_PLANSに保存する。
            //------------------------------------------------------
            // Excelデータの2行目から順次データをDBに書き込む。
            // (1行目はタイトル行のため、読み込まない、最終行も合計のため読み込まない。）
            for (int RowCounter = 1; RowCounter < IntRowCount - 1; RowCounter++)
            {
                strSql.Append("INSERT ");
                strSql.Append("INTO T_PLANS ");
                strSql.Append("( ");
                strSql.Append("    ID, ");
                strSql.Append("    LINE_ID, ");
                strSql.Append("    PERIOD_1, ");
                strSql.Append("    PERIOD_2, ");
                strSql.Append("    PERIOD_3, ");
                strSql.Append("    PERIOD_4, ");
                strSql.Append("    PERIOD_5, ");
                strSql.Append("    PERIOD_6, ");
                strSql.Append("    PERIOD_7, ");
                strSql.Append("    PERIOD_8, ");
                strSql.Append("    PERIOD_9, ");
                strSql.Append("    PERIOD_10, ");
                strSql.Append("    PERIOD_11, ");
                strSql.Append("    PERIOD_12, ");
                strSql.Append("    PERIOD_13, ");
                strSql.Append("    INSERT_DATE, ");
                strSql.Append("    INSERT_ID ");
                strSql.Append(") ");
                strSql.Append("VALUES ");
                strSql.Append("( ");
                strSql.Append("    '" + RowCounter + "', ");
                strSql.Append("    '" + PlanData[RowCounter, 0].Substring(0, 1) + "', ");
                for (int ColCounter = 1; ColCounter < IntColumnCount; ColCounter++)
                {
                    if (!string.IsNullOrEmpty(PlanData[RowCounter, ColCounter]))
                    {
                        if (ColCounter != IntColumnCount)
                        {
                            strSql.Append("    " + Math.Ceiling(decimal.Parse(PlanData[RowCounter, ColCounter])) + ", ");
                        }
                        else
                        {
                            strSql.Append("    " + Math.Ceiling(decimal.Parse(PlanData[RowCounter, ColCounter])) + " ");
                        }
                    }
                    else
                    {
                        strSql.Append("    null, ");
                    }
                }
                strSql.Append("    '" + DTImportTime + "', ");
                strSql.Append("    '" + StrUpdUID + "' ");
                strSql.Append(") ");

                dsnLib.ExecSQLUpdate(strSql.ToString());

                dsnLib.DB_Close();

                strSql.Clear();
            }
        }
    }

    //---------------------------------------------------------------------------//
    //                            進捗ボード表示処理　                           //
    //---------------------------------------------------------------------------//
    public class ProgressBoardModels
    {
        public IEnumerable<SrchRst> SrchRstProgressBoard { get; set; }
        public IEnumerable<SrchTime> SrchRstDisplayTime { get; set; }
        public IEnumerable<string> SrchRstLine { get; set; }

        public void SetSrchRstProgressBoard()
        {
            int IntPeriodCount = 14;

            DateTime DT = DateTime.Today;
            string StrDT = DT.ToString("yyyy/MM/dd");

            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder strSql = new StringBuilder();

            //------------------------------------------------------
            // タイムテーブルを取り込む。
            //------------------------------------------------------
            strSql.Append("SELECT ");
            strSql.Append("    * ");
            strSql.Append("FROM ");
            strSql.Append("    M_TIME_TABLE ");

            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(strSql.ToString());
            sqlRdr = dsnLib.ExecSQLRead(strSql.ToString());

            List<SrchRstTimeTable> lstSrchRstTimeTable = new List<SrchRstTimeTable>();
            string[] StrTimeTableReader = new string[IntPeriodCount];
            int IntCounter = 1;

            StrTimeTableReader[0] = '0'.ToString();
            while (sqlRdr.Read())
            {
                lstSrchRstTimeTable.Add(new SrchRstTimeTable
                {
                    StartTime = sqlRdr["START_TIME"].ToString(),
                    EndTime = sqlRdr["END_TIME"].ToString()
                });

                StrTimeTableReader[IntCounter] = "～" + sqlRdr["END_TIME"].ToString();
                IntCounter++;
            }
            dsnLib.DB_Close();
            strSql.Clear();

            // System.Diagnostics.Debug.WriteLine(StrTimeTable[0]);

            List<SrchTime> LstDisplayTime = new List<SrchTime>();
            LstDisplayTime.Add(new SrchTime
            {
                Period01 = StrTimeTableReader[1],
                Period02 = StrTimeTableReader[2],
                Period03 = StrTimeTableReader[3],
                Period04 = StrTimeTableReader[4],
                Period05 = StrTimeTableReader[5],
                Period06 = StrTimeTableReader[6],
                Period07 = StrTimeTableReader[7],
                Period08 = StrTimeTableReader[8],
                Period09 = StrTimeTableReader[9],
                Period10 = StrTimeTableReader[10],
                Period11 = StrTimeTableReader[11],
                Period12 = StrTimeTableReader[12],
                Period13 = StrTimeTableReader[13]
            });

            SrchRstDisplayTime = LstDisplayTime;

            //------------------------------------------------------
            // ライン情報を取り込む。
            //------------------------------------------------------
            string[,] StrPerformanceReader = new string[1, IntPeriodCount];

            strSql.Append("SELECT ");
            strSql.Append("    LINE_ID ");
            strSql.Append("FROM ");
            strSql.Append("    M_LINE ");

            sqlRdr = dsnLib.ExecSQLRead(strSql.ToString());

            List<string> lstSrchRstLine = new List<string>();

            IntCounter = 0;

            while (sqlRdr.Read())
            {
                lstSrchRstLine.Add(sqlRdr["LINE_ID"].ToString());
            }
            dsnLib.DB_Close();
            strSql.Clear();

            SrchRstLine = lstSrchRstLine;

            // ライン数を取得
            int IntLineCount = (lstSrchRstLine.Count());

            // 配列の行数を算出（LINE数×2）
            int IntAllRowCount = lstSrchRstLine.Count() * 2;

            StrPerformanceReader = new string[IntAllRowCount, IntPeriodCount];

            // 取得したリストを配列に格納
            foreach (string StrLineID in lstSrchRstLine)
            {
                StrPerformanceReader[IntCounter, 0] = StrLineID + "2";
                IntCounter++;
            }

            //------------------------------------------------------
            // 配列の数値を現在時刻まで0で埋める。
            //------------------------------------------------------
            strSql.Append("SELECT ");
            strSql.Append("    ID ");
            strSql.Append("FROM ");
            strSql.Append("    M_TIME_TABLE ");
            strSql.Append("WHERE ");
            strSql.Append("    START_TIME <= CONVERT(TIME, getdate()) ");
            strSql.Append("    AND CONVERT(TIME, getdate()) < END_TIME ");

            sqlRdr = dsnLib.ExecSQLRead(strSql.ToString());
            sqlRdr.Read();

            int IntTimeID = int.Parse(sqlRdr["ID"].ToString()) + 1;

            dsnLib.DB_Close();
            strSql.Clear();

            for (int IntX = 1; IntX < IntTimeID; IntX++)
            {
                for (int IntY = 0; IntY < IntCounter; IntY++)
                {
                    {
                        StrPerformanceReader[IntY, IntX] = '0'.ToString();
                    }
                }
            }

            //------------------------------------------------------
            // 作業計画を取り込む。
            //------------------------------------------------------
            strSql.Append("SELECT ");
            strSql.Append("    M_LINE.LINE_ID, ");
            strSql.Append("    IsNull(T_PLANS.PERIOD_1, 0) AS PERIOD_1, ");
            strSql.Append("    IsNull(T_PLANS.PERIOD_2, 0) AS PERIOD_2, ");
            strSql.Append("    IsNull(T_PLANS.PERIOD_3, 0) AS PERIOD_3, ");
            strSql.Append("    IsNull(T_PLANS.PERIOD_4, 0) AS PERIOD_4, ");
            strSql.Append("    IsNull(T_PLANS.PERIOD_5, 0) AS PERIOD_5, ");
            strSql.Append("    IsNull(T_PLANS.PERIOD_6, 0) AS PERIOD_6, ");
            strSql.Append("    IsNull(T_PLANS.PERIOD_7, 0) AS PERIOD_7, ");
            strSql.Append("    IsNull(T_PLANS.PERIOD_8, 0) AS PERIOD_8, ");
            strSql.Append("    IsNull(T_PLANS.PERIOD_9, 0) AS PERIOD_9, ");
            strSql.Append("    IsNull(T_PLANS.PERIOD_10, 0) AS PERIOD_10, ");
            strSql.Append("    IsNull(T_PLANS.PERIOD_11, 0) AS PERIOD_11, ");
            strSql.Append("    IsNull(T_PLANS.PERIOD_12, 0) AS PERIOD_12, ");
            strSql.Append("    IsNull(T_PLANS.PERIOD_13, 0) AS PERIOD_13 ");
            strSql.Append("FROM ");
            strSql.Append("    M_LINE ");
            strSql.Append("    LEFT JOIN T_PLANS ");
            strSql.Append("        ON M_LINE.LINE_ID = T_PLANS.LINE_ID ");

            int IntStartRow = IntCounter;
            sqlRdr = dsnLib.ExecSQLRead(strSql.ToString());

            while (sqlRdr.Read())
            {
                StrPerformanceReader[IntCounter, 0] = sqlRdr["LINE_ID"].ToString() + "1";

                for (int IntX = 1; IntX < IntPeriodCount; IntX++)
                {
                    {
                        StrPerformanceReader[IntCounter, IntX] = sqlRdr["PERIOD_" + IntX].ToString();
                    }
                }
                IntCounter++;
            }

            dsnLib.DB_Close();
            strSql.Clear();

            //------------------------------------------------------
            // 作業実績を取り込む。
            //------------------------------------------------------
            int IntTimeIndex = 1;
            foreach (SrchRstTimeTable SRTT in lstSrchRstTimeTable)
            {
                strSql.Append("SELECT ");
                strSql.Append("    M_LINE.LINE_ID + '2' AS LINE_ID, ");
                strSql.Append("    " + IntTimeIndex + " AS TIME_ID, ");
                strSql.Append("    IsNull(TBL1.COUNT, 0) AS COUNT ");
                strSql.Append("FROM ");
                strSql.Append("    M_LINE ");
                strSql.Append("    LEFT JOIN (");
                strSql.Append("        SELECT ");
                strSql.Append("            LINE_ID, ");
                strSql.Append("            COUNT(*) AS COUNT ");
                strSql.Append("        FROM ");
                strSql.Append("            T_SERIAL_STATUS_HISTORY ");
                strSql.Append("        WHERE ");
                strSql.Append("            UPDATE_DATE >= '" + StrDT + " " + SRTT.StartTime + "'");
                strSql.Append("            AND UPDATE_DATE < '" + StrDT + " " + SRTT.EndTime + "' ");
                strSql.Append("            AND STATUS = '4010' ");
                strSql.Append("        GROUP BY ");
                strSql.Append("            LINE_ID ");
                strSql.Append("    ) TBL1 ");
                strSql.Append("        ON M_LINE.LINE_ID = TBL1.LINE_ID ");

                sqlRdr = dsnLib.ExecSQLRead(strSql.ToString());

                while (sqlRdr.Read())
                {
                    if (IntTimeID > int.Parse(sqlRdr["TIME_ID"].ToString()))
                    {
                        int IntX = int.Parse(sqlRdr["TIME_ID"].ToString());

                        for (int IntY = 0; IntY < IntCounter; IntY++)
                        {
                            if (StrPerformanceReader[IntY, 0] == sqlRdr["LINE_ID"].ToString())
                            {
                                if (IntX > 1)
                                {
                                    StrPerformanceReader[IntY, IntX] = (int.Parse(StrPerformanceReader[IntY, IntX - 1]) + int.Parse(sqlRdr["COUNT"].ToString())).ToString();
                                }
                                else
                                {
                                    StrPerformanceReader[IntY, IntX] = int.Parse(sqlRdr["COUNT"].ToString()).ToString();
                                }
                            }
                        }
                    }
                }

                dsnLib.DB_Close();

                strSql.Clear();
                IntTimeIndex++;
            }

            // 配列をデータテーブルに代入
            DataTable DTPerformanceReader = new DataTable();

            for (int col = 0; col < StrPerformanceReader.GetLength(1); col++)
            {
                DTPerformanceReader.Columns.Add("Col" + col.ToString());
            }

            for (int rowindex = 0; rowindex < StrPerformanceReader.GetLength(0); rowindex++)
            {
                DataRow row = DTPerformanceReader.NewRow();
                for (int col = 0; col < StrPerformanceReader.GetLength(1); col++)
                {
                    row[col] = StrPerformanceReader[rowindex, col];
                }
                DTPerformanceReader.Rows.Add(row);
            }

            DataRow[] DrPerformanceRows = null;
            DataRow[] DrPerformanceRows1 = null;
            DataRow[] DrPerformanceRows2 = null;
            DataRow DrDataRow = null;

            // 差異計算
            foreach (string StrLineID in lstSrchRstLine)
            {
                DataRow[] DrPlanRows = DTPerformanceReader.Select("Col0 LIKE '" + StrLineID + "1'", "");
                DrPerformanceRows = DTPerformanceReader.Select("Col0 LIKE '" + StrLineID + "2'", "");

                DrDataRow = DTPerformanceReader.NewRow();
                DrDataRow[0] = StrLineID + "3";

                for (int IntColumnCounter = 1; IntColumnCounter < IntTimeID; IntColumnCounter++)
                {
                    DrDataRow[IntColumnCounter] = (int.Parse(DrPerformanceRows[0][IntColumnCounter].ToString()) - int.Parse(DrPlanRows[0][IntColumnCounter].ToString())).ToString();
                }

                DTPerformanceReader.Rows.Add(DrDataRow);
            }

            // 合計欄の作成（計画）
            DrPerformanceRows = DTPerformanceReader.Select("Col0 LIKE '%1'", "");
            DrDataRow = DTPerformanceReader.NewRow();
            DrDataRow[0] = "01";

            for (int IntColumnCounter = 1; IntColumnCounter < IntPeriodCount; IntColumnCounter++)
            {
                int IntTotal = 0;
                for (int IntRowCounter = 0; IntRowCounter < DrPerformanceRows.Count(); IntRowCounter++)
                {
                    IntTotal = IntTotal + int.Parse(DrPerformanceRows[IntRowCounter][IntColumnCounter].ToString());
                }

                DrDataRow[IntColumnCounter] = IntTotal.ToString();
            }
            DTPerformanceReader.Rows.Add(DrDataRow);

            // 合計欄の作成（累計実績）
            DrPerformanceRows = DTPerformanceReader.Select("Col0 LIKE '%2'", "");
            DrDataRow = DTPerformanceReader.NewRow();
            DrDataRow[0] = "02";

            for (int IntColumnCounter = 1; IntColumnCounter < IntTimeID; IntColumnCounter++)
            {
                int IntTotalCum = 0;

                for (int IntRowCounter = 0; IntRowCounter < DrPerformanceRows.Count(); IntRowCounter++)
                {
                    IntTotalCum = IntTotalCum + int.Parse(DrPerformanceRows[IntRowCounter][IntColumnCounter].ToString());
                }

                DrDataRow[IntColumnCounter] = IntTotalCum.ToString();
            }
            DTPerformanceReader.Rows.Add(DrDataRow);

            // 合計欄の作成（累計差異）
            DrPerformanceRows1 = DTPerformanceReader.Select("Col0 LIKE '%1'", "");
            DrPerformanceRows2 = DTPerformanceReader.Select("Col0 LIKE '%2'", "");
            DrDataRow = DTPerformanceReader.NewRow();
            DrDataRow[0] = "03";

            for (int IntColumnCounter = 1; IntColumnCounter < IntTimeID; IntColumnCounter++)
            {
                int IntTotal = 0;
                {
                    IntTotal = int.Parse(DrPerformanceRows2[0][IntColumnCounter].ToString()) - int.Parse(DrPerformanceRows1[0][IntColumnCounter].ToString());
                }

                DrDataRow[IntColumnCounter] = IntTotal.ToString();
            }
            DTPerformanceReader.Rows.Add(DrDataRow);

            // データテーブルをソート
            DataRow[] sortedrows = DTPerformanceReader.Select("", "Col0");

            // データテーブルをリストに追加
            List<SrchRst> LstProgressBoard = new List<SrchRst>();

            string StrID = null;
            string StrLineName = null;
            string StrCheckLine = null;
            string StrCheckChar = null;

            for (int IntY = 0; IntY < DTPerformanceReader.Rows.Count; IntY++)
            {
                StrCheckLine = sortedrows[IntY].Field<string>("Col0").Substring(0, 1);

                if (StrCheckLine == '0'.ToString())
                {
                    StrLineName = "合計";
                }
                else
                {
                    StrLineName = StrCheckLine + "-Line";
                }

                StrCheckChar = sortedrows[IntY].Field<string>("Col0").Substring(1);

                if (StrCheckChar == '1'.ToString())
                {
                    StrID = "計画";
                }
                else if (StrCheckChar == '2'.ToString())
                {
                    StrID = "実績";
                }
                else
                {
                    StrID = "差異";
                }


                LstProgressBoard.Add(new SrchRst
                {
                    LineName = StrLineName,
                    Division = StrID,
                    Period01 = sortedrows[IntY].Field<string>("Col1"),
                    Period02 = sortedrows[IntY].Field<string>("Col2"),
                    Period03 = sortedrows[IntY].Field<string>("Col3"),
                    Period04 = sortedrows[IntY].Field<string>("Col4"),
                    Period05 = sortedrows[IntY].Field<string>("Col5"),
                    Period06 = sortedrows[IntY].Field<string>("Col6"),
                    Period07 = sortedrows[IntY].Field<string>("Col7"),
                    Period08 = sortedrows[IntY].Field<string>("Col8"),
                    Period09 = sortedrows[IntY].Field<string>("Col9"),
                    Period10 = sortedrows[IntY].Field<string>("Col10"),
                    Period11 = sortedrows[IntY].Field<string>("Col11"),
                    Period12 = sortedrows[IntY].Field<string>("Col12"),
                    Period13 = sortedrows[IntY].Field<string>("Col13")
                });
                var a = sortedrows[IntY].Field<string>("Col0");
            }

            SrchRstProgressBoard = LstProgressBoard;

            //System.Diagnostics.Debug.WriteLine(a);
        }

    }

    public class SrchRst
    {
        public string LineName { get; set; }
        public string Division { get; set; }
        public string Period01 { get; set; }
        public string Period02 { get; set; }
        public string Period03 { get; set; }
        public string Period04 { get; set; }
        public string Period05 { get; set; }
        public string Period06 { get; set; }
        public string Period07 { get; set; }
        public string Period08 { get; set; }
        public string Period09 { get; set; }
        public string Period10 { get; set; }
        public string Period11 { get; set; }
        public string Period12 { get; set; }
        public string Period13 { get; set; }
    }

    public class SrchTime
    {
        public string Period01 { get; set; }
        public string Period02 { get; set; }
        public string Period03 { get; set; }
        public string Period04 { get; set; }
        public string Period05 { get; set; }
        public string Period06 { get; set; }
        public string Period07 { get; set; }
        public string Period08 { get; set; }
        public string Period09 { get; set; }
        public string Period10 { get; set; }
        public string Period11 { get; set; }
        public string Period12 { get; set; }
        public string Period13 { get; set; }
    }

    public class SrchLine
    {
        public string Period01 { get; set; }
    }

    public class SrchRstTimeTable
    {
        public string StartTime;
        public string EndTime;
    }

}
