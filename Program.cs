// --- BIBLIOTECAS ---
using System; // Funcionalidades básicas do sistema (Texto, Números, Consola)
using System.Collections.Generic; // Permite usar Listas (List<T>)
using System.Linq; // Permite usar funções matemáticas avançadas como .Sum()
using StudentSys; // Liga este ficheiro à pasta onde estão os teus Modelos (Student, Course, etc.)

class Program
{
    // --- VARIÁVEL GLOBAL ---
    // Criamos uma lista estática para guardar os alunos na memória RAM enquanto o programa corre.
    // É 'static' para poder ser acedida dentro do método Main.
    static List<Student> students = new List<Student>();

    // --- PONTO DE PARTIDA (Main) ---
    static void Main(string[] args)
    {
        // Configura a consola para aceitar caracteres especiais como o símbolo do Euro (€) e acentos.
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // 1. CARREGAR DADOS: Chama o FileService para ler o ficheiro JSON e encher a lista.
        students = FileService.LoadData();
        Console.WriteLine($"[SISTEMA] Dados carregados. Total: {students.Count}");

        // Variável de controlo para manter o menu aberto (ciclo infinito controlado).
        bool running = true;

        // Ciclo 'while': Enquanto 'running' for verdadeiro, o menu volta a aparecer.
        while (running)
        {
            Console.Clear(); // Limpa o texto antigo do ecrã.
            Console.WriteLine("=== GESTÃO ACADÉMICA COMPLETA ===");
            Console.WriteLine($"Total Alunos: {students.Count}"); // Mostra estatística em tempo real.
            Console.WriteLine("----------------------------------");
            // Opções do Menu
            Console.WriteLine("1. Listar Detalhes dos Estudantes");
            Console.WriteLine("2. Adicionar Novo Estudante");
            Console.WriteLine("3. Adicionar Disciplina e Nota");
            Console.WriteLine("4. Atribuir Bolsa");
            Console.WriteLine("5. Remover Estudante");
            Console.WriteLine("6. Guardar Dados (Manual)");
            Console.WriteLine("0. Guardar e Sair");
            Console.Write("\nEscolha: ");

            // Lê a opção que o utilizador escreveu.
            string option = Console.ReadLine();

            // 'Switch' funciona como um semáforo para encaminhar para o método certo.
            switch (option)
            {
                case "1": ListStudents(); break;      // Vai para a listagem
                case "2": AddStudentMenu(); break;    // Vai criar aluno
                case "3": AddGradeMenu(); break;      // Vai lançar notas
                case "4": AddScholarshipMenu(); break;// Vai dar bolsas
                case "5": RemoveStudentMenu(); break; // Vai apagar aluno
                case "6": 
                    FileService.SaveData(students);   // Guarda manualmente
                    Console.WriteLine("Guardado."); 
                    break;
                case "0": 
                    FileService.SaveData(students);   // Guarda antes de fechar (segurança)
                    running = false; // Muda para falso -> O ciclo 'while' para -> O programa fecha.
                    break;
                default: 
                    Console.WriteLine("Inválido.");   // Se escreverem algo errado.
                    break;
            }

            // Pausa para o utilizador conseguir ler a mensagem de sucesso/erro antes de limpar o ecrã.
            if (running) { Console.WriteLine("\n[Enter para continuar...]"); Console.ReadLine(); }
        }
    }

