using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SistemaHorario.Application.Comportamientos;
using SistemaHorario.Application.Interfaces;
using SistemaHorario.Application.Seguridad;
using SistemaHorario.Application.Services;

namespace SistemaHorario.Application
{
    public static class DependencyInjection
    {
        //mediatRLicenseKey: si es null, MediatR la busca en la variable de entorno MEDIATR_LICENSE_KEY
        public static IServiceCollection AddApplication(this IServiceCollection services, string? mediatRLicenseKey = null)
        {
            //CQRS: comandos y consultas de UseCases
            services.AddMediatR(cfg =>
            {
                if (!string.IsNullOrWhiteSpace(mediatRLicenseKey))
                    cfg.LicenseKey = mediatRLicenseKey;

                cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

                //se ejecutan en este orden alrededor de cada handler: errores envuelve a validacion
                cfg.AddOpenBehavior(typeof(ErroresBehavior<,>));
                cfg.AddOpenBehavior(typeof(ValidacionBehavior<,>));
            });

            //validadores de FluentValidation de este ensamblado (los usa ValidacionBehavior)
            var validadores =
                from tipo in typeof(DependencyInjection).Assembly.GetTypes()
                where tipo is { IsClass: true, IsAbstract: false }
                from contrato in tipo.GetInterfaces()
                where contrato.IsGenericType && contrato.GetGenericTypeDefinition() == typeof(IValidator<>)
                select (contrato, tipo);
            foreach (var (contrato, tipo) in validadores)
                services.AddScoped(contrato, tipo);

            services.AddScoped<IReporteService, ReporteService>();
            //compartido por todas las conexiones para contar los intentos de login
            services.AddSingleton(_ => new LimitadorIntentosLogin(LimitesLogin.PorDefecto));
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IAccesoUsuarioService, AccesoUsuarioService>();
            services.AddScoped<IHrActividadesService, HrActividadesService>();

            return services;
        }
    }
}
