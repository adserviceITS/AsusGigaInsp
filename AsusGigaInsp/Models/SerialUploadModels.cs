using AsusGigaInsp.Modules;
using ClosedXML.Excel;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;
using System.Web;

namespace AsusGigaInsp.Models
{
    public class SerialUploadModels
    {
        public string[,] SerialList = new string[1, 1];
        public SerialUploadFile UFUploadFile;

        public void GetExcelData()
        {
            //　Excel取得用変数
            int IntRowCount = 1;
            int IntColumnCount = 1;

            //------------------------------------------------------
            // Excel取込処理
            //------------------------------------------------------

            //ファイルストリーム作成
            //FileStream Fs = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),StrPath), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            // Excelを読み取り専用で開く
            using (var WorkBook = new XLWorkbook(UFUploadFile.ExcelFile.InputStream))
            {
                var WorkSheet = WorkBook.Worksheet(1);

                // テーブル作成
                var Table = WorkSheet.RangeUsed().AsTable();

                //　テーブルの行数、列数を取得し、データ格納配列を定義する。
                IntRowCount = Table.RowCount();
                IntColumnCount = Table.ColumnCount();
                SerialList = new string[IntRowCount, IntColumnCount];

                // テーブルのデータをセル毎に取得
                for (int RowCounter = 0; RowCounter < IntRowCount; RowCounter++)
                {
                    for (int ColCounter = 0; ColCounter < IntColumnCount; ColCounter++)
                    {
                        SerialList[RowCounter, ColCounter] = Table.Row(RowCounter + 1).Cell(ColCounter + 1).Value.ToString();
                    }
                }
            }
        }

        public void InsertSerialListTBL()
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            DateTime DTImportTime = DateTime.Now;
            string StrImportTime = DTImportTime.ToString("yyyyMMddHHmmss");
            string StrUpdUID = HttpContext.Current.Session["ID"].ToString();

            //------------------------------------------------------
            // ExcelデータをT_SERIAL_LISTに保存する。
            //------------------------------------------------------

            // エクセルデータを挿入
            int IntRowCount = SerialList.GetLength(0);
            int IntColumnCount = SerialList.GetLength(1);

            for (int RowCounter = 1; RowCounter < IntRowCount; RowCounter++)
            {
                stbSql.Append("INSERT ");
                stbSql.Append("INTO T_SERIAL_LIST ");
                stbSql.Append("( ");
                stbSql.Append("    ID, ");
                stbSql.Append("    SERIAL_NUMBER, ");
                stbSql.Append("    ASUS_PART_NO, ");
                stbSql.Append("    DESCRIPTION, ");
                stbSql.Append("    SPECIFICATION, ");
                stbSql.Append("    PALLET, ");
                stbSql.Append("    CARTON, ");
                stbSql.Append("    MAC_1, ");
                stbSql.Append("    MAC_2, ");
                stbSql.Append("    MODEL_NAME, ");
                stbSql.Append("    CUSTOMER_MODEL_NAME, ");
                stbSql.Append("    NW, ");
                stbSql.Append("    GW, ");
                stbSql.Append("    EAN_CODE, ");
                stbSql.Append("    UPC_CODE, ");
                stbSql.Append("    BIOS, ");
                stbSql.Append("    IMEI, ");
                stbSql.Append("    IMEI2, ");
                stbSql.Append("    LOCK_CODE, ");
                stbSql.Append("    SHIPPING_DATE, ");
                stbSql.Append("    SO, ");
                stbSql.Append("    SO_LINE, ");
                stbSql.Append("    OSVER, ");
                stbSql.Append("    MODEMVER, ");
                stbSql.Append("    FWVER, ");
                stbSql.Append("    GPSVER, ");
                stbSql.Append("    MAPVER, ");
                stbSql.Append("    MB_MAC, ");
                stbSql.Append("    BCAS_CARD, ");
                stbSql.Append("    H3GBARCODE, ");
                stbSql.Append("    COM_SN1, ");
                stbSql.Append("    COM_SN2, ");
                stbSql.Append("    COM_SN3, ");
                stbSql.Append("    COM_MAC1, ");
                stbSql.Append("    COM_MAC2, ");
                stbSql.Append("    COM_MAC3, ");
                stbSql.Append("    INSERT_DATE, ");
                stbSql.Append("    INSERT_ID, ");
                stbSql.Append("    UPDATE_DATE, ");
                stbSql.Append("    UPDATE_ID ");
                stbSql.Append(") ");
                stbSql.Append("VALUES ");
                stbSql.Append("( ");
                stbSql.Append("    '" + StrImportTime + RowCounter.ToString().PadLeft(6, '0') + "', ");
                for (int ColCounter = 0; ColCounter < IntColumnCount; ColCounter++)
                {
                    if (!string.IsNullOrEmpty(SerialList[RowCounter, ColCounter]))
                    {
                        if (ColCounter != IntColumnCount)
                        {
                            stbSql.Append("    '" + SerialList[RowCounter, ColCounter] + "', ");
                        }
                        else
                        {
                            stbSql.Append("    '" + SerialList[RowCounter, ColCounter] + "' ");
                        }
                    }
                    else
                    {
                        stbSql.Append("    null, ");
                    }
                }
                stbSql.Append("    '" + DTImportTime + "', ");
                stbSql.Append("    '" + StrUpdUID + "', ");
                stbSql.Append("    '" + DTImportTime + "', ");
                stbSql.Append("    '" + StrUpdUID + "' ");
                stbSql.Append(") ");
                //Debug.WriteLine(stbSql.ToString());

                dsnLib.ExecSQLUpdate(stbSql.ToString());

                dsnLib.DB_Close();
                stbSql.Clear();
            }
        }

