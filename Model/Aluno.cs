using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacoes.Model
{
    public class Aluno
    {
        public int Numero { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public int? GrupoID { get; set; } // pode como nao pode estar associado a um grupo (excluido;c)
        public List<Classificacao> Classificacoes { get; set; } = new();
    }

}
