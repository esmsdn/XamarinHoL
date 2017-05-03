using System;
using System.Collections.Generic;

using Xamarin.Forms;

#if __ANDROID__
using Spent.Droid;
using Android.Support.Design.Widget;
using Xamarin.Forms.Platform.Android;
#endif

namespace Spent
{
	public partial class ExpensesPage : ContentPage
	{
		public ExpensesPage()
		{
			InitializeComponent();

			BindingContext = new ExpensesViewModel();

#if __ANDROID__
			ToolbarItems.RemoveAt(0);

			var fab = new FloatingActionButton(Forms.Context)
			{
				UseCompatPadding = true
			};

			fab.Click += (sender, e) =>
			{
				var viewModel = BindingContext as ExpensesViewModel;
				viewModel.AddExpenseCommand.Execute(null);
			};

			relativeLayout.Children.Add(fab.ToView(), 
    			Constraint.RelativeToParent((parent) =>
				{
					return parent.Width - 100;
				}),
			    Constraint.RelativeToParent((parent) =>
				{
					return parent.Height - 100;
				}),
			    Constraint.Constant(75),
			    Constraint.Constant(85));
#endif
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

			MessagingCenter.Subscribe<ExpensesViewModel, string>(this, "Navigate", async (obj, s) =>
			{
				if (s == "NewExpensePage")
				{
					await Navigation.PushAsync(new NewExpensePage());
				}
			});

			MessagingCenter.Subscribe<ExpensesViewModel, Expense>(this, "NavigateToDetail", async (obj, expense) =>
			{
				if (expense != null)
				{
					await Navigation.PushAsync(new ExpenseDetailPage(expense));
				}
			});
		}

		void UnsubscribeFromMessages()
		{
			MessagingCenter.Unsubscribe<ExpensesViewModel, string>(this, "Error");
			MessagingCenter.Unsubscribe<ExpensesViewModel, string>(this, "Navigate");
			MessagingCenter.Unsubscribe<ExpensesViewModel, Expense>(this, "NavigateToDetail");
		}
	}
}