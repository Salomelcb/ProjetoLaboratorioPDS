using GestaoAvaliacoes.Data;
using GestaoAvaliacoes.Model;
using GestaoAvaliacoes.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GestaoAvaliacoes.Views
{
    public partial class EditGrupoNota : Window
    {
        public Grupo GrupoSelecionado { get; set; }
        public List<TarefaNotaVM> TarefasNotas { get; set; }
        private List<Tarefa> TodasAsTarefas;
        private List<Classificacao> ClassificacoesExistentes;

        public EditGrupoNota(Grupo grupo, List<Tarefa> tarefas, List<Classificacao> classificacoesExistentes)
        {
            InitializeComponent();
            GrupoSelecionado = grupo;
            TodasAsTarefas = tarefas;
            ClassificacoesExistentes = classificacoesExistentes;

            TarefasNotas = tarefas.Select(tarefa =>
            {
                var excecoes = new Dictionary<int, double>();

                foreach (var aluno in grupo.Alunos)
                {
                    var valor = classificacoesExistentes
                        .FirstOrDefault(c => c.TarefaId == tarefa.Id && c.AlunoId == aluno.Numero)?.Valor;

                    if (valor.HasValue)
                        excecoes[aluno.Numero] = valor.Value;
                }

                var valoresGrupo = grupo.Alunos
                    .Select(a => classificacoesExistentes.FirstOrDefault(c => c.TarefaId == tarefa.Id && c.AlunoId == a.Numero)?.Valor)
                    .Where(v => v.HasValue)
                    .Select(v => Math.Round(v.Value, 1))
                    .Distinct()
                    .ToList();

                double? notaGrupoOriginal = valoresGrupo.Count == 1 ? valoresGrupo.First() : null;

                return new TarefaNotaVM
                {
                    Tarefa = tarefa,
                    Titulo = tarefa.Titulo,
                    Nota = notaGrupoOriginal ?? 0,              // valor atual 
                    NotaGrupoOriginal = notaGrupoOriginal ?? 0, // valor original
                    NotaTexto = (notaGrupoOriginal ?? 0).ToString("0.0"),

                    Excecoes = excecoes,
                    ManterExcecoes = false
                };
            }).ToList();

            NotasPorTarefaPanel.ItemsSource = TarefasNotas;
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Confirmar_Click(object sender, RoutedEventArgs e)
        {
            txtErro.Text = string.Empty;

            foreach (var item in TarefasNotas)
            {
                if (item.Nota < 0 || item.Nota > 20)
                {
                    txtErro.Text = $"Nota inválida para a tarefa '{item.Titulo}'. Deve estar entre 0 e 20.";
                    return;
                }
            }


            foreach (var item in TarefasNotas)
            {
                foreach (var aluno in GrupoSelecionado.Alunos)
                {
                    double notaFinal;

                    if (item.ManterExcecoes && item.Excecoes.ContainsKey(aluno.Numero))
                        notaFinal = item.Excecoes[aluno.Numero];
                    else
                        notaFinal = item.Nota;

                    var classificacao = ClassificacoesExistentes
                        .FirstOrDefault(c => c.AlunoId == aluno.Numero && c.TarefaId == item.Tarefa.Id);

                    if (classificacao == null)
                    {
                        ClassificacoesExistentes.Add(new Classificacao
                        {
                            AlunoId = aluno.Numero,
                            TarefaId = item.Tarefa.Id,
                            Valor = notaFinal,
                        });
                    }
                    else
                    {
                        classificacao.Valor = notaFinal;
                    }
                }
            }

            NotasStorage.GuardarNotas(ClassificacoesExistentes);
            this.DialogResult = true;
            this.Close();
        }


        private void AbrirExcecoes_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var tarefaVM = button?.Tag as TarefaNotaVM;

            if (tarefaVM != null)
            {
                var janela = new EditAlunoNota(GrupoSelecionado.Alunos, tarefaVM);
                if (janela.ShowDialog() == true)
                {
                }
            }
        }

        private void NumericOnlyInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !System.Text.RegularExpressions.Regex.IsMatch(e.Text, @"[\d,.]");
        }
    }
}
