namespace StudentSys // OBRIGATÓRIO: O tal "sobrenome" para o Student.cs o encontrar
{
    // HERANÇA: Herda de Student (: Student)
    public class InternationalStudent : Student
    {
        // Propriedades exclusivas para alunos internacionais
        public string Country { get; set; }    // País de origem
        public string VisaStatus { get; set; } // Estado do Visto (ex: "Válido")

        // CONSTRUTOR:
        // Recebe os dados todos (nome, id, email, país, visto)
        // Usa ': base(...)' para enviar o nome, id e email para a classe Pai (Student) lidar com eles.
        public InternationalStudent(string name, int id, string email, string country, string visa) 
            : base(name, id, email)
        {
            Country = country;
            VisaStatus = visa;
        }

        // POLIMORFISMO (OVERRIDE):
        // Este método "passa por cima" do método original do pai.
        // Um aluno internacional paga 5000€, ignorando o valor base de 0€.
        public override decimal CalculateTuition()
        {
            return 5000m; 
        }
    }
}