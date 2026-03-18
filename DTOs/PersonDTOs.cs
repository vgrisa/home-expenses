namespace HomeExpenses.DTOs
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// DTO para criar uma nova pessoa
    /// </summary>
    public class CreatePersonDTO
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Nome deve ter entre 1 e 200 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Idade é obrigatória")]
        [Range(0, 150, ErrorMessage = "Idade deve ser entre 0 e 150 anos")]
        public int Age { get; set; }
    }

    /// <summary>
    /// DTO para atualizar uma pessoa
    /// </summary>
    public class UpdatePersonDTO
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Nome deve ter entre 1 e 200 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Idade é obrigatória")]
        [Range(0, 150, ErrorMessage = "Idade deve ser entre 0 e 150 anos")]
        public int Age { get; set; }
    }

    /// <summary>
    /// DTO para retornar dados de uma pessoa
    /// </summary>
    public class PersonResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    /// <summary>
    /// DTO para retornar pessoa com suas transações
    /// </summary>
    public class PersonDetailResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public IEnumerable<TransactionResponseDTO> Transactions { get; set; } = new List<TransactionResponseDTO>();
    }
}
