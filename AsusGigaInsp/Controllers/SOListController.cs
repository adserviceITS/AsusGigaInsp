using AsusGigaInsp.Models;
using System.IO;
using System.Web.Mvc;
using ClosedXML.Excel;
using System;
using AsusGigaInsp.Modules;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace AsusGigaInsp.Controllers
{
    public class SOListController : Controller
    {
        // GET: SOList
        [HttpGet]
        public ActionResult SOListSearch()
        {
            // 検索条件をセット
            SOListModels models = new SOListModels();

            // 最終取り込み日をセット
            models.SetInsertDate();

            // ステータスコンボBOXをセット
            models.SetDropDownListStatusName();

            // 日付のエラー有無コンボBOXをセット
            ViewBag.DropDownDateError = new SelectListItem[]
            {
                new SelectListItem() { Value="0", Text="非稼働日に該当しない"},
                new SelectListItem() { Value="1", Text="非稼働日に該当する"}
            };

            return View(models);
        }

        // POST: SOList/SOListSearch/Search
        // オーダー検索画面/検索ボタン押下時
        [HttpPost]
        public ActionResult SOListSearchResult(SOListModels models)
        {
            // 選択された表示方法を元にWhere句を作成
            models.SetWhere();

            // 最終取り込み日をセット
            models.SetInsertDate();

            // モデルにオーダーリストをセット
            models.SetSrchRstOrderList();

            // ステータスコンボBOXをセット
            models.SetDropDownListStatusName();

            // 日付のエラー有無コンボBOXをセット
            ViewBag.DropDownDateError = new SelectListItem[]
            {
                new SelectListItem() { Value="0", Text="非稼働日に該当しない"},
                new SelectListItem() { Value="1", Text="非稼働日に該当する"}
            };

            return View("SOListSearch", models);
        }

        public ActionResult SOListUpLoad()
        {
            UploadFile UploadFile = new UploadFile();
            return View(UploadFile);
        }

        [HttpPost]
        public ActionResult SOListUpLoad(UploadFile UploadFile)
        {
            SOListUpLoadModels models = new SOListUpLoadModels();

            DateTime DTImportTime = DateTime.Now;
            string StrUpdUID = Session["ID"].ToString();

            // アップロードファイルをモデルにセット
            models.UFUploadFile = UploadFile;

            if (ModelState.IsValid)
            {

                if (UploadFile.ExcelFile.ContentLength > 0)
                {
                    if (UploadFile.ExcelFile.FileName.EndsWith(".xlsx") || UploadFile.ExcelFile.FileName.EndsWith(".xls"))
                    {
                        XLWorkbook Workbook;
                        try
                        {
                            Workbook = new XLWorkbook(UploadFile.ExcelFile.InputStream);
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError(string.Empty, $"ファイルを確認してください。 {ex.Message}");
                            return View();
                        }
                        IXLWorksheet WorkSheet = null;
                        try
                        {
                            WorkSheet = Workbook.Worksheet(1);
                        }
                        catch
                        {
                            ModelState.AddModelError(string.Empty, "sheetが存在しません。");
                            return View();
                        }

                        // Excelデータをモデルにセット
                        models.GetSOExcelData();

                        // バリデーションチェック
                        int IntRowCount = models.SOList.GetLength(0);
                        DSNLibrary dsnLib = new DSNLibrary();
                        StringBuilder stbSql = new StringBuilder();

                        for (int RowCounter = 2; RowCounter < IntRowCount; RowCounter++)
                        {
                            // Excelのデータチェック **********************************************
                            string StrCheckID = models.SOList[RowCounter, 0];
                            string StrCheckSONO = models.SOList[RowCounter, 1];
                            string StrCheckN01 = models.SOList[RowCounter, 19];

                            if (string.IsNullOrEmpty(StrCheckID))
                            {
                                ModelState.AddModelError(string.Empty, (RowCounter + 1) + "行目は行番号が入力されていません。");
                            }

                            if (string.IsNullOrEmpty(StrCheckSONO))
                            {
                                ModelState.AddModelError(string.Empty, (RowCounter + 1) + "行目はSO#が入力されていません。");
                            }

                            if (string.IsNullOrEmpty(StrCheckN01))
                            {
                                ModelState.AddModelError(string.Empty, (RowCounter + 1) + "行目はN01#が入力されていません。");
                            }

                            for (int CheckRow = RowCounter + 1; CheckRow < IntRowCount; CheckRow++)
                            {
                                if (!string.IsNullOrEmpty(StrCheckID))
                                {
                                    if (StrCheckID == models.SOList[CheckRow, 0])
                                    {
                                        ModelState.AddModelError(string.Empty, (RowCounter + 1) + "行目の行番号は" + (CheckRow + 1) + "行目の行番号と重複しています。");
                                    }
                                }

                                if (!string.IsNullOrEmpty(StrCheckSONO))
                                {
                                    if (StrCheckSONO == models.SOList[CheckRow, 1])
                                    {
                                        ModelState.AddModelError(string.Empty, (RowCounter + 1) + "行目のSO#は" + (CheckRow + 1) + "行目のSO#と重複しています。");
                                    }

                                }

                                if (!string.IsNullOrEmpty(StrCheckSONO))
                                {
                                    if (StrCheckSONO == models.SOList[CheckRow, 19])
                                    {
                                        ModelState.AddModelError(string.Empty, (RowCounter + 1) + "行目のN01番号は" + (CheckRow + 1) + "行目のN01番号と重複しています。");
                                    }
                                }
                            }
                        }

                        if (!ModelState.IsValid)
                        {
                            ModelState.AddModelError(string.Empty, "修正後、再度取込んで下さい。");
                            return View();
                        }

                        // T_SO_LISTへのデータ取込み
                        models.InsertSOList(DTImportTime, StrUpdUID);

                        // T_SO_STATUSへの取込み
                        models.UpsertSOStatus(DTImportTime, StrUpdUID);

                        // T_SO_STATUS_HISTORYへの取込み
                        models.InsertSOStatusHistory(DTImportTime, StrUpdUID);


                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "読み込めるのは、.xlsx ファイルと .xls ファイルのみです。");
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "有効なファイルではありません。");
                    return View();
                }
            }
            ViewBag.Message = "取り込みが完了しました。";
            return View();
        }

        public FileResult Export()
        {
            string strSONO = this.Request.QueryString["SONO"];
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
            stbSql.Append("    T_SO_STATUS.DELIVERY_LOCATION ");
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
                    DeliveryLocation = sqlRdr["DELIVERY_LOCATION"].ToString()
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

            SOListUpdateModels mdlSOListUpdate = new SOListUpdateModels();
            DateTime DTNow = DateTime.Now;
            string StrStatusNow = mdlSOListUpdate.NowStatus(strSONO);

            if (StrStatusNow.CompareTo("5010") < 0)
            {
                mdlSOListUpdate.UpdateSOList(Session["ID"].ToString(), DTNow, "5010", strSONO);
                mdlSOListUpdate.UpdateSOListHistory(Session["ID"].ToString(), DTNow, StrStatusNow, "5010", strSONO);

            }


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

            // 内容
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
                WorkSheet.Cell(Counter + 2, 14).Value = lstSrchRstOrderReport[Counter].DeliveryLocation;

                WorkSheet.Cell(Counter + 2, 12).Style.Fill.BackgroundColor = XLColor.FromHtml("#808080");
                WorkSheet.Cell(Counter + 2, 13).Style.Fill.BackgroundColor = XLColor.FromHtml("#808080");
                WorkSheet.Cell(Counter + 2, 15).Style.Fill.BackgroundColor = XLColor.FromHtml("#808080");

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

            //// 集計欄
            //WorkSheet.Cell(Counter + 3, 13).Value = "検品数";
            //WorkSheet.Cell(Counter + 4, 13).Value = "出荷数";
            //WorkSheet.Cell(Counter + 5, 13).Value = "DOA";

            //if (Counter > 0)
            //{
            //    WorkSheet.Cell(Counter + 3, 14).Value = InspectionQuantity + "台";
            //    WorkSheet.Cell(Counter + 4, 14).Value = ShipmentQuantity + "台";
            //    WorkSheet.Cell(Counter + 5, 14).Value = DOAQuantity + "台";
            //}
            //else
            //{
            //    WorkSheet.Cell(Counter + 3, 14).Value = 0 + "台";
            //    WorkSheet.Cell(Counter + 4, 14).Value = 0 + "台";
            //    WorkSheet.Cell(Counter + 5, 14).Value = 0 + "台";
            //}
            WorkSheet.RangeUsed().SetAutoFilter();
            WorkSheet.Columns().AdjustToContents();
            WorkSheet.Columns("C:H").Hide();
            WorkSheet.Column("J").Width = 62;

            // ファイル名
            OutputFileName = DateTime.Now.ToString("yyyy年MM月dd日")
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

            using (MemoryStream stream = new MemoryStream())
            {
                WorkBook.SaveAs(stream);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", OutputFileName);
            }
        }

        public FileResult PalletSheet()
        {
            string prmSONO = this.Request.QueryString["SONO"];
            string OutputFileName = "";

            //----------------------------------------------------------------------------
            // ここからExcel設定
            //----------------------------------------------------------------------------
            var WorkBook = new XLWorkbook();
            // Bookの書式設定
            WorkBook.Style.Font.FontName = "游ゴシック";
            WorkBook.Style.Font.FontSize = 36;

            var WorkSheet = WorkBook.Worksheets.Add("パレットシート");
            // Sheet書式設定
            WorkSheet.Row(1).Height = 115.5;
            WorkSheet.Row(2).Height = 115.5;
            WorkSheet.Row(3).Height = 115.5;
            WorkSheet.Row(4).Height = 115.5;
            WorkSheet.Row(5).Height = 119.25;

            WorkSheet.Column("A").Width = 25.57;
            WorkSheet.Column("B").Width = 44.43;
            WorkSheet.Column("C").Width = 21.57;
            WorkSheet.Column("D").Width = 49.14;

            WorkSheet.Range("A1:D5").Style.Font.Bold = true;
            WorkSheet.Range("A1:D5").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            WorkSheet.Range("A1:D5").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            WorkSheet.Range("B5:C5").Merge();
            WorkSheet.Range("A1:D5").Style
                    .Border.SetTopBorder(XLBorderStyleValues.Thin)
                    .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                    .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                    .Border.SetRightBorder(XLBorderStyleValues.Thin);
            WorkSheet.Range("B5:D5").Style.Alignment.WrapText = true;

            WorkSheet.Cell("B1").Style.NumberFormat.Format = "M月d日";
            WorkSheet.Cell("B2").Style.NumberFormat.Format = "@";
            WorkSheet.Cell("B4").Style.NumberFormat.Format = "M月d日";
            WorkSheet.Cell("D1").Style.NumberFormat.Format = "M月d日";
            WorkSheet.Cell("D2").Style.NumberFormat.Format = "@";
            WorkSheet.Cell("D4").Style.NumberFormat.Format = "@";

            WorkSheet.Cell("B2").Style.Font.FontSize = 72;
            WorkSheet.Cell("B3").Style.Font.FontSize = 22;
            WorkSheet.Cell("D5").Style.Font.FontSize = 24;

            WorkSheet.Cell("B3").Style.Font.FontName = "Calibri";

            WorkSheet.Cell("C3").Style.Alignment.WrapText = true;

            // 見出し
            WorkSheet.Cell("A1").Value = "入荷日";
            WorkSheet.Cell("A2").Value = "SO#";
            WorkSheet.Cell("A3").Value = "Model";
            WorkSheet.Cell("A4").Value = "出荷日";
            WorkSheet.Cell("A5").Value = "出荷先";
            WorkSheet.Cell("C1").Value = "横持日";
            WorkSheet.Cell("C2").Value = "N01#";
            WorkSheet.Cell("C3").Value = "1PL" + Environment.NewLine + "数量";
            WorkSheet.Cell("C4").Value = "PL";

            // 固定値
            WorkSheet.Cell("D1").Value = "";
            WorkSheet.Cell("D3").Value = "135";
            WorkSheet.Cell("D4").Value = "8/8";
            WorkSheet.Cell("D5").Value = "□NG処理待ち" + Environment.NewLine +
                                         "□レポート回答待ち" + Environment.NewLine +
                                         "□ 発送処理完了";

            //----------------------------------------------------------------------------
            // ここからExcel設定
            //----------------------------------------------------------------------------
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            stbSql.Append("SELECT ");
            stbSql.Append("    * ");
            stbSql.Append("FROM ");
            stbSql.Append("    T_SO_STATUS ");
            stbSql.Append("WHERE ");
            stbSql.Append("    T_SO_STATUS.SO_NO = '" + prmSONO + "' ");

            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

            string EstArrivalDate = "";
            string ModelName = "";
            string SiTekEstArrivalDate = "";
            string DeliveryLocation = "";
            string N01NO = "";

            while (sqlRdr.Read())
            {
                EstArrivalDate = sqlRdr["EST_ARRIVAL_DATE"].ToString();
                ModelName = sqlRdr["MODEL_NAME"].ToString();
                SiTekEstArrivalDate = sqlRdr["SI_TEK_EST_ARRIVAL_DATE"].ToString();
                DeliveryLocation = sqlRdr["DELIVERY_LOCATION"].ToString();
                N01NO = sqlRdr["N01_NO"].ToString();
            }

            WorkSheet.Cell("B1").Value = EstArrivalDate;
            WorkSheet.Cell("B2").Value = prmSONO.Substring(prmSONO.Length - 4, 4);
            WorkSheet.Cell("B3").Value = ModelName;
            WorkSheet.Cell("B4").Value = SiTekEstArrivalDate;
            WorkSheet.Cell("B5").Value = DeliveryLocation;
            WorkSheet.Cell("D2").Value = N01NO.Substring(N01NO.Length - 6, 6);

            // ファイル名
            OutputFileName = DateTime.Now.ToString("yyyy年MM月dd日 hh時mm分ss秒")
                            + "_パレットシート_"
                            + prmSONO
                            + ".xlsx";

            using (MemoryStream stream = new MemoryStream())
            {
                WorkBook.SaveAs(stream);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", OutputFileName);
            }
        }

        //GET: SOList/SOListUpdate
        public ActionResult SOListUpdate()
        {
            SOListUpdateModels mdlSOListUpdate = new SOListUpdateModels();

            mdlSOListUpdate.SetSOListDetails(this.Request.QueryString["SONO"]);

            // ステータスコンボBOXをセット
            mdlSOListUpdate.SetDropDownListNewStatusName(this.Request.QueryString["SONO"]);

            // 画面表示
            return View(mdlSOListUpdate);
        }

        // POST: SOList/SOListUpdate
        // オーダー情報編集画面/登録ボタン押下時
        [HttpPost]
        public ActionResult SOListUpdateResult(SOListUpdateModels mdlSOListUpdate)
        {
            DateTime DTNow = DateTime.Now;

            // エラーがなければ処理継続
            if (ModelState.IsValid)
            {
                // ステータスが変更されていればデータ更新
                if (mdlSOListUpdate.EntStatusID != mdlSOListUpdate.CompStatusID)
                {
                    // オーダーリスト更新
                    mdlSOListUpdate.UpdateSOList(Session["ID"].ToString(), DTNow, mdlSOListUpdate.EntStatusID, mdlSOListUpdate.EntSONO);

                    // オーダーリスト履歴更新
                    mdlSOListUpdate.UpdateSOListHistory(Session["ID"].ToString(), DTNow, mdlSOListUpdate.CompStatusID, mdlSOListUpdate.EntStatusID, mdlSOListUpdate.EntSONO);

                    // シリアルリスト更新
                    mdlSOListUpdate.UpdateSerialList(Session["ID"].ToString(), DTNow, mdlSOListUpdate.EntStatusID, mdlSOListUpdate.EntSONO);

                    // シリアルリスト履歴更新
                    mdlSOListUpdate.UpdateSerialListHistory(Session["ID"].ToString(), DTNow, mdlSOListUpdate.EntStatusID, mdlSOListUpdate.EntSONO);
                }

                // 保留フラグが更新されていればデータ更新
                if (mdlSOListUpdate.EntHoldFlg != mdlSOListUpdate.CompHoldFlg)
                {
                    // オーダーリスト更新
                    mdlSOListUpdate.UpdateSOList(Session["ID"].ToString(), DTNow, mdlSOListUpdate.EntStatusID, mdlSOListUpdate.EntSONO);
                }

                // 一時データ（成功メッセージ）を保存
                TempData["msg"] = String.Format(
                  "「{0}」のステータスを変更しました。", mdlSOListUpdate.EntSONO);

                return RedirectToAction("SOListSearch", "SOList");

            }
            // 画面表示
            return this.View("SOListUpdate", mdlSOListUpdate);
        }

        // POST: SOList/SOListUpdate
        // オーダー情報編集画面/削除ボタン押下時
        [HttpPost]
        public ActionResult SOListDeleteResult(SOListUpdateModels mdlSOListUpdate)
        {
            // オーダー情報（T_SO_STATUS）削除
            mdlSOListUpdate.DeleteSOList(Session["ID"].ToString());

            // オーダー情報（T_SERIAL_STATUS）削除
            mdlSOListUpdate.DeleteSOList(Session["ID"].ToString());

            return RedirectToAction("SOListSearch", "SOList");

        }
    }
}
