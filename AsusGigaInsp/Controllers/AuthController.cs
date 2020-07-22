using AsusGigaInsp.Models;
using AsusGigaInsp.Modules;
using System;
using System.Web.Mvc;
using System.Web.Security;

namespace AsusGigaInsp.Controllers
{
    public class AuthController : Controller
    {
        // GET: Auth/Login
        [HttpGet]
        public ActionResult Login()
        {
            LoginModels model = new LoginModels();
            model.SetDropDownListLine();
            return View(model);
        }

        // POST: Auth/Login
        // その内、認証クッキーにログイン情報があればログイン画面をすっ飛ばすようにする。
        [HttpPost]
        public ActionResult Login(LoginModels model)
        {
            // ライン選択用のドロップダウンリストをモデルにセット
            model.SetDropDownListLine();

            // 認証（担当者はIDが登録されていればOK）
            // ユーザーの存在確認
            UserInfo userInfo = new UserInfo(model.Id);
            if (string.IsNullOrEmpty(userInfo.ID))
            {
                ModelState.AddModelError(string.Empty, "ID、または Password が違います");
                return View(model);
            }

            // 管理者の場合は要パスワード
            if (userInfo.AuthorityKbn == "1")
            {
                // パスワード未入力はエラー
                if (String.IsNullOrWhiteSpace(model.Password))
                {
                    ModelState.AddModelError(string.Empty, "管理者はpasswordが必要です");
                    return View(model);
                }

                // 管理者認証
                if (userInfo.Password != model.Password)
                {
                    ModelState.AddModelError(string.Empty, "ID、または Password が違います");
                    return View(model);
                }
            }

            // ユーザー認証 成功
            // 認証クッキーにユーザーIDをセット
            FormsAuthentication.SetAuthCookie(model.Id, false);

            // ユーザーID、ユーザー名をセッションにセット
            Session["ID"] = model.Id;
            Session["UserName"] = userInfo.UserName;

            // 権限情報をモデルにセット
            model.AuthorityKbn = userInfo.AuthorityKbn;

            // 管理者はホワイトボード、管理者以外はライン選択へ
            if (userInfo.AuthorityKbn == "1")
            {
                return RedirectToAction("WhiteBoard", "WhiteBoard");
            }
            else
            {
                // ライン選択モードをオンに設定
                ViewBag.ControllAction = "LineSelect";
                return View(model);
            }
        }

        // GET: Auth/Logout
        public ActionResult Logout()
        {
            // 認証クッキーの削除
            FormsAuthentication.SignOut();
            // セッションの破棄
            Session.Abandon();

            return RedirectToAction("Login");
        }

        [HttpPost]
        public ActionResult SelectLine(LoginModels model)
        {
            // セッションにラインをセット
            Session["LineID"] = model.CondLineID;

            return RedirectToAction("Index", "InspStart");
        }
    }
}
