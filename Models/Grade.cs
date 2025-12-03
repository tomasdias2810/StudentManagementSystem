using System;

namespace StudentSys
{
    public class Grade
    {
        public Course Course { get; set; } // Guarda o objeto Disciplina completo
        public decimal Value { get; set; } // A nota em si (0-20)
        public DateTime Date { get; set; } // Data em que a nota foi lançada

        public Grade(Course course, decimal value)
        {
            Course = course;
            Value = value;
            Date = DateTime.Now; // Pega a data e hora atuais do sistema
        }
    }
}