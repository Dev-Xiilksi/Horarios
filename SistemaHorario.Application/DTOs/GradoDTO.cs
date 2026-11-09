namespace SistemaHorario.Application.DTOs
{
    public class GradoDTO
    {
        public string Colegio { get; set; } = string.Empty;
        public string Nivel { get; set; } = string.Empty;
        public string Grado1 { get; set; } = string.Empty;
        public string? Nombre { get; set; }
        public string? Origen { get; set; }
        public bool? Real { get; set; }

        //Temp
        public string NombreNivel {

            get
            {
                if(Nivel == "E")
                {
                    return $"ETAPA PRE ESCOLAR: {Nombre}" ;
                }else if(Nivel == "K")
                {
                    return $"PREPRIMARIA: {Nombre}";
                }else if(Nivel == "P")
                {
                    return $"PRIMARIA: {Nombre}";
                }
                else if(Nivel == "S")
                {
                    return $"SECUNDARIA: {Nombre}";
                }
                return Nivel;
            }
        }
        public string ID => $"{Nivel}{Grado1}";
    }
}
