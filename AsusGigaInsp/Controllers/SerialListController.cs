using AsusGigaInsp.Models;
using AsusGigaInsp.Modules;
using System.Data.SqlClient;
using System.Text;
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
            models.SetSearchWhere();
            models.SetRstSerialList();
            models.SetDropDownListLine();
            models.SetDropDownListInstruction();
            return View("SerialList", models);
        }

        // POST: SerialList/Search
        [HttpPost]
        public ActionResult Search(SerialListModels models)
        {
            models.SetSearchWhere();
            models.SetRstSerialList();
            models.SetDropDownListLine();
            models.SetDropDownListInstruction();

            return View("SerialList", models);
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
    }
}