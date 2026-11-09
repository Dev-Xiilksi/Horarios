using FluentValidation;
using MediatR;
using SistemaHorario.Application.DTOs;
using SistemaHorario.Application.Interfaces;
using SistemaHorario.Application.Seguridad;
using SistemaHorario.Domain.Constants;
using SistemaHorario.Domain.Entities;
using SistemaHorario.Domain.ValueObjects;
using System.Globalization;
using HorarioVO = SistemaHorario.Domain.ValueObjects.Horario;

namespace SistemaHorario.Application.UseCases.Horario.Commands
{
    public record CrearHorarioCommand(HrHorario1DTO Horario) : IRequest<HrHorario1DTO>;

    //lo aplica ValidacionBehavior antes del handler
    public class CrearHorarioValidator : AbstractValidator<CrearHorarioCommand>
    {
        public CrearHorarioValidator()
        {
            RuleFor(c => c.Horario).NotNull().WithMessage("Faltan los datos del horario.");

            When(c => c.Horario != null, () =>
            {
                RuleFor(c => c.Horario.Empleado).GreaterThan(0).WithMessage("Falta el empleado del horario.");
                RuleFor(c => c.Horario.Ciclo).NotEmpty().WithMessage("Falta el ciclo del horario.");
                RuleFor(c => c.Horario.TipoFranja)
                    .Must(t => t == TipoFranja.Fijo || t == TipoFranja.Variable)
                    .WithMessage("El tipo de horario debe ser fijo o variable.");
                RuleForEach(c => c.Horario.Grados).NotEmpty().WithMessage("Hay un grado sin código.");

                RuleForEach(c => c.Horario.HrHorario2s).Custom((franja, contexto) =>
                {
                    if (!EsHora(franja.Horai) || !EsHora(franja.Horaf))
                        contexto.AddFailure($"Franja {franja.NumeroFila}: las horas deben tener el formato hh:mm:ss.");
                    else if (TimeSpan.Parse(franja.Horaf, CultureInfo.InvariantCulture) < TimeSpan.Parse(franja.Horai, CultureInfo.InvariantCulture))
                        contexto.AddFailure($"Franja {franja.NumeroFila}: la hora final ({franja.Horaf}) es anterior a la inicial ({franja.Horai}).");
                });
            });
        }

        private static bool EsHora(string? valor) =>
            TimeSpan.TryParseExact(valor, @"hh\:mm\:ss", CultureInfo.InvariantCulture, out _);
    }

    public class CrearHorarioHandler : IRequestHandler<CrearHorarioCommand, HrHorario1DTO>
    {
        private readonly IHorarioRepository _repository;
        private readonly IHorarioConsultas _consultas;
        private readonly IUsuarioActual _usuarioActual;
        private readonly IHrBitacoraService _bitacora;

        public CrearHorarioHandler(IHorarioRepository repository, IHorarioConsultas consultas, IUsuarioActual usuarioActual, IHrBitacoraService bitacora)
        {
            _repository = repository;
            _consultas = consultas;
            _usuarioActual = usuarioActual;
            _bitacora = bitacora;
        }

        public async Task<HrHorario1DTO> Handle(CrearHorarioCommand command, CancellationToken cancellationToken)
        {
            var model = command.Horario;
            var usuario = await AccesoDenegadoException.ExigirSesion(_usuarioActual);
            if (!ReglasDeAcceso.PuedeModificarHorarioDe(usuario, model.Empleado))
                throw new AccesoDenegadoException("Solo puede registrar su propio horario.");

            var obj = new HrHorario1
            {
                IdHrg1 = model.IdHrg1,
                Empresa = model.Empresa,
                Empleado = model.Empleado,
                Ciclo = model.Ciclo,
                Area = Convert.ToInt16(model.Area),
                Colegio = model.Colegio,
                Nivel = "",
                Grado = "",
                Seccion = "",
                CorrecionTrabajos = "",
                Estudiantes = 0,
                Observacion = "",
                IdConfirmacion = EstadoConfirmacion.Pendiente,
                Fecha_Registro = DateTime.Now.Date,
                Hora_Registro = DateTime.Now.TimeOfDay,
                TipoFranja = model.TipoFranja,
                HrGrados = model.Grados.Select(codigo =>
                {
                    var nivelGrado = NivelGrado.Parse(codigo);
                    return new HrGrado
                    {
                        IdHorario1 = model.IdHorario1,
                        Colegio = model.Colegio,
                        Nivel = nivelGrado.Nivel,
                        Grado = nivelGrado.Grado,
                        Seccion = "",
                        NivelGrado = nivelGrado.Codigo
                    };
                }).ToList(),
                HrHorario2s = model.HrHorario2s.Select(a => new HrHorario2
                {
                    IdHorario1 = a.IdHorario1,
                    Dia = a.Dia,
                    IdHrg2 = a.IdHrg2,
                    IdMps = a.IdMps,
                    IdHra = a.IdHra,
                    Horai = a.Horai,
                    Horaf = a.Horaf,
                    Tiempo = HorarioVO.CalcularTiempo(a.Horai, a.Horaf),
                    NumeroFila = a.NumeroFila,
                }).ToList(),
            };
            //el horario, sus grados y sus franjas se guardan en un solo SaveChanges (una transaccion)
            await _repository.Agregar(obj);

            await _bitacora.GuardarBitacora(
                usuario.IdEmpleado,
                obj.IdHorario1,
                "Guardado",
                $"Ha ingresado nuevo registro de horario ({obj.IdHorario1}).");

            //ya esta guardado: si releerlo no devuelve nada, se responde con lo guardado en vez de fallar
            return await _consultas.ObtenerPorId(obj.IdHorario1)
                ?? new HrHorario1DTO
                {
                    IdHorario1 = obj.IdHorario1,
                    IdHrg1 = obj.IdHrg1,
                    Empresa = obj.Empresa,
                    Empleado = obj.Empleado,
                    Ciclo = obj.Ciclo,
                    Area = obj.Area ?? 0,
                    Colegio = obj.Colegio,
                    TipoFranja = obj.TipoFranja
                };
        }
    }
}
