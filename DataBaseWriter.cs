using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace UniversalMailTaker
{
    class DataBaseWriter
    {
        private readonly string destinationTable;
        private readonly string[] tableColumns;
        private readonly bool getBackID = false;
        private readonly string connectionTo;

        public DataBaseWriter(string destinationTable, string[] tableColumns, string connectionTo, bool getBackID = false)
        {
            this.destinationTable = destinationTable;
            this.tableColumns = tableColumns;
            this.connectionTo = connectionTo;
            this.getBackID = getBackID;
        }

        //TODO: look up how to to this in bulk copy, to write everything at once
        //in case if getBackID = false, can add everything to bulk copy and write in one piece
        public int WriteToDataBase(string[] passingValues)
        {
            int returnedID = 0;

            using (SqlConnection sqlCon = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionTo].ConnectionString))
            {
                string fullCommand = "set dateformat dmy insert into [dbo].[" + destinationTable + "](";
                string commandSecondPart;

                if (getBackID)
                    commandSecondPart = ") output INSERTED.ID values(";//statement "output" cannot be used with tables that have triggers
                else
                    commandSecondPart = ") values(";

                for (int i = 0; i < tableColumns.Length; i++)
                {
                    fullCommand += "[" + tableColumns[i] + "],";
                    commandSecondPart += "@" + i.ToString() + ",";
                }

                fullCommand = fullCommand.Remove(fullCommand.Length - 1, 1);
                commandSecondPart = commandSecondPart.Remove(commandSecondPart.Length - 1, 1);
                fullCommand += commandSecondPart;

                using (SqlCommand cmd = new SqlCommand(fullCommand, sqlCon))
                {
                    DataBaseColumnInfo dbColumnInfo = new DataBaseColumnInfo(sqlCon, destinationTable);
                    int columnLength;
                    string parameterName;

                    for (int i = 0; i < passingValues.Length; i++)
                    {
                        parameterName = "@" + i.ToString();
                        columnLength = dbColumnInfo.GetColumnLength(tableColumns[i]);
                        cmd.Parameters.Add(parameterName, SqlDbType.NVarChar, columnLength).Value = passingValues[i];
                    }

                    if (sqlCon.State != ConnectionState.Open)
                        sqlCon.Open();
                    if (getBackID)
                        returnedID = (int)cmd.ExecuteScalar();
                    else
                        cmd.ExecuteNonQuery();
                }
            }

            return returnedID;
        }

        public int WriteToDataBase(List<string> passingValues)
        {
            int returnedID = 0;

            using (SqlConnection sqlCon = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionTo].ConnectionString))
            {
                string fullCommand = "set dateformat dmy insert into [dbo].[" + destinationTable + "](";
                string commandSecondPart;

                if (getBackID)
                    commandSecondPart = ") output INSERTED.ID values(";//statement "output" cannot be used with tables that have triggers
                else
                    commandSecondPart = ") values(";

                for (int i = 0; i < tableColumns.Length; i++)
                {
                    fullCommand += "[" + tableColumns[i] + "],";
                    commandSecondPart += "@" + i.ToString() + ",";
                }

                fullCommand = fullCommand.Remove(fullCommand.Length - 1, 1);
                commandSecondPart = commandSecondPart.Remove(commandSecondPart.Length - 1, 1);
                fullCommand += commandSecondPart;

                using (SqlCommand cmd = new SqlCommand(fullCommand, sqlCon))
                {
                    DataBaseColumnInfo dbColumnInfo = new DataBaseColumnInfo(sqlCon, destinationTable);
                    int columnLength;
                    string parameterName;

                    for (int i = 0; i < passingValues.Count; i++)
                    {
                        parameterName = "@" + i.ToString();
                        columnLength = dbColumnInfo.GetColumnLength(tableColumns[i]);
                        cmd.Parameters.Add(parameterName, SqlDbType.NVarChar, columnLength).Value = passingValues[i];
                    }

                    if (sqlCon.State != ConnectionState.Open)
                        sqlCon.Open();
                    if (getBackID)
                        returnedID = (int)cmd.ExecuteScalar();
                    else
                        cmd.ExecuteNonQuery();
                }
            }

            return returnedID;
        }

        public int WriteToDataBase(Dictionary<string, string> passingValues)
        {
            int returnedID = 0;

            using (SqlConnection sqlCon = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionTo].ConnectionString))
            {
                string fullCommand = "set dateformat dmy insert into [dbo].[" + destinationTable + "](";
                string commandSecondPart;

                if (getBackID)
                    commandSecondPart = ") output INSERTED.ID values(";//statement "output" cannot be used with tables that have triggers
                else
                    commandSecondPart = ") values(";

                foreach (KeyValuePair<string, string> kvp in passingValues)
                {
                    fullCommand += "[" + kvp.Key + "],";
                    commandSecondPart += kvp.Value + ",";
                }

                fullCommand = fullCommand.Remove(fullCommand.Length - 1, 1);
                commandSecondPart = commandSecondPart.Remove(commandSecondPart.Length - 1, 1);
                fullCommand += commandSecondPart;

                using (SqlCommand cmd = new SqlCommand(fullCommand, sqlCon))
                {
                    DataBaseColumnInfo dbColumnInfo = new DataBaseColumnInfo(sqlCon, destinationTable);

                    if (sqlCon.State != ConnectionState.Open)
                        sqlCon.Open();
                    if (getBackID)
                        returnedID = (int)cmd.ExecuteScalar();
                    else
                        cmd.ExecuteNonQuery();
                }
            }

            return returnedID;
        }
    }
}
