namespace HomeExpenses.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using HomeExpenses.Services;
    using HomeExpenses.DTOs;

    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _personService;

        public PersonController(IPersonService personService)
        {
            _personService = personService;
        }

        /// <summary>
        /// Obtém todas as pessoas cadastradas
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonResponseDTO>>> GetAll()
        {
            try
            {
                var people = await _personService.GetAllPeopleAsync();
                return Ok(people);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar pessoas", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém uma pessoa pelo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PersonDetailResponseDTO>> GetById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "ID deve ser maior que zero" });

                var person = await _personService.GetPersonByIdAsync(id);
                return Ok(person);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar pessoa", error = ex.Message });
            }
        }

        /// <summary>
        /// Cria uma nova pessoa
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PersonResponseDTO>> Create([FromBody] CreatePersonDTO personDTO)
        {
            try
            {
                if (personDTO == null)
                    return BadRequest(new { message = "Dados da pessoa são obrigatórios" });

                var createdPerson = await _personService.CreatePersonAsync(personDTO);
                return CreatedAtAction(nameof(GetById), new { id = createdPerson.Id }, createdPerson);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao criar pessoa", error = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza uma pessoa existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<PersonResponseDTO>> Update(int id, [FromBody] UpdatePersonDTO personDTO)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "ID deve ser maior que zero" });

                if (personDTO == null)
                    return BadRequest(new { message = "Dados da pessoa são obrigatórios" });

                var updatedPerson = await _personService.UpdatePersonAsync(id, personDTO);
                return Ok(updatedPerson);
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
                return StatusCode(500, new { message = "Erro ao atualizar pessoa", error = ex.Message });
            }
        }

        /// <summary>
        /// Deleta uma pessoa e todas as suas transações
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { message = "ID deve ser maior que zero" });

                await _personService.DeletePersonAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao deletar pessoa", error = ex.Message });
            }
        }
    }
}
