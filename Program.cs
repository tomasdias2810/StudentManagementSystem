// --- BIBLIOTECAS (NAMESPACES) ---
using System; // Funcionalidades base do C# (Console, Texto, Números)
using System.Collections.Generic; // Permite usar Listas (List<T>)
using System.Linq; // Permite usar funções matemáticas avançadas como .Sum()
using StudentSys; // Liga este ficheiro à pasta 'Models' para reconhecer as classes

class Program
{
    // --- VARIÁVEL GLOBAL (BASE DE DADOS EM MEMÓRIA) ---
    // Criamos uma lista estática para guardar os alunos enquanto o programa corre.
    // É 'static' para poder ser usada dentro do método Main sem criar um objeto 'Program'.
    static List<Student> students = new List<Student>();

    // --- PONTO DE PARTIDA (MÉTODO MAIN) ---
    static void Main(string[] args)
    {
        // Configuração para a consola aceitar o símbolo do Euro (€) e acentos.
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // 1. CARREGAR DADOS: Chama o FileService para ler o ficheiro JSON e encher a lista.
        students = FileService.LoadData();
        Console.WriteLine($"[SISTEMA] Dados carregados. Total: {students.Count}");

        // Variável de controlo para manter o programa aberto num ciclo.
        bool running = true;

        // Ciclo 'while': Enquanto 'running' for verdadeiro, o menu volta sempre a aparecer.
        while (running)
        {
            Console.Clear(); // Limpa o ecrã para o menu aparecer limpo no topo.
            
            // --- DESENHO DO MENU ---
            Console.WriteLine("=== GESTÃO ACADÉMICA COMPLETA ===");
            Console.WriteLine($"Total Alunos: {students.Count}"); // Estatística em tempo real
            Console.WriteLine("----------------------------------");
            Console.WriteLine("1. Listar Detalhes dos Estudantes");
            Console.WriteLine("2. Adicionar Novo Estudante");
            Console.WriteLine("3. Adicionar Disciplina e Nota");
            Console.WriteLine("4. Atribuir Bolsa");
            Console.WriteLine("5. Remover Estudante");
            Console.WriteLine("6. Pesquisar Estudante");      // Opção de pesquisa
            Console.WriteLine("7. Guardar Dados (Manual)");    // Opção de segurança
            Console.WriteLine("0. Guardar e Sair");
            Console.Write("\nEscolha: ");

            // Lê a opção escolhida pelo utilizador
            string option = Console.ReadLine();

            // Estrutura 'Switch' para encaminhar a escolha para o método correto.
            switch (option)
            {
                case "1": ListStudents(); break;      // Listar todos
                case "2": AddStudentMenu(); break;    // Criar aluno
                case "3": AddGradeMenu(); break;      // Lançar notas
                case "4": AddScholarshipMenu(); break;// Dar bolsas
                case "5": RemoveStudentMenu(); break; // Apagar aluno
                case "6": SearchStudentMenu(); break; // Pesquisar individual
                case "7": 
                    FileService.SaveData(students);   // Guardar manual (Persistência)
                    Console.WriteLine("Guardado."); 
                    break;
                case "0": 
                    FileService.SaveData(students);   // Guardar automático antes de sair
                    running = false; // Quebra o ciclo while -> O programa fecha.
                    break;
                default: 
                    Console.WriteLine("Inválido.");   // Tratamento de opção errada
                    break;
            }

            // Pausa para o utilizador ler o resultado antes de limpar o ecrã novamente.
            if (running) { Console.WriteLine("\n[Enter para continuar...]"); Console.ReadLine(); }
        }
    }

