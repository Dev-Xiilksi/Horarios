using FluentValidation;
using MediatR;

namespace SistemaHorario.Application.Comportamientos
{
    //Antes del handler: aplica los validadores de la peticion (si tiene) y corta con DatosInvalidosException
    public class ValidacionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validadores;

        public ValidacionBehavior(IEnumerable<IValidator<TRequest>> validadores)
        {
            _validadores = validadores;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var errores = new List<string>();
            foreach (var validador in _validadores)
            {
                var resultado = await validador.ValidateAsync(request, cancellationToken);
                errores.AddRange(resultado.Errors.Select(e => e.ErrorMessage));
            }

            if (errores.Count > 0)
                throw new DatosInvalidosException(errores);

            return await next(cancellationToken);
        }
    }
}