    // --- MÉTODO: REMOVER ESTUDANTE ---
    static void RemoveStudentMenu()
    {
        Console.WriteLine("\n--- REMOVER ESTUDANTE ---");
        Console.Write("Número de Aluno a remover: ");
        
        // Tenta converter o texto para número inteiro.
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            // Usa uma expressão Lambda para encontrar o aluno com aquele ID na lista.
            Student s = students.Find(x => x.StudentID == id);
            
            // Se o resultado for null, o aluno não existe.
            if (s == null) { Console.WriteLine("Estudante não encontrado!"); return; }

            // Remove o objeto da lista em memória.
            students.Remove(s);
            // Atualiza o ficheiro JSON imediatamente para não haver dados fantasma.
            FileService.SaveData(students);
            Console.WriteLine($"Estudante '{s.Name}' foi apagado do sistema.");
        }
        else Console.WriteLine("Número inválido.");
    }

    // --- MÉTODO: LISTAR (AQUI VÊ-SE O POLIMORFISMO) ---
    static void ListStudents()
    {
        Console.WriteLine("\n--- PAUTA DETALHADA ---");
        if (students.Count == 0) { Console.WriteLine("Vazio."); return; }

        // Percorre cada estudante na lista.
        foreach (var s in students)
        {
            // Chama o método DisplayInfo() da classe Student.
            // (Mostra ID, Nome e Média colorida).
            s.DisplayInfo();

            // Se o aluno tiver notas na lista 'Grades'...
            if (s.Grades.Count > 0)
            {
                Console.WriteLine(" -> Histórico de Disciplinas:");
                foreach (var g in s.Grades)
                {
                    // Operador Ternário: Se nota >= 9.5 escreve "APROVADO", senão "REPROVADO".
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
            // POLIMORFISMO: O método CalculateTuition() comporta-se de maneira diferente
            // dependendo se o aluno é Licenciatura (1000), Mestrado (2000) ou Internacional (5000).
            decimal tuition = s.CalculateTuition();
            
            // LINQ: Soma o valor (.Amount) de todas as bolsas na lista.
            decimal totalScholarship = s.Scholarships.Sum(x => x.Amount);

            // TRADUÇÃO: Converte o nome técnico da classe para Português.
            string tipoPortugues = "Desconhecido";
            string tipoIngles = s.GetType().Name; // Pega o nome da classe (ex: UndergraduateStudent)

            if (tipoIngles == "UndergraduateStudent") tipoPortugues = "Licenciatura";
            else if (tipoIngles == "GraduateStudent") tipoPortugues = "Mestrado";
            else if (tipoIngles == "InternationalStudent") tipoPortugues = "Internacional";

            Console.WriteLine("    -------------------------");
            Console.WriteLine($"    Tipo: {tipoPortugues}");
            
            // :0 formata para mostrar sem casas decimais (ex: 1000€).
            Console.WriteLine($"    Propina Anual: {tuition:0}€");
            
            if (totalScholarship > 0) 
            {
                Console.WriteLine($"    Bolsas Atribuídas: {totalScholarship:0}€");
                // Lista cada bolsa individualmente.
                foreach(var b in s.Scholarships)
                {
                    Console.WriteLine($"      (Bolsa {b.Name}: {b.Amount:0}€)");
                }
            }
            
            Console.WriteLine("-----------------------------");
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
            // Pede dados para criar a Disciplina (Objeto Course).
            Console.Write("Nome da Disciplina: "); string cName = Console.ReadLine();
            Console.Write("Código (ex: POO): "); string cCode = Console.ReadLine();
            Console.Write("Nome do Professor: "); string prof = Console.ReadLine();
            Console.Write("Créditos ECTS (ex: 6): ");
            int.TryParse(Console.ReadLine(), out int credits);

            Console.Write("Nota Final (0-20): "); 
            if (decimal.TryParse(Console.ReadLine(), out decimal val))
            {
                // COMPOSIÇÃO: Criamos um objeto Course...
                Course c = new Course(cName, cCode, credits, prof);
                // ...e associamo-lo ao Aluno através de uma Grade.
                s.AddGrade(c, val);
                
                FileService.SaveData(students); // Guarda logo.
                
                string status = val >= 9.5m ? "APROVADO" : "REPROVADO";
                Console.WriteLine($"Nota registada! O aluno está {status}.");
            }
            else Console.WriteLine("Nota inválida.");
        }
        else Console.WriteLine("Número inválido.");
    }

    // --- MÉTODO: CRIAR ALUNO (FÁBRICA DE OBJETOS) ---
    static void AddStudentMenu()
    {
        try // Bloco Try-Catch para apanhar erros (Requisito obrigatório).
        {
            Console.WriteLine("\n--- ADICIONAR ESTUDANTE ---");
            Console.Write("Nome: "); string name = Console.ReadLine();
            Console.Write("Número de Aluno: "); int id = int.Parse(Console.ReadLine()); 
            Console.Write("Email: "); string email = Console.ReadLine();
            Console.WriteLine("1. Licenciatura | 2. Mestrado | 3. Internacional");
            string type = Console.ReadLine();

            // HERANÇA NA PRÁTICA: Dependendo da escolha, criamos uma Subclasse diferente.
            // Todas elas são guardadas na mesma lista de 'Student' (Polimorfismo).
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
                // Recalcula a média para garantir que a verificação é justa.
                s.CalculateGPA(); 
                Console.WriteLine($"Aluno: {s.Name} | Média Atual: {s.GPA:F1}");

                Console.Write("Nome da Bolsa: "); string name = Console.ReadLine();
                Console.Write("Valor (€): ");
                decimal.TryParse(Console.ReadLine(), out decimal amount);
                Console.Write("Média Mínima Exigida: ");
                
                if (decimal.TryParse(Console.ReadLine(), out decimal minGpa)) {
                    // Cria o objeto bolsa.
                    Scholarship bolsa = new Scholarship(name, amount, minGpa);
                    
                    // LÓGICA DE ELEGIBILIDADE: Pergunta à bolsa se o aluno serve.
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