using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using MailKit.Net.Smtp;

using System.Text;
using System.Threading.Tasks;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using StockControlApi.Application.Services.Interfaces;

namespace StockControlApi.Application.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendPasswordResetEmailAsync(string email,string id)
        {
           
                var message = new MimeMessage();
               

                message.From.Add(new MailboxAddress("teste", "teste"));
                message.To.Add(new MailboxAddress(email, email));
                message.Subject = "Redefinição de Senha";

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = $@"
            <h2>Redefinição de Senha</h2>
            <p>Recebemos uma solicitação para redefinir a senha da sua conta.</p>
            <p>Para redefinir sua senha, clique no link abaixo:</p>
            <a href='http://localhost:3000/newpassword?key={id}'>Redefinir Senha</a>
            <p>Se você não fez essa solicitação, ignore este email.</p>";

                message.Body = bodyBuilder.ToMessageBody();

           
            using (var client = new SmtpClient())
            {
                client.Connect("testes", 587, MailKit.Security.SecureSocketOptions.None);

            
                client.Authenticate("testes", "testse");
                client.Send(message);
                client.Disconnect(true);
            }


        }
    }
}
