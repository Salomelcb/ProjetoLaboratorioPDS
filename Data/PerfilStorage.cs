using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using GestaoAvaliacoes.Model;

namespace GestaoAvaliacoes.Data
{
    public static class PerfilStorage
    {
        private static readonly string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "perfil.json");

        public static void SalvarPerfil(Perfil perfil)
        {
            string json = JsonSerializer.Serialize(perfil);
            File.WriteAllText(filePath, json);
        }

        public static Perfil CarregarPerfil()
        {
            if (!File.Exists(filePath))
                return null;

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<Perfil>(json);
        }
    }
}
