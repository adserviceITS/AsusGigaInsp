using AsusGigaInsp.Models;
using System.Web.Mvc;

namespace AsusGigaInsp.Controllers
{
    public class WhiteBoardController : Controller
    {
        // GET: WhiteBoard
        public ActionResult WhiteBoard()
        {
            return View();
        }

        // GET: ProgressBoard
        public ActionResult ProgressBoard()
        {

            ProgressBoardModels models = new ProgressBoardModels();

            // モデルにオーダーリストをセット
            models.SetSrchRstProgressBoard();

            return View("ProgressBoard", models);
        }
    }

}