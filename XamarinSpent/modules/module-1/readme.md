# Module 1: Building Your First Xamarin.Forms App
**Objective**: Learn the basics of building native mobile apps for iOS, Android, and Windows with Xamarin.Forms. Build a  master-detail application in XAML that uses the Model-View-ViewModel (MVVM) design pattern.

##### Prerequisites
Ensure you have the following software installed:

* Visual Studio 2015 Community Edition (or higher) or Xamarin Studio Community Edition (or higher)
* [Xamarin](xamarin.com/download)

Download the starter code for this module to begin.

### Getting Started
In this workshop, you will learn how to build connected, cross-platform apps for iOS, Android, and Windows with Xamarin.Forms and Microsoft Azure. This workshop takes you from start-to-finish in building an app for tracking personal expenses, including storage of receipts.

This solution contains 4 projects:

* Spent - Shared Project that contains all code that is sharable across multiple platforms (models, view models, views, service logic, etc.).
* Spent.Droid - Xamarin.Android application
* Spent.iOS - Xamarin.iOS application
* Spent.Windows - Windows 10 UWP application (can only be run from Visual Studio 2015 on Windows 10).

 ![](/modules/module-1/images/solution-explorer.png)

NuGet is a package manager for .NET that helps us take advantage of existing libraries like JSON.NET to share more code and build apps even quicker. All projects already have NuGet dependencies added, so there is no need to install additional NuGets during the workshop. To download the NuGets already added to the projects by restoring them.

To do this, right-click on the solution name in the **Solution Explorer** and click **Restore NuGet Packages**.

 ![](/modules/module-1/images/restore-nugets.png)

### Module Instructions
Let's get started building our expenses mobile app - Spent!

##### 1. Run the starter code.
To begin, open up the starter code in either Visual Studio or Xamarin Studio. Debug the application for either iOS, Android, or Universal Windows Platform (UWP).

