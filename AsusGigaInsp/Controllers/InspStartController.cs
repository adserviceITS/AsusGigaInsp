using AsusGigaInsp.Models;
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
            models.UpdateStatus();
            models.SetInspStartSerialLists();

            // 続けてバーコード入力出来るように画面表示を継続させる。
            //ViewBag.ControllAction = "InspStart";

            return View("InspStart", models);
        }
    }
}