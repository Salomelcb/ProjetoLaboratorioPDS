using GestaoAvaliacoes.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace GestaoAvaliacoes.Views
{
    public partial class AddAluno : Window
    {
        private readonly List<Aluno> alunosExistentes;

        public AddAluno(List<Aluno> alunosAtuais)
        {
            InitializeComponent();
            alunosExistentes = alunosAtuais;
        }

        public Aluno NovoAluno { get; private set; }

        private void Barra_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ConfirmarAluno_Click(object sender, RoutedEventArgs e)
        {
            string nome = txtNome.Text.Trim();
            string email = txtEmail.Text.Trim();
            string numeroStr = txtNumero.Text.Trim();

            txtErro.Text = "";

            if (string.IsNullOrWhiteSpace(nome) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(numeroStr))
            {
                txtErro.Text = "Todos os campos são obrigatórios.";
                return;
            }

            if (!nome.Contains(" "))
            {
                txtErro.Text = "O nome deve conter nome próprio e último nome.";
                return;
            }

            if (!int.TryParse(numeroStr, out int numero) || numero < 1000 || numero > 99999)
            {
                txtErro.Text = "Número de aluno inválido. Ex: 92456.";
                return;
            }

            // Verificação
            if (alunosExistentes.Any(a => a.Numero == numero))
            {
                txtErro.Text = "Já existe um aluno com esse número.";
                return;
            }

            string emailEsperado = $"al{numero}@alunos.utad.pt";
            if (email != emailEsperado)
            {
                txtErro.Text = "Email inválido. O formato deve ser: alnum@alunos.utad.pt";
                return;
            }

            NovoAluno = new Aluno
            {
                Nome = nome,
                Email = email,
                Numero = numero
            };

            this.DialogResult = true;
            this.Close();
            MessageBox.Show("Aluno adicionado com sucesso!");
        }
    }
}
