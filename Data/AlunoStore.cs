using GestaoAvaliacoes.Model;
using System.IO;
using System.Text.Json;

namespace GestaoAvaliacoes.Data
{
    public static class AlunoStorage
    {
        private static string caminho = "alunos.json";

        public static void GuardarAlunos(List<Aluno> alunos)
        {
            var json = JsonSerializer.Serialize(alunos, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(caminho, json);
        }

        public static List<Aluno> CarregarAlunos()
        {
            if (!File.Exists(caminho)) return new List<Aluno>();

            string json = File.ReadAllText(caminho);
            return JsonSerializer.Deserialize<List<Aluno>>(json) ?? new List<Aluno>();
        }
    }
}