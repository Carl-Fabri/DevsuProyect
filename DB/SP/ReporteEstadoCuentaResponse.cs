using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.SP
{
    public class ReporteEstadoCuentaResponse
    {
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; } = null!;
        public int NumeroCuenta { get; set; }
        public string TipoCuenta { get; set; } = null!;
        public decimal SaldoInicial { get; set; }
        public bool Estado { get; set; }
        public decimal TotalMovimientos { get; set; }
        public decimal SaldoDisponible { get; set; }
    }
}
