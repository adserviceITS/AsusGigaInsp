using AsusGigaInsp.Models;
using System.Web.Mvc;

namespace AsusGigaInsp.Controllers
{
    public class SOReportController : Controller
    {
        // GET: SOReport
        public ActionResult SOReport()
        {
            SOReportModels models = new SOReportModels();

            models.OutPutReport(this.Request.QueryString["SONO"]);

            return RedirectToAction("SOListSearch", "SOList");
        }
    }
}