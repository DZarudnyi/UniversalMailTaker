using System.Data;
using System.Data.SqlClient;

namespace UniversalMailTaker
{
    class DataBaseColumnInfo
    {
        private const string getLengthQuery =
                "SELECT CHARACTER_MAXIMUM_LENGTH " +
                "FROM INFORMATION_SCHEMA.COLUMNS " +
                "WHERE DATA_TYPE='nvarchar' " +
                "AND TABLE_NAME = @table " +
                "AND COLUMN_NAME = @column";

        private readonly string tableName;
        private readonly SqlConnection connection;

        public DataBaseColumnInfo(SqlConnection connection, string tableName)
        {
            this.connection = connection;
            this.tableName = tableName;
        }

        public int GetColumnLength(string column)
        {
            int columnLength;

            using (SqlCommand cmd = new SqlCommand(getLengthQuery, connection))
            {
                cmd.Parameters.Add("@table", SqlDbType.NVarChar).Value = tableName;
                cmd.Parameters.Add("@column", SqlDbType.NVarChar).Value = column;

                //this check might be unnecessary
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                //Перевірити що повертає int, date/time
                columnLength = (int) cmd.ExecuteScalar();
            }
            if (columnLength <= 0)
                columnLength = 4000;
            return columnLength;
        }
    }
}
