using System;
using System.Data.SqlClient;
using System.Diagnostics;

namespace AsusGigaInsp.Modules
{
    public class DSNLibrary
    {
        SqlConnection connection;

        // SQL実行 Select
        public SqlDataReader ExecSQLRead(string strSql)
        {
            try
            {
                connection = new SqlConnection(ConstDef.DB_CONNECTION_STRING);
                SqlCommand command = new SqlCommand(strSql, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                return reader;
            }
            catch (SqlException ex)
            {
                DB_Close();
                Debug.WriteLine(strSql.ToString());
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        // SQL実行 INSERT UPDATE DELETE
        public void ExecSQLUpdate(string strSql)
        {
            try
            {
                connection = new SqlConnection(ConstDef.DB_CONNECTION_STRING);
                SqlCommand command = new SqlCommand(strSql, connection);
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                DB_Close();
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        // DB切断
        public void DB_Close()
        {
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }

                connection.Dispose();
                connection = null;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
