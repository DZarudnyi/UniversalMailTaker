using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.WebServices.Data;

namespace UniversalMailTaker
{
    class MailProcessor
    {
        private readonly Logger log;
        private readonly ExchangeService service;
        private readonly string[] fieldsToRetrieve;
        private readonly DataBaseWriter dbWriter;
        private readonly string attachmentSavingAddress;
        private readonly string executionLink;

        private EmailMessage message;
        private string[] valuesFromMessage;

        public MailProcessor(ExchangeService service, Logger log, string[] fieldsToRetrieve = null, DataBaseWriter dbWriter = null, string attachmentSavingAddress = null, string executionLink = null)
        {
            this.service = service;
            this.log = log;
            this.fieldsToRetrieve = fieldsToRetrieve;
            this.dbWriter = dbWriter;
            this.attachmentSavingAddress = attachmentSavingAddress;
            this.executionLink = executionLink;
            TakeEmailMessage();
        }

        public void ProcessMailBox()
        {
            int returnedID;
            while (message != null)
            {
                //TODO: need to add check for attachment?
                if (attachmentSavingAddress != null)
                    TakeAttachment();
                if (fieldsToRetrieve != null)
                {
                    ProcessMessage(fieldsToRetrieve);
                    returnedID = SaveMessageData();
                    //TODO: Come up with a way to execute link without other tasks; think of ways to execute link, because i don`t always need to add id
                    if (executionLink != null)
                        ExecuteLink(executionLink, returnedID.ToString());
                }
                
                ToNextMessage();
            }
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

        private void ToNextMessage()
        {
            //Delete prev letter and get new one
            DeleteMessage(message);
            TakeEmailMessage();
        }

        private void ProcessMessage(string[] fieldsToRetrieve)
        {
            //Parse the taken message and extract data from it
            MailParser parser = new MailParser(log);
            //TODO: check if valuesFromMessage array needs to be redeclared using "new" for correct work
            valuesFromMessage = parser.ParseMessage(message, fieldsToRetrieve);
        }

        private int SaveMessageData()
        {
            return dbWriter.WriteToDataBase(valuesFromMessage);
        }

        private void TakeAttachment()
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

        //TODO: Create LinkExecutioner class/intefrace, which will be a parent class to all the possible implementations of this method
        //Like, if the link needs an ID, or it should be simply executed, i.e. JDELinkExecute, SocarLinkExecute etc.
        private void ExecuteLink(string link, string placingValue)
        {
            var client = new WebClient();
            //TODO: check the next (commented) line, it doesn`t seems like it will format correctly; it may be nicer approach
            //var content = client.DownloadString(String.Format(link, placingValue));
            var content = client.DownloadString(link + placingValue);
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
