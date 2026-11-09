namespace SistemaHorario.Application.DTOs
{
    public class ContratoDTO
    {
        public short Empresa { get; set; }

        public int Empleado { get; set; }

        public short Contrato { get; set; }

        public DateTime? Fecha { get; set; }

        public short? Tiposeguro { get; set; }

        public short? Tipoper { get; set; }

        public short? Puesto { get; set; }

        public string? Mpago { get; set; }

        public string? Anticipo { get; set; }

        public short? Fpago { get; set; }

        public string? Cuentaban { get; set; }

        public DateTime? Fechai { get; set; }

        public DateTime? Fechaf { get; set; }

        public short? Base { get; set; }

        public short? Jornada { get; set; }

        public string? Horaini { get; set; }

        public string? Horafin { get; set; }

        public decimal? Horas { get; set; }

        public decimal? Semanahoras { get; set; }

        public string? Observa { get; set; }

        public short? Estado { get; set; }

        public DateTime? Fechae { get; set; }

        public short? Tipoempleado { get; set; }

        public short? IdTipoContrato { get; set; }

        public short? IdTemporalidad { get; set; }

        public virtual EmplegenDTO Emplegen { get; set; } = null!;
    }
}
