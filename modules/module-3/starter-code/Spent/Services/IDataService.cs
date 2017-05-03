using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spent
{
	public interface IDataService
	{
		Task AddExpenseAsync(Expense ex);
		Task<IEnumerable<Expense>> GetExpensesAsync();
	}
}