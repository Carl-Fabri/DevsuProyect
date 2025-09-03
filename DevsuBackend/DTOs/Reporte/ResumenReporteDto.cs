namespace DevsuBackend.DTOs.Reporte
{
    public class ResumenReporteDto
    {
        public decimal TotalCreditos { get; set; }
        public decimal TotalDebitos { get; set; }
        public decimal SaldoFinal { get; set; }
        public int TotalMovimientos { get; set; }
        public int CuentasInvolucradas { get; set; }
        public DateTime PeriodoInicio { get; set; }
        public DateTime PeriodoFin { get; set; }
    }
}
