using AsusGigaInsp.Models;
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
            isModels.SetInspStartSerialLists();
            isModels.SetDropDownListLine();

            // セッションのラインをセット。ラインがなかったらまた選ばせる。管理者はログイン時にライン選択しないから。
            if (Session["LineID"] != null)
                isModels.LineID = Session["LineID"].ToString();

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

            // まだ入荷前の場合はエラー
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
                    stbSql.Append("      AND SERIAL_STATUS_ID < '2010' ");

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
            SerialErrMsg.Append("はまだ入荷されていません。");

            dsnLib.DB_Close();

            if (ExistErr)
                ModelState.AddModelError("MasterCartonSerial", SerialErrMsg.ToString());

            if (!ModelState.IsValid)
                return Index();

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

            models.UpdateStatus();
            models.SetInspStartSerialLists();

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
    }
}
