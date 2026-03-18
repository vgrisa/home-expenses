namespace HomeExpenses.Data
{
    using Microsoft.EntityFrameworkCore;
    using HomeExpenses.Enums;
    using HomeExpenses.Entities;

    public class HomeExpensesContext : DbContext
    {
        public HomeExpensesContext(DbContextOptions<HomeExpensesContext> options) : base(options)
        {
        }

        public DbSet<Person> People { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=homeexpenses.sqlite");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Person>()
                .HasMany(p => p.Transactions)
                .WithOne(t => t.Person)
                .HasForeignKey(t => t.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Category>()
                .HasMany(c => c.Transactions)
                .WithOne(t => t.Category)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed de dados iniciais
            modelBuilder.Entity<Category>().HasData(
                // Expense
                new Category { Id = 1, Description = "Despesas com alimentação", Purpose = CategoryPurpose.Expense },
                new Category { Id = 2, Description = "Despesas com transporte", Purpose = CategoryPurpose.Expense },
                new Category { Id = 3, Description = "Despesas com saúde", Purpose = CategoryPurpose.Expense },
                new Category { Id = 4, Description = "Despesas com lazer", Purpose = CategoryPurpose.Expense },
                new Category { Id = 5, Description = "Despesas com educação", Purpose = CategoryPurpose.Expense },

                // Income
                new Category { Id = 6, Description = "Receita de salário", Purpose = CategoryPurpose.Income },
                new Category { Id = 7, Description = "Receita de trabalho freelance", Purpose = CategoryPurpose.Income },
                new Category { Id = 8, Description = "Receita de investimentos", Purpose = CategoryPurpose.Income },

                // Both
                new Category { Id = 9, Description = "Outras transações", Purpose = CategoryPurpose.Both }
            );
        }
    }
}
