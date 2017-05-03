using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Spent
{
	public class ExpensesViewModel : BaseViewModel
	{
		public ObservableCollection<Expense> Expenses { get; set; }
		public Command GetExpensesCommand { get; set; }

		public ExpensesViewModel()
		{
			Expenses = new ObservableCollection<Expense>();
			GetExpensesCommand = new Command(
				async () => await GetExpensesAsync());
			
			GetExpensesAsync();
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
				Expenses.Add(new Expense { Company = "Walmart", Description = "Always low prices.", Amount = "$14.99", Date = DateTime.Now });
				Expenses.Add(new Expense { Company = "Apple", Description = "New iPhone came out - irresistable.", Amount = "$999", Date = DateTime.Now.AddDays(-7) });
				Expenses.Add(new Expense { Company = "Amazon", Description = "Case to protect my new iPhone.", Amount = "$50", Date = DateTime.Now.AddDays(-2) });
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