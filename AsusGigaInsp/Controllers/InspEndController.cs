using AsusGigaInsp.Models;
using AsusGigaInsp.Modules;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AsusGigaInsp.Controllers
{
    public class InspEndController : Controller
    {
        // GET: InspEnd
        public ActionResult Index()
        {
            InspEndModels ieModels = new InspEndModels();
            ieModels.SetDropDownListLine();

            // セッションのラインをセット。ラインがなかったらまた選ばせる。管理者はログイン時にライン選択しないから。
            if (Session["LineID"] != null)
                ieModels.LineID = Session["LineID"].ToString();

            ieModels.SetInspEndSerialLists();

            return View("InspEnd", ieModels);
        }

        // POST: InspEnd/Entry
        [HttpPost]
        public ActionResult Entry(InspEndModels models)
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


            // 既に作業終了していた場合はエラー
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
                    stbSql.Append("      AND SERIAL_STATUS_ID >= '4010' ");

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
            SerialErrMsg.Append("は既に作業完了しています。");

            dsnLib.DB_Close();

            if (ExistErr)
                ModelState.AddModelError("MasterCartonSerial", SerialErrMsg.ToString());

            if (!ModelState.IsValid)
                return Index();

            // まだ作業開始されていない場合はエラー
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
                    stbSql.Append("      AND SERIAL_STATUS_ID < '3010' ");

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
            SerialErrMsg.Append("は検査開始登録されていません。");

            dsnLib.DB_Close();

            if (ExistErr)
                ModelState.AddModelError("MasterCartonSerial", SerialErrMsg.ToString());

            if (!ModelState.IsValid)
                return Index();

            // バリデーションチェック END ****************************************************************************

            models.UpdateStatus();
            models.SetInspEndSerialLists();

            // 更新完了フラグをセット
            ViewBag.CompleteFlg = "true";

            return View("InspEnd", models);
        }

        [HttpPost]
        public ActionResult SelectLine(InspEndModels model)
        {
            // セッションにラインをセット
            Session["LineID"] = model.LineID;

            return Index();
        }
    }
}
