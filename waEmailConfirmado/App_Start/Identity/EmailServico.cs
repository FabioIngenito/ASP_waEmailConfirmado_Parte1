using Microsoft.AspNet.Identity;
using System.Configuration;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

namespace waEmailConfirmado.App_Start.Identity
{
    /// <summary>
    /// * Interface IIdentityMessageService:
    /// A integração de um serviço de envio de mensagens com o AspNet Identity foi fácil quando implementamos a 
    /// interface IIdentityMessageService - importante não apenas para o envio de email, mas também para mensagens 
    /// enviadas por outros meios, como vimos na documentação.
    /// </summary>
    public class EmailServico : IIdentityMessageService
    {
        private readonly string EmailOrigem = ConfigurationManager.AppSettings["emailServico:email_remetente"];
        private readonly string SenhaOrigem = ConfigurationManager.AppSettings["emailServico:email_senha"];

        /// <summary>
        /// Método para enviar um email. Mas... 
        /// 
        /// ... caso a carga seja absurda, tercerizar para uma empresa do ramo. Exemplos:
        /// - https://www.mailgun.com/
        /// - https://sendgrid.com/
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendAsync(IdentityMessage message)
        {
            using (MailMessage mensagemEmail = new MailMessage())
            {
                mensagemEmail.From = new MailAddress(EmailOrigem);
                mensagemEmail.Subject = message.Subject;
                mensagemEmail.To.Add(message.Destination);
                mensagemEmail.Body = message.Body;

                //SMTP - Simple Mail Transport Protocol
                //USE DEFAUT CREDENCIALS: Obtém ou define um valor booleano que controla se o DefaultCredentials é enviado com solicitações.
                //CREDENCIALS: Obtém ou define as credenciais usadas para autenticar o remetente.
                //DELIVERY METHOD: Especifica como as mensagens de e-mail de saída serão tratadas.
                //HOST: GMAIL
                //PORT: 587
                //SSL - Secure Sockets Layer: A troca entre o nosso servidor e do Google será criptografada.
                //TIME OUT: 2 Segundos.
                using (SmtpClient smtpClient = new SmtpClient())
                {
                    smtpClient.UseDefaultCredentials = true;
                    smtpClient.Credentials = new NetworkCredential(EmailOrigem, SenhaOrigem);

                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.Host = "smtp.gmail.com";
                    smtpClient.Port = 587;
                    smtpClient.EnableSsl = true;
                    smtpClient.Timeout = 20_000;

                    //ENVIAR:
                    await smtpClient.SendMailAsync(mensagemEmail);
                }
            }
        }
    }
}