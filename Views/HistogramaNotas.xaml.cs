using System.Windows;

namespace GestaoAvaliacoes.Views
{
    public partial class HistogramaNotas : Window
    {
        public HistogramaNotas(IEnumerable<double> notas)
        {
            InitializeComponent();
            DataContext = new HistogramaNotasVM(notas);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
