using AsusGigaInsp.Models;
using ClosedXML.Excel;
using System;
using System.Web.Mvc;


namespace AsusGigaInsp.Controllers
{
    public class WhiteBoardController : Controller
    {
        // GET: WhiteBoard
        public ActionResult WhiteBoard()
        {
            WhiteBoardModels models = new WhiteBoardModels();

            models.SetSrchRstWhiteBoard();

            return View(models);
        }

        // GET: PlanDataUpLoad
        public ActionResult PlanDataUpLoad()
        {
            PlanUploadFileModels UploadFile = new PlanUploadFileModels();
            return View(UploadFile);
        }

        // POST: PlanDataUpLoad
        [HttpPost]
        public ActionResult PlanDataUpLoad(PlanUploadFileModels UploadFile)
        {
            PlanDataUpLoadModels models = new PlanDataUpLoadModels();

            if (ModelState.IsValid)
            {
                if (UploadFile.PlanDataExcelFile.ContentLength > 0)
                {
                    if (UploadFile.PlanDataExcelFile.FileName.EndsWith(".xlsx") || UploadFile.PlanDataExcelFile.FileName.EndsWith(".xls"))
                    {
                        XLWorkbook Workbook;
                        try
                        {
                            Workbook = new XLWorkbook(UploadFile.PlanDataExcelFile.InputStream);
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError(string.Empty, $"ファイルを確認してください。 {ex.Message}");
                            return View();
                        }
                        IXLWorksheet WorkSheet = null;
                        try
                        {
                            WorkSheet = Workbook.Worksheet("取り込み用");
                        }
                        catch
                        {
                            ModelState.AddModelError(string.Empty, "取り込み用sheetが存在しません。");
                            return View();
                        }

                        models.UpLoadPlanData(Session["ID"].ToString(), UploadFile);

                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "読み込めるのは、.xlsx ファイルと .xls ファイルのみです。");
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "有効なファイルではありません。");
                    return View();
                }
            }
            ViewBag.Message = "取り込みが完了しました。";
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