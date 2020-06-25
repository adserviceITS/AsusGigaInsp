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

        // POST: SerialList/WorkStart
        [HttpPost]
        public ActionResult WorkStart(SerialListModels models)
        {
            models.UpdateStatus("3010");
            models.SetSearchWhere();
            models.SetRstSerialList();
            models.SetDropDownListLine();
            models.SetDropDownListInstruction();

            return View("SerialList", models);
        }

        // POST: SerialList/WorkEnd
        [HttpPost]
        public ActionResult WorkEnd(SerialListModels models)
        {
            models.UpdateStatus("4010");
            models.SetSearchWhere();
            models.SetRstSerialList();
            models.SetDropDownListLine();
            models.SetDropDownListInstruction();

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