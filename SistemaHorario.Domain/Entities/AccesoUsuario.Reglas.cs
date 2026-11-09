namespace SistemaHorario.Domain.Entities
{
    public partial class AccesoUsuario
    {
        //cambia el rol/area del usuario y registra la fecha del cambio
        public void Actualizar(int? idRol, int idEmpleado, short idArea)
        {
            IdRol = idRol;
            IdEmpleado = idEmpleado;
            IdArea = idArea;
            FechaRegistro = DateTime.Now;
        }
    }
}