        public void InsertSerialStatusTBL()
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            DateTime DTImportTime = DateTime.Now;
            string StrImportTime = DTImportTime.ToString("yyyyMMddHHmmss");
            string StrUpdUID = HttpContext.Current.Session["ID"].ToString();

            stbSql.Append("INSERT ");
            stbSql.Append("INTO T_SERIAL_STATUS ");
            stbSql.Append("( ");
            stbSql.Append("    ID, ");
            stbSql.Append("    SERIAL_NUMBER, ");
            stbSql.Append("    ASUS_PART_NO, ");
            stbSql.Append("    DESCRIPTION, ");
            stbSql.Append("    SPECIFICATION, ");
            stbSql.Append("    PALLET, ");
            stbSql.Append("    CARTON, ");
            stbSql.Append("    MAC_1, ");
            stbSql.Append("    MAC_2, ");
            stbSql.Append("    MODEL_NAME, ");
            stbSql.Append("    CUSTOMER_MODEL_NAME, ");
            stbSql.Append("    NW, ");
            stbSql.Append("    GW, ");
            stbSql.Append("    EAN_CODE, ");
            stbSql.Append("    UPC_CODE, ");
            stbSql.Append("    BIOS, ");
            stbSql.Append("    IMEI, ");
            stbSql.Append("    IMEI2, ");
            stbSql.Append("    LOCK_CODE, ");
            stbSql.Append("    SHIPPING_DATE, ");
            stbSql.Append("    SO, ");
            stbSql.Append("    SO_LINE, ");
            stbSql.Append("    OSVER, ");
            stbSql.Append("    MODEMVER, ");
            stbSql.Append("    FWVER, ");
            stbSql.Append("    GPSVER, ");
            stbSql.Append("    MAPVER, ");
            stbSql.Append("    MB_MAC, ");
            stbSql.Append("    BCAS_CARD, ");
            stbSql.Append("    H3GBARCODE, ");
            stbSql.Append("    COM_SN1, ");
            stbSql.Append("    COM_SN2, ");
            stbSql.Append("    COM_SN3, ");
            stbSql.Append("    COM_MAC1, ");
            stbSql.Append("    COM_MAC2, ");
            stbSql.Append("    COM_MAC3, ");
            stbSql.Append("    SERIAL_STATUS_ID, ");
            stbSql.Append("    STATUS_UPDATE_DATE, ");
            stbSql.Append("    SO_NO, ");
            stbSql.Append("    NG_FLG, ");
            stbSql.Append("    NG_REASON, ");
            stbSql.Append("    WORKDAY, ");
            stbSql.Append("    INSTRUCTION, ");
            stbSql.Append("    DESCRIPTION_ADS, ");
            stbSql.Append("    DEL_FLG, ");
            stbSql.Append("    INSERT_DATE, ");
            stbSql.Append("    INSERT_ID, ");
            stbSql.Append("    UPDATE_DATE, ");
            stbSql.Append("    UPDATE_ID ");
            stbSql.Append(") ");
            stbSql.Append("SELECT ");
            stbSql.Append("    SEL.ID, ");
            stbSql.Append("    SEL.SERIAL_NUMBER, ");
            stbSql.Append("    SEL.ASUS_PART_NO, ");
            stbSql.Append("    SEL.DESCRIPTION, ");
            stbSql.Append("    SEL.SPECIFICATION, ");
            stbSql.Append("    SEL.PALLET, ");
            stbSql.Append("    SEL.CARTON, ");
            stbSql.Append("    SEL.MAC_1, ");
            stbSql.Append("    SEL.MAC_2, ");
            stbSql.Append("    SEL.MODEL_NAME, ");
            stbSql.Append("    SEL.CUSTOMER_MODEL_NAME, ");
            stbSql.Append("    SEL.NW, ");
            stbSql.Append("    SEL.GW, ");
            stbSql.Append("    SEL.EAN_CODE, ");
            stbSql.Append("    SEL.UPC_CODE, ");
            stbSql.Append("    SEL.BIOS, ");
            stbSql.Append("    SEL.IMEI, ");
            stbSql.Append("    SEL.IMEI2, ");
            stbSql.Append("    SEL.LOCK_CODE, ");
            stbSql.Append("    SEL.SHIPPING_DATE, ");
            stbSql.Append("    SEL.SO, ");
            stbSql.Append("    SEL.SO_LINE, ");
            stbSql.Append("    SEL.OSVER, ");
            stbSql.Append("    SEL.MODEMVER, ");
            stbSql.Append("    SEL.FWVER, ");
            stbSql.Append("    SEL.GPSVER, ");
            stbSql.Append("    SEL.MAPVER, ");
            stbSql.Append("    SEL.MB_MAC, ");
            stbSql.Append("    SEL.BCAS_CARD, ");
            stbSql.Append("    SEL.H3GBARCODE, ");
            stbSql.Append("    SEL.COM_SN1, ");
            stbSql.Append("    SEL.COM_SN2, ");
            stbSql.Append("    SEL.COM_SN3, ");
            stbSql.Append("    SEL.COM_MAC1, ");
            stbSql.Append("    SEL.COM_MAC2, ");
            stbSql.Append("    SEL.COM_MAC3, ");
            stbSql.Append("    '1010', ");                      // SERIAL_STATUS_ID
            stbSql.Append("    '" + DTImportTime + "', ");      // STATUS_UPDATE_DATE
            stbSql.Append("    SOL.SO_NO, ");                 // SO_NO
            stbSql.Append("    '0', ");                         // NG_FLG
            stbSql.Append("    NULL, ");                        // NG_REASON
            stbSql.Append("    NULL, ");                        // WORKDAY
            stbSql.Append("    NULL, ");                        // INSTRUCTION
            stbSql.Append("    NULL, ");                        // DESCRIPTION_ADS
            stbSql.Append("    '0', ");                         // DEL_FLG
            stbSql.Append("    '" + DTImportTime + "', ");
            stbSql.Append("    '" + StrUpdUID + "', ");
            stbSql.Append("    '" + DTImportTime + "', ");
            stbSql.Append("    '" + StrUpdUID + "' ");
            stbSql.Append("FROM ");
            stbSql.Append("    T_SERIAL_LIST SEL LEFT JOIN T_SO_STATUS SOL ON ");
            stbSql.Append("    SEL.SO = SOL.N01_NO ");
            stbSql.Append("    LEFT JOIN T_SERIAL_STATUS SES ON ");
            stbSql.Append("    SEL.ID = SES.ID ");
            stbSql.Append("WHERE ");
            stbSql.Append("    SES.ID IS NULL ");
            //Debug.WriteLine(stbSql.ToString());

