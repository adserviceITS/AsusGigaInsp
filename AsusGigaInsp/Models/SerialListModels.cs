using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Web;
using AsusGigaInsp.Modules;

namespace AsusGigaInsp.Models
{
    public class SerialListModels
    {
        public IEnumerable<CombInstruction> DropDownListInstruction { get; set; }
        public IEnumerable<CombSerialStatus> DropDownListSerialStatus { get; set; }

        public string SearchSONo { get; set; }
        public string SearchSerialNumber { get; set; }
        public string Search90N { get; set; }
        public string SearchModelName { get; set; }
        public string SearchWorkDayFrom { get; set; }
        public string SearchWorkDayTo { get; set; }
        public string SearchInstruction { get; set; }
        public bool SearchNGFlg { get; set; }
        public string SearchSerialStatus { get; set; }

        private StringBuilder SearchWhere = new StringBuilder();
        public IEnumerable<SerialList> RstSerialList { get; set; }

        public string inputNGReason { get; set; }

        public string SelectSerialID { get; set; }

        public int PageNum { get; set; }
        public int SelectPage { get; set; }
        public int DataCnt { get; set; }

        public StringBuilder stbCsvData = new StringBuilder();

        public void SetDropDownListInstruction()
        {
            // メーカードロップダウンリストを取得
            DropDownList ddList = new DropDownList();
            DropDownListInstruction = ddList.GetDropDownListInstruction();
        }

        public void SetDropDownListSerialStatus()
        {
            // シリアルステータスドロップダウンリストを取得
            DropDownList ddList = new DropDownList();
            DropDownListSerialStatus = ddList.GetDropDownListSerialStatus();
        }

        public void SetPageNum()
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            stbSql.Append("SELECT ");
            stbSql.Append("    COUNT(1) AS COUNT ");
            stbSql.Append("FROM ");
            stbSql.Append("    T_SERIAL_STATUS TSE ");
            stbSql.Append(SearchWhere.ToString());

            //Debug.WriteLine(stbSql.ToString());

            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

            sqlRdr.Read();
            DataCnt = int.Parse(sqlRdr["COUNT"].ToString());

            // ページ数算出
            PageNum = DataCnt / ConstDef.PAGE_ROW_SIZE;
            // 余りがある時
            if ((DataCnt % ConstDef.PAGE_ROW_SIZE) != 0)
            {
                PageNum = PageNum + 1;
            }
            dsnLib.DB_Close();
        }

        public void SetRstSerialList()
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            // スキップする行数を算出
            int SkipRows = ConstDef.PAGE_ROW_SIZE * (SelectPage - 1);

            stbSql.Append("SELECT ");
            stbSql.Append("    TSE.ID, ");
            stbSql.Append("    TSO.SO_NO, ");
            stbSql.Append("    TSO.n90N, ");
            stbSql.Append("    TSE.MODEL_NAME, ");
            stbSql.Append("    TSE.SERIAL_NUMBER, ");
            stbSql.Append("    TSE.NG_FLG, ");
            stbSql.Append("    TPH.FILENAME, ");
            stbSql.Append("    TSE.NG_REASON, ");
            stbSql.Append("    TSE.WORKDAY, ");
            stbSql.Append("    MIN.INSTRUCTION, ");
            stbSql.Append("    TSE.SIDEWAYS_FLG, ");
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
            stbSql.Append("    LEFT JOIN T_PHOTO TPH ON ");
            stbSql.Append("    TSE.SERIAL_NUMBER = TPH.QR ");
            stbSql.Append(SearchWhere.ToString());
            stbSql.Append("ORDER BY ");
            stbSql.Append("    TSE.SO_NO, ");
            stbSql.Append("    TSE.SERIAL_NUMBER ");
            stbSql.Append("OFFSET " + SkipRows + " ROWS ");
            stbSql.Append("FETCH NEXT " + ConstDef.PAGE_ROW_SIZE + " ROWS ONLY ");

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
                    NGPictLink = ConstDef.NG_PICT_PATH + @"/" + sqlRdr["FILENAME"].ToString(),
                    NGReason = sqlRdr["NG_REASON"].ToString(),
                    WorkDay = string.IsNullOrEmpty(sqlRdr["WORKDAY"].ToString()) ? "" : sqlRdr["WORKDAY"].ToString().Substring(0, 10),
                    Instruction = sqlRdr["INSTRUCTION"].ToString(),
                    SidewaysFlg = sqlRdr["SIDEWAYS_FLG"].ToString(),
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

