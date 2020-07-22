using System.Data.SqlClient;
using System.Text;

namespace AsusGigaInsp.Modules
{
    public class UserInfo
    {
        public string ID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string AuthorityKbn { get; set; }

        public UserInfo (string strID)
        {
            StringBuilder stbSql = new StringBuilder();

            stbSql.Append("SELECT * ");
            stbSql.Append("FROM dbo.M_USER ");
            stbSql.Append("WHERE ID = '" + strID + "' ");

            DSNLibrary dsnLib = new DSNLibrary();
            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

            while (sqlRdr.Read())
            {
                ID = sqlRdr["ID"].ToString();
                UserName = sqlRdr["USER_NAME"].ToString();
                Password = sqlRdr["PASS"].ToString();
                AuthorityKbn = sqlRdr["AUTHORITY_KBN"].ToString();
            }

            dsnLib.DB_Close();
        }
    }
}
