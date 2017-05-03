# Module 2: Extending Your First Xamarin.Forms App
**Objective**: Continue to explore Xamarin.Forms features and functionality, including styles, user interface design, and performance enhancements.

##### Prerequisites
Ensure you have the following software installed:

* Visual Studio 2015 Community Edition (or higher) or Xamarin Studio Community Edition (or higher)
* [Xamarin](xamarin.com/download)

Download the starter code for this module to begin, or continue working with the completed code from Module 1.

### Module Instructions
This module builds on Module 1 by extending your app even further to allow you to add your own expenses, including photos of receipts. We also take a look at the `DependencyService`, styles, native control embedding, as well as performance enhancements to build performant mobile apps.

##### 1. Create a data storage service with the `DependencyService`.
The current architecture of Spent isn't bad, but there's definitely room for improvement, especially relating to data. We will likely need a way to access our expense data no matter where we are in the application (such as adding a new expense). Additionally, we also need flexibility in the event that we want to change how our data is stored in the future (such as in the cloud). 

We can use the Xamarin.Forms [`DependencyService`](https://developer.xamarin.com/guides/xamarin-forms/dependency-service/) to reduce coupling even further, and allow us to easily switch out data storage implementations with just one line of code. The `DependencyService` is a dependency resolver. In practice, an interface is defined and the `DependencyService` finds the correct implementation of that interface. This is often used in Xamarin.Forms apps to access platform-specific functionality and features, but we can use it as a regular dependency service as well. 

There are three main parts to the `DependencyService`:

1. **Interface**: Defines the contract and required functionality for the service.
2. **Implementation**: Implementation of the interface contract. We can have multiple implementations.
3. **Registration**: Each implementing class must be registered with the `DependencyService` via a metadata attribute. This enables the `DependencyService` to find the implementing class and supply it at runtime.

For data, this is amazing! We can create an interface that defines a basic data storage contract and have multiple implementations. For example, we could have a cloud implementation that is used in production, as well as a mock data implementation (as we do now) that's used during testing.

Let's create our first service! Start by right-clicking the `Services` folder, adding a new blank C# interface, and naming it `IDataService`. Let's define a two methods that each service must implement, `AddExpenseAsync` and `GetExpensesAsync`.

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spent
{
	public interface IDataService
	{
		Task AddExpenseAsync(Expense ex);
		Task<IEnumerable<Expense>> GetExpensesAsync();
	}
}
```

Next, let's create an implementation of our `IDataService` with mock data. Right-click the `Services` folder, and add a new blank C# class named `MockDataService`. Implement the `IDataService` interface.

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Spent
{
	public class MockDataService : IDataService
	{
		public async Task AddExpenseAsync(Expense ex)
		{
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<Expense>> GetExpensesAsync()
		{
			throw new NotImplementedException();
		}
	}
}
```

Great! Now let's implement our `MockDataService`. We will start by adding a `List<Expense>` for tracking expenses and `bool` for tracking if our mock data store has been initialized.

```csharp
bool isInitialized;
List<Expense> expenses;
```
Next, let's add a method named `Initialize` to intialize the expenses list with some mock data.

```csharp
void Initialize()
{
	if (isInitialized)
		return;

	expenses = new List<Expense>
	{
		new Expense { Company = "Walmart", Description = "Always low prices.", Amount = "$14.99", Date = DateTime.Now },
		new Expense { Company = "Apple", Description = "New iPhone came out - irresistable.", Amount = "$999", Date = DateTime.Now.AddDays(-7) },			new Expense { Company = "Amazon", Description = "Case to protect my new iPhone.", Amount = "$50", Date = DateTime.Now.AddDays(-2) }
	};

    isInitialized = true;
}
```

Now that we have our data initialized, let's add implementations for our `AddExpenseAsync` and `GetExpensesAsync` methods.

```csharp
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
```

The final step required in working with `DependencyService` is registration. We can easily do this by adding the following attribute to the top of the namespace in the `MockDataService` class.

```csharp
[assembly: Xamarin.Forms.Dependency(typeof(Spent.MockDataService))]
```

Jump back over to `ExpensesViewModel` and the `GetExpensesAsync` method. Delete the code in the try block, and replace it with the following.

```csharp
Expenses.Clear();

var expenses = await DependencyService.Get<IDataService>().GetExpensesAsync();
foreach (var expense in expenses)
{
    Expenses.Add(expense);
}
```

In the code above, we first clear the `Expenses` collection before using the `DependencyService` to get the expenses. Finally, we add them to our collection for the user interface to updated.

> **Best Practice**: Remember how `ObservableCollection<T>` fires collection changed events to let our user interface know to update? This means that the user interface is updated for each addition to the collection, which is costly in terms of performance. For best results, use something like `ObservableRangeCollection<T>`, which will only fire the collection changed event one time, resulting in vastly improved performance.

Great! Now we have successfully removed yet another example of tight coupling. Now that we have a more robust and easily-accessible data service, let's implement the logic for adding new expenses.

##### 2. Add new expense view model.
Just as we did for the `ExpensesPage`, we will create a view model to handle all logic for our add expense user interface. Right-click the `View Models` folder, and add a new blank C# class named `NewExpenseViewModel`. Just like all other view models, we will subclass `BaseViewModel` to gain the `INotifyPropertyChanged` functionality, as well as our `IsBusy` property.

Let's start by adding some public properties to our view model to represent the various fields an expense has.

```csharp
public string Company { get; set; }
public string Description { get; set; }
public DateTime DateTime { get; set; }
public string Amount { get; set; }
```

For the receipt, we need to create a traditional property with a backing field so we can call `OnPropertyChanged` when a receipt is attached. We need to do this because our user interface will need to update when a receipt is attached to display the photo.

```csharp
string receipt;
public string Receipt
{
	get { return receipt; }
	set { receipt = value; OnPropertyChanged(); }
}
```

Great, now that we have all the backing properties for our user interface, let's create several commands for attaching the receipt and saving expenses. First, let's create a method that will contain the logic for our command named `AttachReceiptAsync`.

```csharp
using System.Threading.Tasks;
using Xamarin.Forms;
...
async Task AttachReceiptAsync()
{
    try
    {
        // TODO: Add logic to take a photo and attach a receipt.
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

Next, we need to add the `Command` for our user interface to bind to named `AttachReceiptCommand`. Let's also intialize our command in the constructor of our view model.

```csharp
public Command AttachReceiptCommand { get; set; }

public NewExpenseViewModel()
{
    AttachReceiptCommand = new Command(
		async () => await AttachReceiptAsync());
}
```

Now that the boilerplate code is added for our `AttachReceiptCommand`, let's add some logic to attach a receipt. Users will have the option to attach photos of their receipts to their expenses. To do this, we will take advantage of **[Plugins for Xamarin](https://github.com/xamarin/XamarinComponents)**. Plugins for Xamarin are community built NuGet and Components that add cross-platform functionality or abstracts platform specific functionality to a common API. These are both completely cross-platform and extremely small (i.e., they do 1 or 2 things really well with minimal-to-no dependencies). The Plugin API can be accessed on each platform, however, you will most likely only use the common API in a Portable Class Library or Shared Code project. 

For Spent, we will be taking advantage of the [Media Plugin for Xamarin and Windows](https://blog.xamarin.com/getting-started-with-the-media-plugin-for-xamarin/) to take and/or select photos from the user's library to attach receipts. Plugins are distributed via NuGet, and the dependency has already been added for you, so let's add our media logic to the `AttachReceiptAsync` method. 

When using the Media Plugin for Xamarin, it's important that we initialize the plugin by calling it's `Initialize` method.

```csharp
using Plugin.Media;
using Plugin.Media.Abstractions;
...
async Task AttachReceiptAsync()
{
    try
    {
        await CrossMedia.Current.Initialize();
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

It's important to remember that while most mobile phones have cameras, not all do. Our logic must take into account devices where cameras are not available. Lucky for us, the Media Plugin for Xamarin has properties built in to let us know `IsCameraAvailable` and `IsTakePhotoSupported`. If it is available, then we can use the `TakePhotoAsync` method to take a photo; if not, we can use `PickPhotoAsync`, and allow the user to select a photo from their photo library. Finally, we will store the photo's path in the `Receipt` property if it is not null.

```csharp
using Plugin.Media;
using Plugin.Media.Abstractions;
...
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

That's it! In just over ten lines of code, we have pulled platform-specific functionality into our app via Plugins for Xamarin to allow users to take or select photo receipts and attach them to the `Expense`.

Our user interface will also have a save button in the navigation bar. To provide functionality for this in our view model, we will create a backing method named `SaveExpenseAsync`.

```csharp
async Task SaveExpenseAsync()
{
    try
    {
        // TODO: Save our expense.
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

To make this logic accessible from our user interface, let's create a new `Command` named `SaveExpenseCommand` and initialize it in our constructor.

```csharp
public Command SaveExpenseCommand { get; set; }
public Command AttachReceiptCommand { get; set; }

public NewExpenseViewModel()
{
	AttachReceiptCommand = new Command(
	    async () => await AttachReceiptAsync());

	SaveExpenseCommand = new Command(
		async () => await SaveExpenseAsync());
}
```

Time to add the logic to save our expense! Jump back to `SaveExpenseAsync`. In the try block, "new up" a new `Expense` using the properties created earlier in this step. Next, we want to utilize the `MessagingCenter` introduced in Module 1 to send a message to the `ExpensesViewModel` to save the new expense. Because we created a `DependencyService`, we could access our data layer from the `NewExpenseViewModel` without issue - so why send a message back to the `ExpensesViewModel`? By doing this, we can add the new expense directly to the `ObservableCollection<Expense>` that we data bind to so that the user interface automatically updates without requiring interaction from the user (such as a pull-to-refresh). Let's also send a message to our soon-to-be `NewExpensePage` to navigate backwards on the stack.

```csharp
async Task SaveExpenseAsync()
{
    try
    {
		var expense = new Expense
		{
			Company = Company,
			Description = Description,
			Date = DateTime,
			Amount = Amount,
			Receipt = Receipt
		};

		MessagingCenter.Send(this, "AddExpense", expense);
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
```

Excellent! Now that we have our `NewExpenseViewModel` complete, let's hop over to `ExpensesViewModel` and subscribe to the "AddExpense" message. In the constructor for the `ExpensesViewModel`, add the following code to subscribe to the "AddExpense" message, which will add the `Expense` to the `ObservableCollection<Expense>` and save it to our mock data store.

```csharp
MessagingCenter.Subscribe<NewExpenseViewModel, Expense>(this, "AddExpense", async (obj, expense) =>
{
    Expenses.Add(expense);

	await DependencyService.Get<IDataService>().AddExpenseAsync(expense);
});
```

Excellent! Now that we've implemented all of the logic for our user interface in the view model, let's actually create our new expenses page.

##### 3. Add new expense page.
To add a new page, right-click the `Views` folder, and add a new `Forms ContentPage Xaml` page named `NewExpensePage`. Our new page will be fairly similar to the `ExpenseDetailPage`, except for instead of `Label`s we will be using the `Entry` control to allow users to input text (similar to a `TextBox` in Windows). We will also use the `Button` control to allow users to attach a photocopy of a receipt, and the `Image` control to display the attached image. The `Image` control is capable of displaying embedded, local, as well as photos on the web. If the source of the photo is from the web, Xamarin.Forms will automatically cache this photo for a set period of time for you. 

```csharp
<?xml version="1.0" encoding="UTF-8"?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" x:Class="Spent.NewExpensePage"
		Title="New Expense">
	<ContentPage.Content>
		<StackLayout Padding="20">
			<Label Text="Company" TextColor="#4d4d4d"/>
			<Entry Text="{Binding Company}"/>
			<Label Text="Description" TextColor="#4d4d4d"/>
			<Entry Text="{Binding Description}" />
			<Label Text="Date" TextColor="#4d4d4d"/>
			<DatePicker Date="{Binding DateTime}" />
			<Label Text="Amount" TextColor="#4d4d4d"/>
			<Entry Text="{Binding Amount}" />
			<Label Text="Receipt" TextColor="#4d4d4d"/>
			<Button Text="Attach Receipt" Command="{Binding AttachReceiptCommand}" /> 
			<Image Source="{Binding Receipt}"/>
		</StackLayout>
	</ContentPage.Content>
</ContentPage>
```

We will be navigating to this page from our `ExpensesPage`, so we will have a navigation bar at the top of the page. To save items, let's add a button to the navigation bar with the text "Save". We can add a `ToolbarItem` to our `ContentPage.ToolbarItems` property in XAML to achieve this effect.

```csharp
<ContentPage.ToolbarItems>
	<ToolbarItem Text="Save" Command="{Binding SaveExpenseCommand}" />
</ContentPage.ToolbarItems>
```

Excellent! Now let's jump back over to the codebehind to configure our `BindingContext` for the view, as well as subscribe to messages. In the constructor of `NewExpensePage`, set the `BindingContext` property to a new `NewExpenseViewModel`. Additionally, override the `OnAppearing` and `OnDisappearing` methods, and add methods for `SubscribeToMessages` and `UnsubscribeToMessages`, just like we did for `ExpensesPage`.

```csharp
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
	MessagingCenter.Subscribe<NewExpenseViewModel, string>(this, "Error", (obj, s) =>
	{
		DisplayAlert("Error", s, "OK");
	});
}

void UnsubscribeFromMessages()
{
	MessagingCenter.Unsubscribe<NewExpenseViewModel, string>(this, "Error");
}
```

Let's add a new `MessagingCenter` subscription/unsubscription for the "Navigate" message to return us to the `ExpensesPage` after a user saves a new expense.

**Add to `SubscribeToMessages`**
```csharp
MessagingCenter.Subscribe<NewExpenseViewModel, string>(this, "Navigate", async (obj, s) =>
{
	if (s == "ExpensesPage")
	{
		await Navigation.PopAsync();
	}
});
```

**Add to `UnsubscribeFromMessages`**
```csharp
MessagingCenter.Unsubscribe<NewExpenseViewModel, string>(this, "Navigate");
```

Jump back to our `ExpensesPage`, were we will add a new `ToolbarItem` to allow users to navigate to our `NewExpensePage`.

```csharp
<ContentPage.ToolbarItems>
	<ToolbarItem Text="Add" Command="{Binding AddExpenseCommand}" />
</ContentPage.ToolbarItems>
```

We also need to create a new command in the `ExpensesViewModel` named `AddExpenseCommand`, as well as a backing method named `AddExpenseAsync`. Be sure to initialize the command in the constructor of `ExpensesViewModel`.

```csharp
public Command AddExpenseCommand { get; set; }
public ExpensesViewModel()
{
...
    AddExpenseCommand = new Command(
	    () => AddExpense());
...
}

void AddExpense()
{
	if (IsBusy)
	    return;

	IsBusy = true;

	try
	{
		// TODO: Navigate to AddExpensePage.
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

Just as with all other navigation in our app, let's use the `MessagingCenter` to perform navigation.

**Add to try block in `AddExpense`**
```csharp
MessagingCenter.Send(this, "Navigate", "NewExpensePage");
```

Finally, we need to subscribe and unsubscribe to these messages in our `ExpensesPage`.

```csharp
void SubscribeToMessages()
{
...
    MessagingCenter.Subscribe<ExpensesViewModel, string>(this, "Navigate", async (obj, s) =>
    {
    	if (s == "NewExpensePage")
	    { 
		    await Navigation.PushAsync(new NewExpensePage());
	    }
    });
...
}

void UnsubscribeFromMessages()
{
...
MessagingCenter.Unsubscribe<ExpensesViewModel, string>(this, "Navigate");
...
}
```

Build the app, click "Add", and you should now be able to create new expenses, attach photos, and have the new expenses appear in the main `ListView` on the `ExpensesPage`.

 ![](/modules/module-2/images/new-expense-page.png)

##### 4. Style the application.
We are now done with the functional aspects of our unconnected Spent app. In Module 3-4, we'll take a look at connected Spent to the cloud. For the remainder of this module, we will investigate other enhancements we can make to Spent with Xamarin.Forms features.

Styling is a big part of building any type of application. Individually styling controls can be incredibly painful as your application scales. Imagine that you want all heading labels to be a certain size and color, and have a custom font. Each time you create this label, you will have to not only add definitions for each of these properties, but also our code becomes much more unmaintainable. If we decide to change any of the values for heading labels, we now must make the change throughout the app.

The Xamarin.Forms [`ResourceDictionary`](https://developer.xamarin.com/guides/xamarin-forms/xaml/resource-dictionaries/) is a repository for resources that are used by a Xamarin.Forms application. Typical resources that are stored here include styles, control templates, data templates, colors, and converters. By creating a resource in the `ResourceDictionary`, we can increase the maintainability of our code by only having to make the change described above in one place. Depending on the scope of the resource, we can use the `ResourceDictionary` at the control-level, page-level, or application-level.

Let's jump over to `App.xaml` and define some resources for our application to use. Within `Application.Resources`, we can create a new `ResourceDictionary` element and add individual keys and values to the dictionary.

```csharp
<?xml version="1.0" encoding="utf-8"?>
<Application xmlns="http://xamarin.com/schemas/2014/forms" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" x:Class="Spent.App">
  <Application.Resources>
    <ResourceDictionary>
      <Color x:Key="Primary">#53BA9D</Color>
      <Color x:Key="MediumGrayTextColor">#4d4d4d</Color>
    </ResourceDictionary>
  </Application.Resources>
</Application>
```

Resources can be retrieved and applied by using the `StaticResource` markup extension. Open `NewExpensePage.xaml` and alter the XAML markup to use the `MediumGrayTextColor` attribute rather than the raw values.

```csharp
<?xml version="1.0" encoding="UTF-8"?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" x:Class="Spent.NewExpensePage"
		Title="New Expense">
	<ContentPage.ToolbarItems>
		<ToolbarItem Text="Save" Command="{Binding SaveExpenseCommand}" />
	</ContentPage.ToolbarItems>
	<ContentPage.Content>
		<StackLayout Padding="20">
			<Label Text="Company" TextColor="{StaticResource MediumGrayTextColor}"/>
			<Entry Text="{Binding Company}"/>
			<Label Text="Description" TextColor="{StaticResource MediumGrayTextColor}"/>
			<Entry Text="{Binding Description}" />
			<Label Text="Date" TextColor="{StaticResource MediumGrayTextColor}"/>
			<DatePicker Date="{Binding DateTime}" />
			<Label Text="Amount" TextColor="{StaticResource MediumGrayTextColor}"/>
			<Entry Text="{Binding Amount}" />
			<Label Text="Receipt" TextColor="{StaticResource MediumGrayTextColor}"/>
			<Button Text="Attach Receipt" Command="{Binding AttachReceiptCommand}" /> 
			<Image Source="{Binding Receipt}"/>
		</StackLayout>
	</ContentPage.Content>
</ContentPage>
``` 

Resources will be attempted to be retrieved first at the control-level, then the page-level, and finally the application-level. Thus, it's important that you keep the size of your `ResourceDictionary` as small as possible.

Open up `ExpenseDetailPage` and update the XAML to use our application `ResourceDictionary`.

```csharp
<?xml version="1.0" encoding="UTF-8"?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" x:Class="Spent.ExpenseDetailPage"
		Title="Expense Detail">
	<ContentPage.Content>
		<StackLayout Padding="20">
			<Label Text="Company" TextColor="{StaticResource MediumGrayTextColor}"/>
			<Label Text="{Binding Expense.Company}"/>
			<Label Text="Description" TextColor="{StaticResource MediumGrayTextColor}"/>
			<Label Text="{Binding Expense.Description}" />
			<Label Text="Date" TextColor="{StaticResource MediumGrayTextColor}"/>
			<Label Text="{Binding Expense.Date}" />
			<Label Text="Amount" TextColor="{StaticResource MediumGrayTextColor}"/>
			<Label Text="{Binding Expense.Amount}" />
			<Label Text="Receipt" TextColor="{StaticResource MediumGrayTextColor}"/>
			<Image Source="{Binding Expense.Receipt}"/>
		</StackLayout>
	</ContentPage.Content>
</ContentPage>
```

Run the app, and you will see that we continue to use the same theming as before, this time with the more maintainable Xamarin.Forms `ResourceDictionary`.

What if we want to enforce a consistent look across an app (or even multiple applications). Teams working on a family of apps should aim for consistent branding, and Xamarin.Forms **[Styles](https://developer.xamarin.com/guides/xamarin-forms/user-interface/styles/)** can help make that happen. Styles are defined in the `ResourceDictionary` and referenced from XAML. Instead of having to define a `TextColor`, `TextSize`, and `Font`, we can apply a single `Style` for the control, rather than continuing to reference all three properties. Implicit references can be applied to a particular control or page, and don't have to be explictly set as the `Style` for the view. This is great where you have a look that must be consistent for all instances of a control.

In Spent, let's create an implicit style to apply to our `NavigationPage` that alters the `BarBackgroundColor` and `BarTextColor` of the navigation bar. Open up `App.xaml`, and add the following to the `ResourceDictionary`.

```csharp
<Style TargetType="NavigationPage">
    <Setter Property="BarBackgroundColor" Value="{StaticResource Primary}" />
	<Setter Property="BarTextColor" Value="White" />
</Style>
```

Run the app, and you will now notice a beautiful navigation bar on our `ExpensesPage` that was implicitly applied with Xamarin.Forms styles.

 ![](/modules/module-2/images/styles.png)

##### 5. Use native embedding to add native controls.
One of the best features of Xamarin.Forms is that we still have access to 100% of the underlying native controls and features. In this section, we'll investigate how to add platform-specific views to Spent to enhance our Android application.

Platform-specific functionality falls into two main buckets:

1. **User interface** - Controls that the user will see on the screen, such as a `FloatingActionButton` on Android or `UICollectionView` on iOS. 
2. **Features** - Using platform functionality that's not part of the visual experience to the user, such as geolocation, bluetooth, or fingerprint authentication.

Platform-specific features can be brought into Xamarin.Forms apps by using the `DependencyService` introduced earlier in this module. Simply define an interface, implement the interface on each platform to provide the functionality you would like, and register the implementations.

To add or alter functionality in existing Xamarin.Forms controls or create our own, we have many different options available within Xamarin.Forms. The most powerful is **[custom renderers](https://developer.xamarin.com/guides/xamarin-forms/custom-renderer/)**. All controls built in Xamarin.Forms map to a native `Renderer` that use native controls. For example, the `Entry` in Xamarin.Forms maps to a `UITextField` on iOS, `EditText` on Android, and `TextBox` on Windows. We can tap into these renderers and make adjustments to the control's look or functionality. Additionally, we can create custom renderers to bring entirely new controls to Xamarin.Forms, such as charting or mapping controls.

Custom renderers are great because you have pull power over how controls are rendered and work, but they are a bit complex and overweight for most user interface tweaks. What if I just want to alter the `Slider` control to change colors as the value of the control changes? We could use custom renderers, but this is only a matter of altering a few properties. For this, we can use **[effects](https://developer.xamarin.com/guides/xamarin-forms/effects/)**. To use effects, we can create a `PlatformEffect` class in our platform-specific project, override `OnAttached`, and set the property. Then we can attribute our `Effect` and apply it to controls in our Xamarin.Forms user interface.

What if we could just native embed controls directly into our user interface? We can do this with **[native embedding](https://developer.xamarin.com/guides/xamarin-forms/user-interface/layouts/add-platform-controls/)**. This feature requires Shared Projects, as we will have to use compiler directives to make sure we aren't on the wrong platform. 

It's important that we're building mobile apps that take advantage of all the platforms have to offer, and that includes buiding user interfaces the way that platform's users expect. Recently, the `FloatingActionButton` has become the defacto way to "add" something on Android. Let's allow users on Android to use the `FloatingActionButton` to add new expenses.

Open up `ExpensesPage.xaml`. We first need to alter our layouts for the `ExpensesPage` so that we can easily overlay our `FloatingActionButton` on top of the `ListView`. Update the XAML so that the `ListView` is now wrapped in a `StackLayout` and `RelativeLayout`.

```csharp
<?xml version="1.0" encoding="UTF-8"?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" x:Class="Spent.ExpensesPage"
		Title="Expenses">
	<ContentPage.ToolbarItems>
		<ToolbarItem Text="Add" Command="{Binding AddExpenseCommand}" />
	</ContentPage.ToolbarItems>
	<ContentPage.Content>
		<RelativeLayout x:Name="relativeLayout">
			<StackLayout>
				<ListView ItemsSource="{Binding Expenses}"
					IsPullToRefreshEnabled="true"
					IsRefreshing="{Binding IsBusy, Mode=OneWay}"
					RefreshCommand="{Binding GetExpensesCommand}"
					SelectedItem="{Binding SelectedExpenseItem, Mode=TwoWay}">
					<ListView.ItemTemplate>
						<DataTemplate>
							<TextCell Text="{Binding Company}" Detail="{Binding Amount}" TextColor="{Binding Primary}" />
						</DataTemplate>
					</ListView.ItemTemplate>
				</ListView>
			</StackLayout>
		</RelativeLayout>
	</ContentPage.Content>
</ContentPage>
```
Now let's add our `FloatingActionButton` to our layout. We'll start by adding the #if/#endif compiler directive to the top of our `ExpensesPage` codebehind file to bring in necessary namespaces for adding the `FloatingActionButton`.

```csharp
#if __ANDROID__
using Android.Support.Design.Widget;
using Xamarin.Forms.Platform.Android;
#endif
```

Next, let's add another #if/#def compiler directive to our constructor with the following code.

```csharp
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
```

In the code above, we add a new `FloatingActionButton` to our `RelativeLayout`. We also use the `FloatingActionButton.Click` event to execute the `AddExpenseCommand` when the button is clicked. Finally, we remove the "Add" button from `ContentPage.ToolbarItems`, as users on Android should be using the `FloatingActionButton`.

Run the app, and you will notice a beautiful `FloatingActionButton` that, when clicked, navigates users to the add new expense page.

 ![](/modules/module-2/images/native-embedding.png)

> Currently in prerelease is a feature that allows us to add iOS and Android controls directly to our XAML, and even perform data binding and commanding with no additional configuration. 

##### 6. Improve application performance.
Performance is a large part of developing mobile apps. Performance is certainly important in building desktop applications as well, but issues with performance are definitely much more noticable on mobile devices. Couple this with many budget Android (and now iOS) devices with cheaper processors, and performance is suddenly very important.

Xamarin.Forms has many features to help ensure that your application performs well, in addition to the many other platform-specific performance tweaks you can make to build a performant mobile app. There are two key areas where we can gain large wins though - layouts and lists. By optimizing these two areas, we can see signficant performance boosts in our app.

`ListView`s often represent a large portion of mobile apps. By optimizing these, we can see massive performance boosts. One key area is **cell recycling**. Instead of creating potentially hundreds or thousands of cells for every row in a `ListView`, what if we reused a core set of cells (the number of visible cells on the screen plus a few more on top and bottom) to improve performance? We can enable this feature in Xamarin.Forms by simply updating our `ListView.CachingStrategy` to `RecycleElement`. Update the `ListView` in the `ExpensesPage` to use cell recycling.

```csharp
<ListView ItemsSource="{Binding Expenses}"
    IsPullToRefreshEnabled="true"
    IsRefreshing="{Binding IsBusy, Mode=OneWay}"
    RefreshCommand="{Binding GetExpensesCommand}"
    SelectedItem="{Binding SelectedExpenseItem, Mode=TwoWay}"
    CachingStrategy="RecycleElement">
    <ListView.ItemTemplate>
        <DataTemplate>
            <TextCell Text="{Binding Company}" Detail="{Binding Amount}" TextColor="{Binding Primary}" />
		</DataTemplate>
    </ListView.ItemTemplate>
</ListView>
```

Another area for optimization is layouts. Currently, XAML is parsed and inflated at runtime. Xamarin.Forms allows us to use the **XAML compiler** to precompile this XAML for signficant performance boosts, and also gaining functionality like compile-time error checking for XAML.

We can enable this on a per-page or per-application level by simply applying the `XamlCompilation` attribute and setting the `XamlCompilationOptions` to `Compile`. There's not much reason not to use the XAMLC feature in Xamarin.Forms, so let's update `App.xaml.cs` to use the XAML compiler throughout our application.

```csharp
[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
```

Boom! We've now optimized our Xamarin.Forms expenses app, Spent, to use the latest-and-greatest in Xamarin.Forms to build a performant mobile app with native functionality. In Modules 3-4, we will take a look at connecting Spent to the cloud so users can access their expenses from any device.
