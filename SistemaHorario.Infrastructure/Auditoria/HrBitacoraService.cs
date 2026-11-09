using Dapper;
using Microsoft.Extensions.Logging;
using SistemaHorario.Application.Interfaces;
using SistemaHorario.Infrastructure.Persistence;

namespace SistemaHorario.Infrastructure.Auditoria
{
    public class HrBitacoraService : IHrBitacoraService
    {
        private readonly DapperContext _context;
        private readonly ILogger<HrBitacoraService> _logger;

        public HrBitacoraService(DapperContext context, ILogger<HrBitacoraService> logger)
        {
            _context = context;
            _logger = logger;
        }

        //Se llama despues de guardar el horario. Si falla, la operacion ya quedo guardada:
        //no se propaga el error (el usuario veria un fallo de algo que si se hizo), se deja en el log.
        public async Task GuardarBitacora(int? usuario, int horario, string? accion, string? mensaje)
        {
            try
            {
                using var connection = _context.Crearconexion();
                string query = @"insert into hr_bitacora(Usuario,IdHorario,Accion,Mensaje,FechaRegistro,HoraRegistro)
                                 values(@Usuario, @IdHorario, @Accion, @Mensaje, @FechaRegistro, @HoraRegistro)";

                await connection.ExecuteAsync(query, new
                {
                    Usuario = usuario,
                    IdHorario = horario,
                    Accion = accion,
                    Mensaje = mensaje,
                    FechaRegistro = DateTime.Now.Date,
                    HoraRegistro = DateTime.Now.TimeOfDay
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "No se pudo registrar en hr_bitacora: {Accion} del horario {Horario} por {Usuario}", accion, horario, usuario);
            }
        }
    }
}
