using System;
using System.Text;
using System.Data.SqlClient;
using AsusGigaInsp.Modules;
using System.Collections.Generic;

namespace AsusGigaInsp.Models

{
    public class LoginModels
    {
        public string Id { get; set; }
        public string Password { get; set; }
        public string AuthorityKbn = "";
        public IEnumerable<CombLine> DropDownListLine { get; set; }
        public string CondLineID { get; set; }

        public Boolean Auth()
        {
            DSNLibrary dsnLib = new DSNLibrary();

            StringBuilder strSql = new StringBuilder();
            Boolean blExist = false;

            strSql.Append("SELECT ");
            strSql.Append("   ID ");
            strSql.Append("FROM dbo.M_USER ");
            strSql.Append("WHERE ID = '" + Id + "' ");
            strSql.Append("AND PASS = '" + Password + "' ");

            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(strSql.ToString());

            if (sqlRdr.HasRows)
            {
                blExist = true;
            }

            dsnLib.DB_Close();

            if (blExist)
            {
                // ユーザー認証 成功
                return true;
            }
            else
            {
                // ユーザー認証 失敗
                return false;
            }
        }

        public String GetUserName(String strId)
        {
            StringBuilder strSql = new StringBuilder();

            strSql.Append("SELECT USER_NAME ");
            strSql.Append("FROM dbo.M_USER ");
            strSql.Append("WHERE ID = '" + strId + "' ");

            DSNLibrary dsnLib = new DSNLibrary();
            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(strSql.ToString());

            string strUserName = "";

            while (sqlRdr.Read())
            {
                strUserName = sqlRdr["USER_NAME"].ToString();
            }

            dsnLib.DB_Close();

            return strUserName;

        }

        public void SetUserAuthority()
        {
            StringBuilder strSql = new StringBuilder();

            strSql.Append("SELECT AUTHORITY_KBN ");
            strSql.Append("FROM dbo.M_USER ");
            strSql.Append("WHERE ID = '" + Id + "' ");

            DSNLibrary dsnLib = new DSNLibrary();
            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(strSql.ToString());

            while (sqlRdr.Read())
            {
                AuthorityKbn = sqlRdr["AUTHORITY_KBN"].ToString();
            }

            dsnLib.DB_Close();
        }

        public void SetDropDownListLine()
        {
            // ラインドロップダウンリストを取得
            DropDownList ddList = new DropDownList();
            DropDownListLine = ddList.GetDropDownListLine();
        }
    }
}