using GestaoAvaliacoes.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GestaoAvaliacoes.ViewModels
{
    public class AlunosVM : INotifyPropertyChanged
    {
            private string termoPesquisa;
            private ObservableCollection<Aluno> todosAlunos;
            private ObservableCollection<Aluno> alunosFiltrados;

            public string TermoPesquisa
            {
                get => termoPesquisa;
                set
                {
                    termoPesquisa = value;
                    OnPropertyChanged(nameof(TermoPesquisa));
                    FiltrarAlunos();
                }
            }

            public ObservableCollection<Aluno> AlunosFiltrados
            {
                get => alunosFiltrados;
                set
                {
                    alunosFiltrados = value;
                    OnPropertyChanged(nameof(AlunosFiltrados));
                }
            }

            public bool AlunoNaoEncontrado => AlunosFiltrados.Count == 0;

            public AlunosVM()
            {
            todosAlunos = new ObservableCollection<Aluno>
            {
                new Aluno { Nome = "Adriano Vilarinho Ribeiro", Numero = 79026, Email = "al79026@alunos.utad.pt" },
                new Aluno { Nome = "Maria Silva", Numero = 79027, Email = "al79027@alunos.utad.pt" }
            };

                AlunosFiltrados = new ObservableCollection<Aluno>(todosAlunos);
            }

            private void FiltrarAlunos()
            {
                if (string.IsNullOrWhiteSpace(TermoPesquisa))
                {
                    AlunosFiltrados = new ObservableCollection<Aluno>(todosAlunos);
                }
                else
                {
                    var resultado = todosAlunos.Where(a =>
                        a.Nome.Contains(TermoPesquisa, System.StringComparison.InvariantCultureIgnoreCase)).ToList();

                    AlunosFiltrados = new ObservableCollection<Aluno>(resultado);
                }

                OnPropertyChanged(nameof(AlunoNaoEncontrado));
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string prop) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }