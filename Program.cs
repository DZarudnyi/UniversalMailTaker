using Microsoft.Exchange.WebServices.Data;
using System;
using System.Data;

namespace UniversalMailTaker
{
    class Program
    {
        static void Main(string[] args)
        {
            //TODO: Decide whether to create log here, or make a new logger each time inside MailProcessor
            Logger log = new Logger(String.Format("UniversalMailTaker_log_{0}", DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")), @"D:\logs\UniversalMailTaker");

            ExchangeService service;
            DataBaseWriter dbWriter = null;
            string attachmentSavingAddress;
            string executionLink;


            DataTable dataTable = new DataTable();
            DataBaseTableReader.ReadTable(dataTable);
            foreach (DataRow r in dataTable.Select())
            {
                //TODO: work on table columns names
                service = Authenticator.Authenticate(r["mailUserName"].ToString(), 
                                                     r["mailPass"].ToString(), 
                                                     r["domain"].ToString(), 
                                                     r["url"].ToString());
                //we recreate array each time because of the size of the array - it will be different every time
                string[] fieldsToRetrieve = DelimitByComma(r["retrievingFields"].ToString());
                attachmentSavingAddress = r["saveAttachmentTo"].ToString();
                //TODO: check if next line assigns "" to string if empty
                executionLink = r["linkToExecute"].ToString();

                if (fieldsToRetrieve != null)
                {
                    string[] destinationColumns = DelimitByComma(r["destinationColumns"].ToString());
                    dbWriter = new DataBaseWriter(r["destinationTable"].ToString(), 
                                                  destinationColumns, 
                                                  r["connectionServer"].ToString(), 
                                                  executionLink == "");
                }
                MailProcessor mailProcessor = new MailProcessor(service, log, fieldsToRetrieve, dbWriter, attachmentSavingAddress, executionLink);
            }
        }

        //TODO: finish this method for array and check correctness
        private static string[] DelimitByComma(string introString)
        {
            string[] outroList = new string[introString.CountAllOccurances(',') + 1];
            string workingString = introString;
            int pos = 0;

            while (workingString.Length > 0)
            {
                if (workingString.Contains(","))
                {
                    outroList[pos] = workingString.Substring(0, workingString.IndexOf(","));
                    //outroList.Add(workingString.Substring(0, workingString.IndexOf(",")));
                    workingString = workingString.Remove(0, workingString.IndexOf(",") + 1).Trim();
                }
                else
                {
                    outroList[pos] = workingString;
                    //outroList.Add(workingString);
                    workingString = "";
                }
            }

            return outroList;
        }
    }
}
