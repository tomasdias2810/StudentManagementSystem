namespace StudentSys
{
    public class Scholarship
    {
        public string Name { get; set; }    // Nome da bolsa (ex: Mérito)
        public decimal Amount { get; set; } // Valor a descontar na propina

        public Scholarship(string name, decimal amount)
        {
            Name = name;
            Amount = amount;
        }
    }
}