            if (!string.IsNullOrEmpty(SearchSerialStatus))
                //-------------- 2020/07/29 UPDATE START E.KOSHIKAWA ----------------
                // ホワイトボードから特定の項目をクリックして本画面を呼び出す際に使用
                // 0000：拠点内総仕掛から来た場合
                // 0001：完了の投入から来た場合
                // 0002：完了の新品検品作業完了から来た場合
                //SearchWhere.Append("AND TSE.SERIAL_STATUS_ID = '" + SearchSerialStatus + "' ");
                if (SearchSerialStatus == "0000")
                {
                    SearchWhere.Append("AND ((TSE.SERIAL_STATUS_ID = '2010') ");
                    SearchWhere.Append("OR  (TSE.SERIAL_STATUS_ID = '3010') ");
                    SearchWhere.Append("OR  (TSE.SERIAL_STATUS_ID = '4010') ");
                    SearchWhere.Append("OR  (TSE.SERIAL_STATUS_ID = '6010')) ");
                }
                else if (SearchSerialStatus == "0001")
                {
                    SearchWhere.Append("AND TSE.WORKDAY >= '" + DateTime.Now.ToString("yyyy/MM/dd") + "' ");
                    SearchWhere.Append("AND TSE.SERIAL_STATUS_ID >= '3010' ");
                }
                else if (SearchSerialStatus == "0002")
                {
                    SearchWhere.Append("AND TSE.WORKDAY >= '" + DateTime.Now.ToString("yyyy/MM/dd") + "' ");
                    SearchWhere.Append("AND TSE.SERIAL_STATUS_ID >= '4010' ");
                }
                else
                {
                    SearchWhere.Append("AND TSE.SERIAL_STATUS_ID = '" + SearchSerialStatus + "' ");
                }
                //----------- 2020/07/29 UPDATE  END  E.KOSHIKAWA -------------
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
            stbSql.Append("    IsNull(TSE.ID, '') + ',' + ");
            stbSql.Append("    IsNull(TSO.SO_NO, '') + ',' + ");
            stbSql.Append("    IsNull(TSO.n90N, '') + ',' +  ");
            stbSql.Append("    IsNull(TSE.MODEL_NAME, '') + ',' +  ");
            stbSql.Append("    IsNull(TSE.SERIAL_NUMBER, '') + ',' +  ");
            stbSql.Append("    CASE ");
            stbSql.Append("       WHEN TSE.NG_FLG IS NULL THEN '' ");
            stbSql.Append("       WHEN TSE.NG_FLG = '' THEN '' ");
            stbSql.Append("       WHEN TSE.NG_FLG = '0' THEN '' ");
            stbSql.Append("       WHEN TSE.NG_FLG = '1' THEN 'NG' ");
            stbSql.Append("       ELSE 'システムエラー' ");
            stbSql.Append("    END + ',' + ");
            stbSql.Append("    IsNull(TSE.NG_REASON, '') + ',' + ");
            stbSql.Append("    CASE ");
            stbSql.Append("       WHEN TSE.WORKDAY IS NULL THEN '' ");
            stbSql.Append("       WHEN TSE.WORKDAY = '' THEN '' ");
            stbSql.Append("       ELSE CONVERT(nvarchar, TSE.WORKDAY, 111) ");
            stbSql.Append("    END + ',' + ");
            stbSql.Append("    IsNull(MIN.INSTRUCTION, '') + ',' + ");
            stbSql.Append("    CASE ");
            stbSql.Append("       WHEN TSE.SIDEWAYS_FLG IS NULL THEN '' ");
            stbSql.Append("       WHEN TSE.SIDEWAYS_FLG = '' THEN '' ");
            stbSql.Append("       WHEN TSE.SIDEWAYS_FLG = '0' THEN '' ");
            stbSql.Append("       WHEN TSE.SIDEWAYS_FLG = '1' THEN '済' ");
            stbSql.Append("       ELSE 'システムエラー' ");
            stbSql.Append("    END + ',' + ");
            stbSql.Append("    IsNull(TSO.DELIVERY_LOCATION, '') + ',' +  ");
            stbSql.Append("    IsNull(TSE.DESCRIPTION_ADS, '') + ',' +  ");
            stbSql.Append("    IsNull(MSS.SERIAL_STATUS_NAME, '') + ',' +  ");
            stbSql.Append("    CONVERT(nvarchar, IsNull(TSE.STATUS_UPDATE_DATE, ''), 111) AS CSV_DATA ");
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

            // 1行目はヘッダ
            stbCsvData.Append("ID,");
            stbCsvData.Append("SO#,");
            stbCsvData.Append("90N,");
            stbCsvData.Append("ModelName,");
            stbCsvData.Append("シリアル,");
            stbCsvData.Append("NG状況,");
            stbCsvData.Append("NG理由,");
            stbCsvData.Append("作業日,");
            stbCsvData.Append("ASUS様指示,");
            stbCsvData.Append("横持ち,");
            stbCsvData.Append("発送先,");
            stbCsvData.Append("備考,");
            stbCsvData.Append("ステータス,");
            stbCsvData.Append("ステータス変更日");
            stbCsvData.Append("\r\n");

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
        [DisplayName("写真")]
        public string NGPictLink { get; set; }
        [DisplayName("NG理由")]
        public string NGReason { get; set; }
        [DisplayName("作業日")]
        public string WorkDay { get; set; }
        [DisplayName("ASUS様指示")]
        public string Instruction { get; set; }
        [DisplayName("横持ち")]
        public string SidewaysFlg { get; set; }
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
