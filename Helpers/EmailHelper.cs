using System;
using System.Net.Mail;
using System.Text;
//using Microsoft.AspNetCore.Identity.UI.V3.Pages.Account.Manage.Internal;
using Microsoft.Extensions.Configuration;
using test_mvc_webapp.Models;

namespace test_mvc_webapp.Helper {  
    public class EmailHelper
    {
        public bool SendEmail(string userEmail, string confirmationLink)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("kevin@code-crew.org");
            mailMessage.To.Add(new MailAddress(userEmail));
 
            mailMessage.Subject = "Confirm your email";
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = confirmationLink;
 
            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential("apikey", "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");
            client.Host = "smtp.sendgrid.net";
            client.Port = 25;
 
            try
            {
                client.Send(mailMessage);
                Console.WriteLine("Sent an email to UserEmail");
                return true;
            }
            catch (Exception ex)
            {
                // log exception
                Console.WriteLine(ex.StackTrace);
            }
            return false;
        }
    }    
    // public class EmailHelper {  

    //     private string _host;  
    //     private string _from;  
    //     private string _alias;  
    //      public EmailHelper(IConfiguration iConfiguration)
    //      {
    //         var smtpSection = iConfiguration.GetSection("SMTP");
    //         if (smtpSection != null)
    //         {
    //            _host = smtpSection.GetSection("Host").Value;
    //            _from = smtpSection.GetSection("From").Value;
    //            _alias = smtpSection.GetSection("Alias").Value;
    //         }
    //      }
  
    //     public void SendEmail(EmailModel emailModel) {  
    //         try {  
    //             using(SmtpClient client = new SmtpClient(_host)) {  
    //                 MailMessage mailMessage = new MailMessage();  
    //                 mailMessage.From = new MailAddress(_from, _alias);  
    //                 mailMessage.BodyEncoding = Encoding.UTF8;  
    //                 mailMessage.To.Add(emailModel.To);  
    //                 mailMessage.Body = emailModel.Message;  
    //                 mailMessage.Subject = emailModel.Subject;  
    //                 mailMessage.IsBodyHtml = emailModel.IsBodyHtml;  
    //                 client.Send(mailMessage);  
    //             }  
    //         } catch {  
    //             throw;  
    //         }  
    //     }  
    // }  
}  