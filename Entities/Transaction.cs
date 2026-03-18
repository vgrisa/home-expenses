namespace HomeExpenses.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using HomeExpenses.Enums;

    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(400, MinimumLength = 1, ErrorMessage = "Descrição deve ter entre 1 e 400 caracteres")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser um número positivo")]
        public decimal Value { get; set; }

        [Required(ErrorMessage = "Tipo de transação é obrigatório")]
        public TransactionType Type { get; set; }

        [ForeignKey("Person")]
        public int PersonId { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        public virtual Person Person { get; set; } = null!;
        public virtual Category Category { get; set; } = null!;
    }
}
