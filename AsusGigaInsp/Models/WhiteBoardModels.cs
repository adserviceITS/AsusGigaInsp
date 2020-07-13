using AsusGigaInsp.Modules;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System;
using System.Linq;
using DataTable = System.Data.DataTable;

namespace AsusGigaInsp.Models
{
    public class WhiteBoardModels
    {
    }
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
                strSql.Append("    LINE_ID + '2' AS LINE_ID, ");
                strSql.Append("    " + IntTimeIndex + " AS TIME_ID, ");
                strSql.Append("    COUNT(*) AS COUNT ");
                strSql.Append("FROM ");
                strSql.Append("    T_SERIAL_STATUS_HYSTORY ");
                strSql.Append("WHERE ");
                strSql.Append("    UPDATE_DATE >= '" + StrDT + "' ");
                strSql.Append("    AND UPDATE_DATE < '" + StrDT + " " + SRTT.EndTime + "'");
                //strSql.Append("    UPDATE_DATE >= '2020/06/26' ");
                //strSql.Append("    AND UPDATE_DATE < '2020/06/26 17:00:00'");
                strSql.Append("    AND STATUS = '4010' ");
                strSql.Append("GROUP BY ");
                strSql.Append("    LINE_ID ");

                sqlRdr = dsnLib.ExecSQLRead(strSql.ToString());

                while (sqlRdr.Read())
                {
                    int IntX = int.Parse(sqlRdr["TIME_ID"].ToString());

                    for (int IntY = 0; IntY < IntCounter; IntY++)
                    {
                        if (StrPerformanceReader[IntY, 0] == sqlRdr["LINE_ID"].ToString())
                        {
                            StrPerformanceReader[IntY, IntX] = int.Parse(sqlRdr["COUNT"].ToString()).ToString();
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

            // 合計欄の作成（実績）
            DrPerformanceRows = DTPerformanceReader.Select("Col0 LIKE '%2'", "");
            DrDataRow = DTPerformanceReader.NewRow();
            DrDataRow[0] = "02";

            for (int IntColumnCounter = 1; IntColumnCounter < IntTimeID; IntColumnCounter++)
            {
                int IntTotal = 0;
                for (int IntRowCounter = 0; IntRowCounter < DrPerformanceRows.Count(); IntRowCounter++)
                {
                    IntTotal = IntTotal + int.Parse(DrPerformanceRows[IntRowCounter][IntColumnCounter].ToString());
                }

                DrDataRow[IntColumnCounter] = IntTotal.ToString();
            }
            DTPerformanceReader.Rows.Add(DrDataRow);

            // 合計欄の作成（差異）
            DrPerformanceRows = DTPerformanceReader.Select("Col0 LIKE '%3'", "");
            DrDataRow = DTPerformanceReader.NewRow();
            DrDataRow[0] = "03";

            for (int IntColumnCounter = 1; IntColumnCounter < IntTimeID; IntColumnCounter++)
            {
                int IntTotal = 0;
                for (int IntRowCounter = 0; IntRowCounter < DrPerformanceRows.Count(); IntRowCounter++)
                {
                    IntTotal = IntTotal + int.Parse(DrPerformanceRows[IntRowCounter][IntColumnCounter].ToString());
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
