using System;
using System.Threading.Tasks;

using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace Spent
{
	public class NewExpenseViewModel : BaseViewModel
	{
		MediaFile receiptPhoto;

		public Command SaveExpenseCommand { get; set; }
		public Command AttachReceiptCommand { get; set; }

		public string Company { get; set; }
		public string Description { get; set; }
		public DateTime DateTime { get; set; }
		public string Amount { get; set; }

		string receipt;
		public string Receipt
		{
			get { return receipt; }
			set { receipt = value; OnPropertyChanged(); }
		}

		public NewExpenseViewModel()
		{
			SaveExpenseCommand = new Command(
				async () => await SaveExpenseAsync(), () => !IsBusy);

			AttachReceiptCommand = new Command(
				async () => await AttachReceiptAsync(), () => !IsBusy);
		}

		async Task SaveExpenseAsync()
		{
			if (IsBusy)
				return;

			IsBusy = true;

			try
			{
				var expense = new Expense
				{
					Company = Company,
					Description = Description,
					Date = DateTime,
					Amount = Amount
				};

				var expenseData = new object[]
				{
					expense,
					receiptPhoto
				};
				MessagingCenter.Send(this, "AddExpense", expenseData);
				MessagingCenter.Send(this, "Navigate", "ExpensesPage");
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

		async Task AttachReceiptAsync()
		{
			try
			{
				await CrossMedia.Current.Initialize();

				MediaFile photo;
				if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
				{
					photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
					{
						Directory = "Expenses",
						Name = "expense.jpg"
					});
				}
				else
				{
					photo = await CrossMedia.Current.PickPhotoAsync();
				}

				Receipt = photo?.Path;
				receiptPhoto = photo;
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