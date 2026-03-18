namespace HomeExpenses.Entities
{
    using System.ComponentModel.DataAnnotations;
    using HomeExpenses.Enums;

    public class Category
    {
        [Key]
        public int Id { get; set; }

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Finalidade é obrigatória")]
        public CategoryPurpose Purpose { get; set; } = CategoryPurpose.Both;

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
