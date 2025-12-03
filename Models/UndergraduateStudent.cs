namespace StudentSys
{
    // : Student significa "Herda de Student". Ganha automaticamente Nome, ID, Email, etc.
    public class UndergraduateStudent : Student
    {
        // Propriedades específicas apenas para alunos de Licenciatura
        public string Major { get; set; } // Curso
        public int Year { get; set; }     // Ano

        // Construtor que recebe tudo e passa os dados base para o Pai (base)
        public UndergraduateStudent(string name, int id, string email, string major, int year) 
            : base(name, id, email) // Chama o construtor do Student
        {
            Major = major;
            Year = year;
        }

        // --- POLIMORFISMO (OVERRIDE) ---
        // Alteramos o comportamento do cálculo da propina para este tipo específico.
        public override decimal CalculateTuition()
        {
            return 1000m; // Valor fixo para Licenciatura
        }
    }
}