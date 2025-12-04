using System;
using System.Collections.Generic;

namespace StudentSys
{
    public class Student
    {
        // --- ENCAPSULAMENTO ---
        // Propriedades públicas que guardam os dados do aluno.
        public string Name { get; set; }
        public int StudentID { get; set; }
        public string Email { get; set; }
        
        // Esta propriedade é 'private set', ou seja, protegida contra escrita externa.
        // A média só é alterada pelos métodos de cálculo internos.
        public decimal GPA { get; private set; } 

        // --- COMPOSIÇÃO ---
        // Um Aluno é composto por uma lista de Notas e uma lista de Bolsas.
        public List<Grade> Grades { get; set; } = new List<Grade>();
        public List<Scholarship> Scholarships { get; set; } = new List<Scholarship>();

        // --- CONSTRUTOR ---
        // Define como o objeto nasce.
        public Student(string name, int id, string email)
        {
            Name = name;
            StudentID = id;
            Email = email;
        }

        // --- POLIMORFISMO (VIRTUAL) ---
        // 'virtual' indica que este método pode (e deve) ser alterado pelas subclasses.
        // Por defeito retorna 0, mas as subclasses vão ignorar isto e retornar o valor delas.
        public virtual decimal CalculateTuition()
        {
            return 0; 
        }

        // Método para adicionar nota com validação
        public void AddGrade(Course course, decimal gradeValue)
        {
            // Validação simples (0 a 20)
            if (gradeValue < 0 || gradeValue > 20)
            {
                Console.WriteLine($"[ERRO] Nota inválida.");
                return;
            }
            Grades.Add(new Grade(course, gradeValue));
            // Sempre que entra uma nota, a média é recalculada automaticamente.
            CalculateGPA();
        }

        // Algoritmo de cálculo de média aritmética
        public decimal CalculateGPA()
        {
            if (Grades.Count == 0) { GPA = 0; return 0; }
            decimal sum = 0;
            foreach (Grade g in Grades) sum += g.Value;
            GPA = sum / Grades.Count;
            return GPA;
        }

        // Método para mostrar dados (com lógica visual de cores)
        public virtual void DisplayInfo()
        {
            CalculateGPA(); // Garante que a média está fresca.
            
            string status;
            ConsoleColor corStatus;

            // Define se é aprovado ou reprovado
            if (GPA < 9.5m) 
            {
                status = "REPROVADO";
                corStatus = ConsoleColor.Red;
            }
            else
            {
                status = "APROVADO";
                corStatus = ConsoleColor.Green;
            }

            // Escreve os dados formatados
            Console.Write($"Número de Aluno: {StudentID} | Nome: {Name} | Média: {GPA:F1} -> ");
            
            // Muda a cor da consola para o status
            Console.ForegroundColor = corStatus;
            Console.WriteLine(status);
            Console.ResetColor(); // Volta a cor ao normal
        }
    }
}