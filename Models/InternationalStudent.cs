namespace StudentSys
{
    public class InternationalStudent : Student
    {
        public string Country { get; set; }    // País de origem
        public string VisaStatus { get; set; } // Estado do Visto

        public InternationalStudent(string name, int id, string email, string country, string visa) 
            : base(name, id, email)
        {
            Country = country;
            VisaStatus = visa;
        }

        // Polimorfismo: Alunos internacionais pagam o valor mais alto
        public override decimal CalculateTuition()
        {
            return 5000m; 
        }
    }
}