using GestaoAvaliacoes.Model;
using System.Text.RegularExpressions;
using System.Windows;

namespace GestaoAvaliacoes.Views
{
    public partial class EditTarefa : Window
    {
        private List<Tarefa> _tarefasExistentes; 
        public Tarefa TarefaOriginal { get; private set; }

        public int TarefaID { get; set; }
        public string TarefaTitulo { get; set; }
        public string? Descricao { get; set; }
        public double Peso { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }

        public EditTarefa(Tarefa tarefaParaEditar, List<Tarefa> todasAsTarefas)
        {
            InitializeComponent();
            TarefaOriginal = tarefaParaEditar;
            _tarefasExistentes = todasAsTarefas;

            TarefaID = TarefaOriginal.Id;
            TarefaTitulo = TarefaOriginal.Titulo;
            Descricao = TarefaOriginal.Descricao ?? string.Empty; 
            Peso = TarefaOriginal.Peso;
            DataInicio = TarefaOriginal.DataInicio;
            DataFim = TarefaOriginal.DataFim;

            txtID.IsEnabled = false;
            txtTitulo.IsEnabled = false;

            this.DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtErro.Text = string.Empty; 
        }

        public Tarefa ObterTarefaEditada()
        {
            TarefaOriginal.Id = TarefaID;
            TarefaOriginal.Titulo = TarefaTitulo;
            TarefaOriginal.Descricao = txtDescricao.Text;
            TarefaOriginal.Peso = Peso;
            TarefaOriginal.DataInicio = DataInicio;
            TarefaOriginal.DataFim = DataFim;

            return TarefaOriginal;
        }
        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Confirmar_Click(object sender, RoutedEventArgs e)
        {
            txtErro.Text = string.Empty;

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

            //if (_tarefasExistentes.Any(t => t.Id != TarefaOriginal.Id && t.Titulo.Trim().Equals(TarefaTitulo.Trim(), StringComparison.OrdinalIgnoreCase)))
            //{
            //    txtErro.Text = "Já existe outra tarefa com este título. Por favor, escolha um título diferente.";
            //    return;
            //}

            /*if (string.IsNullOrWhiteSpace(Descricao))
            {
                txtErro.Text = "A descrição não pode estar vazia.";
                return;
            }pode sim senhor, mas quem manda pa*/

            if (!double.TryParse(txtPeso.Text, out double peso) || peso < 0 || peso > 100)
            {
                txtErro.Text = "O peso deve ser um número entre 0 e 100.";
                return;
            }
            Peso = peso;

            double somaPesosOutras = _tarefasExistentes
                .Where(t => t.Id != TarefaOriginal.Id)
                .Sum(t => t.Peso);

            if (somaPesosOutras + Peso > 100)
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
            }nada chefe*/

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