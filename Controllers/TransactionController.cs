namespace HomeExpenses.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using HomeExpenses.Services;
    using HomeExpenses.DTOs;
    using HomeExpenses.Enums;

    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        /// <summary>
        /// Obtém todas as transações cadastradas
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionResponseDTO>>> GetAll()
        {
            try
            {
                var transactions = await _transactionService.GetAllTransactionsAsync();
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar transações", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém todas as transações de uma pessoa
        /// </summary>
        [HttpGet("person/{personId}")]
        public async Task<ActionResult<IEnumerable<TransactionResponseDTO>>> GetByPerson(int personId)
        {
            try
            {
                if (personId <= 0)
                    return BadRequest(new { message = "ID da pessoa deve ser maior que zero" });

                var transactions = await _transactionService.GetTransactionsByPersonAsync(personId);
                return Ok(transactions);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar transações da pessoa", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém todas as transações de uma categoria
        /// </summary>
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<TransactionResponseDTO>>> GetByCategory(int categoryId)
        {
            try
            {
                if (categoryId <= 0)
                    return BadRequest(new { message = "ID da categoria deve ser maior que zero" });

                var transactions = await _transactionService.GetTransactionsByCategoryAsync(categoryId);
                return Ok(transactions);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar transações da categoria", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém uma transação pelo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionDetailResponseDTO>> GetById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "ID deve ser maior que zero" });

                var transaction = await _transactionService.GetTransactionByIdAsync(id);
                return Ok(transaction);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar transação", error = ex.Message });
            }
        }

        /// <summary>
        /// Cria uma nova transação
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TransactionResponseDTO>> Create([FromBody] CreateTransactionDTO transactionDTO)
        {
            try
            {
                if (transactionDTO == null)
                    return BadRequest(new { message = "Dados da transação são obrigatórios" });

                var createdTransaction = await _transactionService.CreateTransactionAsync(transactionDTO);
                return CreatedAtAction(nameof(GetById), new { id = createdTransaction.Id }, createdTransaction);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao criar transação", error = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza uma transação existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<TransactionResponseDTO>> Update(int id, [FromBody] UpdateTransactionDTO transactionDTO)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "ID deve ser maior que zero" });

                if (transactionDTO == null)
                    return BadRequest(new { message = "Dados da transação são obrigatórios" });

                var updatedTransaction = await _transactionService.UpdateTransactionAsync(id, transactionDTO);
                return Ok(updatedTransaction);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao atualizar transação", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém as categorias disponíveis para um tipo de transação
        /// </summary>
        [HttpGet("categories/{transactionType}")]
        public async Task<ActionResult<IEnumerable<CategoryResponseDTO>>> GetAvailableCategories(int transactionType)
        {
            try
            {
                if (!Enum.IsDefined(typeof(TransactionType), transactionType))
                    return BadRequest(new { message = "Tipo de transação inválido. Valores válidos: 0 (Despesa) ou 1 (Receita)" });

                var type = (TransactionType)transactionType;
                var categories = await _transactionService.GetAvailableCategoriesAsync(type);

                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar categorias", error = ex.Message });
            }
        }
    }
}
