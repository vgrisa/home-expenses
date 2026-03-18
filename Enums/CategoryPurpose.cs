namespace HomeExpenses.Enums
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
/// Finalidade da categoria: pode ser usada para Despesa, Receita ou Ambas
    /// </summary>
    public enum CategoryPurpose
    {
        [Display(Name = "Despesa")]
        Expense = 0,

        [Display(Name = "Receita")]
        Income = 1,

        [Display(Name = "Ambas")]
        Both = 2
    }
}
