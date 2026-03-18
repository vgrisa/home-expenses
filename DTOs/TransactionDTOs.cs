namespace HomeExpenses.DTOs
{
    using System.ComponentModel.DataAnnotations;
    using HomeExpenses.Enums;

    /// <summary>
    /// DTO para criar uma nova transação
    /// </summary>
    public class CreateTransactionDTO
    {
        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(400, MinimumLength = 1, ErrorMessage = "Descrição deve ter entre 1 e 400 caracteres")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser um número positivo")]
        public decimal Value { get; set; }

        [Required(ErrorMessage = "Tipo de transação é obrigatório")]
        public TransactionType Type { get; set; }

        [Required(ErrorMessage = "ID da pessoa é obrigatório")]
        public int PersonId { get; set; }

        [Required(ErrorMessage = "ID da categoria é obrigatório")]
        public int CategoryId { get; set; }
    }

    /// <summary>
    /// DTO para atualizar uma transação existente
    /// </summary>
    public class UpdateTransactionDTO
    {
        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(400, MinimumLength = 1, ErrorMessage = "Descrição deve ter entre 1 e 400 caracteres")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser um número positivo")]
        public decimal Value { get; set; }

        [Required(ErrorMessage = "Tipo de transação é obrigatório")]
        public TransactionType Type { get; set; }

        [Required(ErrorMessage = "ID da pessoa é obrigatório")]
        public int PersonId { get; set; }

        [Required(ErrorMessage = "ID da categoria é obrigatório")]
        public int CategoryId { get; set; }
    }

    /// <summary>
    /// DTO para retornar dados de uma transação
    /// </summary>
    public class TransactionResponseDTO
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public TransactionType Type { get; set; }
        public int PersonId { get; set; }
        public int CategoryId { get; set; }
        public string? PersonName { get; set; }
        public string? CategoryName { get; set; }
    }

    /// <summary>
    /// DTO para retornar transação com detalhes completos
    /// </summary>
    public class TransactionDetailResponseDTO
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public TransactionType Type { get; set; }
        public PersonResponseDTO Person { get; set; } = null!;
        public CategoryResponseDTO Category { get; set; } = null!;
    }
}
