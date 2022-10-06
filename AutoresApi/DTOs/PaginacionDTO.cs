namespace AutoresApi.DTOs
{
    public class PaginacionDTO
    {
        public int Pagina { get; set; }
        private int recordsPorPagina { get; set; }
        private readonly int CantidadMaximaPorPagina = 50;
        public int RecordsPorPagina
        {
            get { return recordsPorPagina; }
            set { recordsPorPagina = (value > CantidadMaximaPorPagina) ? CantidadMaximaPorPagina : value; }
        }
    }
}
