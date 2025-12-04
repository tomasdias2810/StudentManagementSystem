namespace StudentSys
{
    // HERANÇA: Herda de Student
    public class GraduateStudent : Student
    {
        // Dados específicos de quem está em Mestrado/Doutoramento
        public string ThesisTopic { get; set; } // Tema da Tese
        public string Advisor { get; set; }     // Nome do Orientador

        // Construtor que passa os dados básicos ao pai (: base)
        public GraduateStudent(string name, int id, string email, string thesis, string advisor) 
            : base(name, id, email)
        {
            ThesisTopic = thesis;
            Advisor = advisor;
        }

        // POLIMORFISMO (OVERRIDE):
        // Alunos de mestrado pagam um valor intermédio (2000€).
        public override decimal CalculateTuition()
        {
            return 2000m; 
        }
    }
}