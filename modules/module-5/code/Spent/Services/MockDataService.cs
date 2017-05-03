using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(Spent.MockDataService))]
namespace Spent
{
	public class MockDataService : IDataService
	{
		bool isInitialized;
		List<Expense> expenses;

		void Initialize()
		{
			if (isInitialized)
				return;

			expenses = new List<Expense>
			{
				new Expense { Company = "Walmart", Description = "Dongles for presenting at Microsoft Ignite", Amount = "$14.99", Date = DateTime.Now },
				new Expense { Company = "Amazon", Description = "iPhone headphone jack adapter", Amount = "$49.99", Date = DateTime.Now.AddDays(-3) }
			};
		}

		public async Task AddExpenseAsync(Expense expense)
		{
			Initialize();

			expenses.Add(expense);
		}

		public async Task<IEnumerable<Expense>> GetExpensesAsync()
		{
			Initialize();

			return expenses;
		}
	}
}