            dsnLib.ExecSQLUpdate(stbSql.ToString());

            dsnLib.DB_Close();
            stbSql.Clear();
        }

        public void InsertSerialStatusHystoryTBL()
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            DateTime DTImportTime = DateTime.Now;
            string StrImportTime = DTImportTime.ToString("yyyyMMddHHmmss");
            string StrUpdUID = HttpContext.Current.Session["ID"].ToString();

            stbSql.Append("INSERT ");
            stbSql.Append("INTO T_SERIAL_STATUS_HISTORY ");
            stbSql.Append("( ");
            stbSql.Append("    SERIAL_ID, ");
            stbSql.Append("    SERIAL_NUMBER, ");
            stbSql.Append("    LINE_ID, ");
            stbSql.Append("    SO_NO, ");
            stbSql.Append("    STATUS, ");
            stbSql.Append("    INSERT_DATE, ");
            stbSql.Append("    INSERT_ID, ");
            stbSql.Append("    UPDATE_DATE, ");
            stbSql.Append("    UPDATE_ID ");
            stbSql.Append(") ");
            stbSql.Append("SELECT ");
            stbSql.Append("    SES.ID, ");
            stbSql.Append("    SES.SERIAL_NUMBER, ");
            stbSql.Append("    '', ");
            stbSql.Append("    SES.SO_NO, ");
            stbSql.Append("    '1010', ");
            stbSql.Append("    '" + DTImportTime + "', ");
            stbSql.Append("    '" + StrUpdUID + "', ");
            stbSql.Append("    '" + DTImportTime + "', ");
            stbSql.Append("    '" + StrUpdUID + "' ");
            stbSql.Append("FROM ");
            stbSql.Append("    T_SERIAL_STATUS SES LEFT JOIN T_SERIAL_STATUS_HISTORY SSH ON ");
            stbSql.Append("    SES.ID = SSH.SERIAL_ID ");
            stbSql.Append("WHERE ");
            stbSql.Append("    SSH.SERIAL_ID IS NULL ");
            //Debug.WriteLine(stbSql.ToString());

            dsnLib.ExecSQLUpdate(stbSql.ToString());

            dsnLib.DB_Close();
            stbSql.Clear();
        }
    }

    public class SerialUploadFile
    {
        [Required(ErrorMessage = "ファイルを選択してください。")]
        public HttpPostedFileBase ExcelFile { get; set; }
    }
}