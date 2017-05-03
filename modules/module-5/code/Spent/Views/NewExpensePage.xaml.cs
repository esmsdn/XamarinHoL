using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Spent
{
	public partial class NewExpensePage : ContentPage
	{
		public NewExpensePage()
		{
			InitializeComponent();

			BindingContext = new NewExpenseViewModel();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			SubscribeToMessages();
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			UnsubscribeFromMessages();
		}

		void SubscribeToMessages()
		{
			MessagingCenter.Subscribe<ExpensesViewModel, string>(this, "Error", (obj, s) =>
			{
				DisplayAlert("Error", s, "OK");
			});

			MessagingCenter.Subscribe<NewExpenseViewModel, string>(this, "Navigate", async (obj, s) =>
			{
				if (s == "ExpensesPage")
				{
					await Navigation.PopAsync();
				}
			});
		}

		void UnsubscribeFromMessages()
		{
			MessagingCenter.Unsubscribe<NewExpenseViewModel, string>(this, "Error");
			MessagingCenter.Unsubscribe<NewExpenseViewModel, string>(this, "Navigate");
		}
	}
}