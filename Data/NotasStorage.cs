using GestaoAvaliacoes.Model;
using System.IO;
using System.Text.Json;

namespace GestaoAvaliacoes.Data
{
    public static class NotasStorage
    {
        private static readonly string caminho = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "GestaoAvaliacoes",
            "classificacoes.json"
        );

        public static void GuardarNotas(List<Classificacao> classificacoes)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(caminho)!);

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
            };

            File.WriteAllText(caminho, JsonSerializer.Serialize(classificacoes, options));
        }

        public static List<Classificacao> CarregarNotas()
        {
            if (!File.Exists(caminho)) return new List<Classificacao>();
            return JsonSerializer.Deserialize<List<Classificacao>>(File.ReadAllText(caminho)) ?? new();
        }
    }
}