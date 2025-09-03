namespace DevsuBackend.DTOs.Reporte
{
    public class MovimientoReporteDto
    {
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; }
        public int NumeroCuenta { get; set; }
        public string TipoCuenta { get; set; }
        public decimal SaldoInicial { get; set; }
        public bool EstadoCuenta { get; set; }
        public decimal Movimiento { get; set; }
        public decimal SaldoDisponible { get; set; }
        public string TipoMovimiento { get; set; }
    }
}
