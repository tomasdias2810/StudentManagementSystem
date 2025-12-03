namespace StudentSys
{
    public class GraduateStudent : Student
    {
        // Propriedades específicas de Mestrado
        public string ThesisTopic { get; set; } // Tema da Tese
        public string Advisor { get; set; }     // Orientador

        public GraduateStudent(string name, int id, string email, string thesis, string advisor) 
            : base(name, id, email)
        {
            ThesisTopic = thesis;
            Advisor = advisor;
        }

        // Polimorfismo: Mestrado paga mais caro
        public override decimal CalculateTuition()
        {
            return 2000m; 
        }
    }
}