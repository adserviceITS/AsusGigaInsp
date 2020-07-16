using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Diagnostics;
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

        public string SearchSONo { get; set; }
        public string SearchSerialNumber { get; set; }
        public string Search90N { get; set; }
        public string SearchModelName { get; set; }
        public string SearchWorkDayFrom { get; set; }
        public string SearchWorkDayTo { get; set; }
        public string SearchInstruction { get; set; }
        public bool SearchNGFlg { get; set; }

        private StringBuilder SearchWhere = new StringBuilder();
        public IEnumerable<SerialList> RstSerialList { get; set; }

        public string inputNGReason { get; set; }

        public string SelectSerialID { get; set; }

        public StringBuilder stbCsvData = new StringBuilder();

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
            stbSql.Append("    MIN.INSTRUCTION, ");
            stbSql.Append("    TSO.DELIVERY_LOCATION, ");
            stbSql.Append("    TSE.DESCRIPTION_ADS, ");
            stbSql.Append("    MSS.SERIAL_STATUS_NAME, ");
            stbSql.Append("    TSE.STATUS_UPDATE_DATE ");
            stbSql.Append("FROM ");
            stbSql.Append("    T_SERIAL_STATUS TSE LEFT JOIN T_SO_STATUS TSO ON ");
            stbSql.Append("    TSE.SO_NO = TSO.SO_NO ");
            stbSql.Append("    LEFT JOIN M_SERIAL_STATUS MSS ON ");
            stbSql.Append("    TSE.SERIAL_STATUS_ID = MSS.SERIAL_STATUS_ID ");
            stbSql.Append("    LEFT JOIN M_INSTRUCTION MIN ON ");
            stbSql.Append("    TSE.INSTRUCTION = MIN.INSTRUCTION_ID ");
            stbSql.Append(SearchWhere.ToString());
            stbSql.Append("ORDER BY ");
            stbSql.Append("    TSE.SO_NO, ");
            stbSql.Append("    TSE.SERIAL_NUMBER ");

            //stbSql.Append(stbWhere);
            Debug.WriteLine(stbSql.ToString());

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

        public void SetSearchWhere ()
        {
            SearchWhere.Append("WHERE TSE.DEL_FLG = '0' ");
            
            if (!string.IsNullOrEmpty(SearchSONo))
                SearchWhere.Append("AND TSE.SO_NO = '" + SearchSONo + "' ");

            if (!string.IsNullOrEmpty(SearchSerialNumber))
                SearchWhere.Append("AND TSE.SERIAL_NUMBER = '" + SearchSerialNumber + "' ");

            if (!string.IsNullOrEmpty(Search90N))
                SearchWhere.Append("AND TSE.SO = '" + Search90N + "' ");

            if (!string.IsNullOrEmpty(SearchModelName))
                SearchWhere.Append("AND TSE.MODEL_NAME = '" + SearchModelName + "' ");

            if (!string.IsNullOrEmpty(SearchWorkDayFrom))
                SearchWhere.Append("AND TSE.WORKDAY >= '" + SearchWorkDayFrom + "' ");

            if (!string.IsNullOrEmpty(SearchWorkDayTo))
                SearchWhere.Append("AND TSE.WORKDAY <= '" + SearchWorkDayTo + "' ");

            if (!string.IsNullOrEmpty(SearchInstruction))
                SearchWhere.Append("AND TSE.INSTRUCTION = '" + SearchInstruction + "' ");

            if (SearchNGFlg)
                SearchWhere.Append("AND TSE.NG_FLG = '1' ");
        }

        public void UpdateNgFlg ()
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            string strUID = HttpContext.Current.Session["ID"].ToString();

            // シリアルステータス更新
            stbSql.Append("UPDATE T_SERIAL_STATUS ");
            stbSql.Append("SET ");
            stbSql.Append("    T_SERIAL_STATUS.NG_FLG = '1', ");
            stbSql.Append("    T_SERIAL_STATUS.NG_REASON = '" + inputNGReason + "', ");
            stbSql.Append("    T_SERIAL_STATUS.UPDATE_DATE = GETDATE(), ");
            stbSql.Append("    T_SERIAL_STATUS.UPDATE_ID = '" + strUID + "' ");
            stbSql.Append("WHERE ");
            stbSql.Append("    T_SERIAL_STATUS.ID = '" + SelectSerialID + "' ");

            dsnLib.ExecSQLUpdate(stbSql.ToString());

            stbSql.Clear();
            dsnLib.DB_Close();
        }

        public void MakeCsvData ()
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            stbSql.Append("SELECT ");
            stbSql.Append("    TSE.ID + ',' + ");
            stbSql.Append("    TSO.SO_NO + ',' + ");
            stbSql.Append("    TSO.n90N + ',' +  ");
            stbSql.Append("    TSE.MODEL_NAME + ',' +  ");
            stbSql.Append("    TSE.SERIAL_NUMBER + ',' +  ");
            stbSql.Append("    TSE.NG_FLG + ',' +  ");
            stbSql.Append("    TSE.NG_REASON + ',' +  ");
            stbSql.Append("    CONVERT(nvarchar, TSE.WORKDAY, 111) + ',' +  ");
            stbSql.Append("    MIN.INSTRUCTION + ',' +  ");
            stbSql.Append("    TSO.DELIVERY_LOCATION + ',' +  ");
            stbSql.Append("    TSE.DESCRIPTION_ADS + ',' +  ");
            stbSql.Append("    MSS.SERIAL_STATUS_NAME + ',' +  ");
            stbSql.Append("    CONVERT(nvarchar, TSE.STATUS_UPDATE_DATE, 111) AS CSV_DATA ");
            stbSql.Append("FROM ");
            stbSql.Append("    T_SERIAL_STATUS TSE LEFT JOIN T_SO_STATUS TSO ON ");
            stbSql.Append("    TSE.SO_NO = TSO.SO_NO ");
            stbSql.Append("    LEFT JOIN M_SERIAL_STATUS MSS ON ");
            stbSql.Append("    TSE.SERIAL_STATUS_ID = MSS.SERIAL_STATUS_ID ");
            stbSql.Append("    LEFT JOIN M_INSTRUCTION MIN ON ");
            stbSql.Append("    TSE.INSTRUCTION = MIN.INSTRUCTION_ID ");
            stbSql.Append(SearchWhere.ToString());
            stbSql.Append("ORDER BY ");
            stbSql.Append("    TSE.SO_NO, ");
            stbSql.Append("    TSE.SERIAL_NUMBER ");

            //stbSql.Append(stbWhere);
            Debug.WriteLine(stbSql.ToString());

            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

            while (sqlRdr.Read())
            {
                stbCsvData.Append(sqlRdr["CSV_DATA"].ToString());
                stbCsvData.Append("\r\n");
            }
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
        [DisplayName("NG状況")]
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