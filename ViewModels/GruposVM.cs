using GestaoAvaliacoes.Data;
using GestaoAvaliacoes.Model;
using GestaoAvaliacoes.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using GestaoAvaliacoes.Helpers;

namespace GestaoAvaliacoes.ViewModels
{
    public class GruposVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<Grupo> _grupos;
        public ObservableCollection<Grupo> Grupos
        {
            get { return _grupos; }
            set
            {
                _grupos = value;
                OnPropertyChanged(nameof(Grupos));
            }
        }

        public List<Aluno> TodosOsAlunos { get; set; }

        public ICommand AdicionarGrupoCommand { get; private set; }
        public ICommand EditarGrupoCommand { get; private set; }
        public ICommand EliminarGrupoCommand { get; private set; }

        public GruposVM()
        {
            Grupos = new ObservableCollection<Grupo>();
            InicializarComandos();
            CarregarDadosIniciais();
        }

        private void InicializarComandos()
        {
            AdicionarGrupoCommand = new RelayCommand(AdicionarGrupo);
            EditarGrupoCommand = new RelayCommand(EditarGrupo, CanEditarEliminarGrupo);
            EliminarGrupoCommand = new RelayCommand(EliminarGrupo, CanEditarEliminarGrupo);
        }

        private void CarregarDadosIniciais()
        {
            TodosOsAlunos = AlunoStorage.CarregarAlunos();

            var gruposCarregados = GrupoStorage.CarregarGrupos();

            Grupos.Clear();
            foreach (var grupo in gruposCarregados)
            {
                grupo.Alunos = grupo.Alunos
                                    .Select(alunoDoGrupoCarregado => TodosOsAlunos.FirstOrDefault(a => a.Numero == alunoDoGrupoCarregado.Numero))
                                    .Where(aluno => aluno != null)
                                    .ToList();

                foreach (var aluno in grupo.Alunos)
                {
                    aluno.GrupoID = grupo.ID;
                }

                Grupos.Add(grupo);
            }

            foreach (var aluno in TodosOsAlunos)
            {
                if (!Grupos.Any(g => g.Alunos.Any(a => a.Numero == aluno.Numero)))
                {
                    aluno.GrupoID = null;
                }
            }
            AlunoStorage.GuardarAlunos(TodosOsAlunos);
        }

        private void AdicionarGrupo(object? parameter)
        {
            var addWindow = new AddGrupo(TodosOsAlunos, Grupos.ToList());

            if (addWindow.ShowDialog() == true) 
            {
                var novoGrupo = addWindow.ObterGrupoCriado();

                Grupos.Add(novoGrupo);
                GrupoStorage.GuardarGrupos(Grupos.ToList());
                AlunoStorage.GuardarAlunos(TodosOsAlunos); 
                OnPropertyChanged(nameof(Grupos));
            }
        }

        private void EditarGrupo(object? parameter)
        {
            var grupoParaEditar = parameter as Grupo;

            if (grupoParaEditar != null)
            {
                var editWindow = new EditGrupo(TodosOsAlunos, grupoParaEditar, Grupos.ToList());

                if (editWindow.ShowDialog() == true) 
                {
                    GrupoStorage.GuardarGrupos(Grupos.ToList());
                    AlunoStorage.GuardarAlunos(TodosOsAlunos); 
                    OnPropertyChanged(nameof(Grupos));
                }
            }
        }

        private bool CanEditarEliminarGrupo(object? parameter)
        {
            return parameter is Grupo;
        }

        private void EliminarGrupo(object? parameter)
        {
            var grupoParaEliminar = parameter as Grupo;

            if (grupoParaEliminar != null)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Tem certeza que deseja eliminar o grupo '{grupoParaEliminar.Nome}' (ID: {grupoParaEliminar.ID})?\n" +
                    "Todos os alunos associados a este grupo terão a sua associação removida.",
                    "Confirmar Eliminação",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    Grupos.Remove(grupoParaEliminar);

                    foreach (var aluno in TodosOsAlunos)
                    {
                        if (aluno.GrupoID == grupoParaEliminar.ID)
                        {
                            aluno.GrupoID = null;
                        }
                    }

                    GrupoStorage.GuardarGrupos(Grupos.ToList());
                    AlunoStorage.GuardarAlunos(TodosOsAlunos);
                    OnPropertyChanged(nameof(Grupos));
                }
            }
        }
    }
}