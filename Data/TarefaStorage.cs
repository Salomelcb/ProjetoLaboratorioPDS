using GestaoAvaliacoes.Model;
using System.Text.Json;
using System.IO;

namespace GestaoAvaliacoes.Data
{
    public static class TarefaStorage
    {
        private static string caminho = "tarefas.json";

        public static void GuardarTarefas(List<Tarefa> tarefas)
        {
            var json = JsonSerializer.Serialize(tarefas, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(caminho, json);
        }
        public static List<Tarefa> CarregarTarefas()
        {
            if (!File.Exists(caminho)) return new List<Tarefa>();

            string json = File.ReadAllText(caminho);
            return JsonSerializer.Deserialize<List<Tarefa>>(json) ?? new List<Tarefa>();
        }
    }
}