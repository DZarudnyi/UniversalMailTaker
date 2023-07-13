using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalMailTaker
{
    class JDEMailTaker : MailTaker
    {
        private protected override ExchangeService getService()
        {
            return Authenticator.AuthenticateAdelina(ConfigurationManager.AppSettings["JDElogin"], ConfigurationManager.AppSettings["JDEpas"]);
        }

        public override void GetMail()
        {
            f
        }
    }
}
