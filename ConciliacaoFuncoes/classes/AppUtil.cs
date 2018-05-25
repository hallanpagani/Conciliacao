using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ConciliacaoFuncoes.classes
{
    public static class AppUtil
    {

        public static void EnviarEmail(string email, string nome, string subject, string body)
        {
            
            try
            {
                var fromAddress = new MailAddress("hallanpagani@gmail.com", "Hallan Pagani");
                var toAddress = new MailAddress(email, nome);
                const string fromPassword = "pagani2210123456";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new System.Net.NetworkCredential(fromAddress.Address, fromPassword),
                    Timeout = 20000
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    IsBodyHtml = true,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
            }
            catch
            {

            } 
        }
    }
}
