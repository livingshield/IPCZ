using System;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using IpczApp.Models;

namespace IpczApp.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly PdfService _pdfService;

        public EmailService(IConfiguration configuration, PdfService pdfService)
        {
            _configuration = configuration;
            _pdfService = pdfService;
        }

        public async Task<(bool Success, string Message)> SendResultEmailAsync(string toEmail, ResultViewModel result)
        {
            try
            {
                var smtpHost = _configuration["Smtp:Host"] ?? "smtp.forpsi.com";
                var smtpPort = int.Parse(_configuration["Smtp:Port"] ?? "587");
                var smtpUser = _configuration["Smtp:Username"] ?? "";
                var smtpPass = _configuration["Smtp:Password"] ?? "";
                var fromEmail = _configuration["Smtp:FromEmail"] ?? "scio@ekobio.org";
                var fromName = _configuration["Smtp:FromName"] ?? "aivezdravotnictvi.cz - Triáž";

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, fromEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = $"Výsledky triáže AI gramotnosti ({result.Level}) - aivezdravotnictvi.cz";

                var bodyBuilder = new BodyBuilder();

                var html = new StringBuilder();
                html.AppendLine("<div style='font-family: Arial, sans-serif; color: #333; max-width: 600px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 8px; padding: 20px;'>");
                html.AppendLine("<h2 style='color: #008080; text-align: center;'>Portál aivezdravotnictvi.cz</h2>");
                html.AppendLine("<h3 style='text-align: center;'>Váš výsledný studijní plán AI gramotnosti</h3>");
                html.AppendLine("<hr style='border: none; border-top: 1px solid #eee;' />");

                html.AppendLine($"<p><strong>Role:</strong> {result.Role}</p>");
                html.AppendLine($"<p><strong>Dosáhnutá úroveň:</strong> <span style='color: #008080; font-size: 1.1em;'>{result.Level}</span></p>");
                html.AppendLine($"<p><strong>Celkové skóre:</strong> {result.OverallScore:F0} / 100 %</p>");

                html.AppendLine("<h4>Doporučené kroky pro váš rozvoj:</h4><ul>");
                foreach (var step in result.RecommendedSteps)
                {
                    html.AppendLine($"<li>{step}</li>");
                }
                html.AppendLine("</ul>");

                if (!string.IsNullOrEmpty(result.AiAnalysis))
                {
                    html.AppendLine("<div style='background-color: #f4f8f8; border-left: 4px solid #008080; padding: 12px; margin: 15px 0;'>");
                    html.AppendLine("<strong>AI Doporučení k nejslabším oblastem:</strong><br />");
                    html.AppendLine($"<p>{result.AiAnalysis}</p>");
                    html.AppendLine("</div>");
                }

                if (result.ShowMasterclass)
                {
                    html.AppendLine("<div style='background-color: #fff8e1; border: 1px solid #ffe082; padding: 12px; border-radius: 6px; margin: 15px 0;'>");
                    html.AppendLine("<strong>Exkluzivní nabídka:</strong> Jako člen managementu nemocnice vás zveme do programu <em>Masterclass (Management)</em> pro strategické řízení AI.");
                    html.AppendLine("</div>");
                }

                html.AppendLine("<p style='margin-top: 20px; font-size: 0.95em;'><strong>V příloze tohoto e-mailu naleznete oficiální PDF dokument</strong> s vašimi výsledky a studijním plánem.</p>");
                html.AppendLine("<p style='font-size: 0.9em; color: #777; margin-top: 30px; text-align: center;'>Institut postgraduálního vzdělávání ve zdravotnictví (IPVZ) & MZd</p>");
                html.AppendLine("</div>");

                bodyBuilder.HtmlBody = html.ToString();

                // Připojení vygenerovaného PDF dokumentu jako přílohy
                byte[] pdfBytes = _pdfService.GenerateTriagePdf(result);
                bodyBuilder.Attachments.Add("Vysledky_Triaze_AI_Gramotnosti.pdf", pdfBytes, new ContentType("application", "pdf"));

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtpUser, smtpPass);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return (true, $"Výsledky s PDF přílohou byly úspěšně odeslány na e-mail {toEmail}.");
            }
            catch (Exception ex)
            {
                return (false, $"Chyba při odesílání e-mailu: {ex.Message}");
            }
        }
    }
}
