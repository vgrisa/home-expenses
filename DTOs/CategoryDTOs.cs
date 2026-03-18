namespace HomeExpenses.DTOs
{
    using System.ComponentModel.DataAnnotations;
    using HomeExpenses.Enums;

    /// <summary>
    /// DTO para criar uma nova categoria
    /// </summary>
    public class CreateCategoryDTO
    {
        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(400, MinimumLength = 1, ErrorMessage = "Descrição deve ter entre 1 e 400 caracteres")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Finalidade é obrigatória")]
        public CategoryPurpose Purpose { get; set; }
    }

    /// <summary>
    /// DTO para atualizar uma categoria
    /// </summary>
    public class UpdateCategoryDTO
    {
        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(400, MinimumLength = 1, ErrorMessage = "Descrição deve ter entre 1 e 400 caracteres")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Finalidade é obrigatória")]
        public CategoryPurpose Purpose { get; set; }
    }

    /// <summary>
    /// DTO para retornar dados de uma categoria
    /// </summary>
    public class CategoryResponseDTO
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public CategoryPurpose Purpose { get; set; }
    }

    /// <summary>
    /// DTO para retornar categoria com suas transações
    /// </summary>
    public class CategoryDetailResponseDTO
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public CategoryPurpose Purpose { get; set; }
        public IEnumerable<TransactionResponseDTO> Transactions { get; set; } = new List<TransactionResponseDTO>();
    }
}
