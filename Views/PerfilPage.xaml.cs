
using System.IO;
using System.Windows;
using System.Windows.Controls;
using GestaoAvaliacoes.Model;
using Microsoft.Win32;
using GestaoAvaliacoes.Data;

namespace GestaoAvaliacoes.Views
{
    public partial class PerfilPage : Page
    {
        private Perfil perfilAtual;
        public PerfilPage()
        {
            InitializeComponent();
            perfilAtual = PerfilStorage.CarregarPerfil();

            if (perfilAtual == null)
            {
                // Obtém o nome do utilizador do Windows
                string nomeRaw = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                string nome = nomeRaw.Contains("\\") ? nomeRaw.Split('\\')[1] : nomeRaw;

                perfilAtual = new Perfil
                {
                    Nome = nome,
                    Email = GerarEmail(nome),
                    FotografiaPath = "/Assets/default_profile.png",
                    NumeroTelefone = null,
                };

                PerfilStorage.SalvarPerfil(perfilAtual);
            }

            this.DataContext = perfilAtual;
        }


        private void EditarFoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Selecionar nova foto de perfil",
                Filter = "Imagens (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string imagemSelecionada = openFileDialog.FileName;

                string pastaDestino = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fotos");
                if (!Directory.Exists(pastaDestino))
                    Directory.CreateDirectory(pastaDestino);

                string nomeFicheiro = Path.GetFileName(imagemSelecionada);
                string caminhoDestino = Path.Combine(pastaDestino, nomeFicheiro);

                File.Copy(imagemSelecionada, caminhoDestino, true); 

                perfilAtual.FotografiaPath = caminhoDestino;
                this.DataContext = null;
                this.DataContext = perfilAtual;
                PerfilStorage.SalvarPerfil(perfilAtual);
            }
        }

        private string GerarEmail(string nomeCompleto)
        {
            if (string.IsNullOrWhiteSpace(nomeCompleto))
                return "";

            var partes = nomeCompleto.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (partes.Length < 2)
                return ""; //  inválido

            string inicialPrimeiro = partes[0].Substring(0, 1).ToLower();
            string inicialUltimo = partes[^1].Substring(0, 1).ToLower();

            return $"{inicialPrimeiro}{inicialUltimo}@utad.pt";
        }

        private void EditarPerfil_Click(object sender, RoutedEventArgs e)
        {
            EditPerfil janelaEdicao = new EditPerfil(perfilAtual);
            bool? resultado = janelaEdicao.ShowDialog();

            if (resultado == true)
            {
                perfilAtual.Email = GerarEmail(perfilAtual.Nome);

                // Atualiza
                this.DataContext = null;
                this.DataContext = perfilAtual;

                PerfilStorage.SalvarPerfil(perfilAtual);
            }
        }

    }
}
