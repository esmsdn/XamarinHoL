using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;

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
				new Expense { Company = "Walmart", Description = "Always low prices.", Amount = "$14.99", Date = DateTime.Now },
				new Expense { Company = "Apple", Description = "New iPhone came out - irresistable.", Amount = "$999", Date = DateTime.Now.AddDays(-7) },           
				new Expense { Company = "Amazon", Description = "Case to protect my new iPhone.", Amount = "$50", Date = DateTime.Now.AddDays(-2) }
			};

			isInitialized = true;
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
