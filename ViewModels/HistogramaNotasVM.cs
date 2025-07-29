using System.Collections.ObjectModel;
using System.Windows.Media;


public class BarraHistograma
{
    public string Intervalo { get; set; }
    public int Quantidade { get; set; }
    public double Altura => Quantidade * 15;

    public string Tooltip => $"{Quantidade} aluno(s) com nota entre {Intervalo}";

    public string NotaCentro => Intervalo.Contains('–') ?
        Intervalo.Split('–').Select(int.Parse).Average().ToString("F1") : Intervalo;

    public Brush Cor
    {
        get
        {
            var min = int.Parse(Intervalo.Split('–')[0]);

            if (min < 6)
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F4C7C3")); // Rosa suave
            else if (min < 14)
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE49C")); // Laranja clara
            else
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BEEAD8")); // Verde suave
        }
    }
}



public class HistogramaNotasVM
{
    public ObservableCollection<BarraHistograma> Barras { get; set; }

    public HistogramaNotasVM(IEnumerable<double> notas)
    {
        var contadores = new int[10];

        foreach (var nota in notas)
        {
            if (nota < 2) contadores[0]++;
            else if (nota < 4) contadores[1]++;
            else if (nota < 6) contadores[2]++;
            else if (nota < 8) contadores[3]++;
            else if (nota < 10) contadores[4]++;
            else if (nota < 12) contadores[5]++;
            else if (nota < 14) contadores[6]++;
            else if (nota < 16) contadores[7]++;
            else if (nota < 18) contadores[8]++;
            else contadores[9]++;
        }

        var intervalos = new[]
        {
            "0–2", "2–4", "4–6", "6–8", "8–10",
            "10–12", "12–14", "14–16", "16–18", "18–20"
        };

        Barras = new ObservableCollection<BarraHistograma>();

        for (int i = 0; i < intervalos.Length; i++)
        {
            Barras.Add(new BarraHistograma
            {
                Intervalo = intervalos[i],
                Quantidade = contadores[i]
            });
        }
    }
}
