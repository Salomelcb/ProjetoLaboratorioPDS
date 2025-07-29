using System.Text.RegularExpressions;
using System.Windows;
using GestaoAvaliacoes.Model;

namespace GestaoAvaliacoes.Views
{
    public partial class EditPerfil : Window
    {
        private Perfil perfilOriginal;

        public EditPerfil(Perfil perfil)
        {
            InitializeComponent();
            perfilOriginal = perfil;

            txtNome.Text = perfil.Nome;
            txtEmail.Text = perfil.Email;
            txtNumeroTelefone.Text = perfil.NumeroTelefone ?? "";
        }

        private bool EmailValido(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address.ToLower().EndsWith("@utad.pt");
            }
            catch
            {
                return false;
            }
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            txtErro.Text = ""; 

            string nome = txtNome.Text.Trim();
            string email = txtEmail.Text.Trim();
            string telefone = txtNumeroTelefone.Text.Trim();

            // Verificação de campos obrigatórios
            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(email))
            {
                txtErro.Text = "Nome e Email são obrigatórios.";
                return;
            }

            // Validação de nome composto
            if (nome.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length < 2)
            {
                txtErro.Text = "Informe nome e sobrenome.";
                return;
            }

            // Validação simples de e-mail
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                txtErro.Text = "Email inválido.";
                return;
            }

            // Validação do número de telefone (opcional, mas se estiver preenchido...)
            if (!string.IsNullOrWhiteSpace(telefone))
            {
                // Permite formatos como +351 123 456 789 ou +351123456789
                if (!Regex.IsMatch(telefone, @"^\+351\s?\d{3}\s?\d{3}\s?\d{3}$"))
                {
                    txtErro.Text = "Número de telefone inválido. Ex: +351 123 456 789";
                    return;
                }
            }

            if (!EmailValido(email))
            {
                txtErro.Text = "O email deve terminar com '@utad.pt'.";

                return;
            }

            // Atualiza dados
            perfilOriginal.Nome = nome;
            perfilOriginal.Email = email;
            perfilOriginal.NumeroTelefone = string.IsNullOrWhiteSpace(telefone) ? null : telefone;

            this.DialogResult = true;
            this.Close();
        }
    }
}
