using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.WebServices.Data;

namespace UniversalMailTaker
{
    class MailProcessor
    {
        private readonly Logger log;
        private readonly ExchangeService service;
        private string[] valuesFromMessage;
        private EmailMessage message;

        public MailProcessor(ExchangeService service, Logger log)
        {
            this.service = service;
            this.log = log;
            TakeEmailMessage();
        }

        //TODO: need to add some sort of check for message to have stop condition
        private void TakeEmailMessage()
        {
            SearchFilter sf = new SearchFilter.SearchFilterCollection(LogicalOperator.And, new SearchFilter.IsEqualTo(EmailMessageSchema.IsRead, false));//searching for new letters
            ItemView view = new ItemView(20);

            PropertySet psPropset;

            FindItemsResults<Item> findLetters = service.FindItems(WellKnownFolderName.Inbox, sf, view);

            foreach (Item item in findLetters.Items)
            {
                if (item is EmailMessage)
                {
                    if (item.Body.Text.Contains("</"))
                    {
                        psPropset = new PropertySet
                        {
                            RequestedBodyType = BodyType.HTML,
                            BasePropertySet = BasePropertySet.FirstClassProperties
                        };

                        message = EmailMessage.Bind(service, item.Id, psPropset);
                    }
                    else
                    {
                        psPropset = new PropertySet
                        {
                            RequestedBodyType = BodyType.Text,
                            BasePropertySet = BasePropertySet.FirstClassProperties
                        };

                        message = EmailMessage.Bind(service, item.Id, psPropset);
                    }
                    break;
                }
            }
        }

        public void ToNextMessage()
        {
            //Delete prev letter and get new one
            DeleteMessage(message);
            TakeEmailMessage();
        }

        public void ProcessMessage(string[] fieldsToRetrieve)
        {
            //Parse the taken letter and extract data from it
            MailParser parser = new MailParser(log);
            //TODO: check if valuesFromMessage array needs to be redeclared using "new" for correct work
            valuesFromMessage = parser.ParseMessage(message, fieldsToRetrieve);
        }

        public int SaveMessageData()
        {
            DataBaseWriter writer = new DataBaseWriter();
            return writer.WriteToDataBase(valuesFromMessage);
        }

        public void TakeAttachment()
        {
            //method for taking the message attachments
        }

        private List<string> DelimitByComma(string introString)
        {
            List<string> outroList = new List<string>();
            string workingString = introString;
            while (workingString.Length > 0)
            {
                if (workingString.Contains(","))
                {
                    outroList.Add(workingString.Substring(0, workingString.IndexOf(",")));
                    workingString = workingString.Remove(0, workingString.IndexOf(",") + 1).Trim();
                }
                else
                {
                    outroList.Add(workingString);
                    workingString = "";
                }
            }

            return outroList;
        }

        private string ReplaceQuotes(ref string text)
        {
            if (text == null)
                return "";
            else
                return text.Replace("'", "''");
        }

        public void ExecuteLink(string link, string placingValue)
        {
            var client = new WebClient();
            var content = client.DownloadString(String.Format(link, placingValue));
            log.WriteLine("Send mail link is executed. Result: " + content);
        }

        private void DeleteMessage(EmailMessage message)
        {
            message.IsRead = true;
            message.Update(ConflictResolutionMode.AlwaysOverwrite);
            message.Move(WellKnownFolderName.DeletedItems);
            message = null; //Is there a nicer way to clear inner field? Like message.Delete or smth
            log.WriteLine("Deleted a message.");
        }
    }
}
