using System.Collections.Generic;
using System.IO;           // Para escrever e ler ficheiros
using Newtonsoft.Json;     // A biblioteca que traduz C# para Texto (JSON)

namespace StudentSys
{
    public static class FileService
    {
        // Caminho do ficheiro onde os dados ficam guardados
        private static string filePath = "students.json";

        // Método para guardar (Serialização)
        public static void SaveData(List<Student> students)
        {
            // TypeNameHandling.Auto é CRUCIAL!
            // Permite guardar no JSON que o aluno X é "Undergraduate" e o Y é "International".
            // Sem isto, ao carregar, eles voltariam todos como "Student" genérico e perdíamos as propinas certas.
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            
            // Transforma a lista de objetos num texto bonito (Indented)
            string json = JsonConvert.SerializeObject(students, Formatting.Indented, settings);
            
            // Escreve o texto no disco
            File.WriteAllText(filePath, json);
        }

        // Método para carregar (Deserialização)
        public static List<Student> LoadData()
        {
            // Verifica se o ficheiro existe para não dar erro
            if (!File.Exists(filePath)) return new List<Student>();

            // Lê o texto todo do ficheiro
            string json = File.ReadAllText(filePath);
            
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            
            // Transforma o texto de volta numa Lista de Estudantes
            // O '?? new List<Student>()' serve para criar uma lista vazia se algo correr mal, para o programa não crashar.
            return JsonConvert.DeserializeObject<List<Student>>(json, settings) ?? new List<Student>();
        }
    }
}