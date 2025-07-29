namespace GestaoAvaliacoes.Model
{
    public class Classificacao
    {
        public int Id { get; set; }
        public double? Valor { get; set; }
        public int AlunoId { get; set; }  
        public int TarefaId { get; set; }  
        public Aluno Aluno { get; set; }  
        public Tarefa Tarefa { get; set; }  
    }
}
