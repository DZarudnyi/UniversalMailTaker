using System;
using System.Net;
using System.Threading;
using Microsoft.Exchange.WebServices.Data;

namespace UniversalMailTaker
{
    static class Authenticator
    {
        public static ExchangeService Authenticate(string userName, string pass, string domain, string serviceUrl)
        {
            ExchangeService service = new ExchangeService
            {
                UseDefaultCredentials = false,
                Credentials = new WebCredentials(userName, pass, domain),
                Url = new Uri(serviceUrl)
            };

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            return service;
        }

        private static bool RedirectionCallback(string url)
        {
            return url.ToLower().StartsWith("https://");
        }

        public static ExchangeService AuthenticateAutodiscover(string userName, string pass, string serviceUrl, string mailToDo, Logger log)
        {
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2010_SP2)
            {
                UseDefaultCredentials = false,
                Credentials = new WebCredentials(userName, pass),
                Url = new Uri(serviceUrl)
            };

            //service.TraceEnabled = true;
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    service.AutodiscoverUrl(mailToDo, RedirectionCallback);
                    break;
                }
                catch (Exception E)
                {
                    log.WriteLine("Autodiscover returned: " + E.Message);
                    Thread.Sleep(3000);
                }

                if (i == 4)
                {
                    log.WriteLine("Couldn`t connect to mail server. Autodiscover failed.");
                    throw new Exception("Couldn`t connect to mail server. Autodiscover failed.");
                }
            }

            return service;
        }
    }
}
