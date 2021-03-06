﻿using AsusGigaInsp.Models;
using AsusGigaInsp.Modules;
using System;
using System.Data.SqlClient;
using System.Text;
using System.Web.Mvc;

namespace AsusGigaInsp.Controllers
{
    public class InspStartController : Controller
    {
        // GET: InspStart
        public ActionResult Index()
        {
            InspStartModels isModels = new InspStartModels();
            isModels.SetDropDownListLine();

            // セッションのラインをセット。ラインがなかったらまた選ばせる。管理者はログイン時にライン選択しないから。
            if (Session["LineID"] != null)
                isModels.LineID = Session["LineID"].ToString();

            // ----- ADD START 2020/11/10 E.KOSHIKAWA ----- //
            isModels.SetStbWhere();
            // ----- ADD  END  2020/11/10 E.KOSHIKAWA ----- //

            isModels.SetInspStartSerialLists();
            isModels.SetLineCompCnt();

            // ----- ADD START 2020/11/03 E.KOSHIKAWA ----- //
            isModels.SetSOCompCnt();
            // ----- ADD  END  2020/11/03 E.KOSHIKAWA ----- //

            return View("InspStart", isModels);
        }

        // POST: InspStart/Entry
        [HttpPost]
        public ActionResult Entry(InspStartModels models)
        {
            // バリデーションチェック START ***************************************************************
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();
            string[] SerialNOs = models.MasterCartonSerial.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            models.LineID = Session["LineID"].ToString();

            // シリアルの登録がなければエラー
            StringBuilder SerialErrMsg = new StringBuilder();
            bool ExistErr = false;
            SerialErrMsg.Append("シリアル番号：");
            for (int i = 0; i < SerialNOs.Length; i++)
            {
                if (!String.IsNullOrEmpty(SerialNOs[i]))
                {
                    stbSql.Append("SELECT ");
                    stbSql.Append("   * ");
                    stbSql.Append("FROM dbo.T_SERIAL_STATUS ");
                    stbSql.Append("WHERE SERIAL_NUMBER = '" + SerialNOs[i] + "' ");

                    SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

                    if (!sqlRdr.HasRows)
                    {
                        SerialErrMsg.Append("【" + SerialNOs[i] + "】");
                        ExistErr = true;
                    }
                    stbSql.Clear();
                    sqlRdr.Close();
                }
            }
            SerialErrMsg.Append("は登録されていません！");

            dsnLib.DB_Close();

            if (ExistErr)
                ModelState.AddModelError("MasterCartonSerial", SerialErrMsg.ToString());

            if (!ModelState.IsValid)
                return Index();

            // まだ入荷前の場合はエラー ⇒ 未入荷以前は全てOKとする。
            //SerialErrMsg.Clear();
            //SerialErrMsg.Append("シリアル番号：");
            //for (int i = 0; i < SerialNOs.Length; i++)
            //{
            //    if (!String.IsNullOrEmpty(SerialNOs[i]))
            //    {
            //        stbSql.Append("SELECT ");
            //        stbSql.Append("   * ");
            //        stbSql.Append("FROM dbo.T_SERIAL_STATUS ");
            //        stbSql.Append("WHERE SERIAL_NUMBER = '" + SerialNOs[i] + "' ");
            //        stbSql.Append("      AND SERIAL_STATUS_ID < '2010' ");

            //        SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

            //        if (sqlRdr.HasRows)
            //        {
            //            SerialErrMsg.Append("【" + SerialNOs[i] + "】");
            //            ExistErr = true;
            //        }
            //        stbSql.Clear();
            //        sqlRdr.Close();
            //    }
            //}
            //SerialErrMsg.Append("はまだ入荷されていません。");

            //dsnLib.DB_Close();

            //if (ExistErr)
            //    ModelState.AddModelError("MasterCartonSerial", SerialErrMsg.ToString());

            //if (!ModelState.IsValid)
            //    return Index();

            // 既に作業開始されていた場合はエラー
            SerialErrMsg.Clear();
            SerialErrMsg.Append("シリアル番号：");
            for (int i = 0; i < SerialNOs.Length; i++)
            {
                if (!String.IsNullOrEmpty(SerialNOs[i]))
                {
                    stbSql.Append("SELECT ");
                    stbSql.Append("   * ");
                    stbSql.Append("FROM dbo.T_SERIAL_STATUS ");
                    stbSql.Append("WHERE SERIAL_NUMBER = '" + SerialNOs[i] + "' ");
                    stbSql.Append("      AND SERIAL_STATUS_ID >= '3010' ");

                    SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

                    if (sqlRdr.HasRows)
                    {
                        SerialErrMsg.Append("【" + SerialNOs[i] + "】");
                        ExistErr = true;
                    }
                    stbSql.Clear();
                    sqlRdr.Close();
                }
            }
            SerialErrMsg.Append("は既に作業開始されています。");

            dsnLib.DB_Close();

            if (ExistErr)
                ModelState.AddModelError("MasterCartonSerial", SerialErrMsg.ToString());

            if (!ModelState.IsValid)
                return Index();

            // バリデーションチェック END ****************************************************************************

            models.SetDropDownListLine();
            models.UpdateStatus();

            // ----- ADD START 2020/11/10 E.KOSHIKAWA ----- //
            models.SetStbWhere();
            // ----- ADD  END  2020/11/10 E.KOSHIKAWA ----- //

            models.SetInspStartSerialLists();
            models.SetLineCompCnt();
            // ----- ADD START 2020/11/03 E.KOSHIKAWA ----- //
            models.SetSOCompCnt();
            // ----- ADD  END  2020/11/03 E.KOSHIKAWA ----- //

            // 更新完了フラグをセット
            ViewBag.CompleteFlg = "true";

            return View("InspStart", models);
        }

        [HttpPost]
        public ActionResult SelectLine(InspStartModels model)
        {
            // セッションにラインをセット
            Session["LineID"] = model.LineID;

            return Index();
        }

        [HttpPost]
        public ActionResult LineChange(InspStartModels model)
        {
            // オーダーリスト更新
            model.InspStartLineChange();

            // セッションにラインをセット
            Session["LineID"] = model.LineID;

            return Index();
        }

        // POST: InspStart/Search
        public ActionResult SearchFromOutSide(string SearchLine, string SearchTime)
        {
            InspStartModels models = new InspStartModels();

            if (!string.IsNullOrEmpty(SearchLine))
            {
                string StrSearchLine = SearchLine.Substring(0, 1);

                models.LineID = StrSearchLine;
            }
            else
            {
                models.LineID = "ALL";
            }

            // セッションにラインをセット
            Session["LineID"] = models.LineID;

            if (!string.IsNullOrEmpty(SearchTime))
            {
                models.SrchTime = SearchTime;
            }

            models.SetDropDownListLine();
            models.SetStbWhere();
            models.SetInspStartSerialLists();
            models.SetLineCompCnt();
            models.SetSOCompCnt();

            // セッションのラインをクリア
            Session["LineID"] = null;

            return View("InspStart", models);
        }
    }
}
