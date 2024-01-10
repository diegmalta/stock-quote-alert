using System.Text.Json;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net;
using System.Numerics;
using MailSending;

namespace stock_quote_alert{
    internal class stock_quote {
        private static async Task Main(string[] args) {
            if (args.Length != 3) {
                throw new ArgumentException("Numero de argumentos invalido! Forneca 3 argumentos: 1. ativo a ser monitorado (ex.: PETR4) - 2. preco de referencia para venda (ex.: 22.67) - 3. preco de referencia para compra (ex.: 22.59)");
            }
            string ativo = (string)args[0];
            string sellValue = (string)args[1];
            string buyValue = (string)args[2];

            EmailSender emailSender = new EmailSender("email@gmail.com", "senha", "receiver@gmail.com", ativo, sellValue, buyValue);

            var sq = new stock_quote();
            using (HttpClient client = new HttpClient()) {
                while (true) {
                    string json_data = await client.GetStringAsync(sq.buildAPIEndpoint(ativo));
                    StockQuoteResponse stockData = JsonSerializer.Deserialize<StockQuoteResponse>(json_data);
                    MailMessage message = null;
                    double previousPrice = 0;
                    double currentPrice = Double.Parse(stockData.GlobalQuote.Price);
                    if (previousPrice != currentPrice) {
                        sq.comparePrices(currentPrice, emailSender, sellValue, buyValue);
                    }
                    await Task.Delay(10000);
                }
            }
        }

        private void comparePrices(double currentPrice, EmailSender emailSender, string sellValue, string buyValue) {
            if (currentPrice > Double.Parse(sellValue)) {
                if (!emailSender.sendEmail(emailSender.messageSell)) {
                    Console.WriteLine("Error while sending email!");
                }
            }
            if (currentPrice < Double.Parse(buyValue)) {
                if (!emailSender.sendEmail(emailSender.messageBuy)) {
                    Console.WriteLine("Error while sending email!");
                }
            }
        }

        private Uri buildAPIEndpoint(string ativo) { 
            string QUERY_URL = "https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol=" + ativo + "&apikey=APIKEY";
            return new Uri(QUERY_URL);
        }

    }
}
