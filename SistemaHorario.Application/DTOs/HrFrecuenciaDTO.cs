namespace SistemaHorario.Application.DTOs
{
    public class HrFrecuenciaDTO
    {
        public short IdHrf { get; set; }
        public string? Descripcion { get; set; }
        public bool? Activo { get; set; }

        //TEMP EXTRA HOUT
        public short? seleccionAdmision { get; set; }
        public short? seleccionEncuetros { get; set; }
        public short? seleccionEucaristia { get; set; }
        public short? seleccionCampamento { get; set; }
        public short? seleccionEntrevistas { get; set; }
        public short? seleccionServicioSol { get; set; }
        public short? seleccionRetirosPadres { get; set; }
        public short? seleccionTecho { get; set; }
    }
}
