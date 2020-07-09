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

            // 続けてバーコード入力出来るように画面表示を継続させる。
            //ViewBag.ControllAction = "InspStart";

            return View("InspStart", models);
        }
    }
}