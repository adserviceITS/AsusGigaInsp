using AsusGigaInsp.Models;
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
            return View();
        }

        // POST: Auth/Login
        // その内、認証クッキーにログイン情報があればログイン画面をすっ飛ばすようにする。
        [HttpPost]
        public ActionResult Login(LoginModels model)
        {
            String strErrFlg = "0";

            // ユーザーID未入力はエラー
            if (String.IsNullOrWhiteSpace(model.Id))
            {
                ModelState.AddModelError(string.Empty, "IDを入力して下さい");
                strErrFlg = "1";
            }

            // パスワード未入力はエラー
            if (String.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError(string.Empty, "passwordを入力して下さい");
                strErrFlg = "1";
            }

            if (strErrFlg == "1")
            {
                return View(model);
            }

            // 認証
            if (model.Auth())
            {
                // ユーザー認証 成功
                this.SetUserInfo(model);
                return RedirectToAction("WhiteBord", "WhiteBord");
            } else {
                // ユーザー認証 失敗
                this.ModelState.AddModelError(string.Empty, "指定されたユーザー名またはパスワードが正しくありません。");
                return this.View(model);
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

        private void SetUserInfo(LoginModels model)
        {
            // 認証クッキーにユーザーIDをセット
            FormsAuthentication.SetAuthCookie(model.Id, false);

            // ユーザー名取得
            string strUserName = model.GetUserName(model.Id);

            // ユーザーID、ユーザー名をセッションにセット
            Session["ID"] = model.Id;
            Session["UserName"] = strUserName;
        }

    }
}