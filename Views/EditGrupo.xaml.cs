using GestaoAvaliacoes.Model;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;

namespace GestaoAvaliacoes.Views
{
    public partial class EditGrupo : Window
    {
        private Grupo _grupoOriginal;
        private List<Aluno> _todosAlunosDisponiveis;
        private List<Grupo> _todosOsGruposExistentes;
        public int GrupoID { get; set; }
        public string GrupoNome { get; set; }
        public ObservableCollection<Aluno> AlunosNoGrupo { get; set; }
        public EditGrupo(List<Aluno> alunosDisponiveis, Grupo grupoParaEditar, List<Grupo> todosOsGruposAtuais)
        {
            InitializeComponent();
            _todosAlunosDisponiveis = alunosDisponiveis;
            _grupoOriginal = grupoParaEditar;
            _todosOsGruposExistentes = todosOsGruposAtuais;

            GrupoID = grupoParaEditar.ID;
            GrupoNome = grupoParaEditar.Nome;
            AlunosNoGrupo = new ObservableCollection<Aluno>(grupoParaEditar.Alunos);
            this.DataContext = this;
            txtErro.Text = string.Empty;

            txtID.IsEnabled = false;
            txtNome.IsEnabled = false;
        }

        private void EditarAlunos_Click(object sender, RoutedEventArgs e)
        {
            txtErro.Text = string.Empty;
            var editWindow = new EditListaAlunos(_todosAlunosDisponiveis, AlunosNoGrupo.ToList());

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
            if (string.IsNullOrWhiteSpace(GrupoNome))
            {
                txtErro.Text = "O nome do grupo não pode estar vazio.";
                return;
            }
            if (Regex.IsMatch(GrupoNome.Trim(), @"^\d+$"))
            {
                txtErro.Text = "O nome do grupo não pode ser apenas um número.";
                return;
            }

            if (_todosOsGruposExistentes.Any(g => g.ID != _grupoOriginal.ID && g.Nome.Trim().Equals(GrupoNome.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                txtErro.Text = "Já existe outro grupo com este nome. Por favor, escolha um nome diferente.";
                return;
            }

            if (!int.TryParse(txtID.Text, out int id) || id <= 0)
            {
                txtErro.Text = "O ID deve ser um número inteiro positivo válido.";
                return;
            }
            GrupoID = id;

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

            var outrosGruposParaValidacao = _todosOsGruposExistentes.Where(g => g.ID != _grupoOriginal.ID).ToList();
            if (outrosGruposParaValidacao.Any(g => g.ID == GrupoID))
            {
                txtErro.Text = "Este ID já está em uso por outro grupo.";
                return;
            }

            if (!ValidarAlunosUnicosEmGrupos(AlunosNoGrupo.ToList(), _grupoOriginal.ID, _todosOsGruposExistentes))
            {
                txtErro.Text = "Um ou mais alunos selecionados já pertencem a outro grupo.";
                return;
            }
            //atualiza
            _grupoOriginal.ID = GrupoID;
            _grupoOriginal.Nome = GrupoNome;
            _grupoOriginal.Alunos = AlunosNoGrupo.ToList();

            foreach (var aluno in _todosAlunosDisponiveis)
            {
                if (_grupoOriginal.Alunos.Any(a => a.Numero == aluno.Numero))
                {
                    aluno.GrupoID = _grupoOriginal.ID;
                }
                else if (aluno.GrupoID == _grupoOriginal.ID) // Se o aluno estava neste grupo mas foi removido
                {
                    aluno.GrupoID = null;
                }
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