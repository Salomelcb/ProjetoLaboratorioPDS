using GestaoAvaliacoes.Model;
using System.Text.RegularExpressions;
using System.Windows;

namespace GestaoAvaliacoes.Views
{
    public partial class AddTarefa : Window
    {
        private List<Tarefa> _tarefasExistentes;

        public int TarefaID { get; set; }
        public string TarefaTitulo { get; set; }
        public string? Descricao { get; set; }
        public double Peso { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtErro.Text = string.Empty; 
        }

        public AddTarefa(List<Tarefa> tarefasExistentes)
        {
            InitializeComponent();
            _tarefasExistentes = tarefasExistentes;

            int novoId = _tarefasExistentes.Any() ? _tarefasExistentes.Max(t => t.Id) + 1 : 1;
            TarefaID = novoId;
            TarefaTitulo = $"Tarefa {novoId}";

            txtID.IsEnabled = false;
            txtTitulo.IsEnabled = false;
            
            Descricao = string.Empty;
            Peso = 0;
            DataInicio = DateTime.Today; // Data padrão: hoje
            DataFim = DateTime.Today.AddDays(7);
            this.DataContext = this;
        }

        public Tarefa ObterTarefaCriada()
        {
            return new Tarefa
            {
                Id = TarefaID,
                Titulo = TarefaTitulo,
                Descricao = Descricao,
                Peso = Peso,
                DataInicio = DataInicio,
                DataFim = DataFim,
                Classificacoes = new List<Classificacao>() 
            };
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Confirmar_Click(object sender, RoutedEventArgs e)
        {
            txtErro.Text = string.Empty;

            //int id;
            //if (!int.TryParse(txtID.Text, out id) || id <= 0) 
            //{
            //    txtErro.Text = "O ID deve ser um número inteiro positivo.";
            //    return;
            //}
            //TarefaID = id;

            //if (_tarefasExistentes.Any(t => t.Id == TarefaID))
            //{
            //    txtErro.Text = "Já existe uma tarefa com este ID.";
            //    return;
            //}

            //if (string.IsNullOrWhiteSpace(TarefaTitulo))
            //{
            //    txtErro.Text = "O título da tarefa não pode estar vazio.";
            //    return;
            //}

            //if (Regex.IsMatch(TarefaTitulo.Trim(), @"^\d+$"))
            //{
            //    txtErro.Text = "O título da tarefa não pode ser apenas um número.";
            //    return;
            //}

            //if (_tarefasExistentes.Any(t => t.Titulo.Trim().Equals(TarefaTitulo.Trim(), StringComparison.OrdinalIgnoreCase)))
            //{
            //    txtErro.Text = "Já existe uma tarefa com este título. Por favor, escolha um título diferente.";
            //    return;
            //}

            /*if (string.IsNullOrWhiteSpace(Descricao))
            {
                txtErro.Text = "A descrição não pode estar vazia.";
                return;
            }é opcional*/

            if (!double.TryParse(txtPeso.Text, out double peso) || peso < 0 || peso > 100)
            {
                txtErro.Text = "O peso deve ser um número entre 0 e 100.";
                return;
            }
            Peso = peso; //+e uma percentagem

            double somaPesos = _tarefasExistentes.Sum(t => t.Peso);
            if (somaPesos + Peso > 100)
            {
                txtErro.Text = $"A soma dos pesos das tarefas não pode ultrapassar 100%.";
                return;
            }

            /*if (!DataInicio.HasValue)
            {
                txtErro.Text = "Por favor, selecione uma data de início.";
                return;
            }
            if (!DataFim.HasValue)
            {
                txtErro.Text = "Por favor, selecione uma data de fim.";
                return;
            }outro opcional*/

            if (DataInicio.Value > DataFim.Value)
            {
                txtErro.Text = "A Data de Início não pode ser posterior à Data de Fim.";
                return;
            }

            if (DataFim.Value.Date < DateTime.Today.Date)
            {
                txtErro.Text = "A Data de Fim não pode ser uma data no passado.";
                return;
            }

            this.DialogResult = true;
            this.Close();
        }
    }
}