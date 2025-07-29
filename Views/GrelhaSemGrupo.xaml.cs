using GestaoAvaliacoes.ViewModels;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GestaoAvaliacoes.Views
{
    public partial class GrelhaSemGrupo : Window
    {
        public GrelhaSemGrupo(List<NotasVM> alunosSemGrupo, List<string> titulosTarefas)
        {
            InitializeComponent();

            dgSemGrupo.Columns.Clear();

            dgSemGrupo.Columns.Add(new DataGridTextColumn
            {
                Header = "Aluno",
                Binding = new Binding("NomeAlunoCurto")
            });

            foreach (var titulo in titulosTarefas)
            {
                var coluna = new DataGridTextColumn
                {
                    Header = titulo,
                    Binding = new Binding($"NotasPorTarefa[{titulo}]")
                    {
                        StringFormat = "F1"
                    },
                    ElementStyle = new Style(typeof(TextBlock))
                    {
                        Setters =
                        {
                            new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center),
                            new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center),
                            new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center)
                        }
                    }
                };

                dgSemGrupo.Columns.Add(coluna);
            }

            dgSemGrupo.ItemsSource = alunosSemGrupo;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