    // --- MÉTODO: PESQUISAR ESTUDANTE ---
    static void SearchStudentMenu()
    {
        Console.WriteLine("\n--- PESQUISAR POR NÚMERO ---");
        Console.Write("Introduza o Número de Aluno: ");
        
        // Validação: Tenta converter texto em número.
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            // LINQ/Lambda: Procura na lista o primeiro aluno cujo ID corresponda.
            Student s = students.Find(x => x.StudentID == id);

            if (s != null)
            {
                Console.WriteLine("\n>>> ALUNO ENCONTRADO <<<");
                // Reutilizamos o método auxiliar para mostrar os dados bonitos.
                PrintStudentDetails(s); 
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Aluno não encontrado com esse número.");
                Console.ResetColor();
            }
        }
        else Console.WriteLine("Número inválido.");
    }

    // --- MÉTODO: REMOVER ESTUDANTE ---
    static void RemoveStudentMenu()
    {
        Console.WriteLine("\n--- REMOVER ESTUDANTE ---");
        Console.Write("Número de Aluno a remover: ");
        
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            Student s = students.Find(x => x.StudentID == id);
            
            if (s == null) { Console.WriteLine("Estudante não encontrado!"); return; }

            // Remove o objeto da lista em memória.
            students.Remove(s);
            // Atualiza logo o ficheiro JSON (Persistência).
            FileService.SaveData(students);
            Console.WriteLine($"Estudante '{s.Name}' foi apagado do sistema.");
        }
        else Console.WriteLine("Número inválido.");
    }

    // --- MÉTODO: LISTAR TODOS ---
    static void ListStudents()
    {
        Console.WriteLine("\n--- PAUTA DETALHADA ---");
        if (students.Count == 0) { Console.WriteLine("Vazio."); return; }

        // Ciclo Foreach para percorrer a lista inteira.
        foreach (var s in students)
        {
            PrintStudentDetails(s); // Chama o método auxiliar de impressão.
            Console.WriteLine("-----------------------------");
        }
    }

    // --- MÉTODO AUXILIAR: IMPRIMIR DETALHES ---
    // Criado para evitar repetir código no 'Listar' e no 'Pesquisar'.
    static void PrintStudentDetails(Student s)
    {
        // Chama o DisplayInfo da classe Student (Encapsulamento).
        s.DisplayInfo();

        // Verifica se o aluno tem notas registadas.
        if (s.Grades.Count > 0)
        {
            Console.WriteLine(" -> Histórico de Disciplinas:");
            foreach (var g in s.Grades)
            {
                // Lógica Ternária: Se nota >= 9.5 escreve "APROVADO", senão "REPROVADO".
                string status = g.Value >= 9.5m ? "APROVADO" : "REPROVADO";
                Console.WriteLine($"    * {g.Course.Name} ({g.Course.Credits} ECTS)");
                Console.WriteLine($"      Prof: {g.Course.Professor} | Nota: {g.Value} -> {status}");
            }
        }
        else
        {
            Console.WriteLine(" -> Sem notas registadas.");
        }

        // --- CÁLCULOS FINANCEIROS ---
        // POLIMORFISMO: O método CalculateTuition() comporta-se de forma diferente
        // dependendo se o objeto é Undergraduate (1000€), Graduate (2000€), etc.
        decimal tuition = s.CalculateTuition();
        
        // LINQ: Soma (.Sum) o valor (.Amount) de todas as bolsas na lista.
        decimal totalScholarship = s.Scholarships.Sum(x => x.Amount);

        // --- TRADUÇÃO DE TIPOS ---
        // Converte o nome da classe C# (Inglês) para texto legível em Português.
        string tipoPortugues = "Desconhecido";
        string tipoIngles = s.GetType().Name;

        if (tipoIngles == "UndergraduateStudent") tipoPortugues = "Licenciatura";
        else if (tipoIngles == "GraduateStudent") tipoPortugues = "Mestrado";
        else if (tipoIngles == "InternationalStudent") tipoPortugues = "Internacional";

        Console.WriteLine("    -------------------------");
        Console.WriteLine($"    Tipo: {tipoPortugues}");
        // :0 formata para não mostrar casas decimais (ex: 1000€).
        Console.WriteLine($"    Propina Anual: {tuition:0}€"); 
        
        if (totalScholarship > 0) 
        {
            Console.WriteLine($"    Bolsas Atribuídas: {totalScholarship:0}€");
            foreach(var b in s.Scholarships)
            {
                Console.WriteLine($"      (Bolsa {b.Name}: {b.Amount:0}€)");
            }
        }
    }

    // --- MÉTODO: LANÇAR NOTAS ---
    static void AddGradeMenu()
    {
        Console.WriteLine("\n--- LANÇAR NOTA E DISCIPLINA ---");
        Console.Write("Número de Aluno: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            Student s = students.Find(x => x.StudentID == id);
            if (s == null) { Console.WriteLine("Aluno não encontrado."); return; }

            Console.WriteLine($"Aluno: {s.Name}");
            // Pede dados para criar o objeto 'Course' (Composição).
            Console.Write("Nome da Disciplina: "); string cName = Console.ReadLine();
            Console.Write("Código (ex: POO): "); string cCode = Console.ReadLine();
            Console.Write("Nome do Professor: "); string prof = Console.ReadLine();
            Console.Write("Créditos ECTS (ex: 6): ");
            int.TryParse(Console.ReadLine(), out int credits);

            Console.Write("Nota Final (0-20): "); 
            if (decimal.TryParse(Console.ReadLine(), out decimal val))
            {
                // Cria a disciplina e associa a nota ao aluno.
                Course c = new Course(cName, cCode, credits, prof);
                s.AddGrade(c, val);
                
                FileService.SaveData(students); // Guarda logo.
                
                string status = val >= 9.5m ? "APROVADO" : "REPROVADO";
                Console.WriteLine($"Nota registada! O aluno está {status}.");
            }
            else Console.WriteLine("Nota inválida.");
        }
        else Console.WriteLine("Número inválido.");
    }

    // --- MÉTODO: CRIAR ALUNO ---
    static void AddStudentMenu()
    {
        try // Tratamento de Exceções para evitar que o programa crashe.
        {
            Console.WriteLine("\n--- ADICIONAR ESTUDANTE ---");
            Console.Write("Nome: "); string name = Console.ReadLine();
            Console.Write("Número de Aluno: "); int id = int.Parse(Console.ReadLine()); 
            Console.Write("Email: "); string email = Console.ReadLine();
            Console.WriteLine("1. Licenciatura | 2. Mestrado | 3. Internacional");
            string type = Console.ReadLine();

            // Lógica de HERANÇA: Cria instâncias de subclasses diferentes.
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

    // --- MÉTODO: ATRIBUIR BOLSA ---
    static void AddScholarshipMenu()
    {
        Console.WriteLine("\n--- ATRIBUIR BOLSA ---");
        Console.Write("Número de Aluno: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            Student s = students.Find(x => x.StudentID == id);
            if (s != null)
            {
                s.CalculateGPA(); // Garante que a média está atualizada.
                Console.WriteLine($"Aluno: {s.Name} | Média Atual: {s.GPA:F1}");

                Console.Write("Nome da Bolsa: "); string name = Console.ReadLine();
                Console.Write("Valor (€): ");
                decimal.TryParse(Console.ReadLine(), out decimal amount);
                Console.Write("Média Mínima Exigida: ");
                
                if (decimal.TryParse(Console.ReadLine(), out decimal minGpa)) {
                    Scholarship bolsa = new Scholarship(name, amount, minGpa);
                    
                    // LÓGICA DE NEGÓCIO: Verifica elegibilidade (Requisito slide 6).
                    if (bolsa.IsEligible(s))
                    {
                        s.Scholarships.Add(bolsa); // Adiciona à lista.
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