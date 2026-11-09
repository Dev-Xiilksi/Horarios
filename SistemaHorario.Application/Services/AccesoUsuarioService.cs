using Microsoft.Extensions.Logging;
using SistemaHorario.Application.DTOs;
using SistemaHorario.Application.Interfaces;
using SistemaHorario.Application.Seguridad;
using SistemaHorario.Domain.Entities;

namespace SistemaHorario.Application.Services
{
    public class AccesoUsuarioService : IAccesoUsuarioService
    {
        private readonly IAccesoUsuarioRepository _repository;
        private readonly ILogger<AccesoUsuarioService> _logger;
        private readonly IUsuarioActual _usuarioActual;

        public AccesoUsuarioService(IAccesoUsuarioRepository repository, ILogger<AccesoUsuarioService> logger, IUsuarioActual usuarioActual)
        {
            _repository = repository;
            _usuarioActual = usuarioActual;
            _logger = logger;
        }

        public async Task<AccesoUsuarioDTO> Create(AccesoUsuarioDTO model)
        {
            await ExigirAdministrador();
            try
            {
                var obj = new AccesoUsuario
                {
                    IdRol = model.IdRol,
                    IdEmpleado = model.IdEmpleado,
                    IdArea = Convert.ToInt16(model.IdArea)
                };
                await _repository.Agregar(obj);

                return (await _repository.ObtenerPorId(obj.IdAcceso))!;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el servicio al crear el acceso");
                throw;
            }
        }

        //elimina el acceso del empleado
        public async Task<bool> Delete(int id)
        {
            await ExigirAdministrador();
            try
            {
                var entity = await _repository.BuscarPorEmpleado(id);
                if (entity == null)
                    return false;

                await _repository.Eliminar(entity);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el acceso con Id {id}");
                throw;
            }
        }

        public async Task<List<AccesoUsuarioDTO>> GetAll()
        {
            return await _repository.ObtenerTodos();
        }

        public async Task<AccesoUsuarioDTO> GetById(int educador)
        {
            return (await _repository.ObtenerPorEmpleado(educador))!;
        }

        public async Task<bool> Update(AccesoUsuarioDTO model)
        {
            await ExigirAdministrador();
            try
            {
                var access = await _repository.Buscar(model.IdAcceso)
                    ?? throw new InvalidOperationException("El acceso no fue encontrado.");

                access.Actualizar(model.IdRol, model.IdEmpleado, Convert.ToInt16(model.IdArea));

                if (await _repository.Actualizar(access) == 0)
                    throw new TaskCanceledException("Error al actualizar el registro.");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el acceso con Id {model.IdEmpleado}");
                throw;
            }
        }

        //leer accesos lo necesitan las pantallas de inicio; modificarlos, solo el administrador
        private async Task ExigirAdministrador()
        {
            var usuario = await AccesoDenegadoException.ExigirSesion(_usuarioActual);
            if (!ReglasDeAcceso.PuedeAdministrarAccesos(usuario))
                throw new AccesoDenegadoException("Solo el administrador puede modificar accesos.");
        }
    }
}
