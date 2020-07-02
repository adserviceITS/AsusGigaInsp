using AsusGigaInsp.Models;
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

        // POST: SerialList/InspStart
        [HttpPost]
        public ActionResult InspStart(SerialListModels models)
        {
            // バリデーション
            if (string.IsNullOrWhiteSpace(models.CondMasterCartonStartSerial))
                ModelState.AddModelError("CondMasterCartonStartSerial", "マスターカートンのQRコードを入力して下さい。");

            if (!ModelState.IsValid)
                return Index();

            models.MasterCartonSerials = models.CondMasterCartonStartSerial;
            models.UpdateStatus("3010");
            models.SetSearchWhere();
            models.SetRstSerialList();
            models.SetDropDownListLine();
            models.SetDropDownListInstruction();

            // 続けてバーコード入力出来るように画面表示を継続させる。
            ViewBag.ControllAction = "InspStart";

            return View("SerialList", models);
        }

        // POST: SerialList/WorkEnd
        [HttpPost]
        public ActionResult WorkEnd(SerialListModels models)
        {
            // バリデーション
            if (string.IsNullOrWhiteSpace(models.CondMasterCartonEndSerial))
                ModelState.AddModelError("CondMasterCartonEndSerial", "マスターカートンのQRコードを入力して下さい。");

            if (!ModelState.IsValid)
                return Index();

            models.MasterCartonSerials = models.CondMasterCartonEndSerial;
            models.UpdateStatus("4010");
            models.SetSearchWhere();
            models.SetRstSerialList();
            models.SetDropDownListLine();
            models.SetDropDownListInstruction();

            // 続けてバーコード入力出来るように画面表示を継続させる。
            ViewBag.ControllAction = "InspEnd";

            return View("SerialList", models);
        }

        // POST: SerialList/Edit
        [HttpPost]
        public ActionResult Edit()
        {
            SerialEditModels models = new SerialEditModels();
            models.SelectEditSerialID = Request.QueryString["SerialID"];
            models.SetRstSerialInfo();

            return View("SerialEdit", models);
        }
    }
}