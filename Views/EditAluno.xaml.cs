using System.Windows;
using GestaoAvaliacoes.Model;

namespace GestaoAvaliacoes.Views
{
    
    public partial class EditAluno : Window
    {
        private Aluno alunoOriginal;
        private List<Aluno> todosAlunos;
        public EditAluno(Aluno aluno, List<Aluno> alunosExistentes)
        {
            InitializeComponent();
            alunoOriginal = aluno;
            todosAlunos = alunosExistentes;

            txtNome.Text = aluno.Nome;
            txtEmail.Text = aluno.Email;
            txtNumero.Text = aluno.Numero.ToString();

        }
        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ConfirmarAluno_Click(object sender, RoutedEventArgs e)
        {
            txtErro.Text = ""; 

            string nome = txtNome.Text.Trim();
            string email = txtEmail.Text.Trim();
            string numeroStr = txtNumero.Text.Trim();

            if (string.IsNullOrWhiteSpace(nome) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(numeroStr))
            {
                txtErro.Text = "Preencha todos os campos.";
                return;
            }

            if (nome.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length < 2)
            {
                txtErro.Text = "Informe nome e sobrenome.";
                return;
            }


            if (!int.TryParse(numeroStr, out int numero) || numero < 1000 || numero > 99999)
            {
                txtErro.Text = "Número inválido.";
                return;
            }

            bool emailDuplicado = todosAlunos.Any(a => a.Email == email && a != alunoOriginal);
            bool numeroDuplicado = todosAlunos.Any(a => a.Numero == numero && a != alunoOriginal);

            if (numeroDuplicado)
            {
                txtErro.Text = "Número já existe.";
                return;
            }

            if (emailDuplicado)
            {
                txtErro.Text = "Email já existe.";
                return;
            }

            // Valida e-mail com base no número
            string emailEsperado = $"al{numero}@alunos.utad.pt";
            if (email != emailEsperado)
            {
                txtErro.Text = "Email inválido. O formato deve ser: alnum@alunos.utad.pt";
                return;
            }

            // Se passou nas validações, atualiza
            alunoOriginal.Nome = nome;
            alunoOriginal.Email = email;
            alunoOriginal.Numero = numero;

            this.DialogResult = true;
            this.Close();
        }
    }
}