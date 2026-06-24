using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace MesaPartesDigital.Services
{
    public interface IEmailService
    {
        Task<bool> EnviarCodigoOtpAsync(string correoDestino, string codigoOtp);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> EnviarCodigoOtpAsync(string correoDestino, string codigoOtp)
        {
            try
            {
                // 1. Obtener la configuración del appsettings.json
                var server = _configuration["SmtpSettings:Server"];
                var port = int.Parse(_configuration["SmtpSettings:Port"] ?? "587");
                var senderName = _configuration["SmtpSettings:SenderName"];
                var senderEmail = _configuration["SmtpSettings:SenderEmail"];
                var password = _configuration["SmtpSettings:Password"];

                // 2. Estructurar el mensaje de correo
                var mensaje = new MimeMessage();
                mensaje.From.Add(new MailboxAddress(senderName, senderEmail));
                mensaje.To.Add(new MailboxAddress("", correoDestino));
                mensaje.Subject = "Código de Verificación - Mesa de Partes Digital";

                // Diseño del cuerpo en HTML
                var bodyBuilder = new BodyBuilder
                {
                    //HtmlBody = $@"
                    //<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 8px;'>
                    //    <h2 style='color: #1a73e8; text-align: center;'>Mesa de Partes Digital</h2>
                    //    <p>Estimado(a) usuario,</p>
                    //    <p>Has solicitado un código de verificación para registrar tu documento en nuestra plataforma. Utiliza el siguiente código para continuar con tu trámite:</p>
                    //    <div style='background-color: #f1f3f4; padding: 15px; text-align: center; font-size: 24px; font-weight: bold; letter-spacing: 4px; color: #202124; border-radius: 4px; margin: 20px 0;'>
                    //        {codigoOtp}
                    //    </div>
                    //    <p style='font-size: 0.85rem; color: #5f6368;'>Este código es de un solo uso. Si tú no solicitaste este código, por favor ignora este mensaje.</p>
                    //    <hr style='border: none; border-top: 1px solid #e0e0e0; margin: 20px 0;' />
                    //    <p style='font-size: 0.8rem; color: #9aa0a6; text-align: center;'>Por favor no responder a este correo automático.</p>
                    //</div>"
                    HtmlBody = $@"
<div style='font-family: Arial, sans-serif; max-width: 600px; width: 100%; margin: 0 auto; padding: 0; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.05); box-sizing: border-box;'>
    <!-- Encabezado Institucional con Logo -->
    <div style='background-color: #35af72; padding: 25px 15px; text-align: center;'>
        <img src='https://www.gob.pe/rails/active_storage/representations/redirect/eyJfcmFpbHMiOnsiZGF0YSI6NDk2ODQ0LCJwdXIiOiJibG9iX2lkIn19--1fc9a7807cf6c726e857b951ca1a374a8414a140/eyJfcmFpbHMiOnsiZGF0YSI6eyJmb3JtYXQiOiJwbmciLCJyZXNpemVfdG9fbGltaXQiOltudWxsLDQ4XX0sInB1ciI6InZhcmlhdGlvbiJ9fQ==--830247c4bafe7cadca50817d8559bf1a09e3aa28/paga%20gob.pe.png' 
             alt='Logo MIDAGRI - AGRO RURAL' 
             style='max-width: 100%; height: auto; display: inline-block; min-height: 40px;' />
        <p style='color: #a3e2c1; margin: 10px 0 0 0; font-size: 12px; text-transform: uppercase; font-weight: bold; letter-spacing: 1px;'>Mesa de Partes Digital</p>
    </div>

    <!-- Contenido Principal -->
    <div style='padding: 5%; background-color: #ffffff; box-sizing: border-box;'>
        <p style='color: #333333; font-size: 15px; line-height: 1.5; margin-top: 0;'>Estimado(a) administrado(a),</p>
        
        <p style='color: #555555; font-size: 14px; line-height: 1.6;'>
            Se ha solicitado un código de verificación para continuar con el registro de su documento en la plataforma de la <strong>Mesa de Partes Digital</strong>.
        </p>

        <!-- Bloque del Código OTP Responsivo -->
        <div style='background-color: #f4f9f5; border: 1px dashed #006432; padding: 20px 10px; text-align: center; border-radius: 6px; margin: 25px 0;'>
            <span style='display: block; font-size: 11px; color: #555555; text-transform: uppercase; margin-bottom: 8px; font-weight: bold; letter-spacing: 1px;'>Código de Verificación</span>
            <div style='font-size: 30px; font-weight: bold; letter-spacing: 4px; color: #006432; word-break: break-all;'>
                {codigoOtp}
            </div>
        </div>

        <!-- Alerta de seguridad -->
        <div style='font-size: 12px; color: #777777; line-height: 1.4; background-color: #fff8e7; padding: 12px; border-left: 4px solid #f39c12; border-radius: 4px; box-sizing: border-box;'>
            <strong>Importante:</strong> Este código es de un solo uso. Si usted no solicitó este requerimiento, por favor ignore este mensaje.
        </div>

        <hr style='border: none; border-top: 1px solid #eeeeee; margin: 30px 0;' />

        <!-- Pie de página -->
        <p style='font-size: 11px; color: #999999; text-align: center; line-height: 1.5; margin: 0;'>
            Programa de Desarrollo Productivo Agrario Rural - AGRO RURAL<br>
            <span style='color: #ba2525; font-weight: bold; display: block; margin-top: 5px;'>Por favor, no responda a este correo automático.</span>
        </p>
    </div>
</div>"
                };
                mensaje.Body = bodyBuilder.ToMessageBody();

                // 3. Conexión y envío vía SMTP con MailKit
                using var client = new SmtpClient();

                // Usamos SecureSocketOptions.StartTls porque el puerto 587 requiere iniciar sin SSL y luego elevar a TLS
                await client.ConnectAsync(server, port, SecureSocketOptions.StartTls);

                // Autenticación
                await client.AuthenticateAsync(senderEmail, password);

                // Envío
                await client.SendAsync(mensaje);

                // Desconexión limpia
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                // Loguea el error en tu consola para depurar fallos de credenciales o puertos
                Console.WriteLine($"[EmailService Error] 🚨 Falló el envío SMTP: {ex.Message}");
                return false;
            }
        }
    }
}
