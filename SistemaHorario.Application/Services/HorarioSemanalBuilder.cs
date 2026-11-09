using SistemaHorario.Application.DTOs.Auxiliares;

namespace SistemaHorario.Application.Services
{
    //arma la grilla semanal (una fila por franja, una columna por dia) a partir de las filas de hr_horario2
    public static class HorarioSemanalBuilder
    {
        public static List<FranjasDto> Construir(
            IEnumerable<FranjaHorariaDto> filas,
            IReadOnlyDictionary<int, string> materias,
            IReadOnlyDictionary<short, string> actividades)
        {
            var conNombre = filas.Select(a => new
            {
                a.Horai,
                a.Horaf,
                a.Tiempo,
                a.Dia,
                NombreActividad = NombreActividad(a, materias, actividades)
            }).ToList();

            // agrupa por franja (hora inicio, fin y tiempo)
            return conNombre
                .GroupBy(a => new { a.Horai, a.Horaf, a.Tiempo })
                .Select(g => new
                {
                    HoraInicio = g.Key.Horai,
                    HoraFin = g.Key.Horaf,
                    Tiempo = g.Key.Tiempo,
                    Lunes = g.FirstOrDefault(x => x.Dia.ToLower().Contains("lunes"))?.NombreActividad ?? "",
                    Martes = g.FirstOrDefault(x => x.Dia.ToLower().Contains("martes"))?.NombreActividad ?? "",
                    Miercoles = g.FirstOrDefault(x => x.Dia.ToLower().Contains("miércoles") || x.Dia.ToLower().Contains("miercoles"))?.NombreActividad ?? "",
                    Jueves = g.FirstOrDefault(x => x.Dia.ToLower().Contains("jueves"))?.NombreActividad ?? "",
                    Viernes = g.FirstOrDefault(x => x.Dia.ToLower().Contains("viernes"))?.NombreActividad ?? "",
                    Sabados = g.FirstOrDefault(x => x.Dia.ToLower().Contains("sabádos"))?.NombreActividad ?? ""
                })
                .OrderBy(f => f.HoraInicio)
                .Select((f, index) => new FranjasDto
                {
                    Franja = index + 1,
                    HoraInicio = f.HoraInicio!,
                    HoraFin = f.HoraFin!,
                    Tiempo = f.Tiempo!,
                    Lunes = f.Lunes,
                    Martes = f.Martes,
                    Miercoles = f.Miercoles,
                    Jueves = f.Jueves,
                    Viernes = f.Viernes,
                    Sabados = f.Sabados
                })
                .ToList();
        }

        //la franja muestra la materia si la tiene; si no, la actividad
        private static string NombreActividad(
            FranjaHorariaDto fila,
            IReadOnlyDictionary<int, string> materias,
            IReadOnlyDictionary<short, string> actividades)
        {
            if (fila.IdMps != null)
                return materias.TryGetValue(fila.IdMps.Value, out var materia) ? materia : "Sin asignar";

            return fila.IdHra != null && actividades.TryGetValue(fila.IdHra.Value, out var actividad)
                ? actividad
                : "Sin actividad";
        }
    }
}