> Note that if you are using Visual Studio on Windows, you will need to have to be [connected to a Mac](https://developer.xamarin.com/guides/ios/getting_started/installation/windows/connecting-to-mac/) to build and debug the iOS solution. If you are running Xamarin Studio on Mac, the Universal Windows Platform project cannot be built.

When the app deploys to the simulator or emulator, you will see a single label that says "Welcome to Xamarin.Forms!".

 ![](/modules/module-1/images/welcome-to-xamarin-forms.png)

Let's investigate the key pieces of a Xamarin.Forms application. Expand the `Spent` project to see several empty folders and a few files. Traditional Xamarin apps allow us to share between 70-90% of code, but all user interface logic resides in the individual platform projects. For Xamarin.Forms apps, we can still share all the code we did in a traditional Xamarin app, as well as the user interface logic. All shared code is either written in a [Shared Project](https://developer.xamarin.com/guides/cross-platform/application_fundamentals/shared_projects/) or [Portable Class Library (PCL)](https://developer.xamarin.com/guides/cross-platform/application_fundamentals/pcl/).

The `Application` class is the main entry point for Xamarin.Forms applications. This class selects the main page of the application and handles application lifecycle logic, such as `OnStart`, `OnSleep`, and `OnResume`.

It's natural to ask why we still have platform-specific projects if all code is shared. This is because all Xamarin.Forms apps _are_ native apps, and we still have the ability to drop down into platform-specific code to access 100% of the underlying native APIs. If you jump to the `Spent.Droid` project and open `MainActivity`, you will see two lines that initialize the Xamarin.Forms library and load up a new Xamarin.Forms application:

``` csharp
Xamarin.Forms.Forms.Init(this, bundle);
LoadApplication(new App());
```

Within an application are a series of screens, or **[Pages](https://developer.xamarin.com/guides/xamarin-forms/controls/pages/)**. There are seven different types of pages in Xamarin.Forms, ranging from pages for displaying content (`ContentPage`) to pages that manage navigation (`TabbedPage`, `NavigationPage`, etc.). Pages define a user interface either in XAML or C#. If you define your user interface logic in XAML, you will also have an assicated codebehind file (aptly-named `.xaml.cs`) for the logic for that page. `MainPage` in the `Spent` project is an example of a page. 

Within a page, we can use **[Layouts](https://developer.xamarin.com/guides/xamarin-forms/controls/layouts/)** to let Xamarin.Forms know how to display individual controls within the page. There are two main types: managed and unmanaged layouts. Generally, we will opt for managed layouts, as they smartly "manage" the layout of our controls, no matter what OS or device the app is running on.

##### 3. Add a base view model.
Now that we have a basic introduction to Xamarin.Forms, let's begin building our app! Xamarin.Forms has built-in support for the Model-ViewModel-View (MVVM) design pattern that's common in Windows development. This helps us to separate our user interface logic from our business logic. The **View** contains all user interface logic (or what the user sees when they use your application). The **ViewModel** is an abstraction of the view that contains the logic that drives user interaction with our application. An example for a page with a list of items may be logic to download JSON from the web, deserialize it, and put it into a list for our user interface to display. The **Model** is a domain model or a data-access layer.

To get started with MVVM, we need to use the concept of **[data binding](https://developer.xamarin.com/guides/xamarin-forms/xaml/xaml-basics/data_binding_basics/)**. Data binding is the flow of data between our view and view model, such as when a user pulls-to-refresh to load new data, or types into a textbox. Our user interface should be alerted when anything changes in our view model. To do this, we will implement the `INotifyPropertyChanged` interface. Because this behavior is something we will want to have in all of our view models for this app, let's start by adding a new base view model that we can reuse.

Right-click the `View Models` folder, select `Add -> New File`, select an empty C# class, and name it `BaseViewModel`. Bring in the `System.ComponentModel` namespace, and implement the `INotifyPropertyChanged` interface.

```csharp
using System;
using System.ComponentModel;

namespace Spent
{
	public class BaseViewModel : INotifyPropertyChanged
	{
		public BaseViewModel()
		{
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
```

Next, let's add a method named `OnPropertyChanged` that will raise the `PropertyChanged` event. Whenever a property changes in our view model that needs to be updated in the view, we will call this method.

**C# 6 (Visual Studio 2015 or Xamarin Studio on Mac)**
```csharp
using System.Runtime.CompilerServices;

public void OnPropertyChanged([CallerMemberName] string name = null) =>
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
```

**C# 5 (Visual Studio 2012 or 2013)**
```csharp
public void OnPropertyChanged([CallerMemberName] string name = null)
{
    var changed = PropertyChanged;

            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(name));
}
```

Now, we can call `OnPropertyChanged` whenever a property updates and our user interface will be updated.

We also want to make sure that we aren't duplicating effort in our view models. If a user pulls-to-refresh four times, we should only make one request to update the data (assuming the other three happen before the first returns). To ensure that we aren't duplicating effort, let's add an `IsBusy` property that we will set to `true` when work begins and `false` when work ends.

```csharp
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
```

Awesome! We've now finished our `BaseViewModel` that we can reuse in all future view models.

##### 3. Add expenses model.
Spent's objective is to help users track their expenses. Our main domain object is an expense, so let's add a new `Expense` model for storing information about a particular expense. Right-click the `Models` folder, click `Add -> New File`, select an empty C# class, and name it `Expense`.

Let's give our `Expense` model a few properties that we would expect a particular expense to have.

```csharp

namespace Spent
{
	public class Expense
	{
		public string Company { get; set; }
		public string Description { get; set; }
		public string Amount { get; set; }
		public DateTime Date { get; set; }
		public string Receipt { get; set; }
	}
}
```

This will keep track of the company, description, purchase amount, purchase time, as well as a string that represents a path to our receipt in the phone's local storage.

##### 4. Add a expenses view model.
Spent is going to visualize a list of expenses that the user has incurred. Let's create a view model for our first screen! Create a new C# class in the `View Models` folder named `ExpensesViewModel`, and subclass `BaseViewModel`.

There are many different ways to represent a list of items in .NET, but we will be using a special type named `ObservableCollection<T>`. This is a generic list type that tracks changes to the collection (unlike a regular `List<T>`). This is especially important in MVVM, as we want our user interface to update when this collection updates. Say a user pulls-to-refresh for more data and some new expenses are added; we want to ensure that the user interface updates when new items are added to the collection. By using `ObservableCollection<T>`, we get this behavior for free.

Add new collection to the class for storing our expenses and initialize it in the constructor.

> **Note**: Developers using Visual Studio may notice that namespaces are typed according the ProjectName.FolderName heuristic. Spent may require you to bring in additional namespaces due to this. For example, when you reference the `Expense` model in `ExpensesViewModel`, you may have to add an additional using for `Spent.Models`. 

```csharp
using System;
using System.Collections.ObjectModel;

namespace Spent
{
	public class ExpensesViewModel : BaseViewModel
	{
		public ObservableCollection<Expense> Expenses { get; set; }

		public ExpensesViewModel()
		{
			Expenses = new ObservableCollection<Expense>();
		}
	}
}
```

Now that we have a collection for storing our user's expenses, let's add a method for fetching the user's expenses.

```csharp
using System.Threading.Tasks;
using Xamarin.Forms;
...
async Task GetExpensesAsync()
{
    if (IsBusy)
        return;

    IsBusy = true;

	try
	{
        // Load up user's expenses here.
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
```

The `GetExpensesAsync` method above is a great example of boilerplate for all view model methods. First, we check if `IsBusy` is `true` (in which case we don't need to repeat the operation). If it is not, set `IsBusy` to `true`, then execute the logic for our method. If there are any issues, report that exception to the user via the `MessengingCenter` (which we will cover later in this module). Finally, make sure that `IsBusy` is set to `false` again so you can continue new operations in the view model.

Let's add some mock user data to our method within the `try` block of `GetExpensesAsync`.

```csharp
Expenses.Clear();
Expenses.Add(new Expense { Company = "Walmart", Description = "Always low prices.", Amount = "$14.99", Date = DateTime.Now });
Expenses.Add(new Expense { Company = "Apple", Description = "New iPhone came out - irresistable.", Amount = "$999", Date = DateTime.Now.AddDays(-7) });
Expenses.Add(new Expense { Company = "Amazon", Description = "Case to protect my new iPhone.", Amount = "$50", Date = DateTime.Now.AddDays(-2) });
```

Let's call `GetExpensesAsync` in the constructor to ensure that data is added to our `Expenses` property.

Awesome! Now we have data loading up in our view model. But what if the user wants to initiate a refresh? Right now, the only way to load more data is to restart the application, because the only place we are loading up expenses is in the constructor of the view model. If data binding is the flow of data between view and view model, **Commanding** is the flow of events. Let's add a new `Command` that we can call from our user interface logic to refresh the data.

```csharp
public Command GetExpensesCommand { get; set; }
```

Let's initialize the command in our constructor.

```csharp
GetExpensesCommand = new Command(
    async () => await GetExpensesAsync());
```

We now have a completed `ExpensesViewModel` that loads up a list of `Expenses` and also allows for the user to refresh the list by calling our `GetExpensesCommand`. Let's build our user interface!

##### 4. Add expenses page.
User interfaces in Xamarin.Forms are built by using C# or XAML markup. While there are benefits and drawbacks to each approach, XAML helps us to best implement the MVVM pattern and maintain a separation of our view model and view logic. It also helps in visualizing the visual tree that can be a bit harder to do when defining our user interfaces in C#.

Let's add a page to display our expenses. Right-click the `Views` folder, click `Add -> New File`, add a `Forms -> Forms ContentPage Xaml` if using Xamarin Studio or a `Cross-Platform -> Forms Xaml Page` if using Visual Studio, and name it `ExpensesPage`. Two files will be added: `ExpensesPage.xaml` for defining our user interface, and `ExpensesPage.xaml.cs` (our "codebehind") for hooking up our view to our view model. 

Because we are displaying a list of items, the control that makes the most sense here is the [`ListView`](https://developer.xamarin.com/guides/xamarin-forms/user-interface/listview/) control. Add a `ListView` between the `ContentPage.Content` elements in XAML.

```csharp
<?xml version="1.0" encoding="UTF-8"?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" x:Class="Spent.ExpensesPage">
	<ContentPage.Content>
		<ListView>

		</ListView>
	</ContentPage.Content>
</ContentPage>
```

Next, let's configure our `ListView` to use the `ObservableCollection<Expense>` we defined in our `ExpensesViewModel`. To do this, we need to define the `ItemsSource` property of the `ListView`.

```csharp
<ListView ItemsSource="{Binding Expenses}">

</ListView>
```

This means that all data displayed in the `ListView` is "bound" to the `Expenses` object in our view model. Now that our `ListView` knows what data to display, we need to tell it how to display individual `Expenses`. We can do this by defining the `ListView.ItemTemplate` property.

```csharp
<ListView ItemsSource="{Binding Expenses}">
	<ListView.ItemTemplate>
		<DataTemplate>

		</DataTemplate>
	</ListView.ItemTemplate>
</ListView>
```

The code above is boilerplate code for defining individual cells within a `ListView`. Xamarin.Forms comes with several [prebuilt cells](https://developer.xamarin.com/guides/xamarin-forms/controls/cells/) for you to take advantage of (such as `TextCell`, `ImageCell`, and `SwitchCell`), but you can also build your own. Let's take advantage of the prebuilt `TextCell` to display our `Expense` data.

```csharp
<ListView ItemsSource="{Binding Expenses}">
	<ListView.ItemTemplate>
		<DataTemplate>
			<TextCell Text="{Binding Company}" Detail="{Binding Amount}" />
		</DataTemplate>
	</ListView.ItemTemplate>
</ListView>
```

This defines a `ListView` populated by data from the `Expenses` object. Each cell is an individual `Expense`, so we can data bind directly to the properties of that expense (`Company`, `Description`, `Amount`, etc.). But this begs an important question - how does the view know what objects to bind to? To let our view know where objects being data bound to can be found, we need to set the `BindingContext` for the page. The `BindingContext` is where Xamarin.Forms will look for objects being data bound from the user interface. To configure this, all we need to do is go to `ExpensesPage.xaml.cs` and add the following line to our constructor.

```csharp
public ExpensesPage()
{
	InitializeComponent();

	BindingContext = new ExpensesViewModel();
}
``` 

Ideally, the only logic written in the codebehind file should be configuration of the `BindingContext`. The last thing we need to do is update `App.xaml.cs` to change the `MainPage` property to our new `ExpensesPage`.

```csharp
MainPage = new ExpensesPage();
```

Run the app, and you should now see a list of expenses displayed to you.

 ![](/modules/module-1/images/expenses-list-view.png)

Awesome! Now we have a `ListView` with our expenses that we are fetching from our view model. But what if the user wants to update this data? A common pattern in mobile development when working with `ListView`s is the [pull-to-refresh pattern](https://developer.xamarin.com/guides/xamarin-forms/user-interface/listview/interactivity/#Pull_to_Refresh). Lucky for us, this is built right into Xamarin.Forms; all we have to do is configure a few properties on our `ListView`!

Jumping back to `ExpensesPage.xaml`, let's add the following attributes to our `ListView` to enable the pull-to-refresh pattern.

```csharp
<ListView ItemsSource="{Binding Expenses}"
	IsPullToRefreshEnabled="true"
	IsRefreshing="{Binding IsBusy, Mode=OneWay}"
	RefreshCommand="{Binding GetExpensesCommand}">
	<ListView.ItemTemplate>
		<DataTemplate>
			<TextCell Text="{Binding Company}" Detail="{Binding Amount}" />
		</DataTemplate>
	</ListView.ItemTemplate>
</ListView>
```

Let's walkthrough the various properties we just added:

* `IsPullToRefreshEnabled`: Defines if we allow the user to perform a pull-to-refresh on the `ListView`. If your content requires refreshing, you should set this value to `true`.
* `IsRefreshing`: Defines if the `ListView` is in the process of refreshing. We can data bind to the `IsBusy` property. We set the `Mode` to `OneWay`, which ensures that this value can only be updated from our view model to view, not view to view model (i.e. `TwoWay`). 
* `RefreshCommand`: Defines the `Command` used for actually performing the refresh logic. When the pull-to-refresh action occurs, this `Command` is executed.

Re-run Spent, and you should now be able to pull-to-refresh to load data!

##### 5. Add expense detail page.
Most `ListView`s also allow users to click on individual cells to view a detail screen with more information about the particular object seen in the cell. This is known as **Master/Detail Navigation** and is a very common pattern in mobile development. Replicating this kind of behavior is extremely easy with Xamarin.Forms.

Let's start off by adding a new Xamarin.Forms XAML `ContentPage` to the `Views` folder named `ExpenseDetailPage`. Within this page, we will display all the properties of the `Expense` object, including the receipt photo. Because we will be using more than one `View` within this page (unlike `ExpensesPage`), we will need to use one of Xamarin.Forms' layouts to layout our controls. The easiest and most common layout is the [`StackLayout`](https://developer.xamarin.com/api/type/Xamarin.Forms.StackLayout/), which defines a stack of controls in either the vertical or horizontal orientation. Let's add a new `StackLayout` with a `Padding` of `20` to ensure that our views inside the layout aren't too close to the left, top, right, or bottom of the screen.

```csharp
<?xml version="1.0" encoding="UTF-8"?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" x:Class="Spent.ExpenseDetailPage">
	<ContentPage.Content>
		<StackLayout Padding="20">

		</StackLayout>
	</ContentPage.Content>
</ContentPage>
```

Next, let's use the `Label` and `Image` controls in Xamarin.Forms to display to the user detailed information about their expense.

```csharp
<?xml version="1.0" encoding="UTF-8"?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" x:Class="Spent.ExpenseDetailPage">
	<ContentPage.Content>
		<StackLayout Padding="20">
			<Label Text="Company" TextColor="#4d4d4d"/>
			<Label Text="{Binding Expense.Company}"/>
			<Label Text="Description" TextColor="#4d4d4d"/>
			<Label Text="{Binding Expense.Description}" />
			<Label Text="Date" TextColor="#4d4d4d"/>
			<Label Text="{Binding Expense.Date}" />
			<Label Text="Amount" TextColor="#4d4d4d"/>
			<Label Text="{Binding Expense.Amount}" />
			<Label Text="Receipt" TextColor="#4d4d4d"/>
			<Image Source="{Binding Expense.Receipt}"/>
		</StackLayout>
	</ContentPage.Content>
</ContentPage>
```

Notice that we are performing some data binding here to display expense data. We can access the properties of objects in our `BindingContext` by using the dot syntax, just like if we we wanted to access the values of these properties in C#. Just as with our `ExpensesPage`, we need to provide a `BindingContext` for our data binding to function properly. Open `ExpenseDetailPage.xaml.cs`, add a property named `Expense` and update the constructor to take in an `Expense` as a parameter, set the `Expense` property to this paramater, and set the `BindingContext` to `this`.

```csharp
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
```

Great! Now it's time to implement navigation from our `ListView` to our detail page. We can do this by using the `SelectedItem` property of the `ListView`. Update `ExpensesPage.xaml` to data bind the `ListView.SelectedItem` property to `SelectedExpenseItem`.

```csharp
<ListView ItemsSource="{Binding Expenses}"
	IsPullToRefreshEnabled="true"
	IsRefreshing="{Binding IsBusy, Mode=OneWay}"
	RefreshCommand="{Binding GetExpensesCommand}"
	SelectedItem="{Binding SelectedExpenseItem, Mode=TwoWay}">
	<ListView.ItemTemplate>
		<DataTemplate>
			<TextCell Text="{Binding Company}" Detail="{Binding Amount}" />
		</DataTemplate>
	</ListView.ItemTemplate>
</ListView>
```

When the `SelectedItem` changes, we can navigate to the detail view for the `SelectedItem`. Open up `ExpensesViewModel` and add a property named `SelectedExpenseItem`.

```csharp
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
			// TODO: Navigate to detail.
			SelectedExpenseItem = null;
		}
	}
}
```

In this code, we are updating the `SelectedExpenseItem`, and firing `OnPropertyChanged`. If the selected item is not null, we want to navigate to the detail page. Finally, we set the value to `null` to remove any cell highlighting that happens when a user taps the cell.

To handle navigation, we will need a reference to a `Page`. Let's add a class-level field typed `ExpensesPage`, add a parameter to the constructor of `ExpensesViewModel` to take in an `ExpensesPage`, and this equal to our new `ExpensesPage` field.

```csharp
ExpensesPage page;
public ExpensesViewModel(ExpensesPage expensesPage)
{
	page = expensesPage;
	...
}
```

Next, let's update our `SelectedExpenseItem` property to navigate when the value is changed.

```csharp
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
			ExpensesPage.Navigation.PushAsync(new ExpenseDetailPage(SelectedExpenseItem));
			SelectedExpenseItem = null;
		}
	}
}
```

We also need to update `Expenses.xaml.cs` to pass in `this` as a value to the `ExpensesViewModel` constructor.

```csharp
public partial class ExpensesPage : ContentPage
{
	public ExpensesPage()
	{
		InitializeComponent();

		BindingContext = new ExpensesViewModel(this);
	}
}
```

You may have caught that we are using a `Navigation` property of the page to push a new detail page onto the navigation stack. Right now, `ExpensesPage` is a `ContentPage`, which doesn't contain any ability to do navigation. To gain this ability, we must wrap our `ExpensesPage` in a [`NavigationPage`](https://developer.xamarin.com/api/type/Xamarin.Forms.NavigationPage/) to gain the ability to do push/pop and modal navigation. Jump over to `App.xaml.cs` and update the `MainPage` to the following.

```csharp
MainPage = new NavigationPage(new ExpensesPage());
```

By doing this, our `ExpensesPage.Navigation` property is now available for use! Additionally, by using a `NavigationPage`, we get a nice navigation bar at the top of all of our pages to alert users to what page they are on, and to navigate back to the top of the stack (with a "back" button). To have a title display in the navigation bar, we can update the `Title` property of each page to the appropriate value.

`**ExpensesPage**`
```csharp
<?xml version="1.0" encoding="UTF-8"?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" x:Class="Spent.ExpensesPage"
		Title="Expenses">
...
</ContentPage>

```

`**ExpenseDetailPage**`
```csharp
<?xml version="1.0" encoding="UTF-8"?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" x:Class="Spent.ExpenseDetailPage"
		Title="Expense Detail">
...
</ContentPage>
```

Now, run the application, click on an expense cell, and you will be navigated to a detail page with more information about our expense!

 ![](/modules/module-1/images/expenses-detail-view.png)

##### 6. Navigation with the Messaging Center.
Right now, we have a working master-detail navigation flow that shows a list of expenses, as well as detailed information about each expense. We can clean this up to be even better and reduce tight coupling between our views and view models.

Xamarin.Forms [`MessagingCenter`](https://developer.xamarin.com/guides/xamarin-forms/messaging-center/) enables view models and other components to communicate without having to know anything about each other besides a simple message contract. There are two main parts to the `MessagingCenter`:

1. **Subscribe**: Listen for messages with a certain signature and perform some action when they are received. Mulitple subscribers can be listening for the same message.
2. **Send**: Publish a message for listeners to act upon. If no listeners have subscribed then the message is ignored.

Instead of passing a `Page` around to handle navigation, what if we used the Xamarin.Forms `MessagingCenter` to handle this? Let's update our current app to use this approach.

Let's start by undoing some of the "harm" we have done! Open `ExpensesViewModel` and remove the parameter from the constructor, as well as the `ExpensesPage` field and all references to it in `SelectedExpenseItem`. Jump over to `ExpensesPage.xaml.cs` and update initialization of the `ExpensesViewModel` to have no parameters. 

Next, let's open back up `ExpensesViewModel` and send our first message! Let's send a message by using the following method signature `MessagingCenter.Send(TSender sender, string message, TArgs args)`. 

```csharp
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
```

Note that we pass in `this` as the sender, "NavigateToDetail" as the message, and our selected `Expense` object as an argument. Sending a message is great, but if nobody is listening for messages of that signature, nothing will happen. Jump back over to `ExpensesPage.xaml.cs` and add two new methods: `SubscribeToMessages` and `UnsubscribeFromMessages`. Also, override the `OnAppearing` and `OnDisappearing` lifecycle methods of our `ContentPage` to call `SubscribeToMessages` and `UnsubscribeFromMessages`.

```csharp
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

}

void UnsubscribeFromMessages()
{
		
}
```

It's important that we properly subscribe and unsubscribe to messages from the `MessagingCenter` to avoid null references and possible memory leaks. Let's subscribe to the message sent from our view model in `SubscribeToMessages`.

```csharp
void SubscribeToMessages()
{
	MessagingCenter.Subscribe<ExpensesViewModel, Expense>(this, "NavigateToDetail", async (obj, expense) =>
	{
		if (expense != null)
		{
			await Navigation.PushAsync(new ExpenseDetailPage(expense));
		}
	});

	MessagingCenter.Subscribe<ExpensesViewModel, string>(this, "Error", (obj, s) =>
	{
		DisplayAlert("Error", s, "OK");
	});
}
```

This subscribes us to messages with string "NavigateToDetail" coming from `ExpensesViewModel` with argument(s) `Expense`. If we receive a message, we first check that the argument isn't null, then navigate to our detail page. 

We can easily unsuscribe in `UnsubscribeFromMessages` as well.

```csharp
MessagingCenter.Unsubscribe<ExpensesViewModel, Expense>(this, "NavigateToDetail");
MessagingCenter.Unsubscribe<ExpensesViewModel, string>(this, "Error");
```

Run the app, and navigation should still be working as intended - only this time we are properly avoiding tight coupling between our view model and view.

#### Closing Remarks
In this module, you learned about the basics of building apps with Xamarin.Forms, including creating user interfaces in XAML, navigation, MVVM, data binding and commanding, as well as use of the `MessagingCenter`. In the next module, we'll take a look at polishing off our Xamarin.Forms app before connecting it to the cloud in Modules 3 and 4.
