using GestaoAvaliacoes.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GestaoAvaliacoes.Data;

namespace GestaoAvaliacoes.Views
{
    public partial class TarefasPage : Page
    {
        public ObservableCollection<Tarefa> Tarefas { get; set; }

        public TarefasPage()
        {
            InitializeComponent();
            Tarefas = new ObservableCollection<Tarefa>();
            this.DataContext = this;
            CarregarDadosIniciais();
        }

        private void CarregarDadosIniciais()
        {
            var tarefasCarregadas = TarefaStorage.CarregarTarefas(); 
            Tarefas.Clear();

            foreach (var tarefa in tarefasCarregadas)
            {
                Tarefas.Add(tarefa);
            }

            AtualizarDataGrid();
        }

        private void AtualizarDataGrid()
        {
            string termo = SearchBoxTarefa.Text.ToLower().Trim();

            var tarefasFiltradas = Tarefas.Where(t =>
                t.Titulo.ToLower().Contains(termo) ||
                t.Descricao?.ToLower().Contains(termo) == true ||
                t.Id.ToString().Contains(termo) ||
                t.Peso.ToString().Contains(termo)
            ).ToList();

            dgTarefas.ItemsSource = null;
            dgTarefas.ItemsSource = tarefasFiltradas;

            txtAvisoTarefa.Text = tarefasFiltradas.Count == 0 && !string.IsNullOrWhiteSpace(termo)
                ? "Nenhuma tarefa encontrada."
                : string.Empty;
        }

        private void AddTarefa_Click(object sender, RoutedEventArgs e)
        {
            txtAvisoTarefa.Text = string.Empty;

            var addWindow = new AddTarefa(Tarefas.ToList());

            if (addWindow.ShowDialog() == true)
            {
                var novaTarefa = addWindow.ObterTarefaCriada();
                Tarefas.Add(novaTarefa);
                TarefaStorage.GuardarTarefas(Tarefas.ToList());
                AtualizarDataGrid();
                txtAvisoTarefa.Text = "Tarefa adicionada com sucesso!";
            }
        }

        private void EditTarefa_Click(object sender, RoutedEventArgs e)
        {
            txtAvisoTarefa.Text = string.Empty;

            var button = sender as Button;
            var tarefaSelecionada = button?.DataContext as Tarefa;

            if (tarefaSelecionada == null)
            {
                txtAvisoTarefa.Text = "Selecione uma tarefa para editar.";
                return;
            }

            var editWindow = new EditTarefa(tarefaSelecionada, Tarefas.ToList());

            if (editWindow.ShowDialog() == true)
            {
                var tarefaEditada = editWindow.ObterTarefaEditada();

                var tarefaOriginal = Tarefas.FirstOrDefault(t => t.Id == tarefaEditada.Id);
                if (tarefaOriginal != null)
                {
                    tarefaOriginal.Titulo = tarefaEditada.Titulo;
                    tarefaOriginal.Descricao = tarefaEditada.Descricao;
                    tarefaOriginal.Peso = tarefaEditada.Peso;
                    tarefaOriginal.DataInicio = tarefaEditada.DataInicio;
                    tarefaOriginal.DataFim = tarefaEditada.DataFim;
                }

                TarefaStorage.GuardarTarefas(Tarefas.ToList());
                AtualizarDataGrid();
                txtAvisoTarefa.Text = "Tarefa editada com sucesso!";
            }

        }
        private void EliminarTarefa_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var tarefaParaEliminar = button?.DataContext as Tarefa;

            if (tarefaParaEliminar == null)
            {
                txtAvisoTarefa.Text = "Selecione uma tarefa para eliminar.";
                return;
            }

            var classificacoes = NotasStorage.CarregarNotas();
            bool tarefaTemNotas = classificacoes.Any(c => c.TarefaId == tarefaParaEliminar.Id);

            if (tarefaTemNotas)
            {
                txtAvisoTarefa.Text = $"❌ A tarefa \"{tarefaParaEliminar.Titulo}\" tem notas atribuídas e não pode ser eliminada.";
                return;
            }

            var result = MessageBox.Show(
                $"Tem certeza que deseja eliminar a tarefa '{tarefaParaEliminar.Titulo}' (ID: {tarefaParaEliminar.Id})?",
                "Confirmar Eliminação",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Tarefas.Remove(tarefaParaEliminar);

                int novoId = 1;
                foreach (var tarefa in Tarefas.OrderBy(t => t.Id))
                {
                    tarefa.Id = novoId;
                    tarefa.Titulo = $"Tarefa {novoId}";
                    novoId++;
                }

                TarefaStorage.GuardarTarefas(Tarefas.ToList());
                AtualizarDataGrid();
                txtAvisoTarefa.Text = "Tarefa eliminada com sucesso!";
            }
        }



        private void SearchBoxTarefa_TextChanged(object sender, TextChangedEventArgs e)
        {
            AtualizarDataGrid();
        }

        private void dgTarefas_Loaded(object sender, RoutedEventArgs e)
        {
            AtualizarDataGrid();
        }
    }
}