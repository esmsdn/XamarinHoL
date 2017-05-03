using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Spent
{
	public class BaseViewModel : INotifyPropertyChanged
	{
		private bool isBusy;
		public bool IsBusy
		{
			get { return isBusy; }
			set
			{
				isBusy = value;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged([CallerMemberName] string name = null) =>
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}
}
