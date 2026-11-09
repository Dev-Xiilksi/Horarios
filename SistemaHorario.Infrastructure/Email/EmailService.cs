using SistemaHorario.Infrastructure.Persistence;
using Dapper;
using SistemaHorario.Application.Interfaces;
using SistemaHorario.Application.DTOs;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace SistemaHorario.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly DapperContext _dapper;

        public EmailService(IConfiguration configuration, DapperContext dapper)
        {
            _emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>();
            _dapper = dapper;
        }

        public async Task EnviarCorreo(string destino, string asunto, string mensaje)
        {
            var smtCliente = new SmtpClient(_emailSettings.Host)
            {
                Port = _emailSettings.Port,
                Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password),
                EnableSsl = true,
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_emailSettings.Email),
                Subject = asunto,
                Body = mensaje,
                IsBodyHtml = true
            };

            mail.To.Add(destino);
            await smtCliente.SendMailAsync(mail);
        }

        public async Task RegistrarCorreo(CorreoDTO dto)
        {
            using (var connection = _dapper.Crearconexion())
            {
                var query = @"insert into hr_correos(Area,EncargadoArea,CorreoEncargado,Fecha,Hora)
                              values(@Area,@EncargadoArea,@CorreoEncargado,@FechaRegistro,@HoraRegistro)";

                await connection.ExecuteAsync(query, new
                {
                    Area = dto.Area,
                    EncargadoArea = dto.EncargadoArea,
                    CorreoEncargado = dto.CorreoEncargado,
                    FechaRegistro = DateTime.Now,
                    HoraRegistro = DateTime.Now.TimeOfDay,
                });
            }
        }
        public async Task ObtenerCorreos(CorreoDTO dto)
        {
            using (var connection = _dapper.Crearconexion())
            {
                var query = @"insert into hr_correos(Area,EncargadoArea,CorreoEncargado,Fecha,Hora)
                              values(@Area,@EncargadoArea,@CorreoEncargado,@FechaRegistro,@HoraRegistro)";

                await connection.ExecuteAsync(query, new
                {
                    Area = dto.Area,
                    EncargadoArea = dto.EncargadoArea,
                    CorreoEncargado = dto.CorreoEncargado,
                    FechaRegistro = DateTime.Now,
                    HoraRegistro = DateTime.Now.TimeOfDay,
                });
            }
        }
    }
}
