using GestaoAvaliacoes.Model;
using GestaoAvaliacoes.ViewModels;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace GestaoAvaliacoes.Views
{
    public partial class EditAlunoNota : Window
    {
        private List<AlunoNotaVM> AlunosNotas;
        private TarefaNotaVM TarefaVM;

        public EditAlunoNota(List<Aluno> alunos, TarefaNotaVM tarefaVM)
        {
            InitializeComponent();
            TarefaVM = tarefaVM;

            AlunosNotas = alunos.Select(aluno => new AlunoNotaVM
            {
                Numero = aluno.Numero,
                Nome = aluno.Nome,
                NotaTexto = tarefaVM.Excecoes.TryGetValue(aluno.Numero, out double nota)
                    ? nota.ToString("0.0", CultureInfo.InvariantCulture)
                    : tarefaVM.Nota.ToString("0.0", CultureInfo.InvariantCulture) 
            }).ToList();

            AlunosPanel.ItemsSource = AlunosNotas;
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Confirmar_Click(object sender, RoutedEventArgs e)
        {
            txtErro.Text = string.Empty;
            //TarefaVM.Excecoes.Clear();

            foreach (var alunoNota in AlunosNotas)
            {
                if (!string.IsNullOrWhiteSpace(alunoNota.NotaTexto))
                {
                    if (double.TryParse(alunoNota.NotaTexto.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double nota))
                    {
                        if (nota < 0 || nota > 20)
                        {
                            txtErro.Text = $"Nota inválida para {alunoNota.Nome}. Deve estar entre 0 e 20.";
                            return;
                        }

                        TarefaVM.Excecoes[alunoNota.Numero] = nota;
                    }
                    else
                    {
                        txtErro.Text = $"Nota inválida para {alunoNota.Nome}. Deve ser um número válido.";
                        return;
                    }
                }
            }

            this.DialogResult = true;
            this.Close();
        }

        private void NumericOnlyInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !System.Text.RegularExpressions.Regex.IsMatch(e.Text, @"[\d,.]");
        }

    }
}