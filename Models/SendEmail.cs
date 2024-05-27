using System;
using System.Net;
using System.Net.Mail;

namespace ProjetoFinal_Myte_Grupo3.Models
{
    public static class SendEmail
    {
        public static void Send(string toEmail, string senha)
        {
            try
            {
                using (MailMessage emailMessage = new MailMessage())
                {
                    emailMessage.From = new MailAddress("mytethee@outlook.com", "Myte");
                    emailMessage.To.Add(toEmail);
                    emailMessage.Subject = "Assunto: Bem-vindo ao Nosso Site MyThee!";
                    
                    string body = @"
                        <html>
                        <head>
                            <title>Bem-vindo ao Nosso Site!</title>
                        </head>   
                        <body>
                            <h1>Olá,  " + toEmail + @"</h1>
                            <p>Seja bem-vindo ao Nosso Site MyThee! Estamos felizes em tê-lo conosco.</p>
                            <p>Aqui estão os detalhes da sua conta:</p>
                            <ul>
                                <li><strong>Nome de Usuário:</strong>"+toEmail+@"</li>
                                <li><strong>Senha:</strong> " + senha + @"</li>
                            </ul>
                            <p>Por favor, mantenha essas informações em um local seguro e não compartilhe com ninguém. Se tiver alguma dúvida ou precisar de assistência, não hesite em nos contatar.</p>
                            <p>Agradecemos por se juntar a nós e esperamos que tenha uma excelente experiência em nosso site!</p>
                            <p>Atenciosamente,MyThee<br/></p>
                        </body>
                        </html>
                    ";

                    emailMessage.Body = body;
                    emailMessage.IsBodyHtml = true;
                    emailMessage.Priority = MailPriority.Normal;

                    using (SmtpClient smtpClient = new SmtpClient("smtp-mail.outlook.com", 587))
                    {
                        smtpClient.EnableSsl = true;
                        smtpClient.Timeout = 60 * 100;
                        smtpClient.UseDefaultCredentials = false;

                        string appPassword = "Euteamo345*"; 
                        smtpClient.Credentials = new NetworkCredential("mytethee@outlook.com", appPassword);

                        smtpClient.Send(emailMessage);
                        Console.WriteLine("Email enviado com sucesso para " + toEmail);
                    }
                }
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine("Erro SMTP: " + smtpEx.Message);
                if (smtpEx.InnerException != null)
                {
                    Console.WriteLine("Inner Exception: " + smtpEx.InnerException.Message);
                }
            }
            catch (Exception ex)
            {
                //Log de erro geral
                Console.WriteLine("Ocorreu um erro ao enviar o email: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
                }
            }
        }
    }
}