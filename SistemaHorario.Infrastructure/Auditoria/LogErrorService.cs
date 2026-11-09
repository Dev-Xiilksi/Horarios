using SistemaHorario.Application.Interfaces;
using SistemaHorario.Infrastructure.Persistence;
using Dapper;

namespace SistemaHorario.Infrastructure.Auditoria
{
    public class LogErrorService : ILogErrorService
    {
        private readonly DapperContext _context;

        public LogErrorService(DapperContext context)
        {
            _context = context;
        }

        public async Task GuardarError(Exception ex, string metodo = "", string usuario = "")
        {
            using (var connection = _context.Crearconexion())
            {
                string query = @"INSERT INTO hr_log_errors 
                            (Mensaje, StackTrace, InnerException, Fuente, Metodo, Usuario,Fecha, Hora)
                            VALUES 
                            (@Mensaje, @StackTrace, @InnerException, @Fuente, @Metodo, @Usuario, @Fecha, @Hora)";

                await connection.ExecuteAsync(query, new
                {
                    Mensaje = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.ToString(),
                    Fuente = ex.Source,
                    Metodo = metodo,
                    Usuario = usuario,
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.TimeOfDay
                });
            }
        }
    }
}
