using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Spent
{
	public partial class ExpenseDetailPage : ContentPage
	{
		public Expense Expense { get; set; }

		public ExpenseDetailPage(Expense expense)
		{
			InitializeComponent();

			Expense = expense;
			BindingContext = this;
		}
	}
}