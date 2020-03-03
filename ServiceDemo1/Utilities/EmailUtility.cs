using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDemo1
{
    public class EmailUtility
    {
        public static void SendEmail(string FromMail, string FromMailName, string ToMail, string CCMail, string BCCMail, string Subject, string Body)
        {
            //Create the MailMessage instance 
            MailMessage myMailMessage = new MailMessage();

            MailAddress fromAddress = new MailAddress(FromMail, FromMailName);
            myMailMessage.From = fromAddress;


            //Assign the MailMessage's properties 
            string _Subject = Subject;
            string BodyToSend = Body;

            myMailMessage.Body = BodyToSend;
            myMailMessage.IsBodyHtml = true;


            //myMailMessage.Subject = "Test Service " + _Subject;
            //myMailMessage.CC.Add(new MailAddress("welwrdany@selecteg.com"));


            myMailMessage.Subject = _Subject;

            string[] ToMuliArr = ToMail.Split(',');
            foreach (string ToEMail in ToMuliArr)
            {
                myMailMessage.To.Add(new MailAddress(ToEMail));
            }

            if (!string.IsNullOrEmpty(CCMail))
            {
                string[] CCId = CCMail.Split(',');
                foreach (string CCEmail in CCId)
                {
                    myMailMessage.CC.Add(new MailAddress(CCEmail));
                }
            }

            if (!string.IsNullOrEmpty(BCCMail))
            {
                string[] BCCs = BCCMail.Split(',');
                foreach (string BCCEmail in BCCs)
                {
                    myMailMessage.Bcc.Add(new MailAddress(BCCEmail));
                }
            }


            SmtpClient smtp = new SmtpClient
            {
                EnableSsl = true
            };

            smtp.Send(myMailMessage);
        }
    }
}
