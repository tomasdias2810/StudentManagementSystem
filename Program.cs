using System;
using System.Collections.Generic;
using System.Linq; // Necessário para somar as bolsas (.Sum)
using StudentSys;  // <--- ESTA LINHA É A QUE TIRA O VERMELHO!

class Program
{
    static List<Student> students = new List<Student>();

    static void Main(string[] args)
    {
        // 1. Tentar carregar dados do ficheiro
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
            Console.WriteLine("5. Guardar Dados (Manual)");
            Console.WriteLine("0. Guardar e Sair");
            Console.Write("\nEscolha: ");

            string option = Console.ReadLine();

            switch (option)
            {
                case "1": ListStudents(); break;
                case "2": AddStudentMenu(); break;
                case "3": AddGradeMenu(); break;
                case "4": AddScholarshipMenu(); break;
                case "5": 
                    FileService.SaveData(students); 
                    Console.WriteLine("Guardado com sucesso!"); 
                    break;
                case "0": 
                    FileService.SaveData(students); 
                    Console.WriteLine("A guardar e a sair...");
                    running = false; 
                    break;
                default: 
                    Console.WriteLine("Opção inválida."); 
                    break;
            }

            if (running) { Console.WriteLine("\n[Enter para continuar...]"); Console.ReadLine(); }
        }
    }

    // --- MÉTODOS ---

    static void ListStudents()
    {
        Console.WriteLine("\n--- PAUTA DETALHADA ---");
        if (students.Count == 0) { Console.WriteLine("Vazio."); return; }

        foreach (var s in students)
        {
            // Mostra os dados básicos (ID, Nome, Média Colorida)
            s.DisplayInfo(); 

            // 1. MOSTRAR AS DISCIPLINAS
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

            // 2. PARTE FINANCEIRA
            decimal tuition = s.CalculateTuition();
            decimal totalScholarship = s.Scholarships.Sum(x => x.Amount);
            decimal finalToPay = tuition - totalScholarship;

            Console.WriteLine("    -------------------------");
            Console.WriteLine($"    Tipo: {s.GetType().Name.Replace("Student", "")}");
            Console.WriteLine($"    Propina Base: {tuition}€");
            
            if (totalScholarship > 0) 
            {
                Console.WriteLine($"    Bolsas: -{totalScholarship}€");
            }

            if (finalToPay > 0) Console.WriteLine($"    TOTAL A PAGAR: {finalToPay}€");
            else if (finalToPay < 0) Console.WriteLine($"    CRÉDITO A RECEBER: {-finalToPay}€");
            else Console.WriteLine($"    CONTA SALDADA (0€)");
            
            Console.WriteLine("-----------------------------");
        }
    }

    static void AddGradeMenu()
    {
        Console.WriteLine("\n--- LANÇAR NOTA E DISCIPLINA ---");
        Console.Write("ID do Estudante: ");
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
        else Console.WriteLine("ID inválido.");
    }

    static void AddStudentMenu()
    {
        try 
        {
            Console.WriteLine("\n--- ADICIONAR ESTUDANTE ---");
            Console.Write("Nome: "); string name = Console.ReadLine();
            Console.Write("ID: "); int id = int.Parse(Console.ReadLine()); 
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
        Console.Write("ID: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            Student s = students.Find(x => x.StudentID == id);
            if (s != null)
            {
                Console.Write("Nome da Bolsa: "); string name = Console.ReadLine();
                Console.Write("Valor (€): ");
                if (decimal.TryParse(Console.ReadLine(), out decimal amount)) {
                    s.Scholarships.Add(new Scholarship(name, amount));
                    FileService.SaveData(students);
                    Console.WriteLine("Bolsa atribuída.");
                }
            }
        }
    }
}