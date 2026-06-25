using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace MesaPartesDigital.Services
{
    public interface IEmailService
    {
        Task<bool> EnviarCodigoOtpAsync(string correoDestino, string codigoOtp);
        // 🛠️ Agregamos el método a la interfaz
        Task<bool> EnviarCargoDigitalAsync(string correoDestino, string codigoTramite);
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
                var server = _configuration["SmtpSettings:Server"];
                var port = int.Parse(_configuration["SmtpSettings:Port"] ?? "587");
                var senderName = _configuration["SmtpSettings:SenderName"];
                var senderEmail = _configuration["SmtpSettings:SenderEmail"];
                var password = _configuration["SmtpSettings:Password"];

                var mensaje = new MimeMessage();
                mensaje.From.Add(new MailboxAddress(senderName, senderEmail));
                mensaje.To.Add(new MailboxAddress("", correoDestino));
                mensaje.Subject = "Código de Verificación - Mesa de Partes Digital";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
<div style='font-family: Arial, sans-serif; max-width: 600px; width: 100%; margin: 0 auto; padding: 0; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.05); box-sizing: border-box;'>
    <div style='background-color: #35af72; padding: 25px 15px; text-align: center;'>
        <img src='https://www.gob.pe/rails/active_storage/representations/redirect/eyJfcmFpbHMiOnsiZGF0YSI6NDk2ODQ0LCJwdXIiOiJibG9iX2lkIn19--1fc9a7807cf6c726e857b951ca1a374a8414a140/eyJfcmFpbHMiOnsiZGF0YSI6eyJmb3JtYXQiOiJwbmciLCJyZXNpemVfdG9fbGltaXQiOltudWxsLDQ4XX0sInB1ciI6InZhcmlhdGlvbiJ9fQ==--830247c4bafe7cadca50817d8559bf1a09e3aa28/paga%20gob.pe.png' alt='Logo MIDAGRI - AGRO RURAL' style='max-width: 100%; height: auto; display: inline-block; min-height: 40px;' />
        <p style='color: #a3e2c1; margin: 10px 0 0 0; font-size: 12px; text-transform: uppercase; font-weight: bold; letter-spacing: 1px;'>Mesa de Partes Digital</p>
    </div>
    <div style='padding: 5%; background-color: #ffffff; box-sizing: border-box;'>
        <p style='color: #333333; font-size: 15px; line-height: 1.5; margin-top: 0;'>Estimado(a) administrado(a),</p>
        <p style='color: #555555; font-size: 14px; line-height: 1.6;'>Se ha solicitado un código de verificación para continuar con el registro de su documento en la plataforma de la <strong>Mesa de Partes Digital</strong>.</p>
        <div style='background-color: #f4f9f5; border: 1px dashed #006432; padding: 20px 10px; text-align: center; border-radius: 6px; margin: 25px 0;'>
            <span style='display: block; font-size: 11px; color: #555555; text-transform: uppercase; margin-bottom: 8px; font-weight: bold; letter-spacing: 1px;'>Código de Verificación</span>
            <div style='font-size: 30px; font-weight: bold; letter-spacing: 4px; color: #006432; word-break: break-all;'>{codigoOtp}</div>
        </div>
        <div style='font-size: 12px; color: #777777; line-height: 1.4; background-color: #fff8e7; padding: 12px; border-left: 4px solid #f39c12; border-radius: 4px; box-sizing: border-box;'>
            <strong>Importante:</strong> Este código es de un solo uso. Si usted no solicitó este requerimiento, por favor ignore este mensaje.
        </div>
        <hr style='border: none; border-top: 1px solid #eeeeee; margin: 30px 0;' />
        <p style='font-size: 11px; color: #999999; text-align: center; line-height: 1.5; margin: 0;'>Programa de Desarrollo Productivo Agrario Rural - AGRO RURAL<br><span style='color: #ba2525; font-weight: bold; display: block; margin-top: 5px;'>Por favor, no responda a este correo automático.</span></p>
    </div>
</div>"
                };
                mensaje.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(server, port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(senderEmail, password);
                await client.SendAsync(mensaje);
                await client.DisconnectAsync(true);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EmailService Error] 🚨 Falló el envío SMTP OTP: {ex.Message}");
                return false;
            }
        }

        // 🛠️ Mismo ecosistema de envío idéntico al tuyo pero con la plantilla de cargo digital
        public async Task<bool> EnviarCargoDigitalAsync(string correoDestino, string codigoTramite)
        {
            try
            {
                var server = _configuration["SmtpSettings:Server"];
                var port = int.Parse(_configuration["SmtpSettings:Port"] ?? "587");
                var senderName = _configuration["SmtpSettings:SenderName"];
                var senderEmail = _configuration["SmtpSettings:SenderEmail"];
                var password = _configuration["SmtpSettings:Password"];

                var mensaje = new MimeMessage();
                mensaje.From.Add(new MailboxAddress(senderName, senderEmail));
                mensaje.To.Add(new MailboxAddress("", correoDestino));
                mensaje.Subject = $"Cargo de Recepción Digital - Trámite {codigoTramite}";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
<div style='font-family: Arial, sans-serif; max-width: 600px; width: 100%; margin: 0 auto; padding: 0; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.05); box-sizing: border-box;'>
    <div style='background-color: #35af72; padding: 25px 15px; text-align: center;'>
        <img src='https://www.gob.pe/rails/active_storage/representations/redirect/eyJfcmFpbHMiOnsiZGF0YSI6NDk2ODQ0LCJwdXIiOiJibG9iX2lkIn19--1fc9a7807cf6c726e857b951ca1a374a8414a140/eyJfcmFpbHMiOnsiZGF0YSI6eyJmb3JtYXQiOiJwbmciLCJyZXNpemVfdG9fbGltaXQiOltudWxsLDQ4XX0sInB1ciI6InZhcmlhdGlvbiJ9fQ==--830247c4bafe7cadca50817d8559bf1a09e3aa28/paga%20gob.pe.png' alt='Logo MIDAGRI - AGRO RURAL' style='max-width: 100%; height: auto; display: inline-block; min-height: 40px;' />
        <p style='color: #a3e2c1; margin: 10px 0 0 0; font-size: 12px; text-transform: uppercase; font-weight: bold; letter-spacing: 1px;'>Mesa de Partes Digital</p>
    </div>
    <div style='padding: 5%; background-color: #ffffff; box-sizing: border-box;'>
        <p style='color: #333333; font-size: 15px; line-height: 1.5; margin-top: 0;'>Estimado(a) administrado(a),</p>
        <p style='color: #555555; font-size: 14px; line-height: 1.6;'>Nos complace informarle que su documentación ha sido cargada con éxito en nuestro sistema. A continuación le brindamos los detalles de su registro:</p>
        
        <div style='background-color: #f4f9f5; border: 1px dashed #006432; padding: 20px 10px; text-align: center; border-radius: 6px; margin: 25px 0;'>
            <span style='display: block; font-size: 11px; color: #555555; text-transform: uppercase; margin-bottom: 8px; font-weight: bold; letter-spacing: 1px;'>Código de Trámite Autogenerado</span>
            <div style='font-size: 26px; font-weight: bold; font-family: monospace; letter-spacing: 2px; color: #006432; word-break: break-all;'>{codigoTramite}</div>
        </div>

        <div style='background-color: #fafafa; border: 1px solid #eeeeee; padding: 15px; border-radius: 6px; margin-bottom: 25px;'>
            <table style='width: 100%; font-size: 13px; color: #555555;'>
                <tr><td style='padding: 5px 0; font-weight: bold; width: 40%;'>Canal de Atención:</td><td style='padding: 5px 0;'>Mesa de Partes Virtual</td></tr>
                <tr><td style='padding: 5px 0; font-weight: bold;'>Mail de Seguimiento:</td><td style='padding: 5px 0; color: #006432; font-weight: bold;'>{correoDestino}</td></tr>
                <tr><td style='padding: 5px 0; font-weight: bold;'>Estado del Expediente:</td><td style='padding: 5px 0;'><span style='background-color: #e8f5e9; color: #2e7d32; padding: 2px 8px; border-radius: 10px; font-size: 11px; font-weight: bold;'>ENVIADO</span></td></tr>
            </table>
        </div>

        <hr style='border: none; border-top: 1px solid #eeeeee; margin: 30px 0;' />
        <p style='font-size: 11px; color: #999999; text-align: center; line-height: 1.5; margin: 0;'>Programa de Desarrollo Productivo Agrario Rural - AGRO RURAL<br><span style='color: #ba2525; font-weight: bold; display: block; margin-top: 5px;'>Por favor, no responda a este correo automático.</span></p>
    </div>
</div>"
                };
                mensaje.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(server, port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(senderEmail, password);
                await client.SendAsync(mensaje);
                await client.DisconnectAsync(true);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EmailService Error] 🚨 Falló el envío SMTP Cargo: {ex.Message}");
                return false;
            }
        }
    }
}