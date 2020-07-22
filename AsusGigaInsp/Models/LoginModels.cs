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
        public string AuthorityKbn { get; set; }
        public IEnumerable<CombLine> DropDownListLine { get; set; }
        public string CondLineID { get; set; }

        public void SetDropDownListLine()
        {
            // ラインドロップダウンリストを取得
            DropDownList ddList = new DropDownList();
            DropDownListLine = ddList.GetDropDownListLine();
        }
    }
}
