namespace SistemaHorario.Domain.Constants
{
    public static class AreasEspeciales
    {
        //los coordinadores de estas areas ven y aprueban horarios de todas las areas (asi lo hacian ya las pantallas de inicio)
        public static readonly IReadOnlyCollection<short> ConVisionGlobal = new short[] { 0, 14 };
    }
}
