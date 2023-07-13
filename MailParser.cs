using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace UniversalMailTaker
{
    class MailParser
    {
        private readonly Logger log;

        public MailParser(Logger log)
        {
            this.log = log;
        }

        public string[] ParseMessage(EmailMessage message, string[] fieldsToRetrieve)
        {
            string body, sub, datetimeSent, addressFrom, addressTo;
            string[] valuesFromMessage = new string[fieldsToRetrieve.Length];

            sub = message.Subject;
            body = message.Body.Text;
            datetimeSent = message.DateTimeSent.ToString("d MMMM yyyy г. HH:mm", new CultureInfo("uk-UA"));
            log.WriteLine("Sent date: " + datetimeSent);
            addressTo = message.DisplayTo;
            log.WriteLine("Sent to: " + addressTo);

            addressFrom = message.From.Address.ToString();

            log.WriteLine("Sent from: " + addressFrom);

            ReplaceQuotes(ref addressTo);
            ReplaceQuotes(ref sub);
            ReplaceQuotes(ref body);

            for (int i = 0; i < fieldsToRetrieve.Length; i++)
            //foreach (string field in fieldsToRetrieve)
            {
                if (fieldsToRetrieve[i].ToLower().Contains("phone") || fieldsToRetrieve[i].ToLower().Contains("телефон"))
                    valuesFromMessage[i] = TakePhone(body, fieldsToRetrieve[i]);
                else if (fieldsToRetrieve[i].ToLower().Contains("name") || fieldsToRetrieve[i].ToLower().Contains("ім'я"))
                    valuesFromMessage[i] = TakeName(body, fieldsToRetrieve[i]);
                else
                    valuesFromMessage[i] = TakeLineFromMail(body, fieldsToRetrieve[i]);
            }

            if (valuesFromMessage.Length != fieldsToRetrieve.Length)
            {
                log.WriteLine(String.Format("Exception while trying to retrieve mail. Number of retrieved fields does not match number of required fields."));
                throw new Exception("Could not retrieve all the fields from mail");
            }

            return valuesFromMessage;
        }

        public string TakeLineFromMail(string mailBody, string startPoint)
        {
            try
            {
                string processedText;
                processedText = mailBody.Substring(mailBody.IndexOf(startPoint));
                if (processedText.IndexOf("\n") > 0)
                    processedText = processedText.Remove(processedText.IndexOf("\n"));
                if (processedText.IndexOf("\r") > 0)
                    processedText = processedText.Remove(processedText.IndexOf("\r"));
                if (processedText.Substring(0, 3).Contains(":"))
                    processedText = processedText.Remove(0, processedText.IndexOf(":") + 1);
                processedText.Trim();

                return processedText;
            }
            catch
            {
                return "";
            }
        }

        public string TakeLineFromMail(string mailBody, string startPoint, int lineLength)
        {
            try
            {
                string processedText;
                processedText = mailBody.Substring(mailBody.IndexOf(startPoint));
                if (processedText.IndexOf("\n") > 0)
                    processedText = processedText.Remove(processedText.IndexOf("\n"));
                if (processedText.IndexOf("\r") > 0)
                    processedText = processedText.Remove(processedText.IndexOf("\r"));
                if (processedText.Substring(0, 3).Contains(":"))
                    processedText = processedText.Remove(0, processedText.IndexOf(":") + 1);
                //if (processedText.Contains(":"))
                //    processedText = processedText.Remove(0, processedText.IndexOf(": ") + 2);
                if (processedText.Length > lineLength)
                    processedText = processedText.Substring(0, lineLength);
                processedText.Trim();

                return processedText;
            }
            catch
            {
                return "";
            }
        }

        public string TakeLineBetweenTags(string MailBody, string StartTag, string EndTag)
        {
            string Var;
            Var = MailBody.Substring(MailBody.IndexOf(StartTag) + StartTag.Length);
            Var = Var.Remove(Var.IndexOf(EndTag));
            Var = Var.Replace("\r\n", " ");
            return Var;
        }

        private string TakePhone(string MailBody, string phoneTag)
        {
            string ph = TakeLineFromMail(MailBody, phoneTag, 20); //function takes from the left side, whereas phone needs to be taken from the right side, so i take extra symbols, and then cut leftovers
            ph = (Regex.Replace(ph, @"[^\d]", "", RegexOptions.Compiled)).Right(10);

            return ph;
        }

        private string TakeName(string MailBody, string nameTag)
        {
            string pib = TakeLineFromMail(MailBody, nameTag, 250);
            pib = Regex.Replace(pib, @"^[a-zA-Z]+$", "");

            return pib;
        }

        private static string ReplaceQuotes(ref string text)
        {
            if (text == null)
                return "";
            else
                return text.Replace("'", "''");
        }
    }
}
