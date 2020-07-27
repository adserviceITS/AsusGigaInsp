using AsusGigaInsp.Models;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AsusGigaInsp.Controllers
{
    public class MasterController : Controller
    {
        // GET: Master/UserSearch
        public ActionResult UserSearch()
        {
            UserListModels mdlUserList = new UserListModels();

            // 権限コンボBOXをセット
            mdlUserList.SetDropDownListAuthorityName();

            // 使用中コンボBOXをセット
            ViewBag.DropDownDelFlg = new SelectListItem[]
            {
                new SelectListItem() { Value="0", Text="使用中"},
                new SelectListItem() { Value="1", Text="停止"}
            };

            return View(mdlUserList);
        }

        // POST: Master/UserSearch
        // ユーザー検索画面/検索ボタン押下時
        [HttpPost]
        public ActionResult UserSearchResult(UserListModels mdlUserList)
        {
            // 選択された表示方法を元にWhere句を作成
            mdlUserList.SetUserWhere();

            // モデルにユーザーリストをセット
            mdlUserList.SetSrchRstUserList();

            // 権限コンボBOXをセット
            mdlUserList.SetDropDownListAuthorityName();

            // コンボBOXをセット
            ViewBag.DropDownDelFlg = new SelectListItem[]
            {
                new SelectListItem() { Value="0", Text="使用中"},
                new SelectListItem() { Value="1", Text="停止"}
            };

            return View("UserSearch", mdlUserList);
        }

        //GET: Master/UserEntry
        public ActionResult UserEntry()
        {
            UserEntModels mdlUserEnt = new UserEntModels();

            // Modeをセット
            mdlUserEnt.EntMode = this.Request.QueryString["Mode"];
            // ユーザー更新の場合、ユーザー情報を検索
            if (mdlUserEnt.EntMode == "Update")
            {
                mdlUserEnt.SetUserDetails(this.Request.QueryString["UserID"]);
            }

            // 権限コンボBOXをセット
            mdlUserEnt.SetDropDownListAuthorityName();

            // 使用中コンボBOXをセット
            ViewBag.DropDownDelFlg = new SelectListItem[]
            {
                new SelectListItem() { Value="0", Text="使用中"},
                new SelectListItem() { Value="1", Text="停止"}
            };

            // 画面表示
            return View(mdlUserEnt);
        }

        // POST: Master/UserEntry
        // ユーザー登録画面/登録ボタン押下時
        [HttpPost]
        public ActionResult UserEntryResult(UserEntModels mdlUserEnt)
        {
            // エラーがなければ処理継続
            if (ModelState.IsValid)
            {
                if (mdlUserEnt.EntPass != mdlUserEnt.EntChkPass)
                {
                    // パスワード不一致
                    this.ModelState.AddModelError("EntPass", "パスワードが一致しません。");
                    this.ModelState.AddModelError("EntChkPass", "パスワードを確認してください。");
                    return this.View("UserEntry", mdlUserEnt);
                }
                else
                {
                    if (mdlUserEnt.EntMode == "Add")
                    {
                        // 重複データチェック認証
                        if (mdlUserEnt.ChkUserList())
                        {
                            // ユーザー重複無し
                            mdlUserEnt.AddUser(Session["ID"].ToString());
                            TempData["msg"] = String.Format("ユーザーID「{0}」の登録に成功しました。", mdlUserEnt.EntUserID);
                            return RedirectToAction("UserSearch", "Master");
                        }
                        else
                        {
                            // ユーザー重複あり
                            this.ModelState.AddModelError("EntUserID", "指定されたユーザーIDは既に登録されています。");
                            return this.View("UserEntry", mdlUserEnt);
                        }
                    }
                    else
                    {
                        // M_USER 更新
                        mdlUserEnt.UpdateUser(Session["ID"].ToString());

                        TempData["msg"] = String.Format("ユーザーID「{0}」の登録に成功しました。", mdlUserEnt.EntUserID);
                        return RedirectToAction("UserSearch", "Master");
                    }
                }
            }

            // 画面表示
            return this.View("UserEntry");
        }

        // GET: HolidayDataUpLoad
        public ActionResult HolidayDataUpLoad()
        {
            HolidayUploadFileModels UploadFile = new HolidayUploadFileModels();
            return View(UploadFile);
        }

        // POST: HolidayDataUpLoad
        [HttpPost]
        public ActionResult HolidayDataUpLoad(HolidayUploadFileModels UploadFile)
        {
            HolidayDataUpLoadModels models = new HolidayDataUpLoadModels();

            if (ModelState.IsValid)
            {
                if (UploadFile.HolidayDataExcelFile.ContentLength > 0)
                {
                    if (UploadFile.HolidayDataExcelFile.FileName.EndsWith(".xlsx") || UploadFile.HolidayDataExcelFile.FileName.EndsWith(".xls"))
                    {
                        XLWorkbook Workbook;
                        try
                        {
                            Workbook = new XLWorkbook(UploadFile.HolidayDataExcelFile.InputStream);
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError(string.Empty, $"ファイルを確認してください。 {ex.Message}");
                            return View();
                        }
                        IXLWorksheet WorkSheet = null;
                        try
                        {
                            WorkSheet = Workbook.Worksheet("非稼働日");
                        }
                        catch
                        {
                            ModelState.AddModelError(string.Empty, "非稼働日sheetが存在しません。");
                            return View();
                        }

                        models.UpLoadHolidayData(Session["ID"].ToString(), UploadFile);

                        ViewBag.Message = "取り込みが完了しました。";

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
            return View();
        }
    }
}