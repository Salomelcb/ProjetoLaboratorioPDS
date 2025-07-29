namespace GestaoAvaliacoes.ViewModels
{
    public class NotasVM
    {
        private readonly Dictionary<string, double> pesosPorTarefa;

        public NotasVM(Dictionary<string, double> pesos)
        {
            pesosPorTarefa = pesos;
        }

        public string GrupoNome { get; set; }
        public string NomeAluno { get; set; }

        public Dictionary<string, double?> NotasPorTarefa { get; set; } = new();
        public Dictionary<string, bool> IsExcecao { get; set; } = new();

        public string NomeAlunoCurto
        {
            get
            {
                if (string.IsNullOrWhiteSpace(NomeAluno)) return "";
                var partes = NomeAluno.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (partes.Length == 1) return partes[0];
                return $"{partes[0]} {partes[^1]}";
            }
        }

        public double? MediaFinal
        {
            get
            {
                if (NotasPorTarefa.Count == 0) return null;

                double somaPesada = 0;
                double somaPesos = 0;

                foreach (var par in NotasPorTarefa)
                {
                    if (!par.Value.HasValue) continue;

                    if (pesosPorTarefa.TryGetValue(par.Key, out double peso))
                    {
                        somaPesada += par.Value.Value * peso;
                        somaPesos += peso;
                    }
                }

                return somaPesos > 0 ? Math.Round(somaPesada / somaPesos, 1) : null;
            }
        }

        public bool MostrarGrupo { get; set; }
    }
}
