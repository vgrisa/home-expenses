namespace HomeExpenses.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using HomeExpenses.Services;
    using HomeExpenses.DTOs;

    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Obtém todas as categorias cadastradas
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryResponseDTO>>> GetAll()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar categorias", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém uma categoria pelo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDetailResponseDTO>> GetById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "ID deve ser maior que zero" });

                var category = await _categoryService.GetCategoryByIdAsync(id);
                return Ok(category);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar categoria", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém categorias por finalidade
        /// </summary>
        /// <param name="purpose">0=Despesa, 1=Receita, 2=Ambas</param>
        [HttpGet("purpose/{purpose}")]
        public async Task<ActionResult<IEnumerable<CategoryResponseDTO>>> GetByPurpose(int purpose)
        {
            try
            {
                var categories = await _categoryService.GetCategoriesByPurposeAsync(purpose);
                return Ok(categories);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar categorias por finalidade", error = ex.Message });
            }
        }

        /// <summary>
        /// Cria uma nova categoria
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CategoryResponseDTO>> Create([FromBody] CreateCategoryDTO categoryDTO)
        {
            try
            {
                if (categoryDTO == null)
                    return BadRequest(new { message = "Dados da categoria são obrigatórios" });

                var createdCategory = await _categoryService.CreateCategoryAsync(categoryDTO);
                return CreatedAtAction(nameof(GetById), new { id = createdCategory.Id }, createdCategory);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao criar categoria", error = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza uma categoria existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<CategoryResponseDTO>> Update(int id, [FromBody] UpdateCategoryDTO categoryDTO)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "ID deve ser maior que zero" });

                if (categoryDTO == null)
                    return BadRequest(new { message = "Dados da categoria são obrigatórios" });

                var updatedCategory = await _categoryService.UpdateCategoryAsync(id, categoryDTO);
                return Ok(updatedCategory);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao atualizar categoria", error = ex.Message });
            }
        }
    }
}
