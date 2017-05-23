# Módulo 2: Extendiendo tu primera aplicación de Xamarin.Forms
**Objetivos**: Continua explorando las características y funcionalidades de Xamarin.Forms, incluyendo estilos, diseño de interfaz de usuario y mejoras de rendimiento.

##### Requisitos previos
Asegúrate de que tienes instalado el siguiente software:

* Visual Studio 2015 Community Edition (o superior) o Xamarin Studio Community Edition (o superior)
* [Xamarin] (xamarin.com/download)

Descarga el código inicial de este módulo para comenzar o continua trabajando con el código completo del Módulo 1.

### Instrucciones del módulo
Este módulo esta basado en el Módulo 1 ampliando aún más tu aplicación para permitirte agregar tus propios gastos, incluidas las fotos de los recibos. También echamos un vistazo al `DependencyService`, a los estilos, a la incrustación de control nativo, así como a las mejoras de rendimiento para crear aplicaciones móviles con gran rendimiento.

##### 1. Crea un servicio de almacenamiento de datos con el `DependencyService`.
La arquitectura actual de Spent no es mala, pero definitivamente hay margen para mejorar, especialmente en relación a los datos. Es probable que necesitemos una forma de acceder a nuestros datos de gastos, sin que importe en que parte de la aplicación nos encontremos (como agregar un nuevo gasto). Además, también necesitamos flexibilidad en caso de que deseamos cambiar la forma en la que nuestros datos se almacenarán en el futuro (como la nube por ejemplo).

