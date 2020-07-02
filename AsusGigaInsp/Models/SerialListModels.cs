using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AsusGigaInsp.Modules;

namespace AsusGigaInsp.Models
{
    public class SerialListModels
    {
        public IEnumerable<CombInstruction> DropDownListInstruction { get; set; }
        public IEnumerable<CombLine> DropDownListLine { get; set; }

        public string CondSONo { get; set; }
        public string CondMasterCartonStartSerial { get; set; }
        public string CondMasterCartonEndSerial { get; set; }
        public string CondLineID { get; set; }
        public string CondSerialNumber { get; set; }
        public string Cond90N { get; set; }
        public string CondModelName { get; set; }
        public string CondWorkDayFrom { get; set; }
        public string CondWorkDayTo { get; set; }
        public string CondInstruction { get; set; }
        public bool CondNGFlg { get; set; }

        public string MasterCartonSerials = "";
        private string SearchWhere = "";
        public IEnumerable<SerialList> RstSerialList { get; set; }

        public string SelectEditSerialID;

        public void SetDropDownListInstruction()
        {
            // メーカードロップダウンリストを取得
            DropDownList ddList = new DropDownList();
            DropDownListInstruction = ddList.GetDropDownListInstruction();
        }

        public void SetDropDownListLine()
        {
            // ラインドロップダウンリストを取得
            DropDownList ddList = new DropDownList();
            DropDownListLine = ddList.GetDropDownListLine();
        }

        public void SetRstSerialList()
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            stbSql.Append("SELECT ");
            stbSql.Append("    TSE.ID, ");
            stbSql.Append("    TSO.SO_NO, ");
            stbSql.Append("    TSO.n90N, ");
            stbSql.Append("    TSE.MODEL_NAME, ");
            stbSql.Append("    TSE.SERIAL_NUMBER, ");
            stbSql.Append("    TSE.NG_FLG, ");
            stbSql.Append("    TSE.NG_REASON, ");
            stbSql.Append("    TSE.WORKDAY, ");
            stbSql.Append("    TSE.INSTRUCTION, ");
            stbSql.Append("    TSO.DELIVERY_LOCATION, ");
            stbSql.Append("    TSE.DESCRIPTION_ADS, ");
            stbSql.Append("    MSS.SERIAL_STATUS_NAME, ");
            stbSql.Append("    TSE.STATUS_UPDATE_DATE ");
            stbSql.Append("FROM ");
            stbSql.Append("    T_SERIAL_STATUS TSE LEFT JOIN T_SO_STATUS TSO ON ");
            stbSql.Append("    TSE.SO_NO = TSO.SO_NO ");
            stbSql.Append("    LEFT JOIN M_SERIAL_STATUS MSS ON ");
            stbSql.Append("    TSE.SERIAL_STATUS_ID = MSS.SERIAL_STATUS_ID ");
            stbSql.Append(SearchWhere);
            stbSql.Append("ORDER BY ");
            stbSql.Append("    TSE.SO_NO, ");
            stbSql.Append("    TSE.SERIAL_NUMBER ");

            //stbSql.Append(stbWhere);
            //Debug.WriteLine(stbSql.ToString());

            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());
            
            List<SerialList> lstRstSerialList = new List<SerialList>();

            while (sqlRdr.Read())
            {
                lstRstSerialList.Add(new SerialList
                {
                    SerialID = sqlRdr["ID"].ToString(),
                    SONo = sqlRdr["SO_NO"].ToString(),
                    n90N = sqlRdr["n90N"].ToString(),
                    ModelName = sqlRdr["MODEL_NAME"].ToString(),
                    SerialNumber = sqlRdr["SERIAL_NUMBER"].ToString(),
                    NGFlg = sqlRdr["NG_FLG"].ToString(),
                    NGReason = sqlRdr["NG_REASON"].ToString(),
                    WorkDay = sqlRdr["WORKDAY"].ToString(),
                    Instruction = sqlRdr["INSTRUCTION"].ToString(),
                    ShippingAddress = sqlRdr["DELIVERY_LOCATION"].ToString(),
                    DescriptionAds = sqlRdr["DESCRIPTION_ADS"].ToString(),
                    StatusName = sqlRdr["SERIAL_STATUS_NAME"].ToString(),
                    StatusUpdateDate = sqlRdr["STATUS_UPDATE_DATE"].ToString()
                });
            }
            dsnLib.DB_Close();

