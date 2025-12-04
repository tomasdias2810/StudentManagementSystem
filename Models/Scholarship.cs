namespace StudentSys
{
    public class Scholarship
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public decimal MinGPA { get; set; } // Propriedade nova: Média Mínima

        public Scholarship(string name, decimal amount, decimal minGpa)
        {
            Name = name;
            Amount = amount;
            MinGPA = minGpa;
        }

        // --- LÓGICA DE NEGÓCIO ---
        // Este método recebe um Aluno e decide se ele pode receber a bolsa.
        // Cumpre o requisito do Slide 6: "bool IsEligible(Student s)"
        public bool IsEligible(Student s)
        {
            // Atualiza a média antes de comparar
            s.CalculateGPA();
            
            // Se a média do aluno for maior ou igual ao mínimo exigido...
            if (s.GPA >= MinGPA)
            {
                return true; // ...aprovado!
            }
            else
            {
                return false; // ...reprovado.
            }
        }
    }
}