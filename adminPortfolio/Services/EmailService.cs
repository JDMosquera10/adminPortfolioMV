using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using static System.Runtime.InteropServices.JavaScript.JSType;
using adminProfolio.Settings;
using adminProfolio.Interfaces;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> options)
    {
        _settings = options.Value;
    }

    public async Task EnviarCodigoVerificacion(string email, string nombre, string codigo)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Admin", _settings.From));
        message.To.Add(new MailboxAddress(nombre, email));
        message.Subject = "Código de verificación";

        var htmlBody = $@"
    <!DOCTYPE html>
    <html>
    <head>
      <meta charset=""utf-8"">
      <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
      <title>Verificación de correo electrónico</title>
      <style>
        /* Estilos generales */
        body {{
          font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
          line-height: 1.6;
          color: #333333;
          margin: 0;
          padding: 0;
          background-color: #f9f9f9;
        }}
        .container {{
          max-width: 600px;
          margin: 0 auto;
          padding: 20px;
          background-color: #ffffff;
          border-radius: 8px;
          box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
        }}
        .header {{
          text-align: center;
          padding: 20px 0;
          border-bottom: 1px solid #eeeeee;
        }}
        .logo {{
          max-height: 60px;
          margin-bottom: 10px;
        }}
        .content {{
          padding: 30px 20px;
          text-align: center;
        }}
        .verification-code {{
          font-size: 32px;
          font-weight: bold;
          color: #4a6ee0;
          padding: 15px 25px;
          margin: 25px 0;
          display: inline-block;
          background-color: #f0f4ff;
          border-radius: 6px;
          letter-spacing: 4px;
          border: 1px dashed #b1c3ff;
        }}
        .footer {{
          text-align: center;
          padding-top: 20px;
          margin-top: 20px;
          color: #888888;
          font-size: 0.9em;
          border-top: 1px solid #eeeeee;
        }}
        .text-highlight {{
          color: #4a6ee0;
          font-weight: 600;
        }}
        .btn {{
          display: inline-block;
          background-color: #4a6ee0;
          color: white;
          text-decoration: none;
          padding: 12px 25px;
          border-radius: 5px;
          font-weight: bold;
          margin-top: 15px;
          transition: background-color 0.3s;
        }}
        .btn:hover {{
          background-color: #3a5dca;
        }}
        .warning {{
          background-color: #fff8e1;
          padding: 15px;
          border-radius: 5px;
          margin-top: 30px;
          font-size: 0.9em;
          color: #856404;
          border-left: 4px solid #ffd54f;
        }}
      </style>
    </head>
    <body>
      <div class=""container"">
        <div class=""header"">
          <h1>Administrador de portafolios</h1>
        </div>
        <div class=""content"">
          <h2>Verificación de Correo Electrónico</h2>
          <p>Hola <span class=""text-highlight"">{nombre}</span>,</p>
          <p>Gracias por registrarte en <strong>Administrador de portafolios</strong>. Para completar tu registro, por favor utiliza el siguiente código de verificación:</p>
          
          <div class=""verification-code"">{codigo}</div>
          
          <p>Este código expirará en <strong>5 minutos</strong>.</p>
          
          <a href='http://adminportafolio.com' class=""btn"">Verificar mi cuenta</a>
          
          <div class=""warning"">
            <p>Si no solicitaste este código, puedes ignorar este correo. Si no reconoces esta actividad, por favor contacta a nuestro equipo de soporte.</p>
          </div>
        </div>
        <div class=""footer"">
          <p>&copy; Administrador de portafolio. Todos los derechos reservados.</p>
          <p>Este es un correo automático, por favor no respondas a este mensaje.</p>
        </div>
      </div>
    </body>
    </html>
    ";

        message.Body = new TextPart("html")
        {
            Text = htmlBody
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.Host, _settings.Port, _settings.UseSSL);
        await client.AuthenticateAsync(_settings.Username, _settings.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}

