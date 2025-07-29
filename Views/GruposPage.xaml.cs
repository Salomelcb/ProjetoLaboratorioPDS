using GestaoAvaliacoes.Data;
using GestaoAvaliacoes.Model;
using GestaoAvaliacoes.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace GestaoAvaliacoes.Views
{
    public partial class GruposPage : Page
    {
        public ObservableCollection<Grupo> Grupos { get; set; }
        public List<Aluno> TodosOsAlunos { get; set; }

        public GruposPage()
        {
            InitializeComponent();
            Grupos = new ObservableCollection<Grupo>();
            TodosOsAlunos = new List<Aluno>();
            this.DataContext = this;
            CarregarDadosIniciais();
        }

        private void CarregarDadosIniciais()
        {
            TodosOsAlunos = AlunoStorage.CarregarAlunos();
            var gruposCarregados = GrupoStorage.CarregarGrupos();
            Grupos.Clear();

            foreach (var grupo in gruposCarregados)
            {
                grupo.Alunos = TodosOsAlunos
                    .Where(a => a.GrupoID == grupo.ID)
                    .ToList();

                Grupos.Add(grupo);
            }


            foreach (var aluno in TodosOsAlunos)
            {
                if (!Grupos.Any(g => g.Alunos.Any(a => a.Numero == aluno.Numero)))
                {
                    aluno.GrupoID = null;
                }
            }
            AlunoStorage.GuardarAlunos(TodosOsAlunos);
            AtualizarDataGrid();

        }

        private void AtualizarDataGrid()
        {
            string termo = SearchBoxGrupo.Text.ToLower().Trim();
            var gruposFiltrados = Grupos.Where(g =>
                g.Nome.ToLower().Contains(termo) ||
                g.ID.ToString().Contains(termo) ||
                g.Alunos.Any(a => a.Nome.ToLower().Contains(termo))
            ).ToList();

            dgGrupos.ItemsSource = null;
            dgGrupos.ItemsSource = gruposFiltrados;

            txtAvisoGrupo.Text = gruposFiltrados.Count == 0 && !string.IsNullOrWhiteSpace(termo)
                ? "Nenhum grupo encontrado."
                : string.Empty;
        }

        private void AddGrupo_Click(object sender, RoutedEventArgs e)
        {
            txtAvisoGrupo.Text = string.Empty;
            var addWindow = new AddGrupo(TodosOsAlunos, Grupos.ToList());

            if (addWindow.ShowDialog() == true)
            {
                int novoId = Grupos.Any() ? Grupos.Max(g => g.ID) + 1 : 1;

                var novoGrupo = addWindow.ObterGrupoCriado();
                novoGrupo.ID = novoId;
                novoGrupo.Nome = $"Grupo {novoId}";

                Grupos.Add(novoGrupo);


                foreach (var alunoNoNovoGrupo in novoGrupo.Alunos)
                {
                    var alunoGlobal = TodosOsAlunos.FirstOrDefault(a => a.Numero == alunoNoNovoGrupo.Numero);
                    if (alunoGlobal != null)
                    {
                        alunoGlobal.GrupoID = novoGrupo.ID;
                    }
                }

                GrupoStorage.GuardarGrupos(Grupos.ToList());
                AlunoStorage.GuardarAlunos(TodosOsAlunos);

                AtualizarDataGrid();
                txtAvisoGrupo.Text = "Grupo adicionado com sucesso!";
            }
        }

        private void EditarGrupo_Click(object sender, RoutedEventArgs e)
        {
            txtAvisoGrupo.Text = string.Empty;
            var button = sender as Button;
            var grupoSelecionado = button?.DataContext as Grupo;

            if (grupoSelecionado == null)
            {
                txtAvisoGrupo.Text = "Selecione um grupo para editar.";
                return;
            }

            var editWindow = new EditGrupo(TodosOsAlunos, grupoSelecionado, Grupos.ToList());
            if (editWindow.ShowDialog() == true) 
            {
                foreach (var aluno in TodosOsAlunos)
                {
                    if (grupoSelecionado.Alunos.Any(a => a.Numero == aluno.Numero))
                    {
                        aluno.GrupoID = grupoSelecionado.ID;
                    }
                    else if (aluno.GrupoID == grupoSelecionado.ID && !grupoSelecionado.Alunos.Any(a => a.Numero == aluno.Numero))
                    {
                        aluno.GrupoID = null;
                    }
                }
                GrupoStorage.GuardarGrupos(Grupos.ToList());
                AlunoStorage.GuardarAlunos(TodosOsAlunos);
                AtualizarDataGrid();
                txtAvisoGrupo.Text = "Grupo editado com sucesso!";
            }
        }

        private void SearchBoxGrupo_TextChanged(object sender, TextChangedEventArgs e)
        {
            AtualizarDataGrid();
        }


        private void CarregarGrupos_Click(object sender, RoutedEventArgs e)
        {
            AtualizarDataGrid();
        }

        private void dgGrupos_Loaded(object sender, RoutedEventArgs e)
        {
            AtualizarDataGrid();
        }
        private void EliminarGrupo_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var grupoParaEliminar = button?.DataContext as Grupo;

            if (grupoParaEliminar == null)
            {
                txtAvisoGrupo.Text = "Selecione um grupo para eliminar.";
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Tem certeza que deseja eliminar o grupo '{grupoParaEliminar.Nome}' (ID: {grupoParaEliminar.ID})?\n" +
                "Todos os alunos associados a este grupo terão a sua associação removida.",
                "Confirmar Eliminação",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Grupos.Remove(grupoParaEliminar);

                foreach (var aluno in TodosOsAlunos)
                {
                    if (aluno.GrupoID == grupoParaEliminar.ID)
                    {
                        aluno.GrupoID = null;
                    }
                }

                // Reatribuir IDs sequenciais
                int novoId = 1;
                foreach (var grupo in Grupos.OrderBy(g => g.ID).ToList())
                {
                    grupo.ID = novoId;
                    grupo.Nome = $"Grupo {novoId}";

                    foreach (var aluno in grupo.Alunos)
                    {
                        aluno.GrupoID = grupo.ID;
                    }

                    novoId++;
                }

                GrupoStorage.GuardarGrupos(Grupos.ToList());
                AlunoStorage.GuardarAlunos(TodosOsAlunos);

                AtualizarDataGrid();
                txtAvisoGrupo.Text = "Grupo eliminado com sucesso!";
            }
        }
    }
}