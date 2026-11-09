using System.Globalization;

namespace SistemaHorario.Domain.ValueObjects
{
    public sealed record Horario
    {
        public Horario(TimeSpan horaI, TimeSpan horaF)
        {
            if (horaF < horaI)
                throw new ArgumentException("La hora final no puede ser menor que la hora inicial.");

            HoraI = horaI;
            HoraF = horaF;
        }
        public string Horai => HoraI.ToString(@"hh\:mm");
        public string Horaf => HoraF.ToString(@"hh\:mm");

        public TimeSpan Duracion => HoraF - HoraI;

        public static Horario FromString(string? horai, string? horaf)
        {
            var inicio = string.IsNullOrWhiteSpace(horai)
                ? TimeSpan.Zero
                : TimeSpan.Parse(horai);

            var fin = string.IsNullOrWhiteSpace(horaf)
                ? TimeSpan.Zero
                : TimeSpan.Parse(horaf);

            return new Horario(inicio, fin);
        }

        //calcula el tiempo de una franja con horas en formato hh:mm:ss
        public static string CalcularTiempo(string horai, string horaf)
        {
            var horario = new Horario(
                TimeSpan.ParseExact(horai, @"hh\:mm\:ss", CultureInfo.InvariantCulture),
                TimeSpan.ParseExact(horaf, @"hh\:mm\:ss", CultureInfo.InvariantCulture));

            return horario.Duracion.ToString(@"hh\:mm\:ss");
        }

        private TimeSpan HoraI { get; init; }
        private TimeSpan HoraF { get; init; }
    }
}
