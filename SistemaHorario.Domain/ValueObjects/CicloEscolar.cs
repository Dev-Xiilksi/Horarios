namespace SistemaHorario.Domain.ValueObjects
{
    public static class CicloEscolar
    {
        //el ciclo escolar corresponde al año calendario
        public static string Actual() => DateTime.Now.Year.ToString();
    }
}
