namespace SistemaHorario.Application.Comportamientos
{
    //los datos enviados no cumplen las reglas; el mensaje se puede mostrar en pantalla
    public class DatosInvalidosException : Exception
    {
        public DatosInvalidosException(IReadOnlyList<string> errores)
            : base(string.Join(Environment.NewLine, errores))
        {
            Errores = errores;
        }

        public IReadOnlyList<string> Errores { get; }
    }
}
