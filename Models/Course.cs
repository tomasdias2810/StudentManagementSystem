namespace StudentSys
{
    public class Course
    {
        public string Name { get; set; }      // Nome da disciplina
        public string Code { get; set; }      // Código (ex: POO)
        public int Credits { get; set; }      // Créditos ECTS
        public string Professor { get; set; } // Nome do Docente

        public Course(string name, string code, int credits, string professor)
        {
            Name = name;
            Code = code;
            Credits = credits;
            Professor = professor;
        }
    }
    } 