Podemos utilizar el Xamarin.Forms [`DependencyService`] (https://developer.xamarin.com/guides/xamarin-forms/dependency-service/) para reducir aún más el acoplamiento y permitirnos cambiar fácilmente las implementaciones de almacenamiento de datos con una sola línea de código. El `DependencyService` resuelve dependencias. En la práctica, se define una interfaz y el `DependencyService` encuentra la implementación correcta para esa interfaz. Esto se utiliza a menudo en las aplicaciones de Xamarin.Forms para acceder a las funcionalidades y características específicas de la plataforma, pero también podemos usarla como un servicio de dependencia regular.

Hay tres partes principales en el `DependencyService`:

1. **Interface**: Define el contrato y la funcionalidad requerida para el servicio.
2. **Implementation**: Implementación del contrato de interfaz. Podemos tener múltiples implementaciones.
3. **Registration**: Cada clase de implementación debe estar registrada con el `DependencyService` a través de un atributo de metadatos. Esto permite que `DependencyService` encuentre la clase de implementación y lo suministre en tiempo de ejecución.

Para los datos, esto es increíble! Podemos crear una interfaz que defina un contrato básico de almacenamiento de datos y que tenga múltiples implementaciones. Por ejemplo, podríamos tener una implementación en la nube que se utilice en la producción, además de una implementación de datos falsos (como hemos hecho hasta ahota) que se utilice durante las pruebas.

¡Creemos nuestro primer servicio! Comienza haciendo clic derecho en la carpeta `Services`, añadiendo una nueva interfaz C# en blanco y llamándola `IDataService`. Vamos a definir dos métodos que cada servicio debe implementar, `AddExpenseAsync` y `GetExpensesAsync`.

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

A continuación, crea una implementación de nuestro `IDataService` con datos simulados. Haz clic con el botón derecho en la carpeta `Services` y agrega una nueva clase C# en blanco llamada `MockDataService`. Implementa la interfaz `IDataService`.

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

¡Genial! Ahora implementa `MockDataService`. Comienza añadiendo una `List<Expense>` para hacer un seguimiento de los gastos y `bool` para controlar si nuestro almacen simulado ha sido inicializado.

```csharp
bool isInitialized;
List<Expense> expenses;
```

A continuación, vamos a agregar un método llamado `Initialize` para inicializar la lista de gastos con algunos datos simulados.

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

Ahora que tenemos nuestros datos inicializados, vamos a agregar implementaciones para nuestros métodos `AddExpenseAsync` y `GetExpensesAsync`.

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

El último paso necesario para trabajar con `DependencyService` es el registro. Podemos hacerlo fácilmente añadiendo el siguiente atributo a la parte superior del namespace de la clase `MockDataService`.

```csharp
[assembly: Xamarin.Forms.Dependency(typeof(Spent.MockDataService))]
```

Vuelve al método `GetExpensesAsync` de `ExpensesViewModel`. Elimina el código del bloque try y reemplázalo por lo siguiente.

```csharp
Expenses.Clear();

var expenses = await DependencyService.Get<IDataService>().GetExpensesAsync();
foreach (var expense in expenses)
{
    Expenses.Add(expense);
}
```

En el código anterior, primero borramos la colección `Expenses` antes de usar el `DependencyService` para obtener los gastos. Finalmente, los añadimos a nuestra colección para que la interfaz de usuario se actualice.

> **Mejores Prácticas**: ¿Recuerdas cómo `ObservableCollection<T>` lanza eventos de cambios de colección para que nuestra interfaz de usuario sepa que tiene que actualizarse? Esto significa que la interfaz de usuario se actualiza para cada adición a la colección, que es costosa en términos de rendimiento. Para obtener mejores resultados, usa `ObservableRangeCollection <T>`, que sólo disparará la colección que cambió el evento una vez, lo que proporcionará una mejora de rendimiento.

¡Genial! Hemos eliminado con éxito otro ejemplo de acoplamiento. Ahora que tenemos un servicio de datos más robusto y fácilmente accesible, implementemos la lógica para agregar nuevos gastos.

##### 2. Agregar nuevo modelo de vista de gastos.
//TODO
De igual forma que hiciste con `ExpensesPage`, crea un modelo de vista para administrar toda la lógica de la interfaz de usuario de añadir gastos. Haz clic derecho en la carpeta `ViewModels` y agrega una nueva clase C# en blanco denominada `NewExpenseViewModel`. Al igual que los demás modelos de vista, subclase `BaseViewModel` para obtener la funcionalidad` INotifyPropertyChanged`, así como nuestra propiedad `IsBusy`.

Empieza añadiendo algunas propiedades públicas al modelo de vista para representar los diversos campos que tiene un gasto.

```csharp
public string Company { get; set; }
public string Description { get; set; }
public DateTime DateTime { get; set; }
public string Amount { get; set; }
```

Para el recibo, necesitamos crear una propiedad tradicional con un campo de respaldo para que podamos llamar a `OnPropertyChanged` cuando se adjunte un recibo. Tenemos que hacer esto porque nuestra interfaz de usuario tendrá que actualizarse cuando se adjunte un recibo para mostrar la foto.

```csharp
string receipt;
public string Receipt
{
	get { return receipt; }
	set { receipt = value; OnPropertyChanged(); }
}
```
 
Genial, ahora que tenemos todas las propiedades de respaldo para nuestra interfaz de usuario, vamos a crear varios comandos para adjuntar el recibo y guardar los gastos. En primer lugar, vamos a crear un método que contendrá la lógica de nuestro comando llamado `AttachReceiptAsync`.

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

A continuación, debemos añadir el `Command` para que nuestra interfaz de usuario se pueda enlazarse con 'AttachReceiptCommand`. También hay que inicializar nuestro comando en el constructor del modelo de vista.

```csharp
public Command AttachReceiptCommand { get; set; }

public NewExpenseViewModel()
{
    AttachReceiptCommand = new Command(
		async () => await AttachReceiptAsync());
}
```

Ahora que el código se agrega a 'AttachReceiptCommand`, vamos a agregar up poco de lógica para adjuntar un recibo. Los usuarios tendrán la opción de adjuntar fotos de sus recibos a sus gastos. Para ello, aprovecharemos **[Plugins para Xamarin] (https://github.com/xamarin/XamarinComponents)**. Los Plugins para Xamarin son NuGet y Componentes que añaden funcionalidad multiplataforma o abstrae la funcionalidad específica de la plataforma a una API común. Éstos son completamente completamente multiplataforma y muy pequeños (es decir, hacen 1 o 2 cosas realmente bien con dependencias mínimas). Se puede acceder a la API del Plugin en cada plataforma, sin embargo, lo más probable es que solo utilices la API común en un proyecto de tipo PCL o de código compartido.

//TODO
Para Spent, estaremos aprovechando el [Media Plugin para Xamarin y Windows] (https://blog.xamarin.com/getting-started-with-the-media-plugin-for-xamarin/) para tomar y/o seleccionar fotos de la biblioteca del usuario para adjuntar recibos. Los complementos se distribuyen a través de NuGet, y la dependencia ya ha sido añadida, así que vamos a agregar nuestra lógica de media al método `AttachReceiptAsync`.

Cuando se utiliza el Media Plugin para Xamarin, es importante que inicialices el complemento llamando a su método `Initialize`.

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

Es importante recordar que mientras que la mayoría de los teléfonos móviles tienen cámaras, no todos la tienen. Nuestra lógica debe tener en cuenta los dispositivos en los que las cámaras no están disponibles. Por suerte para nosotros, el Media Plugin para Xamarin tiene propiedades incorporadas para hacernos saber `IsCameraAvailable` e `IsTakePhotoSupported`. Si está disponible, podemos usar el método `TakePhotoAsync` para sacar una foto; Sino, podemos utilizar `PickPhotoAsync`, y permitir al usuario seleccionar una foto de su biblioteca de fotos. Finalmente, almacenaremos la ruta de la foto en la propiedad `Receipt`si no es nula.

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

¡Ya está! En poco más de diez líneas de código, hemos extraído la funcionalidad específica de la plataforma en nuestra aplicación a través de Plugins para Xamarin permitiendo a los usuarios sacar o seleccionar recibos de fotos y adjuntarlos a `Expense`.

Nuestra interfaz de usuario también tendrá un botón de guardar en la barra de navegación. Para proporcionarle funcionalidad a esto en nuestro modelo de vista, crearemos un método de respaldo llamado `SaveExpenseAsync`.

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

Para hacer accesible esta lógica desde nuestra interfaz de usuario, vamos a crear un nuevo `Command` llamado `SaveExpenseCommand` e iniciarlizarlo en nuestro constructor.

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

¡Es hora de añadir lógica para guardar el gasto! Vuelve a `SaveExpenseAsync`. En el bloque try, "new up" un nuevo `Expense` utilizando las propiedades creadas anteriormente a este paso. A continuación, queremos utilizar el `MessagingCenter` añadido en el Módulo 1 para enviar un mensaje a `ExpensesViewModel` para guardar el nuevo gasto. Ya que creamos un `DependencyService`, podríamos acceder a nuestra capa de datos desde el `NewExpenseViewModel` sin problema - así que ¿por qué enviar un mensaje de vuelta a `ExpensesViewModel`? Si hacemos esto, podemos agregar el nuevo gasto directamente a `ObservableCollection <Expense>` al que los datos se enlazarán para que la interfaz de usuario se actualice automáticamente sin requerir interacción del usuario (como pull-to-refresh). Enviemos también un mensaje a nuestro futuro "NewExpensePage" para retroceder en la navegación.

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

¡Excelente! Ahora que tenemos nuestro `NewExpenseViewModel` completo, pasemos a `ExpensesViewModel` y nos suscribiremos al mensaje "AddExpense". En el constructor de `ExpensesViewModel`, añade el siguiente código para suscribirte al mensaje "AddExpense", que agregará el `Expense` al `ObservableCollection<Expense>` y lo guardará en nuestro almacén de datos simulado.

```csharp
MessagingCenter.Subscribe<NewExpenseViewModel, Expense>(this, "AddExpense", async (obj, expense) =>
{
    Expenses.Add(expense);

	await DependencyService.Get<IDataService>().AddExpenseAsync(expense);
});
```

Excellent! Now that we've implemented all of the logic for our user interface in the view model, let's actually create our new expenses page.
¡Excelente! Ahora que hemos implementado toda la lógica de nuestra interfaz de usuario en el modelo de vista, vamos a crear nuestra nueva página de gastos.

##### 3. Añade una nueva página de gastos.
Para agregar una nueva página, haz clic con el botón derecho en la carpeta `Vistas` y añade una nueva página` Forms ContentPage Xaml` llamada `NewExpensePage`. Nuestra nueva página será bastante similar a `ExpenseDetailPage`, excepto que en lugar de `Label`s usaremos el control `Entry` para permitir a los usuarios introducir texto (similar a` TextBox` en Windows). También usaremos el control `Button` para permitir a los usuarios adjuntar una fotocopia de un recibo, y el control` Image` para mostrar la imagen adjunta. El control `Image` es capaz de mostrarse incrustado, local, así como fotos en la web. Si la fuente de la foto es de la web, Xamarin.Forms automáticamente cachea esta foto durante un período de tiempo establecido para ti.

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

Navegaremos a esta página desde `ExpensesPage`, por lo que tendremos una barra de navegación en la parte superior de la página. Para guardar elementos, vamos a agregar un botón a la barra de navegación con el texto "Guardar". Podemos agregar un `ToolbarItem` a nuestra propiedad `ContentPage.ToolbarItems` en XAML para lograr dicho efecto.

```csharp
<ContentPage.ToolbarItems>
	<ToolbarItem Text="Save" Command="{Binding SaveExpenseCommand}" />
</ContentPage.ToolbarItems>
```

¡Excelente! Ahora vamos a volver al codebehind para configurar nuestro `BindingContext` para la vista, así como suscribirnos a los mensajes. En el constructor de `NewExpensePage`, establece el valor de la propiedad `BindingContext` a un nuevo `NewExpenseViewModel`. Además, reemplaza los métodos `OnAppearing` y `OnDisappearing`, y añade métodos para `SubscribeToMessages` y `UnsubscribeToMessages`, de igual forma que hicimos para `ExpensesPage`.

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

Añade un nueva suscripción/baja de `MessagingCenter` para que el mensaje de "Navigate" nos redirija a `ExpensesPage` una vez que el usuario guarde un nuevo gasto.

**Añade a `SubscribeToMessages`**
```csharp
MessagingCenter.Subscribe<NewExpenseViewModel, string>(this, "Navigate", async (obj, s) =>
{
	if (s == "ExpensesPage")
	{
		await Navigation.PopAsync();
	}
});
```

**Añade a `UnsubscribeFromMessages`**
```csharp
MessagingCenter.Unsubscribe<NewExpenseViewModel, string>(this, "Navigate");
```

Vuelve a `ExpensesPage`, donde añadirás un nuevo `ToolbarItem` para permitir a los usuarios navegar a `NewExpensePage`.

```csharp
<ContentPage.ToolbarItems>
	<ToolbarItem Text="Add" Command="{Binding AddExpenseCommand}" />
</ContentPage.ToolbarItems>
```

También necesitamos crear un nuevo comando en `ExpensesViewModel` llamado `AddExpenseCommand`, así como un método de respaldo llamado `AddExpenseAsync`. Asegúrate de inicializar el comando en el constructor de `ExpensesViewModel`.

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

Al igual que el resto de la navegación en la aplicación, utilicemos el `MessagingCenter` para realizar la navegación.

**Add to try block in `AddExpense`**
```csharp
MessagingCenter.Send(this, "Navigate", "NewExpensePage");
```

Finalmente, tenemos que suscribirnos y darnos de baja de estos mensajes en `ExpensesPage`.

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

Compila la aplicación, presiona "Add", y deberías poder crear nuevos gastos, adjuntar fotos y que se muestren los gastos en el `ListView` principal en `ExpensesPage`.

 ![](/modules/module-2/images/new-expense-page.png)

##### 4. Estiliza la aplicación.
Ahora hemos terminado con los aspectos funcionales de nuestra aplicación Spent. En el Módulo 3-4, echaremos un vistazo a la conexión de Spent a la nube. Durante el resto de este módulo, vamos a investigar otras mejoras que podemos hacer con características de Xamarin.Forms.

Dar estilo es una gran parte del desarrollo de cualquier tipo de aplicación. Los controles individuales de estilo pueden ser increíblemente dolorosos a medida que la aplicación escala. Imagina que quieres que todas las etiquetas de encabezado tengan un cierto tamaño y color y que tengan una fuente personalizada. Cada vez que crees esta etiqueta, tendrás que añadir no sólo definiciones para cada una de estas propiedades, sino que también el código será mucho más inasequible. Si decidimos cambiar cualquier valor de las etiquetas de encabezado, deberemos realizar el cambio en toda la aplicación.

El [`ResourceDictionary`] (https://developer.xamarin.com/guides/xamarin-forms/xaml/resource-dictionaries/) de Xamarin.Forms es un repositorio de recursos utilizados por una aplicación Xamarin.Forms. Los recursos típicos que se almacenan aquí incluyen estilos, plantillas de control, plantillas de datos, colores y convertidores. Al crear un recurso en el `ResourceDictionary`, podemos aumentar la capacidad de mantenimiento de nuestro código realizando solo el cambio descrito anteriormente en un solo lugar. Dependiendo del alcance del recurso, podemos usar el `ResourceDictionary` a nivel de control, a nivel de página o a nivel de aplicación.

Dirigete a `App.xaml` y define algunos recursos a utilizar por la aplicación. Dentro de `Application.Resources`, podemos crear un nuevo elemento `ResourceDictionary` y agregar claves y valores individuales al diccionario.

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

Los recursos se pueden obtener y aplicar utilizando la extensión de marcado `StaticResource`. Abre `NewExpensePage.xaml` y modifica el XAML para utilizar el atributo `MediumGrayTextColor` en lugar de los valores sin procesar.

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

Se intentará recuperar los recursos obtenidos a nivel de control, luego a nivel de página y finalmente a nivel de aplicación. Por lo tanto, es importante que mantengas el tamaño de tu `ResourceDictionary` lo más pequeño posible.

Abre `ExpenseDetailPage` y actualiza el XAML para usar el `ResourceDictionary`.

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

Ejecuta la aplicación, y verás que se sigue utilizando el mismo tema que antes, pero ahora con Xamarin.Forms `ResourceDictionary`, que es más fácil de mantener.

¿Qué pasa si queremos reforzar una apariencia consistente en una aplicación (o incluso en varias aplicaciones). Los equipos que trabajan en una familia de aplicaciones deben apuntar a una marca coherente, y Xamarin.Forms ** [Styles] (https://developer.xamarin.com/guides/xamarin-forms/user-interface/styles/) ** ayuda a que eso suceda. Los estilos se definen en el `ResourceDictionary` y se referencian desde XAML. En lugar de tener que definir un `TextColor`, `TextSize` y `Font`, podemos aplicar un solo `Style` para el control, en lugar de seguir haciendo referencia a las tres propiedades. Pueden aplicarse referencias implícitas a un control o página en particular y no tienen que establecerse explícitamente como "Style" para la vista. Esto es genial cuando tienes un aspecto que debe ser consistente en todas las instancias de un control.

En Spent, vamos a crear un estilo implícito para aplicarlo a nuestro `NavigationPage` que altera el `BarBackgroundColor` y `BarTextColor` de la barra de navegación. Abre `App.xaml` y añade lo siguiente al` ResourceDictionary`.

```csharp
<Style TargetType="NavigationPage">
    <Setter Property="BarBackgroundColor" Value="{StaticResource Primary}" />
	<Setter Property="BarTextColor" Value="White" />
</Style>
```

Ejecuta la aplicación, y ahora verás una hermosa barra de navegación en tu `ExpensesPage` que se aplicó implícitamente con los estilos Xamarin.Forms.

 ![](/modules/module-2/images/styles.png)

##### 5. Utilizar incrustación nativa para agregar controles nativos.
Una de las mejores características de Xamarin.Forms es que todavía tenemos acceso al 100% de los controles y características nativas subyacentes. En esta sección, investigaremos cómo agregar vistas específicas de la plataforma a Spent para mejorar nuestra aplicación de Android.

La funcionalidad específica cada la plataforma se divide en dos grupos principales:

1. **Interfaz de usuario** - Controles que el usuario verá en la pantalla, como un `FloatingActionButton` en Android o` UICollectionView` en iOS.
2. **Características** - Uso de la funcionalidad de la plataforma que no forma parte de la experiencia visual para el usuario, como geolocalización, bluetooth o autenticación de huellas digitales.

Las características específicas de la plataforma pueden introducirse en aplicaciones Xamarin.Forms mediante el `DependencyService` introducido anteriormente en este módulo. Simplemente defina una interfaz, implemente la interfaz en cada plataforma para proporcionar la funcionalidad que desea y registre las implementaciones.

Para agregar o alterar la funcionalidad en los controles existentes de Xamarin.Forms o crear nuestros propios, tenemos muchas opciones diferentes disponibles dentro de Xamarin.Forms. El más potente es ** [renderizador personalizado] (https://developer.xamarin.com/guides/xamarin-forms/custom-renderer/) **. Todos los controles construidos en Xamarin.Forms se corresponden con un  `Renderer` nativo que utiliza controles nativos. Por ejemplo, `Entry` en Xamarin.Forms asigna a` UITextField` en iOS, `EditText` en Android y` TextBox` en Windows. Podemos aprovechar estos renderizadores y hacer ajustes a la apariencia o funcionalidad del control. Además, podemos crear renderizadores personalizados para llevar controles completamente nuevos a Xamarin.Forms, como mapas o controles de mapeo.

Los renderizadores personalizados son excelentes porque tienes el poder sobre cómo se procesan y funcionan los controles, pero son un poco complejos y pesados para la mayoría de los ajustes de la interfaz de usuario. ¿Qué pasa si sólo quiero cambiar el control `Slider` para cambiar los colores a medida que cambia el valor del control? Podríamos usar renderizadores personalizados, pero esto es sólo una cuestión de modificar algunas propiedades. Para ello, podemos utilizar **[efectos] (https://developer.xamarin.com/guides/xamarin-forms/effects/)**. Para utilizar los efectos, podemos crear una clase `PlatformEffect` en nuestro proyecto específico de plataforma, override ` OnAttached` y establecer la propiedad. Despues podemos atribuir nuestro `Effect` y aplicarlo a los controles en nuestra interfaz de usuario Xamarin.Forms.

¿Qué pasaría si pudiéramos incorporar controles nativos directamente en nuestra interfaz de usuario? Podemos hacer esto con **[incrustación nativa] (https://developer.xamarin.com/guides/xamarin-forms/user-interface/layouts/add-platform-controls/)**. Esta característica requiere Proyectos Compartidos, ya que tendremos que usar las directivas del compilador para asegurarnos de que no estamos en la plataforma equivocada.

Es importante que construyamos aplicaciones móviles que aprovechen todo lo que las plataformas ofrecen, y eso incluye la creación de interfaces de usuario de la manera que los usuarios de la plataforma esperan. Recientemente, el `FloatingActionButton` se ha convertido en la manera correcta de" añadir "algo en Android. Permitamos que los usuarios de Android utilicen el `FloatingActionButton` para agregar nuevos gastos.

Abre 'ExpensesPage.xaml`. Primero necesitamos modificar nuestros diseños de `ExpensesPage` para poder superponer fácilmente nuestro` FloatingActionButton` en la parte superior del `ListView`. Actualiza el XAML para que el `ListView` esté ahora envuelto en un` StackLayout` y `RelativeLayout`.

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

Ahora vamos a añadir nuestro `FloatingActionButton` a nuestro diseño. Comenzaremos añadiendo la directiva de compilador # if/# endif a la parte superior de nuestro archivo `ExpensesPage` para obtener los espacios de nombres necesarios para agregar el `FloatingActionButton`.

```csharp
#if __ANDROID__
using Android.Support.Design.Widget;
using Xamarin.Forms.Platform.Android;
#endif
```

Ahora, añadamos otra directiva de compilador #if/#def a nuestro constructor con el siguiente código.

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

En el código anterior, añadimos un nuevo `FloatingActionButton` a nuestro `RelativeLayout`. También utilizamos el evento `FloatingActionButton.Click` para ejecutar `AddExpenseCommand` cuando se haga clic en el botón. Finalmente, eliminamos el botón "Añadir" de `ContentPage.ToolbarItems`, ya que los usuarios de Android deberían usar el `FloatingActionButton`.

Ejecuta la aplicación y notarás un bonito `FloatingActionButton` que, al hacer clic, navega por los usuarios para añadir la página de nuevos gastos.

 ![](/modules/module-2/images/native-embedding.png)

> Actualmente en prerelease es una característica que nos permite agregar controles de iOS y Android directamente a nuestro XAML, e incluso realizar vinculación de datos y comandos sin configuración adicional.

##### 6. Mejora del rendimiento de la aplicación
El rendimiento es una parte importante del desarrollo de aplicaciones móviles. El rendimiento es sin duda importante en la construcción de aplicaciones de escritorio, pero los problemas de rendimiento se notan más en los dispositivos móviles. Sumale a esto dispositivos Android económicos (y ahora iOS) con procesadores más baratos, y el rendimiento de repente es muy importante.

Xamarin.Forms tiene muchas características que ayudan a asegurar que la aplicación funcione bien, además de mucho otros ajustes de rendimiento específicos de cada plataforma que puedes aprovechar para crear una aplicación móvil con alto rendimiento. Hay dos áreas clave en las que podemos tomar ventaja en nuestro desarrollo - diseños y listas. Al optimizar estas dos áreas, podemos ver un aumento significativo de rendimiento en nuestra aplicación.

Los `ListView`s a menudo representan una gran parte de las aplicaciones móviles. Al optimizarlos, podemos ver aumentos de rendimiento masivos. Un área clave es **reciclaje de células**. En lugar de crear potencialmente cientos o miles de celdas para cada fila en un `ListView`, ¿qué pasaría si reutilizasemos un núcleo de células (el número de celdas visibles en la pantalla y algunos más en la parte superior e inferior) para mejorar el rendimiento? Podemos activar esta función en Xamarin.Forms actualizando nuestro `ListView.CachingStrategy` a `RecycleElement`. Actualiza el `ListView` en el `ExpensesPage` para utilizar el reciclaje de células.

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

Otra área de optimización son los diseños. En la actualidad, XAML se analiza e infla en tiempo de ejecución. Xamarin.Forms nos permite usar el **compilador XAML** para precompilar este XAML y aumentar significativamente el rendimiento, además de también ganar funcionalidad como la comprobación de errores en tiempo de compilación para XAML.

Podemos habilitar esto a nivel de página o de aplicación utilizando el atributo `XamlCompilation` y definiendo `XamlCompilationOptions` a `Compile`. No hay motivo por el cual no utilizar la característica XAMLC en Xamarin.Forms, así que vamos a actualizar `App.xaml.cs` para usar el compilador XAML en toda nuestra aplicación.


```csharp
[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
```

¡Boom! Hemos optimizado nuestra aplicación de gastos de Xamarin.Forms, Spent, para utilizar lo último y mejor de Xamarin.Forms para crear una aplicación móvil con funcionalidad nativa. En los Módulos 3-4, veremos cómo conectar a Spent a la nube para que los usuarios puedan acceder a sus gastos desde cualquier dispositivo.