            RstSerialList = lstRstSerialList;
        }

        public void SetSearchWhere()
        {
            StringBuilder stbWhere = new StringBuilder();
            stbWhere.Append("WHERE TSE.DEL_FLG = '0' ");

            // マスターカートンQR
            if (!string.IsNullOrEmpty(MasterCartonSerials))
            {
                string[] SerialNOs = MasterCartonSerials.Split(',');
                stbWhere.Append(" AND TSE.SERIAL_NUMBER IN ( ");
                for (int i = 0; i < SerialNOs.Length; i++)
                {
                    if (i + 1 != SerialNOs.Length)
                    {
                        stbWhere.Append("'" + SerialNOs[i] + "', ");
                    }
                    else
                    {
                        stbWhere.Append("'" + SerialNOs[i] + "') ");
                    }
                }
            }

            SearchWhere = stbWhere.ToString();

        }

        // マスターカートンシリアルのステータス更新処理
        public void UpdateStatus(string UpdateStatusID)
        {
            string[] SerialNOs = MasterCartonSerials.Split(',');

            StringBuilder stbWhere = new StringBuilder();
            stbWhere.Append("T_SERIAL_STATUS.SERIAL_NUMBER IN ( ");

            for (int i = 0; i < SerialNOs.Length; i++)
            {
                if (i + 1 != SerialNOs.Length)
                {
                    stbWhere.Append("'" + SerialNOs[i] + "', ");
                }
                else
                {
                    stbWhere.Append("'" + SerialNOs[i] + "') ");
                }
            }

            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            string strID = HttpContext.Current.Session["ID"].ToString();

            // シリアルステータス更新
            stbSql.Append("UPDATE T_SERIAL_STATUS ");
            stbSql.Append("SET ");
            stbSql.Append("    T_SERIAL_STATUS.SERIAL_STATUS_ID = '" + UpdateStatusID + "', ");
            stbSql.Append("    T_SERIAL_STATUS.STATUS_UPDATE_DATE = GETDATE(), ");
            stbSql.Append("    T_SERIAL_STATUS.UPDATE_DATE = GETDATE(), ");
            stbSql.Append("    T_SERIAL_STATUS.UPDATE_ID = '" + strID + "' ");
            stbSql.Append("WHERE ");
            stbSql.Append(stbWhere.ToString());

            dsnLib.ExecSQLUpdate(stbSql.ToString());

            stbSql.Clear();

            stbSql.Append("UPDATE T_SO_STATUS ");
            stbSql.Append("SET ");
            stbSql.Append("    T_SO_STATUS.SO_STATUS_ID = '" + UpdateStatusID + "', ");
            stbSql.Append("    T_SO_STATUS.ST_CHANGE_DATE = GETDATE(), ");
            stbSql.Append("    T_SO_STATUS.UPDATE_DATE = GETDATE(), ");
            stbSql.Append("    T_SO_STATUS.UPDATE_ID = '" + strID + "' ");
            stbSql.Append("WHERE EXISTS ( ");
            stbSql.Append("    SELECT * FROM T_SERIAL_STATUS ");
            stbSql.Append("    WHERE T_SO_STATUS.SO_NO = T_SERIAL_STATUS.SO_NO AND ");
            stbSql.Append(stbWhere.ToString() + ") ");

            dsnLib.ExecSQLUpdate(stbSql.ToString());

            stbSql.Clear();

            stbSql.Append("INSERT INTO T_SERIAL_STATUS_HYSTORY ");
            stbSql.Append("SELECT ");
            stbSql.Append("    T_SERIAL_STATUS.ID, ");
            stbSql.Append("    T_SERIAL_STATUS.SERIAL_NUMBER, ");
            stbSql.Append("    '" + CondLineID + "', ");
            stbSql.Append("    T_SERIAL_STATUS.SO_NO, ");
            stbSql.Append("    T_SERIAL_STATUS.SERIAL_STATUS_ID, ");
            stbSql.Append("    GETDATE(), ");
            stbSql.Append("    '" + strID + "', ");
            stbSql.Append("    GETDATE(), ");
            stbSql.Append("    '" + strID + "' ");
            stbSql.Append("FROM T_SERIAL_STATUS ");
            stbSql.Append("WHERE ");
            stbSql.Append(stbWhere.ToString());

            dsnLib.ExecSQLUpdate(stbSql.ToString());

            stbSql.Clear();

            stbSql.Append("INSERT INTO T_SO_STATUS_HYSTORY ");
            stbSql.Append("SELECT ");
            stbSql.Append("    T_SO_STATUS_HYSTORY.SO_ID, ");
            stbSql.Append("    MAX(T_SO_STATUS_HYSTORY.SEQ) + 1, ");
            stbSql.Append("    T_SO_STATUS_HYSTORY.SO_NO, ");
            stbSql.Append("    MAX(T_SO_STATUS_HYSTORY.NOW_STATUS), ");
            stbSql.Append("    '" + UpdateStatusID + "', ");
            stbSql.Append("    GETDATE(), ");
            stbSql.Append("    '" + strID + "', ");
            stbSql.Append("    GETDATE(), ");
            stbSql.Append("    '" + strID + "' ");
            stbSql.Append("FROM T_SO_STATUS_HYSTORY LEFT JOIN T_SERIAL_STATUS ON ");
            stbSql.Append("     T_SO_STATUS_HYSTORY.SO_NO = T_SERIAL_STATUS.SO_NO ");
            stbSql.Append("WHERE ");
            stbSql.Append(stbWhere.ToString());
            stbSql.Append("GROUP BY  ");
            stbSql.Append("     T_SO_STATUS_HYSTORY.SO_ID,  ");
            stbSql.Append("     T_SO_STATUS_HYSTORY.SO_NO  ");

            dsnLib.ExecSQLUpdate(stbSql.ToString());

            dsnLib.DB_Close();
        }

        
    }

    public class SerialList
    {
        public string SerialID { get; set; }
        [DisplayName("SO#")]
        public string SONo { get; set; }
        [DisplayName("90N")]
        public string n90N { get; set; }
        [DisplayName("Model Name")]
        public string ModelName { get; set; }
        [DisplayName("シリアル")]
        public string SerialNumber { get; set; }
        [DisplayName("NGフラグ")]
        public string NGFlg { get; set; }
        [DisplayName("NG理由")]
        public string NGReason { get; set; }
        [DisplayName("作業日")]
        public string WorkDay { get; set; }
        [DisplayName("ASUS様指示")]
        public string Instruction { get; set; }
        [DisplayName("発送先")]
        public string ShippingAddress { get; set; }
        [DisplayName("備考")]
        public string DescriptionAds { get; set; }
        [DisplayName("ステータス")]
        public string StatusName { get; set; }
        [DisplayName("ステータス変更日")]
        public string StatusUpdateDate { get; set; }
    }
}