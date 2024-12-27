using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Mail;
using MonitPro.Models.Common;
using MonitPro.Common.Library;

namespace MonitPro.Models.Common
{
    public class MonitProMailer
    {

        public void SendEmail(MonitProEmail monitProEmail)
        {

            MailMessage mail = new MailMessage();
            String username = System.Configuration.ConfigurationManager.AppSettings["UserName"];  // Replace with your SMTP username.
            String password = System.Configuration.ConfigurationManager.AppSettings["Password"];   // Replace with your SMTP password.
            String host = System.Configuration.ConfigurationManager.AppSettings["smtp"];
            int port = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Port"]);
            SmtpClient SmtpServer = new SmtpClient(host, port);
            mail.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["FromAddressIIR"]);
            mail.To.Add(monitProEmail.ToAddress);
            if (monitProEmail.CC != null && monitProEmail.CC != string.Empty)
            {
                mail.CC.Add(monitProEmail.CC);
            }
            if (monitProEmail.BCC != null && monitProEmail.BCC != string.Empty)
            {
                mail.Bcc.Add(monitProEmail.BCC);
            }

            mail.Subject = monitProEmail.Subject;
            mail.Body += " <html>";
            mail.Body += "<body>";
            mail.Body += "<table>";
            mail.Body += "<tr>";
            mail.Body += "<td> " + monitProEmail.Body + " </td>";
            mail.Body += "</tr>";        
            mail.Body += "<tr>";
            mail.Body += "<td>" + System.Configuration.ConfigurationManager.AppSettings["Link"] + "</td>";
            mail.Body += "</tr>";
            mail.Body += "<tr>";
            mail.Body += "<td> " + monitProEmail.Body1 + " </td>";   
            mail.Body += "</tr>";
            mail.Body += "</table>";
            mail.Body += "</body>";
            mail.Body += "</html>";

            mail.IsBodyHtml = true;
            SmtpServer.Credentials = new System.Net.NetworkCredential(username, password);
            SmtpServer.EnableSsl = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["EnableSsl"]);
            SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 |
            SecurityProtocolType.Tls;

            try
            {
                SmtpServer.Send(mail);
            }

            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
             
            }
        }
    }
}