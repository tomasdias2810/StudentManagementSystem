using System;
using System.Collections.Generic; // Necessário para usar Listas

namespace StudentSys // O "sobrenome" que agrupa as nossas classes
{
    public class Student
    {
        // --- PROPRIEDADES (ENCAPSULAMENTO) ---
        // Guardam os dados do aluno. 'get' permite ler, 'set' permite escrever.
        public string Name { get; set; }
        public int StudentID { get; set; }
        public string Email { get; set; }
        
        // A média (GPA) tem 'private set', ou seja, ninguém de fora pode mudar a média diretamente.
        // Só a própria classe Student pode atualizar a média (através do método CalculateGPA).
        public decimal GPA { get; private set; } 

        // --- COMPOSIÇÃO ---
        // Um Aluno "TEM" (Has-A) várias notas e várias bolsas.
        // Inicializamos logo com 'new List...' para não dar erro de "Null Reference".
        public List<Grade> Grades { get; set; } = new List<Grade>();
        public List<Scholarship> Scholarships { get; set; } = new List<Scholarship>();

        // --- CONSTRUTOR ---
        // Executado quando fazemos 'new Student(...)'. Obriga a dar Nome, ID e Email.
        public Student(string name, int id, string email)
        {
            Name = name;
            StudentID = id;
            Email = email;
        }

        // --- MÉTODO VIRTUAL (POLIMORFISMO) ---
        // 'virtual' significa: "Este método pode ser alterado (sobreposto) pelas classes filhas".
        // Por defeito devolve 0, mas o Undergraduate e Graduate vão mudar isto.
        public virtual decimal CalculateTuition()
        {
            return 0; 
        }

        // --- MÉTODOS DE LÓGICA ---
        
        // Adiciona uma nota à lista e recalcula a média imediatamente
        public void AddGrade(Course course, decimal gradeValue)
        {
            // Validação simples
            if (gradeValue < 0 || gradeValue > 20)
            {
                Console.WriteLine($"[ERRO] Nota inválida.");
                return;
            }
            
            // Cria o objeto Grade e guarda na lista
            Grades.Add(new Grade(course, gradeValue));
            
            // Atualiza a média
            CalculateGPA();
        }

        // Calcula a média aritmética das notas
        public decimal CalculateGPA()
        {
            if (Grades.Count == 0) { GPA = 0; return 0; }
            
            decimal sum = 0;
            // Percorre todas as notas na lista
            foreach (Grade g in Grades) 
            {
                sum += g.Value;
            }
            
            // Divide a soma pelo número de notas
            GPA = sum / Grades.Count;
            return GPA;
        }

        // Método para mostrar dados na consola. Também é 'virtual' para poder ser alterado se quisermos.
        public virtual void DisplayInfo()
        {
            // :F1 formata o número para ter apenas 1 casa decimal (ex: 14.5)
            Console.WriteLine($"ID: {StudentID} | Nome: {Name} | Média: {GPA:F1}");
        }
    }
}