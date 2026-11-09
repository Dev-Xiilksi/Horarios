namespace SistemaHorario.Domain.ValueObjects
{
    //codigo compuesto: primer caracter = nivel, resto = grado (ej. "P1" -> nivel "P", grado "1")
    public sealed record NivelGrado
    {
        private NivelGrado(string codigo)
        {
            Codigo = codigo;
            Nivel = codigo.Substring(0, 1);
            Grado = codigo.Substring(1);
        }

        public string Codigo { get; }
        public string Nivel { get; }
        public string Grado { get; }

        public static NivelGrado Parse(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("El código de nivel/grado no puede estar vacío.", nameof(codigo));

            return new NivelGrado(codigo);
        }
    }
}
