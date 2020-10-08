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
            ieModels.SetLineCompCnt();

            return View("InspEnd", ieModels);
        }

        // POST: InspEnd/Entry
        [HttpPost]
        public ActionResult Entry(InspEndModels models)
        {
            models.LineID = Session["LineID"].ToString();

            string ErrMsg = ValidationCheck(models.MasterCartonSerial);

            if (!string.IsNullOrEmpty(ErrMsg))
                ModelState.AddModelError("MasterCartonSerial", ErrMsg);

            if (!ModelState.IsValid)
                return Index();

            models.SetDropDownListLine();
            models.UpdateStatus();
            models.SetInspEndSerialLists();
            models.SetLineCompCnt();

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

        [HttpPost]
        public ActionResult LineChange(InspEndModels model)
        {
            // オーダーリスト更新
            model.InspEndLineChange();

            // セッションにラインをセット
            Session["LineID"] = model.LineID;

            return Index();
        }

        // 2020/9/29 マスターカートンチェック追加
        [HttpPost]
        public ActionResult CheckMasterCartonSerial(string prmMasterCartonSerial)
        {
            string ErrMsg = ValidationCheck(prmMasterCartonSerial);

            return Json(new { ErrMsg = ErrMsg }, JsonRequestBehavior.AllowGet);
        }

        private string ValidationCheck (string MasterCartonSerial)
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();
            string[] SerialNOs = MasterCartonSerial.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

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
                return SerialErrMsg.ToString();


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
                return SerialErrMsg.ToString();

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
                return SerialErrMsg.ToString();

            SerialErrMsg.Clear();
            return SerialErrMsg.ToString();

        }

    }
}
