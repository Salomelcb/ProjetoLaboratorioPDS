using GestaoAvaliacoes.Model;

namespace GestaoAvaliacoes.ViewModels
{
    public class TarefaNotaVM
    {
        public Tarefa Tarefa { get; set; }
        public string Titulo { get; set; } = "";

        public double Nota { get; set; }

        public double NotaGrupoOriginal { get; set; }

        private string _notaTexto = "";

        public string NotaTexto
        {
            get => _notaTexto;
            set
            {
                _notaTexto = value;
                if (double.TryParse(value.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double result))
                    Nota = result;
            }
        }



        public Dictionary<int, double> Excecoes { get; set; } = new();

        public bool TemExcecoes => Excecoes.Count > 0;
        public bool ExcecoesAtivas => ManterExcecoes && Excecoes.Any();

        public bool ManterExcecoes { get; set; } = true;
    }
}
