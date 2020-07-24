using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace AsusGigaInsp.Modules
{
    public class DropDownList
    {
        // ASUS様指示
        public List<CombInstruction> GetDropDownListInstruction()
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            stbSql.Append("SELECT ");
            stbSql.Append("    M_INSTRUCTION.INSTRUCTION_ID, ");
            stbSql.Append("    M_INSTRUCTION.INSTRUCTION ");
            stbSql.Append("FROM ");
            stbSql.Append("    M_INSTRUCTION ");
            stbSql.Append("ORDER BY ");
            stbSql.Append("    M_INSTRUCTION.INSTRUCTION_ID ");

            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

            List<CombInstruction> DropDownListInstruction = new List<CombInstruction>();

            while (sqlRdr.Read())
            {
                DropDownListInstruction.Add(new CombInstruction
                {
                    InstructionID = sqlRdr["INSTRUCTION_ID"].ToString(),
                    Instruction = sqlRdr["INSTRUCTION"].ToString()
                });
            }
            sqlRdr.Close();
            dsnLib.DB_Close();

            return DropDownListInstruction;
        }

        // LINE
        public List<CombLine> GetDropDownListLine()
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            stbSql.Append("SELECT ");
            stbSql.Append("    M_LINE.LINE_ID, ");
            stbSql.Append("    M_LINE.LINE_NAME ");
            stbSql.Append("FROM ");
            stbSql.Append("    M_LINE ");
            stbSql.Append("ORDER BY ");
            stbSql.Append("    M_LINE.LINE_ID ");

            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

            List<CombLine> DropDownListLine = new List<CombLine>();

            while (sqlRdr.Read())
            {
                DropDownListLine.Add(new CombLine
                {
                    LineID = sqlRdr["LINE_ID"].ToString(),
                    LineName = sqlRdr["LINE_NAME"].ToString()
                });
            }
            sqlRdr.Close();
            dsnLib.DB_Close();

            return DropDownListLine;
        }

        // シリアルステータス
        public List<CombSerialStatus> GetDropDownListSerialStatus()
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            stbSql.Append("SELECT ");
            stbSql.Append("    M_SERIAL_STATUS.SERIAL_STATUS_ID, ");
            stbSql.Append("    M_SERIAL_STATUS.SERIAL_STATUS_NAME ");
            stbSql.Append("FROM ");
            stbSql.Append("    M_SERIAL_STATUS ");
            stbSql.Append("ORDER BY ");
            stbSql.Append("    M_SERIAL_STATUS.SERIAL_STATUS_ID ");

            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

            List<CombSerialStatus> DropDownListSerialStatus = new List<CombSerialStatus>();

            while (sqlRdr.Read())
            {
                DropDownListSerialStatus.Add(new CombSerialStatus
                {
                    SerialStatusID = sqlRdr["SERIAL_STATUS_ID"].ToString(),
                    SerialStatusName = sqlRdr["SERIAL_STATUS_NAME"].ToString()
                });
            }
            sqlRdr.Close();
            dsnLib.DB_Close();

            return DropDownListSerialStatus;
        }

        // 権限
        public List<CombAuthorityName> GetDropDownListAuthorityName()
        {
            DSNLibrary dsnLib = new DSNLibrary();
            StringBuilder stbSql = new StringBuilder();

            stbSql.Append("SELECT ");
            stbSql.Append("    ID, ");
            stbSql.Append("    AUTHORITY_NAME ");
            stbSql.Append("FROM ");
            stbSql.Append("    M_AUTHORITY ");
            stbSql.Append("WHERE ");
            stbSql.Append("    DEL_FLG = '0' ");
            stbSql.Append("ORDER BY ");
            stbSql.Append("    ID ");

            SqlDataReader sqlRdr = dsnLib.ExecSQLRead(stbSql.ToString());

            List<CombAuthorityName> DropDownAuthorityName = new List<CombAuthorityName>();

            while (sqlRdr.Read())
            {
                DropDownAuthorityName.Add(new CombAuthorityName
                {
                    ID = sqlRdr["ID"].ToString(),
                    AuthorityName = sqlRdr["AUTHORITY_NAME"].ToString()
                });
            }
            dsnLib.DB_Close();

            return DropDownAuthorityName;
        }

    }

    public class CombInstruction
    {
        public string InstructionID { get; set; }
        public string Instruction { get; set; }
    }

    public class CombLine
    {
        public string LineID { get; set; }
        public string LineName { get; set; }
    }

    public class CombSerialStatus
    {
        public string SerialStatusID { get; set; }
        public string SerialStatusName { get; set; }
    }

    public class CombAuthorityName
    {
        public string ID { get; set; }
        public string AuthorityName { get; set; }
    }

}
