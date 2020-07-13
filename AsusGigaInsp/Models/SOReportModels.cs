using ClosedXML.Excel;
using AsusGigaInsp.Modules;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System;
using System.IO;

namespace AsusGigaInsp.Models
{
    public class SOReportModels
    {
        public void OutPutReport(string strSONO)
        {
            int RecCount = 0;
            int InspectionQuantity = 0;
            int ShipmentQuantity = 0;
            int DOAQuantity = 0;
            int Counter = 0;

            // ファイル名作成用変数
            string SOListSONO = "";
            string SOListModelName = "";
            string SOListDeliveryLocation = "";
            int SOListShippingQuantity = 0;
            string SOListN01NO = "";
            string OutputFileName = "";

            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            //----------------------------------------------------------------------------
            // ファイル名作成のためオーダー情報を取得する。
            //----------------------------------------------------------------------------
            stbSql.Append("SELECT ");
            stbSql.Append("    * ");
            stbSql.Append("FROM ");
            stbSql.Append("    T_SO_STATUS ");
            stbSql.Append("WHERE ");
            stbSql.Append("    T_SO_STATUS.SO_NO = '" + strSONO + "' ");
            stbSql.Append("    AND T_SO_STATUS.DEL_FLG = '0' ");

            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

            while (sqlRdr.Read())
            {
                SOListSONO = sqlRdr["SO_NO"].ToString();
                SOListModelName = sqlRdr["MODEL_NAME"].ToString();
                SOListDeliveryLocation = sqlRdr["DELIVERY_LOCATION"].ToString();
                SOListShippingQuantity = int.Parse(sqlRdr["SHIPPING_QUANTITY"].ToString());
                SOListN01NO = sqlRdr["N01_NO"].ToString();
            }

            stbSql.Clear();

            //----------------------------------------------------------------------------
            // ここからExcelの内容
            //----------------------------------------------------------------------------
            stbSql.Append("SELECT ");
            stbSql.Append("    ROW_NUMBER() OVER(ORDER BY SERIAL_NUMBER ASC) REC_NUM, ");
            stbSql.Append("    T_SERIAL_STATUS.SERIAL_NUMBER, ");
            stbSql.Append("    NULL AS DIGIT15, ");
            stbSql.Append("    NULL AS DIGIT8, ");
            stbSql.Append("    NULL AS DIGIT9, ");
            stbSql.Append("    NULL AS DIGIT11, ");
            stbSql.Append("    NULL AS DIGIT12, ");
            stbSql.Append("    NULL AS SERIAL_DIGIT_VARIFICATION, ");
            stbSql.Append("    IIF(T_SERIAL_STATUS.NG_FLG = '1', 'NG', '') AS NG_FLG, ");
            stbSql.Append("    T_SERIAL_STATUS.NG_REASON, ");
            stbSql.Append("    T_SERIAL_STATUS.WORKDAY, ");
            stbSql.Append("    M_INSTRUCTION.INSTRUCTION, ");
            stbSql.Append("    IIF(T_SERIAL_STATUS.NG_FLG = '1', 'New　Credit', '') AS RMA_TYPE, ");
            stbSql.Append("    IIF(T_SERIAL_STATUS.NG_FLG = '1', '', T_SO_STATUS.DELIVERY_LOCATION) AS DELIVERY_LOCATION, ");
            stbSql.Append("    IIF(T_SERIAL_STATUS.NG_FLG='1', 'REF', '') AS OTHER ");
            stbSql.Append("FROM ");
            stbSql.Append("    T_SO_STATUS ");
            stbSql.Append("    LEFT JOIN T_SERIAL_STATUS ");
            stbSql.Append("        ON T_SO_STATUS.SO_NO = T_SERIAL_STATUS.SO_NO ");
            stbSql.Append("    LEFT JOIN M_INSTRUCTION ");
            stbSql.Append("        ON T_SERIAL_STATUS.INSTRUCTION = M_INSTRUCTION.INSTRUCTION_ID ");
            stbSql.Append("WHERE ");
            stbSql.Append("    T_SO_STATUS.SO_NO = '" + strSONO + "' ");
            stbSql.Append("    AND T_SO_STATUS.DEL_FLG = '0' ");
            stbSql.Append("    AND T_SERIAL_STATUS.DEL_FLG = '0' ");

            List<SrchRstOrderReport> lstSrchRstOrderReport = new List<SrchRstOrderReport>();

            sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

            while (sqlRdr.Read())
            {
                lstSrchRstOrderReport.Add(new SrchRstOrderReport
                {
                    RecNum = int.Parse(sqlRdr["REC_NUM"].ToString()),
                    SerialNumber = sqlRdr["SERIAL_NUMBER"].ToString(),
                    Digit15 = sqlRdr["DIGIT15"].ToString(),
                    Digit8 = sqlRdr["DIGIT8"].ToString(),
                    Digit9 = sqlRdr["DIGIT9"].ToString(),
                    Digit11 = sqlRdr["DIGIT11"].ToString(),
                    Digit12 = sqlRdr["DIGIT12"].ToString(),
                    SerialDigitVarification = sqlRdr["SERIAL_DIGIT_VARIFICATION"].ToString(),
                    NGFLG = sqlRdr["NG_FLG"].ToString(),
                    NGReason = sqlRdr["NG_REASON"].ToString(),
                    WorkDay = string.IsNullOrEmpty(sqlRdr["WORKDAY"].ToString()) ? (DateTime?)null : DateTime.Parse(sqlRdr["WORKDAY"].ToString()),
                    Instruction = sqlRdr["INSTRUCTION"].ToString(),
                    RmaType = sqlRdr["RMA_TYPE"].ToString(),
                    DeliveryLocation = sqlRdr["DELIVERY_LOCATION"].ToString(),
                    Other = sqlRdr["OTHER"].ToString(),
                });

                RecCount++;

                //　DOA数、出荷数の判定
                if (sqlRdr["NG_FLG"].ToString() == "1")
                {
                    DOAQuantity++;
                }
                else
                {
                    ShipmentQuantity++;
                }

                //　検品数の判定（作業日が記載されていれば検品数に加える）
                if (sqlRdr["WORKDAY"].ToString() != null)
                {
                    InspectionQuantity++;
                }
            }
            dsnLib.DB_Close();

            //----------------------------------------------------------------------------
            // ここからExcel設定
            //----------------------------------------------------------------------------
            var WorkBook = new XLWorkbook();
            // Bookの書式設定
            WorkBook.Style.Font.FontName = "Meiryo UI";
            WorkBook.Style.Font.FontSize = 11;

            var WorkSheet = WorkBook.Worksheets.Add("検品スキャン");
            // Sheet書式変更
            WorkSheet.Row(1).Height = 51;
            WorkSheet.Column("K").Style.NumberFormat.Format = "M/d";
            WorkSheet.SheetView.FreezeRows(1);
            WorkSheet.Range("B1:K1").Style.Fill.BackgroundColor = XLColor.FromHtml("#808000");
            WorkSheet.Range("B1:KI1").Style.Font.FontColor = XLColor.White;
            WorkSheet.Range("B1:KI1").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            WorkSheet.Range("L1:O1").Style.Font.Bold = true;
            WorkSheet.Range("L1:O1").Style.Fill.BackgroundColor = XLColor.FromHtml("#92D050");
            WorkSheet.Range("L1:O1").Style.Font.FontColor = XLColor.Black;


            // 見出し
            WorkSheet.Cell("C1").Value = "15桁用";
            WorkSheet.Cell("D1").Value = "8桁用";
            WorkSheet.Cell("E1").Value = "9桁用";
            WorkSheet.Cell("F1").Value = "11桁用";
            WorkSheet.Cell("G1").Value = "12桁用";
            WorkSheet.Cell("H1").Value = "SN桁数照合";
            WorkSheet.Cell("I1").Value = "ＮＧフラグ";
            WorkSheet.Cell("J1").Value = "備考";
            WorkSheet.Cell("K1").Value = "作業日";
            WorkSheet.Cell("L1").Value = "ASUS様指示";
            WorkSheet.Cell("M1").Value = "RMA Type";
            WorkSheet.Cell("N1").Value = "発送先";
            WorkSheet.Cell("O1").Value = "その他";
            WorkSheet.Range("A1:O1").Style
                    .Border.SetTopBorder(XLBorderStyleValues.Thin)
                    .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                    .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                    .Border.SetRightBorder(XLBorderStyleValues.Thin);
            WorkSheet.Cell("B1").Style
                    .Border.SetLeftBorder(XLBorderStyleValues.Medium)
                    .Border.SetRightBorder(XLBorderStyleValues.Medium);


            while (Counter < RecCount)
            {
                WorkSheet.Cell(Counter + 2, 1).Value = lstSrchRstOrderReport[Counter].RecNum;
                WorkSheet.Cell(Counter + 2, 2).Value = lstSrchRstOrderReport[Counter].SerialNumber;
                WorkSheet.Cell(Counter + 2, 3).Value = lstSrchRstOrderReport[Counter].Digit15;
                WorkSheet.Cell(Counter + 2, 4).Value = lstSrchRstOrderReport[Counter].Digit8;
                WorkSheet.Cell(Counter + 2, 5).Value = lstSrchRstOrderReport[Counter].Digit9;
                WorkSheet.Cell(Counter + 2, 6).Value = lstSrchRstOrderReport[Counter].Digit11;
                WorkSheet.Cell(Counter + 2, 7).Value = lstSrchRstOrderReport[Counter].Digit12;
                WorkSheet.Cell(Counter + 2, 8).Value = lstSrchRstOrderReport[Counter].SerialDigitVarification;
                WorkSheet.Cell(Counter + 2, 9).Value = lstSrchRstOrderReport[Counter].NGFLG;
                WorkSheet.Cell(Counter + 2, 10).Value = lstSrchRstOrderReport[Counter].NGReason;
                WorkSheet.Cell(Counter + 2, 11).Value = lstSrchRstOrderReport[Counter].WorkDay;
                WorkSheet.Cell(Counter + 2, 12).Value = lstSrchRstOrderReport[Counter].Instruction;
                WorkSheet.Cell(Counter + 2, 13).Value = lstSrchRstOrderReport[Counter].RmaType;
                WorkSheet.Cell(Counter + 2, 14).Value = lstSrchRstOrderReport[Counter].DeliveryLocation;
                WorkSheet.Cell(Counter + 2, 15).Value = lstSrchRstOrderReport[Counter].Other;

                if (lstSrchRstOrderReport[Counter].Instruction == null)
                {
                    WorkSheet.Cell(Counter + 2, 12).Style.Font.FontColor = XLColor.Red;
                    WorkSheet.Cell(Counter + 2, 12).Style.Fill.BackgroundColor = XLColor.FromHtml("#808080");
                }

                if (lstSrchRstOrderReport[Counter].RmaType == null)
                {
                    WorkSheet.Cell(Counter + 2, 13).Style.Font.FontColor = XLColor.Red;
                    WorkSheet.Cell(Counter + 2, 13).Style.Fill.BackgroundColor = XLColor.FromHtml("#808080");
                }

                if (lstSrchRstOrderReport[Counter].Other == null)
                {
                    WorkSheet.Cell(Counter + 2, 15).Style.Font.FontColor = XLColor.Red;
                    WorkSheet.Cell(Counter + 2, 15).Style.Fill.BackgroundColor = XLColor.FromHtml("#808080");
                }

                WorkSheet.Range(Counter + 2, 2, Counter + 2, 15).Style
                        .Border.SetTopBorder(XLBorderStyleValues.Thin)
                        .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                        .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                        .Border.SetRightBorder(XLBorderStyleValues.Thin);
                WorkSheet.Cell(Counter + 2, 2).Style
                        .Border.SetLeftBorder(XLBorderStyleValues.Medium)
                        .Border.SetRightBorder(XLBorderStyleValues.Medium);

                Counter++;

            }

            WorkSheet.Cell(Counter + 3, 13).Value = "検品数";
            WorkSheet.Cell(Counter + 4, 13).Value = "出荷数";
            WorkSheet.Cell(Counter + 5, 13).Value = "DOA";

            if (Counter > 0)
            {
                WorkSheet.Cell(Counter + 3, 14).Value = InspectionQuantity + "台";
                WorkSheet.Cell(Counter + 4, 14).Value = ShipmentQuantity + "台";
                WorkSheet.Cell(Counter + 5, 14).Value = DOAQuantity + "台";
            }
            else
            {
                WorkSheet.Cell(Counter + 3, 14).Value = 0 + "台";
                WorkSheet.Cell(Counter + 4, 14).Value = 0 + "台";
                WorkSheet.Cell(Counter + 5, 14).Value = 0 + "台";
            }

            WorkSheet.Columns().AdjustToContents();
            WorkSheet.Columns("C:H").Hide();

            OutputFileName = @"C:\Temp\"
                            + DateTime.Now.ToString("yyyy年MM月dd日")
                            + "_検品_"
                            + SOListModelName
                            + "("
                            + SOListDeliveryLocation
                            + "："
                            + SOListShippingQuantity + "台)_"
                            + SOListSONO
                            + "_"
                            + SOListN01NO
                            + ".xlsx";

            //WorkBook.SaveAs(OutputFileName);

            using (MemoryStream stream = new MemoryStream())
            {
                WorkBook.SaveAs(stream);
            }
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
}