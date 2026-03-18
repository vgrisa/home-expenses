namespace HomeExpenses.Services
{
    using HomeExpenses.Data;
    using HomeExpenses;
    using HomeExpenses.DTOs;
    using Microsoft.EntityFrameworkCore;
    using HomeExpenses.Entities;

    public interface IPersonService
    {
        Task<IEnumerable<PersonResponseDTO>> GetAllPeopleAsync();
        Task<PersonDetailResponseDTO> GetPersonByIdAsync(int id);
        Task<PersonResponseDTO> CreatePersonAsync(CreatePersonDTO personDTO);
        Task<PersonResponseDTO> UpdatePersonAsync(int id, UpdatePersonDTO personDTO);
        Task<bool> DeletePersonAsync(int id);
    }

    public class PersonService : IPersonService
    {
        private readonly HomeExpensesContext _context;

        public PersonService(HomeExpensesContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PersonResponseDTO>> GetAllPeopleAsync()
        {
            return await _context.People
             .OrderBy(p => p.Name)
          .Select(p => new PersonResponseDTO
          {
              Id = p.Id,
              Name = p.Name,
              Age = p.Age
          })
                     .ToListAsync();
        }

        public async Task<PersonDetailResponseDTO> GetPersonByIdAsync(int id)
        {
            var person = await _context.People
                 .Include(p => p.Transactions)
     .FirstOrDefaultAsync(p => p.Id == id);

            if (person == null)
                throw new KeyNotFoundException($"Pessoa com ID {id} não encontrada.");

            return new PersonDetailResponseDTO
            {
                Id = person.Id,
                Name = person.Name,
                Age = person.Age,
                Transactions = person.Transactions.Select(t => new TransactionResponseDTO
                {
                    Id = t.Id,
                    Description = t.Description,
                    Value = t.Value,
                    Type = t.Type,
                    PersonId = t.PersonId,
                    CategoryId = t.CategoryId,
                    PersonName = person.Name,
                    CategoryName = t.Category?.Description
                }).ToList()
            };
        }

        public async Task<PersonResponseDTO> CreatePersonAsync(CreatePersonDTO personDTO)
        {
            if (personDTO == null)
                throw new ArgumentNullException(nameof(personDTO));

            if (string.IsNullOrWhiteSpace(personDTO.Name))
                throw new ArgumentException("Nome é obrigatório.", nameof(personDTO.Name));

            if (personDTO.Age < 0 || personDTO.Age > 150)
                throw new ArgumentException("Idade deve ser entre 0 e 150 anos.", nameof(personDTO.Age));

            var person = new Person
            {
                Name = personDTO.Name,
                Age = personDTO.Age
            };

            _context.People.Add(person);
            await _context.SaveChangesAsync();

            return new PersonResponseDTO
            {
                Id = person.Id,
                Name = person.Name,
                Age = person.Age
            };
        }

        public async Task<PersonResponseDTO> UpdatePersonAsync(int id, UpdatePersonDTO personDTO)
        {
            var person = await _context.People.FindAsync(id);

            if (person == null)
                throw new KeyNotFoundException($"Pessoa com ID {id} não encontrada.");

            if (string.IsNullOrWhiteSpace(personDTO.Name))
                throw new ArgumentException("Nome é obrigatório.", nameof(personDTO.Name));

            if (personDTO.Age < 0 || personDTO.Age > 150)
                throw new ArgumentException("Idade deve ser entre 0 e 150 anos.", nameof(personDTO.Age));

            person.Name = personDTO.Name;
            person.Age = personDTO.Age;

            _context.People.Update(person);
            await _context.SaveChangesAsync();

            return new PersonResponseDTO
            {
                Id = person.Id,
                Name = person.Name,
                Age = person.Age
            };
        }

        public async Task<bool> DeletePersonAsync(int id)
        {
            var person = await _context.People.FindAsync(id);

            if (person == null)
                throw new KeyNotFoundException($"Pessoa com ID {id} não encontrada.");

            _context.People.Remove(person);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
