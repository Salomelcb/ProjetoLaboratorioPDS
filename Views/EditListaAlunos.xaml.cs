using GestaoAvaliacoes.Data;
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
using System.Windows.Shapes;


namespace GestaoAvaliacoes.Views
{
    public partial class EditListaAlunos : Window
    {
        public List<Aluno> ResultadoAlunosGrupo { get; private set; }
        private List<Aluno> alunosDisponiveisOriginais;
        private ObservableCollection<Aluno> todosAlunos;
        private ObservableCollection<Aluno> alunosNoGrupo;

        public EditListaAlunos(List<Aluno> todosAlunos, List<Aluno> alunosDoGrupo = null)
        {
            InitializeComponent();

            this.todosAlunos = new ObservableCollection<Aluno>(todosAlunos);
            this.alunosNoGrupo = new ObservableCollection<Aluno>(alunosDoGrupo ?? new List<Aluno>());

            AtualizarListas();
        }

        private void AtualizarListas()
        {
            var alunosDisponiveis = todosAlunos
                .Where(a => !alunosNoGrupo.Any(g => g.Numero == a.Numero))
                .ToList();
            
            alunosDisponiveisOriginais = alunosDisponiveis;

            FiltrarDisponiveis(SearchBoxDisponiveis?.Text ?? ""); 
            lstGrupo.ItemsSource = alunosNoGrupo;
        }

        private void FiltrarDisponiveis(string texto)
        {
            var filtrados = alunosDisponiveisOriginais
                .Where(a => !string.IsNullOrEmpty(a.Nome) && a.Nome.Contains(texto, StringComparison.OrdinalIgnoreCase))
                .ToList();

            lstDisponiveis.ItemsSource = new ObservableCollection<Aluno>(filtrados);
        }

        private void SearchBoxDisponiveis_TextChanged(object sender, TextChangedEventArgs e)
        {
            FiltrarDisponiveis(SearchBoxDisponiveis.Text);
        }

        private void MoverParaGrupo_Click(object sender, RoutedEventArgs e)
        {
            var selecionados = lstDisponiveis.SelectedItems.Cast<Aluno>().ToList();
            foreach (var aluno in selecionados)
            {
                alunosNoGrupo.Add(aluno);
            }
            AtualizarListas();
        }

        private void RemoverDoGrupo_Click(object sender, RoutedEventArgs e)
        {
            var selecionados = lstGrupo.SelectedItems.Cast<Aluno>().ToList();
            foreach (var aluno in selecionados)
            {
                alunosNoGrupo.Remove(aluno);
            }
            AtualizarListas();
        }


        private void Confirmar_Click(object sender, RoutedEventArgs e)
        {
            ResultadoAlunosGrupo = alunosNoGrupo.ToList();
            DialogResult = true;
            Close();
        }


        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}