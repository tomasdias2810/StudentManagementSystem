// --- IMPORTAÇÃO DE FERRAMENTAS ---
using System; // Permite usar coisas básicas como Console.WriteLine e tipos como int, string
using System.Collections.Generic; // Permite usar Listas (List<T>)
using System.Linq; // Permite usar funções matemáticas avançadas nas listas, como .Sum() (Soma)
using StudentSys; // Importa as nossas classes (Student, Course, etc.) que estão na pasta Models

// Declaração da classe principal do programa
class Program
{
    // --- "BASE DE DADOS" EM MEMÓRIA ---
    // Criamos uma lista estática para guardar os estudantes enquanto o programa corre.
    // É 'static' porque vai ser usada dentro do método Main (que também é static).
    static List<Student> students = new List<Student>();

    // --- PONTO DE PARTIDA (O INÍCIO DO PROGRAMA) ---
    static void Main(string[] args)
    {
        // 1. Tentar carregar dados antigos do ficheiro assim que o programa abre
        students = FileService.LoadData();
        
        // Mostra uma mensagem de confirmação com quantos alunos foram carregados
        Console.WriteLine($"[SISTEMA] Dados carregados. Total de alunos: {students.Count}");

        // Variável de controlo para manter o menu aberto
        bool running = true;

        // Início do ciclo do Menu (enquanto 'running' for verdadeiro, o programa não fecha)
        while (running)
        {
            Console.Clear(); // Limpa o ecrã para o menu aparecer sempre limpo no topo
            
            // Desenho do Menu na consola
            Console.WriteLine("=== GESTÃO ACADÉMICA COMPLETA ===");
            Console.WriteLine($"Total Alunos: {students.Count}"); // Mostra contagem em tempo real
            Console.WriteLine("----------------------------------");
            Console.WriteLine("1. Listar Detalhes dos Estudantes");
            Console.WriteLine("2. Adicionar Novo Estudante");
            Console.WriteLine("3. Adicionar Disciplina e Nota");
            Console.WriteLine("4. Atribuir Bolsa");
            Console.WriteLine("5. Guardar Dados");
            Console.WriteLine("0. Guardar e Sair");
            Console.Write("\nEscolha: "); // Pede ao utilizador para escolher

            // Lê o que o utilizador escreveu
            string option = Console.ReadLine();

            // Decide o que fazer com base na opção escolhida
            switch (option)
            {
                case "1": 
                    ListStudents(); // Chama o método que lista os alunos
                    break;
                case "2": 
                    AddStudentMenu(); // Chama o método para criar alunos
                    break;
                case "3": 
                    AddGradeMenu(); // Chama o método para dar notas
                    break;
                case "4": 
                    AddScholarshipMenu(); // Chama o método para dar bolsas
                    break;
                case "5": 
                    FileService.SaveData(students); // Chama o serviço para gravar no ficheiro JSON
                    Console.WriteLine("Guardado."); 
                    break;
                case "0": 
                    FileService.SaveData(students); // Guarda antes de sair (segurança)
                    running = false; // Muda a variável para false, o que quebra o ciclo 'while' e fecha o programa
                    break;
                default: 
                    Console.WriteLine("Inválido."); // Se escreverem algo que não existe (ex: "9")
                    break;
            }

            // Se o programa ainda estiver a correr, faz uma pausa antes de limpar o ecrã de novo
            if (running) 
            { 
                Console.WriteLine("\n[Enter para continuar...]"); 
                Console.ReadLine(); // Fica à espera de um Enter
            }
        }
    }

    // --- MÉTODOS AUXILIARES (A LÓGICA DO PROGRAMA) ---

    // Método para listar todos os alunos e as suas informações financeiras/académicas
    static void ListStudents()
    {
        Console.WriteLine("\n--- PAUTA DETALHADA ---");
        
        // Se a lista estiver vazia, avisa e sai do método
        if (students.Count == 0) { Console.WriteLine("Vazio."); return; }

        // Percorre cada estudante (s) na lista de estudantes
        foreach (var s in students)
        {
            s.DisplayInfo(); // Chama o método da classe Student que mostra ID, Nome e Média

            // --- PARTE ACADÉMICA: Mostrar Notas ---
            if (s.Grades.Count > 0) // Se o aluno tiver notas...
            {
                Console.WriteLine(" -> Histórico de Disciplinas:");
                foreach (var g in s.Grades) // Percorre cada nota
                {
                    // Operador Ternário: Se nota >= 9.5 escreve "APROVADO", senão "REPROVADO"
                    string status = g.Value >= 9.5m ? "APROVADO" : "REPROVADO";
                    
                    // Mostra Nome da Cadeira, Créditos, Professor e a Nota com o Status
                    Console.WriteLine($"    * {g.Course.Name} ({g.Course.Credits} ECTS)");
                    Console.WriteLine($"      Prof: {g.Course.Professor} | Nota: {g.Value} -> {status}");
                }
            }
            else
            {
                Console.WriteLine(" -> Sem notas registadas.");
            }

            // --- PARTE FINANCEIRA: Calcular Saldo ---
            // 1. Calcula quanto o aluno tem de pagar base (Polimorfismo: depende se é Licenciatura, Mestrado, etc.)
            decimal tuition = s.CalculateTuition();
            
            // 2. Soma todas as bolsas que o aluno tem (usa o Linq .Sum())
            decimal totalScholarship = s.Scholarships.Sum(x => x.Amount);
            
            // 3. Conta final: O que deve MENOS o que recebe de bolsa
            decimal finalToPay = tuition - totalScholarship;

            Console.WriteLine("    -------------------------");
            // Truque para mostrar o nome do tipo de aluno limpo (ex: tira o "StudentSys." se aparecer)
            Console.WriteLine($"    Tipo: {s.GetType().Name.Replace("Student", "")}");
            Console.WriteLine($"    Propina Base: {tuition}€");
            
            // Só mostra as bolsas se o aluno tiver alguma
            if (totalScholarship > 0) Console.WriteLine($"    Bolsas: -{totalScholarship}€");

            // Lógica para mostrar a mensagem certa dependendo do saldo
            if (finalToPay > 0) 
                Console.WriteLine($"    TOTAL A PAGAR: {finalToPay}€"); // Deve dinheiro
            else if (finalToPay < 0) 
                Console.WriteLine($"    CRÉDITO A RECEBER: {-finalToPay}€"); // A escola deve dinheiro (mostramos positivo)
            else 
                Console.WriteLine($"    CONTA SALDADA (0€)"); // Conta a zero
            
            Console.WriteLine("-----------------------------");
        }
    }

