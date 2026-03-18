namespace HomeExpenses.Services
{
    using HomeExpenses.Data;
    using HomeExpenses;
    using HomeExpenses.DTOs;
    using HomeExpenses.Enums;
    using Microsoft.EntityFrameworkCore;
    using HomeExpenses.Entities;

    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDTO>> GetAllCategoriesAsync();
        Task<CategoryDetailResponseDTO> GetCategoryByIdAsync(int id);
        Task<IEnumerable<CategoryResponseDTO>> GetCategoriesByPurposeAsync(int purpose);
        Task<CategoryResponseDTO> CreateCategoryAsync(CreateCategoryDTO categoryDTO);
        Task<CategoryResponseDTO> UpdateCategoryAsync(int id, UpdateCategoryDTO categoryDTO);
    }

    public class CategoryService : ICategoryService
    {
        private readonly HomeExpensesContext _context;

        public CategoryService(HomeExpensesContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryResponseDTO>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .OrderBy(c => c.Description)
                .Select(c => new CategoryResponseDTO
                {
                    Id = c.Id,
                    Description = c.Description,
                    Purpose = c.Purpose
                }).ToListAsync();
        }

        public async Task<CategoryDetailResponseDTO> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories
               .Include(c => c.Transactions)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                throw new KeyNotFoundException($"Categoria com ID {id} não encontrada.");

            return new CategoryDetailResponseDTO
            {
                Id = category.Id,
                Description = category.Description,
                Purpose = category.Purpose,
                Transactions = category.Transactions.Select(t => new TransactionResponseDTO
                {
                    Id = t.Id,
                    Description = t.Description,
                    Value = t.Value,
                    Type = t.Type,
                    PersonId = t.PersonId,
                    CategoryId = t.CategoryId,
                    PersonName = t.Person?.Name,
                    CategoryName = category.Description
                }).ToList()
            };
        }

        public async Task<IEnumerable<CategoryResponseDTO>> GetCategoriesByPurposeAsync(int purpose)
        {
            if (!Enum.IsDefined(typeof(CategoryPurpose), purpose))
                throw new ArgumentException("Finalidade inválida. Valores válidos: 0 (Despesa), 1 (Receita), 2 (Ambas)");

            return await _context.Categories
                .Where(c => c.Purpose == (CategoryPurpose)purpose)
                .OrderBy(c => c.Description)
                .Select(c => new CategoryResponseDTO
                {
                    Id = c.Id,
                    Description = c.Description,
                    Purpose = c.Purpose
                }).ToListAsync();
        }

        public async Task<CategoryResponseDTO> CreateCategoryAsync(CreateCategoryDTO categoryDTO)
        {
            if (categoryDTO == null)
                throw new ArgumentNullException(nameof(categoryDTO));

            if (string.IsNullOrWhiteSpace(categoryDTO.Description))
                throw new ArgumentException("Descrição é obrigatória.", nameof(categoryDTO.Description));

            if (categoryDTO.Description.Length > 400)
                throw new ArgumentException("Descrição deve ter no máximo 400 caracteres.", nameof(categoryDTO.Description));

            var category = new Category
            {
                Description = categoryDTO.Description,
                Purpose = categoryDTO.Purpose
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return new CategoryResponseDTO
            {
                Id = category.Id,
                Description = category.Description,
                Purpose = category.Purpose
            };
        }

        public async Task<CategoryResponseDTO> UpdateCategoryAsync(int id, UpdateCategoryDTO categoryDTO)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                throw new KeyNotFoundException($"Categoria com ID {id} não encontrada.");

            if (string.IsNullOrWhiteSpace(categoryDTO.Description))
                throw new ArgumentException("Descrição é obrigatória.", nameof(categoryDTO.Description));

            if (categoryDTO.Description.Length > 400)
                throw new ArgumentException("Descrição deve ter no máximo 400 caracteres.", nameof(categoryDTO.Description));

            category.Description = categoryDTO.Description;
            category.Purpose = categoryDTO.Purpose;

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return new CategoryResponseDTO
            {
                Id = category.Id,
                Description = category.Description,
                Purpose = category.Purpose
            };
        }
    }
}
