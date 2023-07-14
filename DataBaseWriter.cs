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
        private readonly string connectionServer;

        public DataBaseWriter(string destinationTable, string[] tableColumns, string connectionServer, bool getBackID = false)
        {
            this.destinationTable = destinationTable;
            this.tableColumns = tableColumns;
            this.connectionServer = connectionServer;
            this.getBackID = getBackID;
        }

        //TODO: look up how to to this in bulk copy, to write everything at once
        public int WriteToDataBase(string[] passingValues)
        {
            int returnedID = 0;

            using (SqlConnection sqlCon = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionServer].ConnectionString))
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

            using (SqlConnection sqlCon = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionServer].ConnectionString))
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

            using (SqlConnection sqlCon = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionServer].ConnectionString))
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

        //Obsolete command, it is here only for demonstration pusposes
        private int CmdExecute(string queryTable, List<string> tableColumns, List<string> passingValues, bool getBackID)
        {
            int returnedID = 0;
            using (SqlConnection sqlCon = new SqlConnection(ConfigurationManager.ConnectionStrings["connection2"].ConnectionString))
            {
                string fullCommand = "set dateformat dmy insert into [dbo].[" + queryTable + "](";
                for (int i = 0; i < tableColumns.Count; i++)
                    fullCommand += "[" + tableColumns[i] + "],";
                if (getBackID)
                    fullCommand = fullCommand.Remove(fullCommand.Length - 1, 1).Insert(fullCommand.Length - 1, ") output INSERTED.ID values(");//statement "output" cannot be used with tables that have triggers
                else
                    fullCommand = fullCommand.Remove(fullCommand.Length - 1, 1).Insert(fullCommand.Length - 1, ") values(");
                for (int i = 0; i < passingValues.Count; i++)
                    fullCommand += "'" + passingValues[i] + "',";
                fullCommand = fullCommand.Remove(fullCommand.Length - 1, 1).Insert(fullCommand.Length - 1, ")");

                using (SqlCommand cmd = new SqlCommand(fullCommand, sqlCon))
                {
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
