using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Models
{
    public class Persona
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Genero { get; set; } = null!;
        public int Edad { get; set; }
        public string Identificacion { get; set; } = null!;
        public string Direccion { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public string PersonaType { get; set; } = "Persona";
    }
    public class Cliente : Persona
    {
        public int ClienteId { get; set; }
        public string Contrasena { get; set; }
        public bool Estado { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
    }
}
