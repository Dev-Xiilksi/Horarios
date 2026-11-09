namespace SistemaHorario.Application.DTOs
{
    public class ResponseDTO<T>
    {
        public bool status { get; set; } = false;
        public string message { get; set; } = string.Empty;
        public T? value { get; set; }
    }
}
