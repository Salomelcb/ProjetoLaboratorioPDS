using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacoes.Model
{
    public class Grupo
    {
        public int ID { get; set; }
        public string Nome { get; set; }
        public List<Aluno> Alunos { get; set; } = new();

        public string AlunosNomes => string.Join(", ", Alunos?.Select(a => a.Nome) ?? Enumerable.Empty<string>());
    }


}
