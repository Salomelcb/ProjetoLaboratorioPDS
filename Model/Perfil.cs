using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacoes.Model
{
    public class Perfil  
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string? FotografiaPath { get; set; }
        public string? NumeroTelefone { get; set; }  
    }

}
