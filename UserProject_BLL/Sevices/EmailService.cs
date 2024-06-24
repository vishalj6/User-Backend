using MailKit.Net.Smtp;
using MimeKit;
using UserProject_DAL.Interfaces;

public class EmailService : IEmailService
{
    public async Task<string> SendOTPEmail(string userEmail, string firstName)
    {
        try
        {
            // Generate OTP
            Random random = new Random();
            string otp = random.Next(100000, 999999).ToString();

            // Create the email message
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Vishal Jadeja", "vishaljadeja6@gmail.com"));
            message.To.Add(new MailboxAddress(firstName, userEmail));
            message.Subject = "Your OTP Code";

            // Set email body
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"<html lang=""en"">
                    <head>
                        <meta charset=""UTF-8"" />
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
                        <title>OTP Verification</title>
                    </head>
                    <body style=""margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f7f7f7; text-align: center;"">
                        <div style=""min-height:400px;background-color: #ffffff; border-radius: 10px; box-shadow: 0px 10px 20px rgba(0, 0, 0, 0.5); border: 1px solid #00000050; padding: 0px; text-align: center; max-width: 400px; margin: auto; width: 100%;"">
                            <div style=""background-color: #0b335d; border-radius: 10px 10px 0 0; color: #ffffff; padding: 20px; font-size: 1.5rem; font-weight: bold; margin-bottom: 20px;"">OTP Verification</div>
                            <h1 style=""color: #0b335d; font-size: 1.5rem; margin-bottom: 20px;"">Hello {firstName}!</h1>
                            <div style=""font-size: 3rem; margin-bottom: 20px; animation: bounce 0.5s infinite alternate;"">&#128274;</div>
                            <p style=""color: #333; margin: 0 10px; margin-bottom: 20px;"">Your One-Time Password (OTP) is:</p>
                            <div style=""font-size: 2rem; font-weight: bold; color: #ff5733; margin-bottom: 20px;"">{otp}</div>
                            <p style=""color: #666; font-size: 0.8rem; margin-bottom: 10px;"">If you didn't request this, ignore this email.</p>
                            <p style=""margin-top: 20px; margin-bottom: 30px; color: #0b335d; font-weight: bold;"">Best regards,<br />Vishal Jadeja</p>
                        </div>
                    </body>
                </html>"
            };

            message.Body = bodyBuilder.ToMessageBody();

            // Configure SMTP client
            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync("vishaljadeja6@gmail.com", "rekd jfoe egod dxbz"); // Use your app password here

            // Send the email
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            // Log success
            Console.WriteLine($"OTP email sent successfully to {userEmail}");

            return otp;
        }
        catch (SmtpCommandException smtpEx)
        {
            Console.WriteLine($"SMTP Error: {smtpEx.Message}");
            Console.WriteLine($"Status Code: {smtpEx.StatusCode}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General Error: {ex.Message}");
            throw;
        }
    }
}
