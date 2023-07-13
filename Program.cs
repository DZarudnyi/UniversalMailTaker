using System;

namespace UniversalMailTaker
{

    class Program
    {
        static void Main(string[] args)
        {
            Logger log = new Logger(String.Format("UniversalMailTaker_log_{0}", DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")), @"D:\logs\UniversalMailTaker");

            MailProcessor mailProcessor = new MailProcessor("login", "pas", log);
            //while (mailProcessor is)
            //maybe it`s better to pass only ID from DB to functions, while they can take the data by themselves, so i don`t need to create global parameters

            int returningID = 0;
            if () //need to take attachments
                mailProcessor.TakeAttachment();

            if ()//need to save some data from message
            {
                mailProcessor.ProcessMessage();
                if () //need to execute link
                {
                    returningID = mailProcessor.SaveMessageData();
                    mailProcessor.ExecuteLink();
                }
                mailProcessor.ToNextMessage();
            }
        }
    }
}