    // Método para adicionar notas e disciplinas
    static void AddGradeMenu()
    {
        Console.WriteLine("\n--- LANÇAR NOTA E DISCIPLINA ---");
        Console.Write("ID do Estudante: ");
        
        // Tenta converter o texto para número (ID). Se conseguir, guarda em 'id'.
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            // Procura na lista um estudante com esse ID
            Student s = students.Find(x => x.StudentID == id);
            
            // Se não encontrar (for null), avisa e sai
            if (s == null) { Console.WriteLine("Aluno não encontrado."); return; }

            Console.WriteLine($"Aluno: {s.Name}");
            
            // Pede os dados da disciplina
            Console.Write("Nome da Disciplina: "); 
            string cName = Console.ReadLine();
            
            Console.Write("Código (ex: POO): "); 
            string cCode = Console.ReadLine();
            
            Console.Write("Nome do Professor: "); 
            string prof = Console.ReadLine();

            Console.Write("Créditos ECTS (ex: 6): ");
            int.TryParse(Console.ReadLine(), out int credits); // Converte créditos para número

            // Pede a Nota final
            Console.Write("Nota Final (0-20): "); 
            if (decimal.TryParse(Console.ReadLine(), out decimal val))
            {
                // Cria um novo objeto Disciplina (Course) com os dados
                Course c = new Course(cName, cCode, credits, prof);
                
                // Adiciona a nota ao aluno (o método AddGrade já recalcula a média)
                s.AddGrade(c, val);
                
                // Guarda logo no ficheiro para não perder dados
                FileService.SaveData(students);
                
                // Dá feedback imediato se passou ou não
                string status = val >= 9.5m ? "APROVADO" : "REPROVADO";
                Console.WriteLine($"Nota registada! O aluno está {status} a esta cadeira.");
            }
            else Console.WriteLine("Nota inválida."); // Se a nota não for número
        }
        else Console.WriteLine("ID inválido."); // Se o ID não for número
    }

    // Método para criar novos alunos
    static void AddStudentMenu()
    {
        try 
        {
            Console.WriteLine("\n--- ADICIONAR ESTUDANTE ---");
            // Pede os dados comuns a todos
            Console.Write("Nome: "); string name = Console.ReadLine();
            Console.Write("ID: "); int id = int.Parse(Console.ReadLine()); 
            Console.Write("Email: "); string email = Console.ReadLine();
            
            Console.WriteLine("1. Licenciatura | 2. Mestrado | 3. Internacional");
            string type = Console.ReadLine();

            // Decide que tipo de Classe criar com base na escolha (Polimorfismo/Herança)
            if (type == "1") {
                Console.Write("Curso: "); string major = Console.ReadLine();
                Console.Write("Ano: "); int year = int.Parse(Console.ReadLine());
                // Cria um UndergraduateStudent e adiciona à lista genérica de Student
                students.Add(new UndergraduateStudent(name, id, email, major, year));
            }
            else if (type == "2") {
                Console.Write("Tese: "); string thesis = Console.ReadLine();
                Console.Write("Orientador: "); string advisor = Console.ReadLine();
                // Cria um GraduateStudent
                students.Add(new GraduateStudent(name, id, email, thesis, advisor));
            }
            else if (type == "3") {
                Console.Write("País: "); string country = Console.ReadLine();
                Console.Write("Visto: "); string visa = Console.ReadLine();
                // Cria um InternationalStudent
                students.Add(new InternationalStudent(name, id, email, country, visa));
            }
            
            FileService.SaveData(students); // Guarda no disco
            Console.WriteLine("Guardado!");
        }
        catch { Console.WriteLine("Erro nos dados."); } // Apanha erros se o utilizador meter letras onde deviam ser números
    }

    // Método para dar dinheiro (bolsas) aos alunos
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
                    // Cria o objeto Bolsa e adiciona à lista de bolsas do aluno
                    s.Scholarships.Add(new Scholarship(name, amount));
                    
                    FileService.SaveData(students); // Guarda
                    Console.WriteLine("Bolsa atribuída.");
                }
            }
        }
    }
}