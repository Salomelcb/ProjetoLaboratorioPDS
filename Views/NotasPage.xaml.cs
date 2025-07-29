using GestaoAvaliacoes.Data;
using GestaoAvaliacoes.Model;
using GestaoAvaliacoes.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GestaoAvaliacoes.Views
{
    public partial class NotasPage : Page
    {
        private List<Tarefa> tarefas;
        private List<Grupo> grupos;
        private List<Aluno> alunos;
        private List<Classificacao> classificacoes;

        private List<NotasVM> alunosSemGrupoVM = new(); 

        public NotasPage()
        {
            InitializeComponent();
            CarregarNotas();
        }

        private void CarregarNotas()
        {
            alunos = AlunoStorage.CarregarAlunos();
            grupos = GrupoStorage.CarregarGrupos();
            tarefas = TarefaStorage.CarregarTarefas();
            classificacoes = NotasStorage.CarregarNotas();

            if (tarefas.Count == 0 || alunos.Count == 0)
            {
                txtAviso.Text = "Sem dados de tarefas ou alunos.";
                dgNotas.ItemsSource = null;
                return;
            }

            var classificacaoLookup = classificacoes
                .Where(c => c.Valor.HasValue)
                .ToDictionary(c => (c.AlunoId, c.TarefaId), c => c.Valor!.Value);

            var pesosPorTarefa = tarefas.ToDictionary(t => t.Titulo, t => t.Peso);

            var grelha = alunos
                .Where(a => a.GrupoID != null)
                .GroupBy(a => a.GrupoID)
                .SelectMany(grupoAlunos =>
                {
                    var grupo = grupos.FirstOrDefault(g => g.ID == grupoAlunos.Key);
                    string nomeGrupo = grupo?.Nome ?? "Grupo Desconhecido";

                    return grupoAlunos.Select((aluno, index) =>
                    {
                        var notasPorTarefa = new Dictionary<string, double?>();
                        var isExcecao = new Dictionary<string, bool>();

                        foreach (var tarefa in tarefas)
                        {
                            classificacaoLookup.TryGetValue((aluno.Numero, tarefa.Id), out var valor);
                            notasPorTarefa[tarefa.Titulo] = valor;

                            var grupoNotas = grupo?.Alunos
                                .Select(a =>
                                    classificacaoLookup.TryGetValue((a.Numero, tarefa.Id), out var gv)
                                        ? (double?)System.Math.Round(gv, 1)
                                        : null)
                                .Where(v => v != null)
                                .Select(v => v.Value)
                                .ToList() ?? new List<double>();

                            isExcecao[tarefa.Titulo] = grupoNotas.Distinct().Count() > 1;
                        }

                        return new NotasVM(pesosPorTarefa)
                        {
                            GrupoNome = nomeGrupo,
                            NomeAluno = aluno.Nome,
                            NotasPorTarefa = notasPorTarefa,
                            IsExcecao = isExcecao,
                            MostrarGrupo = index == 0
                        };
                    });
                })
                .OrderBy(vm =>
                {
                    var partes = vm.GrupoNome.Split(' ');
                    return int.TryParse(partes.Last(), out int num) ? num : int.MaxValue;
                })
                .ToList();

            // Remove colunas dinâmicas antigas
            for (int i = dgNotas.Columns.Count - 1; i >= 0; i--)
            {
                if (dgNotas.Columns[i].Header is string header &&
                    tarefas.Any(t => t.Titulo == header))
                {
                    dgNotas.Columns.RemoveAt(i);
                }
            }

            // Adiciona novas colunas dinâmicas
            int insertIndex = 2;
            foreach (var tarefa in tarefas)
            {
                var coluna = new DataGridTextColumn
                {
                    Header = tarefa.Titulo,
                    Binding = new Binding
                    {
                        Path = new PropertyPath($"NotasPorTarefa[{tarefa.Titulo}]"),
                        Mode = BindingMode.OneWay,
                        StringFormat = "F1"
                    },
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters =
                        {
                            new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center),
                            new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center),
                            new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center)
                        }
                    }
                };

                dgNotas.Columns.Insert(insertIndex++, coluna);
            }

            dgNotas.ItemsSource = grelha;
            txtAviso.Text = grelha.Count == 0 ? "Nenhum aluno para mostrar." : "";
        }

        private void AbrirSemGrupo_Click(object sender, RoutedEventArgs e)
        {
            var pesosPorTarefa = tarefas.ToDictionary(t => t.Titulo, t => t.Peso);

            alunosSemGrupoVM = alunos
                .Where(a => a.GrupoID == null)
                .Select(aluno =>
                {
                    var notasPorTarefa = new Dictionary<string, double?>();
                    foreach (var tarefa in tarefas)
                    {
                        var classificacao = classificacoes.FirstOrDefault(c => c.TarefaId == tarefa.Id && c.AlunoId == aluno.Numero);
                        if (classificacao?.Valor != null)
                            notasPorTarefa[tarefa.Titulo] = classificacao.Valor;
                    }

                    return new NotasVM(pesosPorTarefa)
                    {
                        GrupoNome = "Sem Grupo",
                        NomeAluno = aluno.Nome,
                        NotasPorTarefa = notasPorTarefa,
                        IsExcecao = new(),
                        MostrarGrupo = true
                    };
                })
                .ToList();

            var janela = new GrelhaSemGrupo(alunosSemGrupoVM, tarefas.Select(t => t.Titulo).ToList());
            janela.ShowDialog();
        }

        private void AbrirHistograma_Click(object sender, RoutedEventArgs e)
        {
            var alunosComGrupo = dgNotas.ItemsSource as IEnumerable<NotasVM> ?? Enumerable.Empty<NotasVM>();
            var todosAlunos = alunosComGrupo.Concat(alunosSemGrupoVM ?? Enumerable.Empty<NotasVM>());

            var notas = todosAlunos
                .Where(a => a.MediaFinal.HasValue)
                .Select(a => a.MediaFinal.Value);

            var histo = new HistogramaNotas(notas);
            histo.ShowDialog();
        }

        private void EditarGrupo_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is NotasVM vm)
            {
                var grupo = grupos.FirstOrDefault(g => g.Nome == vm.GrupoNome);
                if (grupo == null)
                {
                    txtAviso.Text = "Grupo não encontrado.";
                    return;
                }

                var classificacoes = NotasStorage.CarregarNotas();
                var janela = new EditGrupoNota(grupo, tarefas, classificacoes);
                if (janela.ShowDialog() == true)
                {
                    CarregarNotas();
                    txtAviso.Text = "Notas atualizadas com sucesso.";
                }
            }
        }
    }
}
