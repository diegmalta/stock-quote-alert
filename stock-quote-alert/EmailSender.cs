using System;
using System.Net;
using System.Net.Mail;

namespace MailSending {
    public class EmailSender {

        public string senderEmail { get; set; }
        public string receiverEmail { get; set; }
        private string senderPassword { get; set; }

        public MailMessage messageSell { get; set; }
        public MailMessage messageBuy { get; set; }

        SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);

    public EmailSender(string senderEmail, string senderPassword, string receiverEmail, string stock, string stockSell, string stockBuy) {
            this.smtpClient.EnableSsl = true;
            this.smtpClient.UseDefaultCredentials = false;
            this.smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
            this.senderEmail = senderEmail;
            this.senderPassword = senderPassword;
            this.receiverEmail = receiverEmail;
            this.messageBuy = new MailMessage(senderEmail, receiverEmail);
            this.messageBuy.Subject = "INOA - " + stock + " stock quote is lower!";
            this.messageBuy.Body = "The stock quote is below " + stockBuy + " now.";
            this.messageSell = new MailMessage(senderEmail, receiverEmail);
            this.messageSell.Subject = "INOA - " + stock + " stock quote is higher!";
            this.messageSell.Body = "The stock quote is above " + stockSell + " now.";
        }

        public bool sendEmail(MailMessage message) {
            try {
                this.smtpClient.Send(message);
                return true;
            }
            catch (Exception e) {
                return false;
            }
        }

    }
}
