namespace SistemaHorario.Application.DTOs.Auxiliares
{
    public class ColaboradorDTO
    {
        public int IdHorario { get; set; }
        public string? Horario { get; set; }
        public int? NumEmpleado { get; set; }
        public string? Educador { get; set; }
        public string? Area { get; set; }
        public string? Colegio { get; set; }
        public string? Nivel { get; set; }
        public string? Grado { get; set; }
        public string? Seccion { get; set; }
        public string? CorreccionTrabajo 
        {
            get => _correccionTrabajo switch
            {
                "A" => "Alto",
                "M" => "Medio",
                "B" => "Bajo",
                _ => null
            };
            set => _correccionTrabajo = value;
            
        }
        public short? NoEstudiantes { get; set; }
        public string? Observacion { get; set; }
        public int Codigo { get; set; }
        public int? Idconfirmacion { get; set; }
        public string? FechaRegistro { get; set; }
        public TimeSpan? HoraRegistro { get; set; }
        public string? TipoFranja { get; set; }

        //auxiliary
        public string? _correccionTrabajo { get; set; }
    }
}
