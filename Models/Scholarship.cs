namespace StudentSys
{
    // Uma classe simples para guardar dados de uma bolsa
    public class Scholarship
    {
        // Nome da bolsa (ex: "Bolsa de Mérito", "SAS")
        public string Name { get; set; }
        
        // Valor monetário da bolsa (ex: 500, 1000)
        public decimal Amount { get; set; }

        // Construtor simples para obrigar a preencher os dados
        public Scholarship(string name, decimal amount)
        {
            Name = name;
            Amount = amount;
        }
    }
}