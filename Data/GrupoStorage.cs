using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using GestaoAvaliacoes.Model;
using System.IO;
using System.Text.Json;

namespace GestaoAvaliacoes.Data
{
    public static class GrupoStorage
    {
        private static string caminho = "grupos.json";

        public static void GuardarGrupos(List<Grupo> grupos)
        {
            var json = JsonSerializer.Serialize(grupos, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(caminho, json);
        }

        public static List<Grupo> CarregarGrupos()
        {
            if (!File.Exists(caminho)) return new List<Grupo>();

            string json = File.ReadAllText(caminho);
            return JsonSerializer.Deserialize<List<Grupo>>(json) ?? new List<Grupo>();
        }
    }
}