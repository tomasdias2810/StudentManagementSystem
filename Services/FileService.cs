using System.Collections.Generic;
using System.IO;           // Para escrever e ler ficheiros no disco
using Newtonsoft.Json;     // Biblioteca externa para lidar com formato JSON
using StudentSys;

namespace StudentSys
{
    // Classe estática (Static): Não precisa de ser instanciada (new FileService).
    // Funciona como uma caixa de ferramentas.
    public static class FileService
    {
        // Define o nome do ficheiro onde os dados ficam guardados.
        private static string filePath = "students.json";

        // --- SERIALIZAÇÃO (GUARDAR) ---
        // Transforma a lista de objetos C# em texto JSON.
        public static void SaveData(List<Student> students)
        {
            // 'TypeNameHandling.Auto' é o segredo! 
            // Permite guardar no ficheiro QUE TIPO de aluno é (Licenciatura, Mestrado...).
            // Sem isto, perdíamos o Polimorfismo ao carregar.
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            
            // Converte para texto formatado (Indented).
            string json = JsonConvert.SerializeObject(students, Formatting.Indented, settings);
            
            // Escreve o texto no disco rígido.
            File.WriteAllText(filePath, json);
        }

        // --- DESERIALIZAÇÃO (CARREGAR) ---
        // Transforma o texto JSON de volta em objetos C#.
        public static List<Student> LoadData()
        {
            // Verifica se o ficheiro existe para o programa não crashar na primeira vez.
            if (!File.Exists(filePath)) return new List<Student>();

            // Lê todo o texto do ficheiro.
            string json = File.ReadAllText(filePath);
            
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            
            // Converte o texto em Lista de Estudantes.
            // O operador '??' garante que se algo falhar, devolvemos uma lista vazia em vez de null.
            return JsonConvert.DeserializeObject<List<Student>>(json, settings) ?? new List<Student>();
        }
    }
}