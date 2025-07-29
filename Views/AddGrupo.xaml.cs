using GestaoAvaliacoes.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class AddGrupo : Window
    {
        private List<Aluno> _todosAlunosDisponiveis; // Alunos carregados
        private List<Grupo> _todosOsGruposExistentes; // lista

        public int GrupoID { get; set; }
        public string GrupoNome { get; set; }
        public ObservableCollection<Aluno> AlunosNoGrupo { get; set; }

        public AddGrupo(List<Aluno> alunosDisponiveis, List<Grupo> todosOsGruposAtuais)
        {
            InitializeComponent();

            _todosOsGruposExistentes = todosOsGruposAtuais;
            int novoId = _todosOsGruposExistentes.Any() ? _todosOsGruposExistentes.Max(g => g.ID) + 1 : 1;
            GrupoID = novoId;
            GrupoNome = $"Grupo {novoId}";

            txtID.IsEnabled = false;
            txtNome.IsEnabled = false;

            _todosAlunosDisponiveis = alunosDisponiveis;

            //GrupoID = 0; 
            //GrupoNome = string.Empty;
            AlunosNoGrupo = new ObservableCollection<Aluno>(); // Começa vazia ao adicionar um novo grupo

            this.DataContext = this;
            txtErro.Text = string.Empty;
        }

        public Grupo ObterGrupoCriado()
        {
            var novoGrupo = new Grupo
            {
                ID = GrupoID,
                Nome = GrupoNome,
                Alunos = AlunosNoGrupo.ToList()
            };
            return novoGrupo;
        }

        private void EditarAlunos_Click(object sender, RoutedEventArgs e)
        {
            txtErro.Text = string.Empty;
            var alunosAtuaisNoGrupo = AlunosNoGrupo.ToList();

            var editWindow = new EditListaAlunos(_todosAlunosDisponiveis, alunosAtuaisNoGrupo);

            if (editWindow.ShowDialog() == true)
            {
                AlunosNoGrupo.Clear();
                foreach (var aluno in editWindow.ResultadoAlunosGrupo)
                {
                    AlunosNoGrupo.Add(aluno);
                }
            }
        }
        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Confirmar_Click(object sender, RoutedEventArgs e)
        {
            txtErro.Text = string.Empty;

            /*if (string.IsNullOrWhiteSpace(GrupoNome))
            {
                txtErro.Text = "O nome do grupo não pode estar vazio.";
                return;
            }

            if (Regex.IsMatch(GrupoNome.Trim(), @"^\d+$")) // Verifica se contém apenas dígitos
            {
                txtErro.Text = "O nome do grupo não pode ser apenas um número.";
                return;
            }

            if (_todosOsGruposExistentes.Any(g => g.Nome.Trim().Equals(GrupoNome.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                txtErro.Text = "Já existe um grupo com este nome. Por favor, escolha um nome diferente.";
                return;
            }

            if (!int.TryParse(txtID.Text, out int id) || id <= 0)
            {
                txtErro.Text = "O ID deve ser um número inteiro positivo válido.";
                return;
            }*/
            //GrupoID = id;

            //0-N Alunos no grupo
            //if (AlunosNoGrupo.Count < 2)
            //{
            //    txtErro.Text = "Um grupo deve ter no mínimo 2 alunos.";
            //    return;
            //}
            //if (AlunosNoGrupo.Count > 4)
            //{
            //    txtErro.Text = "Um grupo pode ter no máximo 4 alunos.";
            //    return;
            //}

            if (_todosOsGruposExistentes.Any(g => g.ID == GrupoID))
            {
                txtErro.Text = "Este ID já está em uso por outro grupo.";
                return;
            }

            if (!ValidarAlunosUnicosEmGrupos(AlunosNoGrupo.ToList(), 0, _todosOsGruposExistentes))
            {
                txtErro.Text = "Um ou mais alunos selecionados já pertencem a outro grupo.";
                return;
            }

            this.DialogResult = true;
            this.Close();
        }

        private bool ValidarAlunosUnicosEmGrupos(List<Aluno> alunosDoGrupoAtual, int grupoAtualId, List<Grupo> todosOsGrupos)
        {
            foreach (var alunoDoGrupoAtual in alunosDoGrupoAtual)
            {
                bool alunoJaEstaEmOutroGrupo = todosOsGrupos
                                                .Any(g => g.ID != grupoAtualId &&
                                                            g.Alunos.Any(a => a.Numero == alunoDoGrupoAtual.Numero));
                if (alunoJaEstaEmOutroGrupo)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
