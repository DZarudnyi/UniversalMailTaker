using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalMailTaker
{
    class DataBaseTableReader
    {
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
