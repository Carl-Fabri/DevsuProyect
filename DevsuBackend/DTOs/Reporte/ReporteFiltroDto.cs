namespace DevsuBackend.DTOs.Reporte
{
    public class ReporteFiltroDto
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? ClienteId { get; set; }
        public int? CuentaId { get; set; }
        public string TipoMovimiento { get; set; }
        public decimal? MontoMinimo { get; set; }
        public decimal? MontoMaximo { get; set; }
    }
}
