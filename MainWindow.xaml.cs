using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GestaoAvaliacoes;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        NavigationFrame.Navigate(new System.Uri("Views/HomePage.xaml", UriKind.RelativeOrAbsolute));
    }
    private void BG_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        Tg_Btn.IsChecked = false;
    }

    private void btnHome_MouseEnter(object sender, MouseEventArgs e)
    {
        if (Tg_Btn.IsChecked == false)
        {
            Popup.PlacementTarget = btnHome;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            Header.PopupText.Text = "Home";
        }
    }

    private void btnHome_MouseLeave(object sender, MouseEventArgs e)
    {
        Popup.Visibility = Visibility.Collapsed;
        Popup.IsOpen = false;
    }

    private void btnGrupos_MouseEnter(object sender, MouseEventArgs e)
    {
        if (Tg_Btn.IsChecked == false)
        {
            Popup.PlacementTarget = btnGrupos;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            Header.PopupText.Text = "Grupos";
        }
    }

    private void btnGrupos_MouseLeave(object sender, MouseEventArgs e)
    {
        Popup.Visibility = Visibility.Collapsed;
        Popup.IsOpen = false;
    }

    private void btnAlunos_MouseEnter(object sender, MouseEventArgs e)
    {
        if (Tg_Btn.IsChecked == false)
        {
            Popup.PlacementTarget = btnAlunos;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            Header.PopupText.Text = "Alunos";
        }
    }

    private void btnAlunos_MouseLeave(object sender, MouseEventArgs e)
    {
        Popup.Visibility = Visibility.Collapsed;
        Popup.IsOpen = false;
    }

    private void btnTarefas_MouseEnter(object sender, MouseEventArgs e)
    {
        if (Tg_Btn.IsChecked == false)
        {
            Popup.PlacementTarget = btnTarefas;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            Header.PopupText.Text = "Tarefas";
        }
    }

    private void btnTarefas_MouseLeave(object sender, MouseEventArgs e)
    {
        Popup.Visibility = Visibility.Collapsed;
        Popup.IsOpen = false;
    }

    private void btnNotas_MouseEnter(object sender, MouseEventArgs e)
    {
        if (Tg_Btn.IsChecked == false)
        {
            Popup.PlacementTarget = btnNotas;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            Header.PopupText.Text = "Notas";
        }
    }

    private void btnNotas_MouseLeave(object sender, MouseEventArgs e)
    {
        Popup.Visibility = Visibility.Collapsed;
        Popup.IsOpen = false;
    }

    private void btnPerfil_MouseEnter(object sender, MouseEventArgs e)
    {
        if (Tg_Btn.IsChecked == false)
        {
            Popup.PlacementTarget = btnPerfil;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            Header.PopupText.Text = "Perfil";
        }
    }

    private void btnPerfil_MouseLeave(object sender, MouseEventArgs e)
    {
        Popup.Visibility = Visibility.Collapsed;
        Popup.IsOpen = false;
    }

    private void btnClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void btnRestore_Click(object sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.Normal)
            WindowState = WindowState.Maximized;
        else
            WindowState = WindowState.Normal;
    }

    private void btnMinimize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }
    private void btnHome_Click(object sender, RoutedEventArgs e)
    {
        NavigationFrame.Navigate(new System.Uri("Views/HomePage.xaml", UriKind.RelativeOrAbsolute));
    }

    private void btnGrupos_Click(object sender, RoutedEventArgs e)
    {
        NavigationFrame.Navigate(new System.Uri("Views/GruposPage.xaml", UriKind.RelativeOrAbsolute));
    }

    private void btnAlunos_Click(object sender, RoutedEventArgs e)
    {
        NavigationFrame.Navigate(new System.Uri("Views/AlunosPage.xaml", UriKind.RelativeOrAbsolute));
    }

    private void btnTarefas_Click(object sender, RoutedEventArgs e)
    {
        NavigationFrame.Navigate(new System.Uri("Views/TarefasPage.xaml", UriKind.RelativeOrAbsolute));
    }

    private void btnNotas_Click(object sender, RoutedEventArgs e)
    {
        NavigationFrame.Navigate(new System.Uri("Views/NotasPage.xaml", UriKind.RelativeOrAbsolute));
    }

    private void btnPerfil_Click(object sender, RoutedEventArgs e)
    {
        NavigationFrame.Navigate(new System.Uri("Views/PerfilPage.xaml", UriKind.RelativeOrAbsolute));
    }
}
