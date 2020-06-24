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
}