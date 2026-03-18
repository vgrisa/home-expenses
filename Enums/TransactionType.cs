namespace HomeExpenses.Enums
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Tipo de transação: Despesa ou Receita
    /// </summary>
    public enum TransactionType
    {
        [Display(Name = "Despesa")]
        Expense = 0,

        [Display(Name = "Receita")]
        Income = 1
    }
}
