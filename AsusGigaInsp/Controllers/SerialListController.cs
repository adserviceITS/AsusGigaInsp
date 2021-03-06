﻿using AsusGigaInsp.Models;
using AsusGigaInsp.Modules;
using ClosedXML.Excel;
using System;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AsusGigaInsp.Controllers
{
    public class SerialListController : Controller
    {
        // GET: SerialList/Index
        [HttpGet]
        public ActionResult Index()
        {
            SerialListModels models = new SerialListModels();
            // models.SetSearchWhere();
            // models.SetRstSerialList();
            // 初期設定
            models.DataCnt = 0;
            models.SetDropDownListSerialStatus();
            models.SetDropDownListInstruction();
            return View("SerialList", models);
        }

        // POST: SerialList/Search
        [HttpPost]
        public ActionResult Search(SerialListModels models)
        {
            models.SelectPage = 1;
            models.SetSearchWhere();
            models.SetPageNum();
            models.SetRstSerialList();
            models.SetDropDownListSerialStatus();
            models.SetDropDownListInstruction();

            return View("SerialList", models);
        }

        public ActionResult SerialUpLoad()
        {
            SerialUploadFile UploadFile = new SerialUploadFile();
            return View(UploadFile);
        }

        [HttpPost]
        public ActionResult SerialUpLoad(SerialUploadFile UploadFile)
        {
            SerialUploadModels models = new SerialUploadModels();

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
                            ModelState.AddModelError(String.Empty, $"ファイルを確認してください。 {ex.Message}");
                            return View();
                        }
                        IXLWorksheet WorkSheet = null;
                        try
                        {
                            WorkSheet = Workbook.Worksheet(1);
                        }
                        catch
                        {
                            ModelState.AddModelError(String.Empty, "sheetが存在しません。");
                            return View();
                        }

                        // Exxcelデータをモデルにセット
                        models.GetExcelData();

                        // バリデーションチェック
                        int IntRowCount = models.SerialList.GetLength(0);
                        DSNLibrary dsnLib = new DSNLibrary();
                        StringBuilder stbSql = new StringBuilder();
                        SqlDataReader sqlRdr;

                        // ----- INSERT START 2020/10/28 E.KOSHIKAWA -----
                        // 最初にExcelファイル内のデータをチェックする
                        // Serial Number、SOの未入力、重複のチェックを行う。

                        // Excelデータの2行目から順次データを読込む。
                        // (１行目はタイトル行のため、読み込まない。）
                        for (int RowCounter = 1; RowCounter < IntRowCount; RowCounter++)
                        {
                            // Excelのデータチェック **********************************************
                            string StrCheckSerialNo = models.SerialList[RowCounter, 0];
                            string StrCheckSO = models.SerialList[RowCounter, 19];

                           if (string.IsNullOrEmpty(StrCheckSerialNo))
                            {
                                ModelState.AddModelError(string.Empty, (RowCounter + 1) + "行目はSerial Numberが入力されていません。");
                            }

                            if (string.IsNullOrEmpty(StrCheckSO))
                            {
                                ModelState.AddModelError(string.Empty, (RowCounter + 1) + "行目はSOが入力されていません。");
                            }

                            for (int CheckRow = RowCounter + 1; CheckRow < IntRowCount; CheckRow++)
                            {
                                if (!string.IsNullOrEmpty(StrCheckSerialNo))
                                {
                                    if (StrCheckSerialNo == models.SerialList[CheckRow, 0])
                                    {
                                        ModelState.AddModelError(string.Empty, (RowCounter + 1) + "行目のSerial Numberは" + (CheckRow + 1) + "行目のSerial Numberと重複しています。");
                                    }
                                }
                            }
                        }

                        if (!ModelState.IsValid)
                        {
                            ModelState.AddModelError(string.Empty, "修正後、再度取込んで下さい。");
                            return View();
                        }

                        // Excelデータの2行目から順次データを読込む。
                        // (１行目はタイトル行のため、読み込まない。シリアルナンバーは1項目目）
                        for (int RowCounter = 1; RowCounter < IntRowCount; RowCounter++)
                        {
                            // Excelのシリアルが既に取込まれていないかをチェック **********************************************
                            stbSql.Append("SELECT ");
                            stbSql.Append("   * ");
                            stbSql.Append("FROM dbo.T_SERIAL_STATUS ");
                            stbSql.Append("WHERE SERIAL_NUMBER = '" + models.SerialList[RowCounter,0] + "' ");

                            sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

                            if (sqlRdr.HasRows)
                                ModelState.AddModelError(string.Empty, (RowCounter + 1) + "行目のシリアル【" + models.SerialList[RowCounter, 0] + "】は既に登録されています。");

                            stbSql.Clear();
                            sqlRdr.Close();
                            dsnLib.DB_Close();

                            // SO_NOの存在チェック **********************************************
                            stbSql.Append("SELECT ");
                            stbSql.Append("   * ");
                            stbSql.Append("FROM dbo.T_SO_STATUS ");
                            stbSql.Append("WHERE N01_NO = '" + models.SerialList[RowCounter, 19] + "' ");

                            sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

                            if (!sqlRdr.HasRows)
                                ModelState.AddModelError(string.Empty, (RowCounter + 1) + "行目のシリアル【" + models.SerialList[RowCounter, 0] + "】のSO#が登録されていません。");

                            stbSql.Clear();
                            sqlRdr.Close();
                            dsnLib.DB_Close();
                        }

                        if (!ModelState.IsValid)
                        {
                            ModelState.AddModelError(string.Empty, "修正後、再度取込んで下さい。");
                            return View();
                        }

                        // T_SERIAL_LISTへのデータ取込み
                        models.InsertSerialListTBL();

                        // T_SERIAL_STATUSへの取込み
                        models.InsertSerialStatusTBL();

                        // T_SERIAL_STATUS_HYSTORYへの取込み
                        models.InsertSerialStatusHystoryTBL();

                        // ----- INSERT START 2020/11/05 E.KOSHIKAWA -----
                        // 取り込んだSO(N01#)を取得
                        string StrInsertSO = models.SerialList[1, 19];

                        stbSql.Append("SELECT ");
                        stbSql.Append("    count(*) AS COUNT ");
                        stbSql.Append("FROM ");
                        stbSql.Append("    T_SERIAL_STATUS ");
                        stbSql.Append("WHERE ");
                        stbSql.Append("    SO = '" + StrInsertSO + "' ");

                        sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

                        sqlRdr.Read();

                        int SerialCnt = int.Parse(sqlRdr["COUNT"].ToString());

                        stbSql.Clear();
                        sqlRdr.Close();
                        dsnLib.DB_Close();

                        stbSql.Append("SELECT ");
                        stbSql.Append("    SHIPPING_QUANTITY ");
                        stbSql.Append("FROM ");
                        stbSql.Append("    T_SO_STATUS ");
                        stbSql.Append("WHERE ");
                        stbSql.Append("    N01_NO = '" + StrInsertSO + "' ");

                        sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

                        sqlRdr.Read();

                        int SOCnt = int.Parse(sqlRdr["SHIPPING_QUANTITY"].ToString());

                        sqlRdr.Close();
                        dsnLib.DB_Close();

                        if (SOCnt >= SerialCnt)
                        {
                            DateTime DTImportTime = DateTime.Now;

                            // T_SO_STATUSの更新
                            models.UpdateSoStatusTBL(StrInsertSO, DTImportTime);

                            // T_SO_STATUS_HISTORYに追加
                            models.InsertSoStatusHistoryTBL(StrInsertSO, DTImportTime);

                            // T_SERIAL_STATUSの更新
                            models.UpdateSerialStatusTBL(StrInsertSO, DTImportTime);

                            // T_SERIAL_STATUS_HYSTORYに追加
                            models.InsertSerialStatusHistoryTBL_2010(StrInsertSO, DTImportTime);
                        }

                    }
                    else
                    {
                        ModelState.AddModelError(String.Empty, "読み込めるのは、.xlsx ファイルと .xls ファイルのみです。");
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "有効なファイルではありません。");
                    return View();
                }
            }
            ViewBag.Message = "取り込みが完了しました。";
            return View();
        }

        [HttpPost]
        public ActionResult DLcsv(SerialListModels models)
        {
            // Contentをクリア
            Response.ClearContent();

            // Contentを設定
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("shift-jis");
            Response.ContentType = "text/csv";

            // 表示ファイル名を指定
            DateTime DTImportTime = DateTime.Now;
            string StrImportTime = DTImportTime.ToString("yyyyMMddHHmmss");
            string viewFileName = HttpUtility.UrlEncode(StrImportTime + "_シリアルリスト.csv");
            Response.AddHeader("Content-Disposition", "attachment;filename=" + viewFileName);

            // CSVデータを作成
            models.SetSearchWhere();
            models.MakeCsvData();

            // CSVデータを書き込み
            Response.Write(models.stbCsvData.ToString());

            // ダウンロード実行
            Response.Flush();
            Response.End();

            return View();
        }

        // POST: SerialList/Edit
        [HttpPost]
        public ActionResult Edit(SerialListModels ListModels)
        {
            SerialEditModels EditModels = new SerialEditModels();
            EditModels.SerialID = ListModels.SelectSerialID;
            EditModels.SetSerialInfo();
            EditModels.SetDropDownListInstruction();

            return View("SerialEdit", EditModels);
        }

        // POST: SerialList/EditEntry
        [HttpPost]
        public ActionResult EditEntry(SerialEditModels models)
        {
            models.SetDropDownListInstruction();

            // バリデーションチェック START ***************************************************************
            // 変更されたシリアルが重複していないかチェック
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            stbSql.Append("SELECT ");
            stbSql.Append("   * ");
            stbSql.Append("FROM dbo.T_SERIAL_STATUS ");
            stbSql.Append("WHERE ID <> '" + models.SerialID + "' ");
            stbSql.Append("  AND SERIAL_NUMBER = '" + models.SerialNumber + "' ");
            stbSql.Append("  AND DEL_FLG = '0' ");

            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

            if (sqlRdr.HasRows)
                ModelState.AddModelError(string.Empty, "入力されたシリアルは既に登録されています。");

            stbSql.Clear();
            sqlRdr.Close();

            if (!ModelState.IsValid)
                return View("SerialEdit", models);

            // バリデーションチェック END ****************************************************************************

            models.UpdateSerialInfo();

            // 更新完了メッセージをセット
            ViewBag.CompleteMSG = "登録完了しました。";

            models.SetSerialInfo();

            return View("SerialEdit", models);
        }

        // POST: SerialList/Delete
        [HttpPost]
        public ActionResult Delete(SerialEditModels models)
        {
            models.UpdateDelFlg();

            models.SetDropDownListInstruction();
            models.SetSerialInfo();

            ViewBag.CompleteMSG = "削除完了しました。";

            return View("SerialEdit", models);
        }

        // POST: SerialList/NG
        [HttpPost]
        public ActionResult NG(SerialListModels models)
        {
            models.UpdateNgFlg();
            return Search(models);
        }

        // GET: SerialList/BackSerialList
        [HttpGet]
        public ActionResult BackSerialList()
        {
            return Index();
        }

        // POST: SerialList/PageSearch
        [HttpPost]
        public ActionResult PageSearch(SerialListModels models)
        {
            models.SetSearchWhere();
            models.SetPageNum();
            models.SetRstSerialList();
            models.SetDropDownListSerialStatus();
            models.SetDropDownListInstruction();

            return View("SerialList", models);
        }

        // POST: SerialList/Search
        public ActionResult SearchFromOutSide(string SearchKey)
        {
            SerialListModels models = new SerialListModels();

            models.SelectPage = 1;
            if (!string.IsNullOrEmpty(SearchKey))
            {
                int IntLen = SearchKey.Length;

                if (IntLen > 4)
                {
                    models.SearchSONo = SearchKey;
                }
                else
                {
                    models.SearchSerialStatus = SearchKey;
                }
            }
            models.SetSearchWhere();
            models.SetPageNum();
            models.SetRstSerialList();
            models.SetDropDownListSerialStatus();
            models.SetDropDownListInstruction();

            return View("SerialList", models);
        }

    }
}
