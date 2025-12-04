using System;
using System.Collections.Generic;
using System.Linq; 
using StudentSys;

class Program
{
    static List<Student> students = new List<Student>();

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        students = FileService.LoadData();
        Console.WriteLine($"[SISTEMA] Dados carregados. Total: {students.Count}");

        bool running = true;
        while (running)
        {
            Console.Clear();
            Console.WriteLine("=== GESTÃO ACADÉMICA COMPLETA ===");
            Console.WriteLine($"Total Alunos: {students.Count}");
            Console.WriteLine("----------------------------------");
            Console.WriteLine("1. Listar Detalhes dos Estudantes");
            Console.WriteLine("2. Adicionar Novo Estudante");
            Console.WriteLine("3. Adicionar Disciplina e Nota");
            Console.WriteLine("4. Atribuir Bolsa");
            Console.WriteLine("5. Remover Estudante");
            Console.WriteLine("6. Guardar Dados (Manual)");
            Console.WriteLine("0. Guardar e Sair");
            Console.Write("\nEscolha: ");

            string option = Console.ReadLine();

            switch (option)
            {
                case "1": ListStudents(); break;
                case "2": AddStudentMenu(); break;
                case "3": AddGradeMenu(); break;
                case "4": AddScholarshipMenu(); break;
                case "5": RemoveStudentMenu(); break;
                case "6": 
                    FileService.SaveData(students); 
                    Console.WriteLine("Guardado."); 
                    break;
                case "0": 
                    FileService.SaveData(students); 
                    running = false; 
                    break;
                default: 
                    Console.WriteLine("Inválido."); 
                    break;
            }

            if (running) { Console.WriteLine("\n[Enter para continuar...]"); Console.ReadLine(); }
        }
    }

    // --- MÉTODOS ---

    static void RemoveStudentMenu()
    {
        Console.WriteLine("\n--- REMOVER ESTUDANTE ---");
        Console.Write("Número de Aluno a remover: "); // MUDADO
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            Student s = students.Find(x => x.StudentID == id);
            if (s == null) { Console.WriteLine("Estudante não encontrado!"); return; }
            students.Remove(s);
            FileService.SaveData(students);
            Console.WriteLine($"Estudante '{s.Name}' foi apagado do sistema.");
        }
        else Console.WriteLine("Número inválido.");
    }

    static void ListStudents()
    {
        Console.WriteLine("\n--- PAUTA DETALHADA ---");
        if (students.Count == 0) { Console.WriteLine("Vazio."); return; }

        foreach (var s in students)
        {
            s.DisplayInfo();

            if (s.Grades.Count > 0)
            {
                Console.WriteLine(" -> Histórico de Disciplinas:");
                foreach (var g in s.Grades)
                {
                    string status = g.Value >= 9.5m ? "APROVADO" : "REPROVADO";
                    Console.WriteLine($"    * {g.Course.Name} ({g.Course.Credits} ECTS)");
                    Console.WriteLine($"      Prof: {g.Course.Professor} | Nota: {g.Value} -> {status}");
                }
            }
            else
            {
                Console.WriteLine(" -> Sem notas registadas.");
            }

            decimal tuition = s.CalculateTuition();
            decimal totalScholarship = s.Scholarships.Sum(x => x.Amount);

            string tipoPortugues = "Desconhecido";
            string tipoIngles = s.GetType().Name;

            if (tipoIngles == "UndergraduateStudent") tipoPortugues = "Licenciatura";
            else if (tipoIngles == "GraduateStudent") tipoPortugues = "Mestrado";
            else if (tipoIngles == "InternationalStudent") tipoPortugues = "Internacional";

            Console.WriteLine("    -------------------------");
            Console.WriteLine($"    Tipo: {tipoPortugues}");
            Console.WriteLine($"    Propina Base: {tuition:0}€");
            
            if (totalScholarship > 0) 
            {
                Console.WriteLine($"    Bolsas Atribuídas: {totalScholarship:0}€");
                foreach(var b in s.Scholarships)
                {
                    Console.WriteLine($"      (Bolsa {b.Name}: {b.Amount:0}€)");
                }
            }
            
            Console.WriteLine("-----------------------------");
        }
    }

    static void AddGradeMenu()
    {
        Console.WriteLine("\n--- LANÇAR NOTA E DISCIPLINA ---");
        Console.Write("Número de Aluno: "); // MUDADO
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            Student s = students.Find(x => x.StudentID == id);
            if (s == null) { Console.WriteLine("Aluno não encontrado."); return; }

            Console.WriteLine($"Aluno: {s.Name}");
            Console.Write("Nome da Disciplina: "); string cName = Console.ReadLine();
            Console.Write("Código (ex: POO): "); string cCode = Console.ReadLine();
            Console.Write("Nome do Professor: "); string prof = Console.ReadLine();
            Console.Write("Créditos ECTS (ex: 6): ");
            int.TryParse(Console.ReadLine(), out int credits);

            Console.Write("Nota Final (0-20): "); 
            if (decimal.TryParse(Console.ReadLine(), out decimal val))
            {
                Course c = new Course(cName, cCode, credits, prof);
                s.AddGrade(c, val);
                FileService.SaveData(students);
                string status = val >= 9.5m ? "APROVADO" : "REPROVADO";
                Console.WriteLine($"Nota registada! O aluno está {status}.");
            }
            else Console.WriteLine("Nota inválida.");
        }
        else Console.WriteLine("Número inválido.");
    }

    static void AddStudentMenu()
    {
        try 
        {
            Console.WriteLine("\n--- ADICIONAR ESTUDANTE ---");
            Console.Write("Nome: "); string name = Console.ReadLine();
            Console.Write("Número de Aluno: "); int id = int.Parse(Console.ReadLine()); // MUDADO
            Console.Write("Email: "); string email = Console.ReadLine();
            Console.WriteLine("1. Licenciatura | 2. Mestrado | 3. Internacional");
            string type = Console.ReadLine();

            if (type == "1") {
                Console.Write("Curso: "); string major = Console.ReadLine();
                Console.Write("Ano: "); int year = int.Parse(Console.ReadLine());
                students.Add(new UndergraduateStudent(name, id, email, major, year));
            }
            else if (type == "2") {
                Console.Write("Tese: "); string thesis = Console.ReadLine();
                Console.Write("Orientador: "); string advisor = Console.ReadLine();
                students.Add(new GraduateStudent(name, id, email, thesis, advisor));
            }
            else if (type == "3") {
                Console.Write("País: "); string country = Console.ReadLine();
                Console.Write("Visto: "); string visa = Console.ReadLine();
                students.Add(new InternationalStudent(name, id, email, country, visa));
            }
            
            FileService.SaveData(students);
            Console.WriteLine("Guardado!");
        }
        catch { Console.WriteLine("Erro nos dados."); }
    }

    static void AddScholarshipMenu()
    {
        Console.WriteLine("\n--- ATRIBUIR BOLSA ---");
        Console.Write("Número de Aluno: "); // MUDADO
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            Student s = students.Find(x => x.StudentID == id);
            if (s != null)
            {
                Console.Write("Nome da Bolsa: "); string name = Console.ReadLine();
                Console.Write("Valor (€): ");
                decimal.TryParse(Console.ReadLine(), out decimal amount);
                Console.Write("Média Mínima Exigida: ");
                
                if (decimal.TryParse(Console.ReadLine(), out decimal minGpa)) {
                    Scholarship bolsa = new Scholarship(name, amount, minGpa);
                    if (bolsa.IsEligible(s))
                    {
                        s.Scholarships.Add(bolsa);
                        FileService.SaveData(students);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(">>> SUCESSO! Bolsa atribuída.");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($">>> RECUSADO. Média insuficiente.");
                        Console.ResetColor();
                    }
                }
            }
            else Console.WriteLine("Aluno não encontrado.");
        }
    }
}