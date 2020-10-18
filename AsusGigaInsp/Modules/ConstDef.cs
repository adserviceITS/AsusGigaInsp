using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AsusGigaInsp.Modules
{
    // 定数は大文字とアンダースコアで定義する。
    public class ConstDef
    {
        /* DB定義変数 */
        /* ローカル開発用 */
        //public const string DB_CONNECTION_STRING = @"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;Database=SIVA";

        /* 開発機用 */
        public const string DB_DATA_SOURCE = "192.168.113.20";
        public const string DB_DATA_BASE = "SIVA";
        public const string DB_USER_ID = "sa";
        public const string DB_PASSWORD = "takota";

        /* 本番機用 */
        //public const string DB_DATA_SOURCE = "192.168.121.181";
        //public const string DB_DATA_BASE = "SIVA";
        //public const string DB_USER_ID = "sa";
        //public const string DB_PASSWORD = "takota";

        public const string DB_CONNECTION_STRING = @"Data Source=" + DB_DATA_SOURCE + ";"
                                                 + @"Database=" + DB_DATA_BASE + ";"
                                                 + @"Integrated Security=False;"
                                                 + @"User ID=" + DB_USER_ID + ";"
                                                 + @"Password=" + DB_PASSWORD;

        // 1画面に表示する行数
        public const int PAGE_ROW_SIZE = 500;

        // NG写真のパス
        public const string NG_PICT_PATH = @"../../img/photos_giga";
    }
}
