namespace GestaoAvaliacoes.Model
{
    public class Tarefa
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string? Descricao { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public double Peso { get; set; }

        public List<Classificacao> Classificacoes { get; set; } = new();

        public Tarefa()
        {
            Titulo = string.Empty; 
            Classificacoes = new List<Classificacao>(); 
        }
    }
}