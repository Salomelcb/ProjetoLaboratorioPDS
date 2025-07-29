using GestaoAvaliacoes.Model;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.IO;
using GestaoAvaliacoes.Data;

namespace GestaoAvaliacoes.Views
{
    public partial class AlunosPage : Page
    {
        private ObservableCollection<Aluno> alunos = new ObservableCollection<Aluno>();
        public AlunosPage()
        {
            InitializeComponent();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var alunosCarregados = AlunoStorage.CarregarAlunos();
            foreach (var aluno in alunosCarregados)
                alunos.Add(aluno);

            dgAlunos.ItemsSource = alunos;
        }


        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string termo = SearchBox.Text.ToLower();

            if (string.IsNullOrWhiteSpace(termo))
            {
                dgAlunos.ItemsSource = alunos;
            }
            else
            {
                var filtrados = alunos.Where(a =>
                    a.Nome.ToLower().Contains(termo) ||
                    a.Email.ToLower().Contains(termo) ||
                    a.Numero.ToString().Contains(termo)).ToList();

                dgAlunos.ItemsSource = filtrados;
            }
        }

        private void AddAluno_Click(object sender, RoutedEventArgs e)
        {
            var janela = new AddAluno(alunos.ToList());
            bool? resultado = janela.ShowDialog();
            if (resultado == true && janela.NovoAluno != null)
            {
                alunos.Add(janela.NovoAluno);
                OrdenarAlunos();
                AlunoStorage.GuardarAlunos(alunos.ToList());
                if (string.IsNullOrWhiteSpace(SearchBox.Text))
                {
                    dgAlunos.ItemsSource = alunos;
                }
                else
                {
                    SearchBox_TextChanged(null, null); 
                }
            }
        }

        private void CarregarLista_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Ficheiros CSV (*.csv)|*.csv",
                Title = "Selecionar Lista de Alunos"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string[] linhas = File.ReadAllLines(openFileDialog.FileName, Encoding.GetEncoding("Windows-1252"));
                    List<string> duplicados = new();
                    int adicionados = 0;

                    for (int i = 1; i < linhas.Length; i++) // Ignora cabeçalho
                    {
                        var partes = linhas[i].Split(',');

                        if (partes.Length >= 3 &&
                            int.TryParse(partes[1].Trim(), out int numero))
                        {
                            if (!alunos.Any(a => a.Numero == numero))
                            {
                                alunos.Add(new Aluno
                                {
                                    Nome = StringUtils.CapitalizarNome(partes[0].Trim()),
                                    Numero = numero,
                                    Email = partes[2].Trim()
                                });
                                adicionados++;
                            }
                            else
                            {
                                duplicados.Add(partes[0].Trim());
                            }
                        }
                    }

                    OrdenarAlunos();
                    AlunoStorage.GuardarAlunos(alunos.ToList());

                    string mensagem = $"Foram adicionados {adicionados} alunos.";
                    if (duplicados.Any())
                    {
                        mensagem += $"\n{duplicados.Count} aluno{(duplicados.Count == 1 ? "" : "s")} já existiam e foram ignorados.";
                    }

                    txtAviso.Text = mensagem;
                }
                catch (Exception ex)
                {
                    txtAviso.Text = "Erro ao carregar ficheiro: " + ex.Message;
                }
            }
        }

        private void EditarAluno_Click(object sender, RoutedEventArgs e)
        {
            var alunoSelecionado = dgAlunos.SelectedItem as Aluno;
            if (alunoSelecionado == null) return;

            var janelaEditar = new EditAluno(alunoSelecionado, alunos.ToList());
            bool? resultado = janelaEditar.ShowDialog();

            if (resultado == true)
            {
                dgAlunos.Items.Refresh();
                OrdenarAlunos();
                AlunoStorage.GuardarAlunos(alunos.ToList());
                if (string.IsNullOrWhiteSpace(SearchBox.Text))
                {
                    dgAlunos.ItemsSource = alunos;
                }
                else
                {
                    SearchBox_TextChanged(null, null); 
                }
            }
        }


        private void EliminarAluno_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var aluno = button?.DataContext as Aluno;

            if (aluno != null)
            {
                var classificacoes = NotasStorage.CarregarNotas();
                bool temNotas = classificacoes.Any(c => c.AlunoId == aluno.Numero);

                if (temNotas)
                {
                    txtAviso.Text = $"❌ O aluno \"{aluno.Nome}\" tem notas atribuídas e não pode ser eliminado.";
                    return;
                }

                var confirm = MessageBox.Show($"Deseja mesmo eliminar o aluno {aluno.Nome}?",
                                              "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (confirm == MessageBoxResult.Yes)
                {
                    alunos.Remove(aluno);

                    var grupos = GrupoStorage.CarregarGrupos();
                    foreach (var grupo in grupos)
                    {
                        grupo.Alunos.RemoveAll(a => a.Numero == aluno.Numero);
                    }

                    GrupoStorage.GuardarGrupos(grupos);
                    AlunoStorage.GuardarAlunos(alunos.ToList());

                    OrdenarAlunos();
                    dgAlunos.ItemsSource = string.IsNullOrWhiteSpace(SearchBox.Text) ? alunos : alunos.Where(a =>
                        a.Nome.ToLower().Contains(SearchBox.Text.ToLower()) ||
                        a.Email.ToLower().Contains(SearchBox.Text.ToLower()) ||
                        a.Numero.ToString().Contains(SearchBox.Text)).ToList();

                    txtAviso.Text = "Aluno eliminado com sucesso.";
                }
            }
        }


        private void OrdenarAlunos()
        {
            var ordenados = alunos
                .OrderBy(a => a.Numero)
                .ToList();

            alunos.Clear();
            foreach (var aluno in ordenados)
                alunos.Add(aluno);
        }

        private void dgAlunos_Loaded(object sender, RoutedEventArgs e)
        {
            dgAlunos.Items.SortDescriptions.Clear();
            dgAlunos.Items.SortDescriptions.Add(
                new System.ComponentModel.SortDescription("Numero", System.ComponentModel.ListSortDirection.Ascending));
            dgAlunos.Items.Refresh();
        }

    }

    public static class StringUtils
    {
        public static string CapitalizarNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return nome;

            return string.Join(" ", nome
                .ToLower()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(palavra => char.ToUpper(palavra[0]) + palavra.Substring(1)));
        }
    }

}