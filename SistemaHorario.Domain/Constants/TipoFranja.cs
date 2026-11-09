namespace SistemaHorario.Domain.Constants
{
    public static class TipoFranja
    {
        public const int Fijo = 1;
        public const int Variable = 2;

        public static string Descripcion(int tipoFranja) =>
            tipoFranja == Fijo ? "Hr. Fijo" : "Hr. Variable";
    }
}
