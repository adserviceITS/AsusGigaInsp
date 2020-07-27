using AsusGigaInsp.Modules;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace AsusGigaInsp.Models
{
    public class MasterModels
    {
    }
    public class UserListModels
    {
        public IEnumerable<CombAuthorityName> DropDownListAuthorityName { get; set; }

        public string SrchMode { get; set; }

        public string SrchUserID { get; set; }
        public string SrchUserName { get; set; }
        public string SrchEmployeeNO { get; set; }
        public string SrchMailAddress { get; set; }
        public string SrchAuthorityKBN { get; set; }
        public string SrchDelFLG { get; set; }

        public IEnumerable<SrchRstUser> SrchRstUserList { get; set; }
        private StringBuilder stbWhere = new StringBuilder();
        bool blWhereFlg = false;

        public void SetDropDownListAuthorityName()
        {
            // 権限ドロップダウンリストを取得
            DropDownList ddList = new DropDownList();
            DropDownListAuthorityName = ddList.GetDropDownListAuthorityName();
        }

        public void SetSrchRstUserList()
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            stbSql.Append("SELECT ");
            stbSql.Append("    M_USER.ID, ");
            stbSql.Append("    M_USER.USER_NAME, ");
            stbSql.Append("    M_USER.EMPLOYEE_NUMBER, ");
            stbSql.Append("    M_USER.MAIL, ");
            stbSql.Append("    TBL1.AUTHORITY_NAME, ");
            stbSql.Append("    IIF(M_USER.DEL_FLG='0','使用中','停止') AS DEL_FLG ");
            stbSql.Append("FROM ");
            stbSql.Append("    M_USER ");
            stbSql.Append("    LEFT JOIN ");
            stbSql.Append("    ( ");
            stbSql.Append("        SELECT ");
            stbSql.Append("            ID,  ");
            stbSql.Append("            AUTHORITY_NAME ");
            stbSql.Append("        FROM ");
            stbSql.Append("            M_AUTHORITY ");
            stbSql.Append("        WHERE ");
            stbSql.Append("            DEL_FLG = '0' ");
            stbSql.Append("    ) TBL1 ");
            stbSql.Append("        ON M_USER.AUTHORITY_KBN = TBL1.ID ");

            if (blWhereFlg) stbSql.Append(stbWhere);
            Debug.WriteLine(stbSql.ToString());

            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

            List<SrchRstUser> lstSrchRstUser = new List<SrchRstUser>();

            while (sqlRdr.Read())
            {
                lstSrchRstUser.Add(new SrchRstUser
                {
                    ID = sqlRdr["ID"].ToString(),
                    USER_NAME = sqlRdr["USER_NAME"].ToString(),
                    EMPLOYEE_NUMBER = sqlRdr["EMPLOYEE_NUMBER"].ToString(),
                    MAIL = sqlRdr["MAIL"].ToString(),
                    AUTHORITY_NAME = sqlRdr["AUTHORITY_NAME"].ToString(),
                    DEL_FLG = sqlRdr["DEL_FLG"].ToString()
                });
            }
            dsnLib.DB_Close();

            SrchRstUserList = lstSrchRstUser;
        }

        public void SetUserWhere()
        {
            stbWhere.Append("WHERE ");

            if (!string.IsNullOrEmpty(SrchUserID))
            {
                stbWhere.Append("M_USER.ID = '" + SrchUserID + "' ");
                blWhereFlg = true;
            }
            else
            {
                if (!string.IsNullOrEmpty(SrchUserName))
                {
                    if (blWhereFlg) stbWhere.Append("AND ");
                    else blWhereFlg = true;

                    stbWhere.Append("M_USER.USER_NAME Like N'%" + SrchUserName + "%' ");
                }

                if (!string.IsNullOrEmpty(SrchEmployeeNO))
                {
                    if (blWhereFlg) stbWhere.Append("AND ");
                    else blWhereFlg = true;

                    stbWhere.Append("M_USER.EMPLOYEE_NUMBER Like N'%" + SrchEmployeeNO + "%' ");
                }

                if (!string.IsNullOrEmpty(SrchMailAddress))
                {
                    if (blWhereFlg) stbWhere.Append("AND ");
                    else blWhereFlg = true;

                    stbWhere.Append("M_USER.MAIL Like N'%" + SrchMailAddress + "%' ");
                }

                if (!string.IsNullOrEmpty(SrchAuthorityKBN))
                {
                    if (blWhereFlg) stbWhere.Append("AND ");
                    else blWhereFlg = true;

                    stbWhere.Append("M_USER.AUTHORITY_KBN = N'" + SrchAuthorityKBN + "' ");
                }

                if (!string.IsNullOrEmpty(SrchDelFLG))
                {
                    if (blWhereFlg) stbWhere.Append("AND ");
                    else blWhereFlg = true;

                    stbWhere.Append("M_USER.DEL_FLG = N'" + SrchDelFLG + "' ");
                }
            }
        }
    }

    public class SrchRstUser
    {
        [DisplayName("ユーザーID")]
        public string ID { get; set; }
        [DisplayName("ユーザー名")]
        public string USER_NAME { get; set; }
        [DisplayName("社員番号")]
        public string EMPLOYEE_NUMBER { get; set; }
        [DisplayName("メールアドレス")]
        public string MAIL { get; set; }
        [DisplayName("権限")]
        public string AUTHORITY_NAME { get; set; }
        [DisplayName("使用中")]
        public string DEL_FLG { get; set; }
    }

    public class UserEntModels
    {
        public IEnumerable<CombAuthorityName> DropDownListAuthorityName { get; set; }

        public string EntMode { get; set; }

        [DisplayName("ユーザーID")]
        [Required(ErrorMessage = "{0}は必須です。")]
        [MinLength(3, ErrorMessage = "{0}は3文字以上で入力してください。")]
        [RegularExpression(@"[a-zA-Z0-9]+", ErrorMessage = "{0}は半角英数字で入力してください。")]
        public string EntUserID { get; set; }

        public string EntUserName { get; set; }
        public string EntEmployeeNo { get; set; }

        [Required(ErrorMessage = "権限は必須です。")]
        public string EntAuthorityKBN { get; set; }

        [EmailAddress(ErrorMessage = "正しいメールアドレスではありません。")]
        public string EntMail { get; set; }

        [DisplayName("パスワード")]
        [Required(ErrorMessage = "{0}は必須です。")]
        [MinLength(3, ErrorMessage = "{0}は3文字以上で入力してください。")]
        [RegularExpression(@"[a-zA-Z0-9 -/:-@\[-\`\{-\~]+", ErrorMessage = "{0}は半角英数字記号で入力してください。")]
        public string EntPass { get; set; }

        [DisplayName("パスワード確認")]
        [Required(ErrorMessage = "{0}は必須です。")]
        [MinLength(3, ErrorMessage = "{0}は3文字以上で入力してください。")]
        [RegularExpression(@"[a-zA-Z0-9 -/:-@\[-\`\{-\~]+", ErrorMessage = "{0}は半角英数字記号で入力してください。")]
        public string EntChkPass { get; set; }

        [Required(ErrorMessage = "使用中は必須です。")]
        public string EntDelFLG { get; set; }

        public void SetDropDownListAuthorityName()
        {
            // 権限ドロップダウンリストを取得
            DropDownList ddList = new DropDownList();
            DropDownListAuthorityName = ddList.GetDropDownListAuthorityName();
        }

        //ユーザー検索時の検索結果セット
        public void SetUserDetails(string UserID)
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            stbSql.Append("SELECT ");
            stbSql.Append("    * ");
            stbSql.Append("FROM ");
            stbSql.Append("    M_USER ");
            stbSql.Append("WHERE ");
            stbSql.Append("   ID = N'" + UserID + "' ");

            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

            while (sqlRdr.Read())
            {
                EntUserID = sqlRdr["ID"].ToString();
                EntUserName = sqlRdr["USER_NAME"].ToString();
                EntEmployeeNo = sqlRdr["EMPLOYEE_NUMBER"].ToString();
                EntMail = sqlRdr["MAIL"].ToString();
                EntAuthorityKBN = sqlRdr["AUTHORITY_KBN"].ToString();
                EntPass = sqlRdr["PASS"].ToString();
                EntChkPass = sqlRdr["PASS"].ToString();
                EntDelFLG = sqlRdr["DEL_FLG"].ToString();
            }
            dsnLib.DB_Close();
        }

        // 重複ユーザーの確認処理
        public Boolean ChkUserList()
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            Boolean blExist = false;

            stbSql.Append("SELECT ");
            stbSql.Append("    * ");
            stbSql.Append("FROM ");
            stbSql.Append("    M_USER ");
            stbSql.Append("WHERE ");
            stbSql.Append("    ID = N'" + EntUserID + "' ");

            Debug.WriteLine(stbSql.ToString());
            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

            if (sqlRdr.HasRows)
            {
                blExist = true;
            }

            dsnLib.DB_Close();

            if (blExist)
            {
                // 当該ユーザーが既に存在している（登録NG）
                return false;
            }
            else
            {
                // 当該ユーザー情報なし（登録OK）
                return true;
            }
        }

        // 既存ユーザーの更新処理
        public void UpdateUser(string strUpdUID)
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder strSql = new StringBuilder();

            strSql.Append("UPDATE M_USER ");
            strSql.Append("SET ");
            strSql.Append("    USER_NAME = N'" + EntUserName + "', ");
            strSql.Append("    EMPLOYEE_NUMBER = N'" + EntEmployeeNo + "', ");
            strSql.Append("    PASS = N'" + EntPass + "', ");
            strSql.Append("    MAIL = N'" + EntMail + "', ");
            strSql.Append("    AUTHORITY_KBN = N'" + EntAuthorityKBN + "', ");
            strSql.Append("    DEL_FLG = N'" + EntDelFLG + "', ");
            strSql.Append("    UPDATE_DATE = GETDATE(), ");
            strSql.Append("    UPDATE_ID = N'" + strUpdUID + "' ");
            strSql.Append("WHERE ");
            strSql.Append("    ID = N'" + EntUserID + "' ");

            dsnLib.ExecSQLUpdate(strSql.ToString());
            dsnLib.DB_Close();
        }

        // 新規ユーザーの登録処理
        public void AddUser(string strUpdUID)
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder strSql = new StringBuilder();

            DateTime dtNow = System.DateTime.Now;

            strSql.Append("INSERT INTO ");
            strSql.Append("    M_USER ");
            strSql.Append("VALUES ");
            strSql.Append("    (");
            strSql.Append("    N'" + EntUserID + "', ");
            strSql.Append("    N'" + EntUserName + "', ");
            strSql.Append("    N'" + EntEmployeeNo + "', ");
            strSql.Append("    null, ");
            strSql.Append("    N'" + EntPass + "', ");
            strSql.Append("    N'" + EntMail + "', ");
            strSql.Append("    N'" + EntAuthorityKBN + "', ");
            strSql.Append("    N'" + EntDelFLG + "', ");
            strSql.Append("    GETDATE(), ");
            strSql.Append("    N'" + strUpdUID + "', ");
            strSql.Append("    GETDATE(), ");
            strSql.Append("    N'" + strUpdUID + "') ");

            Debug.WriteLine(strSql.ToString());
            dsnLib.ExecSQLUpdate(strSql.ToString());
            dsnLib.DB_Close();
        }
    }
    //---------------------------------------------------------------------------//
    //                             非稼働日取込処理                              //
    //---------------------------------------------------------------------------//
    // 取込ファイル取得用クラス
    public class HolidayUploadFileModels
    {
        [Required(ErrorMessage = "ファイルを選択してください。")]
        public HttpPostedFileBase HolidayDataExcelFile { get; set; }
    }

    // 取込処理
    public class HolidayDataUpLoadModels
    {
        public void UpLoadHolidayData(string StrUpdUID, HolidayUploadFileModels UFUploadFile)
        {
            //　Excel取得用変数
            string[,] HolidayData = new string[1, 1];
            int IntRowCount = 0;
            int IntColumnCount = 0;

            //------------------------------------------------------
            // Excel取込処理
            //------------------------------------------------------

            using (var WorkBook = new XLWorkbook(UFUploadFile.HolidayDataExcelFile.InputStream))
            {
                var WorkSheet = WorkBook.Worksheet("非稼働日");

                // テーブル作成
                var Table = WorkSheet.RangeUsed().AsTable();

                //　テーブルの行数、列数を取得し、データ格納配列を定義する。
                IntRowCount = Table.RowCount();
                IntColumnCount = Table.ColumnCount();
                HolidayData = new string[IntRowCount, IntColumnCount];

                // テーブルのデータをセル毎に取得
                for (int RowCounter = 0; RowCounter < IntRowCount; RowCounter++)
                {
                    for (int ColCounter = 0; ColCounter < IntColumnCount; ColCounter++)
                    {
                        HolidayData[RowCounter, ColCounter] = Table.Row(RowCounter + 1).Cell(ColCounter + 1).Value.ToString();
                    }
                }
            }

            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder strSql = new StringBuilder();

            DateTime DTImportTime = DateTime.Now;

            //------------------------------------------------------
            // 既存のT_HOLIDAYのレコードを削除する
            //------------------------------------------------------
            strSql.Append("DELETE ");
            strSql.Append("FROM ");
            strSql.Append("    M_HOLIDAY ");

            dsnLib.ExecSQLUpdate(strSql.ToString());
            dsnLib.DB_Close();
            strSql.Clear();

            //------------------------------------------------------
            // ExcelデータをT_HOLIDAYに保存する。
            //------------------------------------------------------
            // Excelデータの2行目から順次データをDBに書き込む。
            // (1行目はタイトル行のため、読み込まない）
            DateTime DTNow = DateTime.Now;
            for (int RowCounter = 1; RowCounter < IntRowCount; RowCounter++)
            {
                strSql.Append("INSERT ");
                strSql.Append("INTO M_HOLIDAY ");
                strSql.Append("( ");
                strSql.Append("    HOLIDAY, ");
                strSql.Append("    INSERT_DATE, ");
                strSql.Append("    INSERT_ID ");
                strSql.Append(") ");
                strSql.Append("VALUES ");
                strSql.Append("( ");
                strSql.Append("    N'" + HolidayData[RowCounter, 0] + "', ");
                strSql.Append("    '" + DTNow + "', ");
                strSql.Append("    N'" + StrUpdUID + "' ");
                strSql.Append(") ");

                dsnLib.ExecSQLUpdate(strSql.ToString());

                dsnLib.DB_Close();

                strSql.Clear();
            }

            //------------------------------------------------------
            // T_SO_CHANGE_CONTROLを更新する。
            //------------------------------------------------------
            strSql.Append("UPDATE ");
            strSql.Append("    T_SO_CHANGE_CONTROL ");
            strSql.Append("SET ");
            strSql.Append("    T_SO_CHANGE_CONTROL.EST_ARRIVAL_DATE_WARNING_FLG = UDTBL.EST_ARRIVAL_DATE_WARNING_FLG, ");
            strSql.Append("    T_SO_CHANGE_CONTROL.PREF_REPORTING_DATE_WARNING_FLG = UDTBL.PREF_REPORTING_DATE_WARNING_FLG, ");
            strSql.Append("    T_SO_CHANGE_CONTROL.SI_TEK_EST_ARRIVAL_DATE_WARNING_FLG = UDTBL.SI_TEK_EST_ARRIVAL_DATE_WARNING_FLG, ");
            strSql.Append("    T_SO_CHANGE_CONTROL.UPDATE_DATE = '" + DTNow + "', ");
            strSql.Append("    T_SO_CHANGE_CONTROL.UPDATE_ID = '" + StrUpdUID + "' ");
            strSql.Append("FROM ");
            strSql.Append("    T_SO_CHANGE_CONTROL ");
            strSql.Append("    INNER JOIN ");
            strSql.Append("    ( ");
            strSql.Append("        SELECT ");
            strSql.Append("            T_SO_STATUS.SO_NO, ");
            strSql.Append("            IsNull(TBL1.EST_ARRIVAL_DATE_WARNING_FLG, '0') AS EST_ARRIVAL_DATE_WARNING_FLG, ");
            strSql.Append("            IsNull(TBL2.PREF_REPORTING_DATE_WARNING_FLG, '0')     AS PREF_REPORTING_DATE_WARNING_FLG, ");
            strSql.Append("            IsNull(TBL3.SI_TEK_EST_ARRIVAL_DATE_WARNING_FLG, '0') AS SI_TEK_EST_ARRIVAL_DATE_WARNING_FLG ");
            strSql.Append("        FROM ");
            strSql.Append("            T_SO_STATUS ");
            strSql.Append("            LEFT JOIN WK_T_SO_STATUS ");
            strSql.Append("                ON T_SO_STATUS.SO_NO = WK_T_SO_STATUS.SO_NO ");
            strSql.Append("            LEFT JOIN ");
            strSql.Append("            ( ");
            strSql.Append("                SELECT ");
            strSql.Append("                    T_SO_STATUS.SO_NO, ");
            strSql.Append("                    '1' AS EST_ARRIVAL_DATE_WARNING_FLG ");
            strSql.Append("                FROM ");
            strSql.Append("                    T_SO_STATUS ");
            strSql.Append("                    INNER JOIN M_HOLIDAY ");
            strSql.Append("                        ON T_SO_STATUS.EST_ARRIVAL_DATE = M_HOLIDAY.HOLIDAY ");
            strSql.Append("            ) TBL1 ");
            strSql.Append("                ON T_SO_STATUS.SO_NO = TBL1.SO_NO ");
            strSql.Append("            LEFT JOIN ");
            strSql.Append("            ( ");
            strSql.Append("                SELECT ");
            strSql.Append("                    T_SO_STATUS.SO_NO, ");
            strSql.Append("                    '1' AS PREF_REPORTING_DATE_WARNING_FLG ");
            strSql.Append("                FROM ");
            strSql.Append("                    T_SO_STATUS ");
            strSql.Append("                    INNER JOIN M_HOLIDAY ");
            strSql.Append("                        ON T_SO_STATUS.PREF_REPORTING_DATE = M_HOLIDAY.HOLIDAY ");
            strSql.Append("            ) TBL2 ");
            strSql.Append("                ON T_SO_STATUS.SO_NO = TBL2.SO_NO ");
            strSql.Append("            LEFT JOIN ");
            strSql.Append("            ( ");
            strSql.Append("                SELECT ");
            strSql.Append("                    T_SO_STATUS.SO_NO, ");
            strSql.Append("                    '1' AS SI_TEK_EST_ARRIVAL_DATE_WARNING_FLG ");
            strSql.Append("                FROM ");
            strSql.Append("                    T_SO_STATUS ");
            strSql.Append("                    INNER JOIN M_HOLIDAY ");
            strSql.Append("                        ON T_SO_STATUS.SI_TEK_EST_ARRIVAL_DATE = M_HOLIDAY.HOLIDAY ");
            strSql.Append("            ) TBL3 ");
            strSql.Append("                ON T_SO_STATUS.SO_NO = TBL3.SO_NO ");
            strSql.Append("    ) UDTBL ");
            strSql.Append("ON ");
            strSql.Append("    T_SO_CHANGE_CONTROL.SO_NO = UDTBL.SO_NO");

            dsnLib.ExecSQLUpdate(strSql.ToString());

            dsnLib.DB_Close();

            strSql.Clear();

        }
    }
}