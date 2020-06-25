using AsusGigaInsp.Models;
using System.Web.Mvc;

namespace AsusGigaInsp.Controllers
{
    public class SOListController : Controller
    {
        // GET: SOList
        [HttpGet]
        public ActionResult SOListSearch()
        {
            // 検索条件をセット
            SOListModels models = new SOListModels();

            //models.SetSrchRstOrderList();

            // ステータスコンボBOXをセット
            models.SetDropDownListStatusName();

            return View(models);
        }

        // POST: SOList/SOListSearch/Search
        // オーダー検索画面/検索ボタン押下時
        [HttpPost]
        public ActionResult SOListSearchResult(SOListModels models)
        {
            // 選択された表示方法を元にWhere句を作成
            models.SetWhere();

            // モデルにオーダーリストをセット
            models.SetSrchRstOrderList();

            // ステータスコンボBOXをセット
            models.SetDropDownListStatusName();

            // 検索結果表示モードをセット
            models.SrchMode = "Search";

            return View("SOListSearch", models);
        }

        //GET: SOList/SOListUpdate
        public ActionResult SOListUpdate()
        {
            SOListUpdateModels mdlSOListUpdate = new SOListUpdateModels();

            mdlSOListUpdate.SetSOListDetails(this.Request.QueryString["SOID"]);

            // 画面表示
            return View(mdlSOListUpdate);
        }

        // POST: SOList/SOListUpdate
        // 取引先登録画面/登録ボタン押下時
        [HttpPost]
        public ActionResult SOListUpdateResult(SOListUpdateModels mdlSOListUpdate)
        {
            // エラーがなければ処理継続
            if (ModelState.IsValid)
            {
                bool blErrFLG = true;

                // SO#が変更されていれば重複チェック
                if (mdlSOListUpdate.EntSONO != mdlSOListUpdate.CompSONO)
                {
                    if (mdlSOListUpdate.ChkSONO()) { }
                    else
                    {
                        // SO#重複あり
                        this.ModelState.AddModelError("EntSONO", "指定されたSO#は既に登録されています。");
                        blErrFLG = false;
                    }
                }

                // N01#が変更されていれば重複チェック
                if (mdlSOListUpdate.EntN01 != mdlSOListUpdate.CompN01)
                {
                    // SO# 重複チェック
                    if (mdlSOListUpdate.ChkN01()) { }
                    else
                    {
                        // N01#重複あり
                        this.ModelState.AddModelError("EntN01", "指定されたN01#は既に登録されています。");
                        blErrFLG = false;
                    }
                }

                // 重複がなければ登録する。
                if (blErrFLG)
                {
                    // オーダー情報（T_SO_STATUS）更新
                    mdlSOListUpdate.UpdateSOList(Session["ID"].ToString());

                    // SO#が変更されていればシリアル情報（T_SERIAL_STATUS）のSO#を更新
                    if (mdlSOListUpdate.EntSONO != mdlSOListUpdate.CompSONO)
                    {
                        mdlSOListUpdate.UpdateSONO(Session["ID"].ToString());
                    }

                    // N01#が変更されていればシリアル情報（T_SERIAL_STATUS）のN01#を更新
                    if (mdlSOListUpdate.EntN01 != mdlSOListUpdate.CompN01)
                    {
                        mdlSOListUpdate.UpdateN01(Session["ID"].ToString());
                    }

                    return RedirectToAction("SOListSearch", "SOList");

                }
                else
                {
                    return this.View("SOListUpdate", mdlSOListUpdate);
                }
            }
            // 画面表示
            return this.View("SOListUpdate", mdlSOListUpdate);
        }
    }
}