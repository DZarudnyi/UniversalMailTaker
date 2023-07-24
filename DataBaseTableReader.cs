using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace UniversalMailTaker
{
    class DataBaseTableReader
    {
        //TODO: finish this query based on the final tables
        private const string getTableQuery =
                "SELECT FIELDS " +
                "FROM MY_TABLE " +
                "WHERE isActive = 1 " +
                "AND TABLE_NAME = @table " +
                "AND COLUMN_NAME = @column";

        public static void ReadTable(DataTable table)
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["con"];
            using (SqlConnection sqlCon = new SqlConnection(settings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(getTableQuery, sqlCon))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        sqlCon.Open();
                        da.Fill(table);
                    }
                }
            }
        }
    }
}
