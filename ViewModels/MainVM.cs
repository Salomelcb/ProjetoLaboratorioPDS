using GestaoAvaliacoes.Views;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using GestaoAvaliacoes.Helpers;
using System.Runtime.CompilerServices;

namespace GestaoAvaliacoes.ViewModels
{
    class MainVM : INotifyPropertyChanged
    {
        private Page _currentPage;
        public Page CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateGruposCommand { get; }
        public ICommand NavigateAlunosCommand { get; }
        public ICommand NavigateNotasCommand { get; }
        public ICommand NavigateTarefasCommand { get; }
        public ICommand NavigatePerfilCommand { get; }

        public MainVM()
        {
            CurrentPage = new HomePage();

            NavigateHomeCommand = new RelayCommand(_ => CurrentPage = new HomePage());
            NavigateGruposCommand = new RelayCommand(_ => CurrentPage = new GruposPage());
            NavigateAlunosCommand = new RelayCommand(_ => CurrentPage = new AlunosPage());
            NavigateNotasCommand = new RelayCommand(_ => CurrentPage = new NotasPage());
            NavigateTarefasCommand = new RelayCommand(_ => CurrentPage = new TarefasPage());
            NavigatePerfilCommand = new RelayCommand(_ => CurrentPage = new PerfilPage());

        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
