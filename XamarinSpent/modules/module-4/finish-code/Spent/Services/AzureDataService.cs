using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;

using Xamarin.Forms;

[assembly: Dependency(typeof(Spent.AzureDataService))]
namespace Spent
{
	public class AzureDataService : IDataService
	{
		bool isInitialized;
		IMobileServiceSyncTable<Expense> expensesTable;

		public MobileServiceClient MobileService { get; set; }

		public AzureDataService()
		{
			MobileService = new MobileServiceClient("http://spent-ingite-lab.azurewebsites.net/", null);
		}

		async Task Initialize()
		{
			if (isInitialized)
				return;

			var store = new MobileServiceSQLiteStore("app.db");
			store.DefineTable<Expense>();
			await MobileService.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());
			expensesTable = MobileService.GetSyncTable<Expense>();

			isInitialized = true;
		}

		public async Task AddExpenseAsync(Expense ex)
		{
			await Initialize();

			await expensesTable.InsertAsync(ex);
			await SyncExpenses();
		}

		public async Task<IEnumerable<Expense>> GetExpensesAsync()
		{
			await Initialize();

			await SyncExpenses();

			return await expensesTable.ToEnumerableAsync();
		}

		async Task SyncExpenses()
		{
			try
			{
				await MobileService.SyncContext.PushAsync();
				await expensesTable.PullAsync($"all{typeof(Expense).Name}", expensesTable.CreateQuery());
			}
			catch (Exception)
			{
				System.Diagnostics.Debug.WriteLine("An error syncing occurred. That is OK, as we have offline sync.");
			}
		}
	}
}