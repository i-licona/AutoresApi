namespace AutoresApi.DTOs
{
    public class ColeccionDeRecursos<T>:Recurso where T : Recurso
    {
        public List<T> Data { get; set; }
        public int Status { get; set; }
    }
}
