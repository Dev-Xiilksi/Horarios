namespace SistemaHorario.Application.DTOs.Auxiliares
{
    //materia que el maestro imparte en una seccion (maestrosporseccion + materias)
    public class MateriaImpartidaDto
    {
        public int IdMps { get; set; }
        public string? Colegio { get; set; }
        public string? Nivel { get; set; }
        public string? Grado { get; set; }
        public string? Seccion { get; set; }
        public string? NombreMateria { get; set; }
    }
}
