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
        // public const string DB_CONNECTION_STRING = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\projects\AsusGigaInsp\LocalDB\AsusGigaInsp.mdf;Integrated Security=True;Connect Timeout=30";

        /* 開発機用 */
        public const string DB_DATA_SOURCE = "192.168.113.20";
        public const string DB_DATA_BASE = "AsusGigaInsp";
        public const string DB_USER_ID = "sa";
        public const string DB_PASSWORD = "takota";

        /* 本番機用 */
        //public const string DB_DATA_SOURCE = "192.168.0.1";
        //public const string DB_USER_ID = "sa";
        //public const string DB_PASSWORD = "takota";

        public const string DB_CONNECTION_STRING = @"Data Source=" + DB_DATA_SOURCE + ";"
                                                 + @"Database=" + DB_DATA_BASE + ";"
                                                 + @"Integrated Security=False;"
                                                 + @"User ID=" + DB_USER_ID + ";"
                                                 + @"Password=" + DB_PASSWORD;

    }
}