using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TradingSystem
{
    class EmailSender
    {
        
        public static void bl_ProcessCompleted(object sender, string[] information)
        {
            Console.WriteLine("Sending an email to the back office");

            // Sender's email address and credentials
            string senderEmail = "nicolas@gmail.com"; // I should assign an email to each trader and insert the corresponding email or a dictionary with a find statement (to retreive the email) that will be connected to the trader class
            string senderPassword = "zzzzzzzzz";

            // Recipient's email address
            string recipientEmail = "nicolas@tsm.fr";

            // SMTP server details
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;

            // Create a new MailMessage
            MailMessage mailMessage = new MailMessage(senderEmail, recipientEmail);
            mailMessage.Subject = "Trade Details";
            mailMessage.Body = "The desk " + information[0] + information[1] + "of" + information[2] + "at" + information[3];

            // Set up the SMTP client
            SmtpClient smtpClient = new SmtpClient(smtpServer);
            smtpClient.Port = smtpPort;
            smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
            smtpClient.EnableSsl = true;

            try
            {
                // Send the email
                smtpClient.Send(mailMessage);
                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }
    }
}
