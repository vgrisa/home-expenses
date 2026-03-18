namespace HomeExpenses.Services
{
    using HomeExpenses.Data;
    using HomeExpenses;
    using HomeExpenses.DTOs;
    using HomeExpenses.Enums;
    using Microsoft.EntityFrameworkCore;
    using HomeExpenses.Entities;

    public interface ITransactionService
    {
        Task<IEnumerable<TransactionResponseDTO>> GetAllTransactionsAsync();
        Task<IEnumerable<TransactionResponseDTO>> GetTransactionsByPersonAsync(int personId);
        Task<IEnumerable<TransactionResponseDTO>> GetTransactionsByCategoryAsync(int categoryId);
        Task<TransactionDetailResponseDTO> GetTransactionByIdAsync(int id);
        Task<TransactionResponseDTO> CreateTransactionAsync(CreateTransactionDTO transactionDTO);
        Task<TransactionResponseDTO> UpdateTransactionAsync(int id, UpdateTransactionDTO transactionDTO);
        Task<IEnumerable<CategoryResponseDTO>> GetAvailableCategoriesAsync(TransactionType type);
    }

    public class TransactionService : ITransactionService
    {
        private readonly HomeExpensesContext _context;

        public TransactionService(HomeExpensesContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TransactionResponseDTO>> GetAllTransactionsAsync()
        {
            return await _context.Transactions
                .Include(t => t.Person)
                .Include(t => t.Category)
                .OrderBy(t => t.PersonId)
                .Select(t => new TransactionResponseDTO
                {
                    Id = t.Id,
                    Description = t.Description,
                    Value = t.Value,
                    Type = t.Type,
                    PersonId = t.PersonId,
                    CategoryId = t.CategoryId,
                    PersonName = t.Person.Name,
                    CategoryName = t.Category.Description
                }).ToListAsync();
        }

        public async Task<IEnumerable<TransactionResponseDTO>> GetTransactionsByPersonAsync(int personId)
        {
            var person = await _context.People.FindAsync(personId);
            if (person == null)
                throw new KeyNotFoundException($"Pessoa com ID {personId} não encontrada.");

            return await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.PersonId == personId)
                .OrderBy(t => t.PersonId)
                .Select(t => new TransactionResponseDTO
                {
                    Id = t.Id,
                    Description = t.Description,
                    Value = t.Value,
                    Type = t.Type,
                    PersonId = t.PersonId,
                    CategoryId = t.CategoryId,
                    PersonName = person.Name,
                    CategoryName = t.Category.Description
                }).ToListAsync();
        }

        public async Task<IEnumerable<TransactionResponseDTO>> GetTransactionsByCategoryAsync(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
                throw new KeyNotFoundException($"Categoria com ID {categoryId} não encontrada.");

            return await _context.Transactions
                .Include(t => t.Person)
                .Where(t => t.CategoryId == categoryId)
                .OrderBy(t => t.PersonId)
                .Select(t => new TransactionResponseDTO
                {
                    Id = t.Id,
                    Description = t.Description,
                    Value = t.Value,
                    Type = t.Type,
                    PersonId = t.PersonId,
                    CategoryId = t.CategoryId,
                    PersonName = t.Person.Name,
                    CategoryName = category.Description
                }).ToListAsync();
        }

        public async Task<TransactionDetailResponseDTO> GetTransactionByIdAsync(int id)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Person)
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null)
                throw new KeyNotFoundException($"Transação com ID {id} não encontrada.");

            return new TransactionDetailResponseDTO
            {
                Id = transaction.Id,
                Description = transaction.Description,
                Value = transaction.Value,
                Type = transaction.Type,
                Person = new PersonResponseDTO
                {
                    Id = transaction.Person.Id,
                    Name = transaction.Person.Name,
                    Age = transaction.Person.Age
                },
                Category = new CategoryResponseDTO
                {
                    Id = transaction.Category.Id,
                    Description = transaction.Category.Description,
                    Purpose = transaction.Category.Purpose
                }
            };
        }

        public async Task<TransactionResponseDTO> CreateTransactionAsync(CreateTransactionDTO transactionDTO)
        {
            if (transactionDTO == null)
                throw new ArgumentNullException(nameof(transactionDTO));

            if (string.IsNullOrWhiteSpace(transactionDTO.Description))
                throw new ArgumentException("Descrição é obrigatória.", nameof(transactionDTO.Description));

            if (transactionDTO.Value <= 0)
                throw new ArgumentException("Valor deve ser positivo.", nameof(transactionDTO.Value));

            var person = await _context.People.FindAsync(transactionDTO.PersonId);
            if (person == null)
                throw new KeyNotFoundException($"Pessoa com ID {transactionDTO.PersonId} não encontrada.");

            var category = await _context.Categories.FindAsync(transactionDTO.CategoryId);
            if (category == null)
                throw new KeyNotFoundException($"Categoria com ID {transactionDTO.CategoryId} não encontrada.");

            if (person.Age < 18 && transactionDTO.Type == TransactionType.Income)
            {
                throw new InvalidOperationException(
                    $"Pessoas menores de 18 anos ({person.Name}, {person.Age} anos) podem realizar apenas despesas, não receitas.");
            }

            if (!IsCategoryCompatibleWithTransactionType(category.Purpose, transactionDTO.Type))
            {
                string categoryPurposeName = GetDisplayName(category.Purpose);
                string transactionTypeName = GetDisplayName(transactionDTO.Type);
                throw new InvalidOperationException(
                    $"Categoria '{category.Description}' é destinada para {categoryPurposeName}, mas a transação é do tipo {transactionTypeName}.");
            }

            var transaction = new Transaction
            {
                Description = transactionDTO.Description,
                Value = transactionDTO.Value,
                Type = transactionDTO.Type,
                PersonId = transactionDTO.PersonId,
                CategoryId = transactionDTO.CategoryId
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return new TransactionResponseDTO
            {
                Id = transaction.Id,
                Description = transaction.Description,
                Value = transaction.Value,
                Type = transaction.Type,
                PersonId = transaction.PersonId,
                CategoryId = transaction.CategoryId,
                PersonName = person.Name,
                CategoryName = category.Description
            };
        }

        public async Task<TransactionResponseDTO> UpdateTransactionAsync(int id, UpdateTransactionDTO transactionDTO)
        {
            if (transactionDTO == null)
                throw new ArgumentNullException(nameof(transactionDTO));

            var transaction = await _context.Transactions
                .Include(t => t.Person)
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null)
                throw new KeyNotFoundException($"Transação com ID {id} não encontrada.");

            if (string.IsNullOrWhiteSpace(transactionDTO.Description))
                throw new ArgumentException("Descrição é obrigatória.", nameof(transactionDTO.Description));

            if (transactionDTO.Value <= 0)
                throw new ArgumentException("Valor deve ser positivo.", nameof(transactionDTO.Value));

            var person = await _context.People.FindAsync(transactionDTO.PersonId);
            if (person == null)
                throw new KeyNotFoundException($"Pessoa com ID {transactionDTO.PersonId} não encontrada.");

            var category = await _context.Categories.FindAsync(transactionDTO.CategoryId);
            if (category == null)
                throw new KeyNotFoundException($"Categoria com ID {transactionDTO.CategoryId} não encontrada.");

            if (person.Age < 18 && transactionDTO.Type == TransactionType.Income)
            {
                throw new InvalidOperationException(
                    $"Pessoas menores de 18 anos ({person.Name}, {person.Age} anos) podem realizar apenas despesas, não receitas.");
            }

            if (!IsCategoryCompatibleWithTransactionType(category.Purpose, transactionDTO.Type))
            {
                string categoryPurposeName = GetDisplayName(category.Purpose);
                string transactionTypeName = GetDisplayName(transactionDTO.Type);
                throw new InvalidOperationException(
                    $"Categoria '{category.Description}' é destinada para {categoryPurposeName}, mas a transação é do tipo {transactionTypeName}.");
            }

            transaction.Description = transactionDTO.Description;
            transaction.Value = transactionDTO.Value;
            transaction.Type = transactionDTO.Type;
            transaction.PersonId = transactionDTO.PersonId;
            transaction.CategoryId = transactionDTO.CategoryId;

            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();

            return new TransactionResponseDTO
            {
                Id = transaction.Id,
                Description = transaction.Description,
                Value = transaction.Value,
                Type = transaction.Type,
                PersonId = transaction.PersonId,
                CategoryId = transaction.CategoryId,
                PersonName = person.Name,
                CategoryName = category.Description
            };
        }

        public async Task<IEnumerable<CategoryResponseDTO>> GetAvailableCategoriesAsync(TransactionType type)
        {
            return await _context.Categories
                .Where(c => c.Purpose == CategoryPurpose.Both ||
                    (c.Purpose == CategoryPurpose.Expense && type == TransactionType.Expense) ||
                    (c.Purpose == CategoryPurpose.Income && type == TransactionType.Income))
                .OrderBy(c => c.Description)
                .Select(c => new CategoryResponseDTO
                {
                    Id = c.Id,
                    Description = c.Description,
                    Purpose = c.Purpose
                }).ToListAsync();
        }

        private bool IsCategoryCompatibleWithTransactionType(CategoryPurpose purpose, TransactionType type)
        {
            return purpose switch
            {
                CategoryPurpose.Both => true,
                CategoryPurpose.Expense => type == TransactionType.Expense,
                CategoryPurpose.Income => type == TransactionType.Income,
                _ => false
            };
        }

        private string GetDisplayName(Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var attribute = (System.ComponentModel.DataAnnotations.DisplayAttribute?)
                Attribute.GetCustomAttribute(fieldInfo!, typeof(System.ComponentModel.DataAnnotations.DisplayAttribute));
            return attribute?.Name ?? value.ToString();
        }
    }
}
