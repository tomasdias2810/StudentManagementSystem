namespace StudentSys
{
    public class Scholarship
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public decimal MinGPA { get; set; } // NOVO: Média mínima exigida

        // Construtor atualizado
        public Scholarship(string name, decimal amount, decimal minGpa)
        {
            Name = name;
            Amount = amount;
            MinGPA = minGpa;
        }

        // O MÉTODO DO SLIDE 6
        // Verifica se a média do aluno é maior ou igual ao mínimo exigido
        public bool IsEligible(Student s)
        {
            // Garante que a média está atualizada antes de comparar
            s.CalculateGPA();
            
            if (s.GPA >= MinGPA)
            {
                return true; // Qualifica-se
            }
            else
            {
                return false; // Não se qualifica
            }
        }
    }
}