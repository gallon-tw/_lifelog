using System;
using System.Data;
using System.Data.SqlClient;

namespace LifeLog.Models
{
    public class MydbContext
    {
        public string conn_string = "Server=.\\SQLEXPRESS;Database=mydb;Trusted_Connection=True;";

        public DataTable find_by_sql(string sql)
        {
            SqlConnection connection = new SqlConnection(conn_string);
            SqlCommand command = new SqlCommand(sql, connection);
            DataTable dt = new DataTable();
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
              
                dt.Load(reader);
                return dt;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return dt;
            }
            finally
            {
                connection.Close();
            }
        }

        public void update_by_sql(string sql)
        {
            SqlConnection connection = new SqlConnection(conn_string);
            SqlCommand command = new SqlCommand(sql, connection);
            try
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                connection.Close();
            }
        }
    }

   
}