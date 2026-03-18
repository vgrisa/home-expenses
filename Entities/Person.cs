namespace HomeExpenses.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class Person
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Nome deve ter entre 1 e 200 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Idade é obrigatória")]
        [Range(0, 150, ErrorMessage = "Idade deve ser entre 0 e 150 anos")]
        public int Age { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
