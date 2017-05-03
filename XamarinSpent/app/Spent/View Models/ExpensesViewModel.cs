using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace Spent
{
	public class ExpensesViewModel : BaseViewModel
	{
		public ObservableCollection<Expense> Expenses { get; set; }

		public Command GetExpensesCommand { get; set; }
		public Command AddExpenseCommand { get; set; }

		public ExpensesViewModel()
		{
			Expenses = new ObservableCollection<Expense>();
			GetExpensesCommand = new Command(
				async () => await GetExpensesAsync());
			AddExpenseCommand = new Command(
				() => NavigateToAddExpense());

			MessagingCenter.Subscribe<NewExpenseViewModel, object[]>(this, "AddExpense", async (obj, expenseData) =>
			{
				var expense = expenseData[0] as Expense;
				var photo = expenseData[1] as MediaFile;
				Expenses.Add(expense);

				if (photo != null)
				{
					// Connect to the Azure Storage account.
					// NOTE: You should use SAS tokens instead of Shared Keys in production applications.
					var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=spendlab;AccountKey=KOX9AA3BxHdXQdUhMFQ0E/8rPkZigUxC9YZnXn8F9SBkEUopYdcTta+ujkfHD0B2IHHhbTTHJRr9T0GO5pd+LA==");
					var blobClient = storageAccount.CreateCloudBlobClient();

					// Create the blob container if it doesn't already exist.
					var container = blobClient.GetContainerReference("receipts");
					await container.CreateIfNotExistsAsync();

					// Upload the blob to Azure Storage.
					var blockBlob = container.GetBlockBlobReference(Guid.NewGuid().ToString());
					await blockBlob.UploadFromStreamAsync(photo.GetStream());
					expense.Receipt = blockBlob.Uri.ToString();
				}

				await DependencyService.Get<IDataService>().AddExpenseAsync(expense);
			});

			GetExpensesAsync();
		}

		~ExpensesViewModel()
		{
			MessagingCenter.Unsubscribe<NewExpensePage, string>(this, "AddExpense");
		}

		Expense selectedExpenseItem;
		public Expense SelectedExpenseItem
		{
			get { return selectedExpenseItem; }
			set
			{
				selectedExpenseItem = value;
				OnPropertyChanged();

				if (selectedExpenseItem != null)
				{
					MessagingCenter.Send(this, "NavigateToDetail", SelectedExpenseItem);
					SelectedExpenseItem = null;
				}
			}
		}

		async Task GetExpensesAsync()
		{
			if (IsBusy)
				return;

			IsBusy = true;

			try
			{
				Expenses.Clear();

				var expenses = await DependencyService.Get<IDataService>().GetExpensesAsync();
				foreach (var expense in expenses)
				{
					Expenses.Add(expense);
				}
			}
			catch (Exception ex)
			{
				MessagingCenter.Send(this, "Error", ex.Message);
			}
			finally
			{
				IsBusy = false;
			}
		}

		void NavigateToAddExpense()
		{
			if (IsBusy)
				return;

			IsBusy = true;

			try
			{
				MessagingCenter.Send(this, "Navigate", "NewExpensePage");
			}
			catch (Exception ex)
			{
				MessagingCenter.Send(this, "Error", ex.Message);
			}
			finally
			{
				IsBusy = false;
			}
		}
	}
}