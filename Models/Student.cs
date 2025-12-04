using System;
using System.Collections.Generic;

namespace StudentSys
{
    public class Student
    {
        public string Name { get; set; }
        public int StudentID { get; set; }
        public string Email { get; set; }
        public decimal GPA { get; private set; } 

        public List<Grade> Grades { get; set; } = new List<Grade>();
        public List<Scholarship> Scholarships { get; set; } = new List<Scholarship>();

        public Student(string name, int id, string email)
        {
            Name = name;
            StudentID = id;
            Email = email;
        }

        public virtual decimal CalculateTuition()
        {
            return 0; 
        }

        public void AddGrade(Course course, decimal gradeValue)
        {
            if (gradeValue < 0 || gradeValue > 20)
            {
                Console.WriteLine($"[ERRO] Nota inválida.");
                return;
            }
            Grades.Add(new Grade(course, gradeValue));
            CalculateGPA();
        }

        public decimal CalculateGPA()
        {
            if (Grades.Count == 0) { GPA = 0; return 0; }
            decimal sum = 0;
            foreach (Grade g in Grades) sum += g.Value;
            GPA = sum / Grades.Count;
            return GPA;
        }

        // --- AQUI ESTÁ A MUDANÇA ---
        public virtual void DisplayInfo()
        {
            CalculateGPA(); // Garante que a média está atualizada

            // Lógica: Se for menor ou igual a 9.5 é Reprovado
            string status;
            ConsoleColor corStatus;

            if (GPA <= 9.5m) 
            {
                status = "REPROVADO";
                corStatus = ConsoleColor.Red; // Vermelho se chumbar
            }
            else
            {
                status = "APROVADO";
                corStatus = ConsoleColor.Green; // Verde se passar
            }

            // Escreve o ID e Nome normalmente
            Console.Write($"ID: {StudentID} | Nome: {Name} | Média: {GPA:F1} -> ");
            
            // Muda a cor só para o Status
            Console.ForegroundColor = corStatus;
            Console.WriteLine(status);
            
            // Volta a cor ao normal (importante!)
            Console.ResetColor();
        }
    }
}