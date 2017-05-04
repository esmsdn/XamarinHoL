# Módulo 1: Construyendo tu primera aplicación Xamarin.Forms
**Objetivo**: Aprender las bases del desarrollo de aplicaciones móviles nativas para iOS, Android y Windows con Xamarin.Forms. Construye una aplicación de tipo "master-detail" en XAML que utiliza el patrón Modelo-Vista-VistaModelo (MVVM).

##### Prerequisitos
Asegurate que tienes instalado el siguiente software:

* Visual Studio 2015 Community Edition (o superior) o Xamarin Studio Community Edition (o superior)
* [Xamarin](xamarin.com/download)

Descarga el código inicial de este módulo para empezar.

### Empezando
En este taller, aprenderas a desarrollar aplicaciones multiplataformas conectadas para iOS, Android y Windows con Xamarin.Forms y Microsoft Azure. Este taller te guiará de inicio a fin en el desarrollo de una aplicación qué hará un seguimiento de gastos personales, incluyendo el almacenamiento de recibos.

Esta solución contiene 4 proyectos:

* Spent - Proyecto Compartido que contiene todo el código compartible a través de las multiples plataformas (modelos, view models, vistas, lógica de servicio, etc.).
* Spent.Droid - Aplicación Xamarin.Android 
* Spent.iOS - Aplicación Xamarin.iOS
* Spent.Windows - Aplicación Windows 10 UWP (solo puede ser ejecutada desde Visual Studio 2015 en Windows 10).

 ![](/modules/module-1/images/solution-explorer.png)

NuGet es un gestor de paquetes para .NET que nos ayuda a aprovechar las ventajas que nos ofrecen las librerías existentes como JSON.NET para compartir más código y desarrollar aplicaciones de manera más rápida. Todos los proyectos ya contienen las dependencias NuGet, por tanto no existe necesidad de instalar paquetes NuGet adicionales a lo largo del taller. Para descargar los Nuggets asociados a los proyectos hay que restaurarlos.

Para hacer esto, haz click derecho sobre el nombre de la solución en el **Explorador de Soluciones** y elige **Restaurar Paquetes NuGet**.

 ![](/modules/module-1/images/restore-nugets.png)

### Instrucciones del módulo
Comenzamos a construir nuestra aplicacion de gastos - Spent!

##### 1. Ejecuta el código inicial.
Para empezar, abre el código inicial en Visual Studio o Xamarin Studio. Depura la aplicación para iOS, Android o Universal Windows Platform (UWP).

> Nota: Si estás utilizando Visual Studio en Windows, tendrás que estar [conectado a un Mac](https://developer.xamarin.com/guides/ios/getting_started/installation/windows/connecting-to-mac/) para crear y depurar la solución iOS. Si estás ejecutando Xamarin Studio en Mac, no podrás crear el proyecto de Universal Windows Platform. 

Cuando la aplicación se despliega en el simulador o emulador, verás una sola etiqueta que dice "Welcome to Xamarin.Forms!".

 ![](/modules/module-1/images/welcome-to-xamarin-forms.png)

Investiguemos las piezas clave de una aplicación Xamarin.Forms. Expande el proyecto `Spent` para ver varias carpetas vacías y algunos archivos. Las aplicaciones Xamarin tradicionales nos permiten compartir entre el 70-90% del código, pero toda la lógica de la interfaz de usuario reside en los proyectos individuales de la plataforma. Para aplicaciones Xamarin.Forms, aun podemos compartir todo el código que desarrollamos en una aplicación Xamarin tradicional, y también la lógica de la interfaz de usuario. Todo el código compartido está desarrollado en un [Proyecto Compartido] (https://developer.xamarin.com/guides/cross-platform/application_fundamentals/shared_projects/) o [Portable Class Library (PCL)](https://developer.xamarin.com/guides/cross-platform/application_fundamentals/pcl/).

La clase `Application` es el punto de entrada principal de las aplicaciones Xamarin.Forms. Esta clase selecciona la página principal de la aplicación y administra la lógica del ciclo de vida de la aplicación, como `OnStart`,` OnSleep` y `OnResume`.

Es natural preguntarse por qué todavía tenemos proyectos específicos de cada plataforma si se comparte todo el código. Esto se debe a que todas las aplicaciones de Xamarin.Forms _son_ aplicaciones nativas, y todavía tenemos la capacidad de acceder al código específico de la plataforma para acceder al 100% de las API nativas subyacentes. Si entras al proyecto `Spent.Droid` y abres` MainActivity`, verás dos líneas que inicializarán la librería de Xamarin.Forms y cargarán una nueva aplicación Xamarin.Forms:

``` csharp
Xamarin.Forms.Forms.Init(this, bundle);
LoadApplication(new App());
```
Dentro de una aplicación existen una serie de pantallas, o ** [Páginas] (https://developer.xamarin.com/guides/xamarin-forms/controls/pages/) **. Existen siete tipos diferentes de páginas en Xamarin.Forms, desde páginas para mostrar contenido (`ContentPage`) hasta páginas que gestionan la navegación (` TabbedPage`, `NavigationPage`, etc.). Las páginas definen una interfaz de usuario en XAML o C #. Si defines la lógica de la interfaz de usuario en XAML, también tendrás un archivo de codebehind asociado (llamado `.xaml.cs`) para la lógica de esa página. `MainPage` en el proyecto` Spent` es un ejemplo de una página.

Dentro de una página, podemos usar **[Layouts] (https://developer.xamarin.com/guides/xamarin-forms/controls/layouts/)** para que Xamarin.Forms sepa cómo mostrar controles individuales dentro de la página. Existen dos tipos principales: diseños gestionados y no gestionados. En general, vamos a optar por diseños gestionados, ya que inteligentemente "administran" el diseño de nuestros controles, sin importar el sistema operativo o dispositivo en el que se ejecuta la aplicación.

##### 3. Agrega un view model base.
Ahora que hemos visto una introducción básica a Xamarin.Forms, vamos a empezar a construir nuestra aplicación! Xamarin.Forms tiene soporte incorporado para el patrón de diseño Model-ViewModel-View (MVVM) que es común en el desarrollo de aplicaciones Windows. Esto nos ayuda a separar nuestra lógica de interfaz de usuario de nuestra lógica de negocio. La ** Vista ** contiene toda la lógica de la interfaz de usuario (o lo que el usuario ve cuando usa tu aplicación). ** ViewModel ** es una abstracción de la vista que contiene la lógica que impulsa la interacción del usuario con nuestra aplicación. Un ejemplo para una página con una lista de elementos puede ser lógica para descargar JSON desde la web, deserializarla y ponerla en una lista para que nuestra interfaz de usuario se muestre. ** Modelo ** es un modelo de dominio o una capa de acceso a datos.

Para comenzar con MVVM, necesitamos utilizar el concepto de **[vinculación de datos] (https://developer.xamarin.com/guides/xamarin-forms/xaml/xaml-basics/data_binding_basics/)**. La vinculación de datos es el flujo de datos entre nuestra vista y el modelo de vista, como cuando un usuario tira de la actualización para cargar nuevos datos o los escribe en un cuadro de texto. Nuestra interfaz de usuario debe ser alertada cuando algo cambie en nuestro modelo de vista. Para ello, implementaremos la interfaz `INotifyPropertyChanged`. Debido a que este comportamiento es algo que queremos tener en todos nuestros modelos de vista para esta aplicación, vamos a empezar por agregar un nuevo modelo de vista base que podemos reutilizar.

Haga clic con el botón derecho del ratón en la carpeta `Ver modelos`, seleccione` Añadir -> Nuevo archivo`, seleccione una clase C # vacía, y llámela `BaseViewModel`. Introduzca el espacio de nombres `System.ComponentModel` e implemente la interfaz` INotifyPropertyChanged`.

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

A continuación, vamos a agregar un método llamado `OnPropertyChanged` que activará el evento `PropertyChanged`. Cada vez que una propiedad cambie en nuestro modelo de vista que necesita ser actualizada en la vista, llamaremos a este método.

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


Ahora, podemos llamar a `OnPropertyChanged` siempre que una propiedad se actualice y nuestra interfaz de usuario se actualizará.

También queremos asegurarnos de que no estamos duplicando el esfuerzo en nuestros modelos de vista. Si un usuario refresca cuatro veces, sólo debemos realizar una solicitud para actualizar los datos (suponiendo que las otras tres se produzcan antes de que se devuelvan los primeros datos). Para asegurarnos de que no estamos duplicando el esfuerzo, agreguemos una propiedad `IsBusy` que estableceremos en` true` cuando comience el trabajo y `false` cuando termine el trabajo.

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
El objetivo de Spent es ayudar a los usuarios a controlar sus gastos. Nuestro principal objeto de dominio es un gasto, por lo que vamos a añadir un nuevo modelo `Expense` para almacenar información sobre un gasto particular. Haz clic con el botón derecho del ratón en la carpeta `Modelos`, haz clic en `Agregar -> Nuevo archivo`, selecciona una clase C # vacía y llámala `Expense`.

Vamos a darle a nuestro modelo `Expense` algunas propiedades que esperamos que tenga un gasto particular.

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

##### 4. Add a expenses view model.
Spent va a visualizar una lista de los gastos que el usuario ha incurrido. ¡Vamos a crear un modelo de vista para nuestra pantalla inicial! Crea una nueva clase C# en la carpeta `View Models` que se llame `ExpensesViewModel`, y una subclase `BaseViewModel`.

Hay muchas formas de representar una lista de elementos en .NET, pero usaremos un tipo especial llamado `ObservableCollection <T>`. Este es un tipo de lista genérica que rastrea los cambios a la colección (a diferencia de `List<T>`). Esto es especialmente importante en MVVM, ya que queremos que nuestra interfaz de usuario se actualice cuando se actualice esta colección. Digamos que un usuario refresca para obtener más datos y se agregan nuevos gastos; queremos asegurarnos que la interfaz de usuario se actualiza cuando se agregan nuevos elementos a la colección. Utilizando `ObservableCollection <T>`, obtendremos este comportamiento.

Agrega una nueva colección a la clase para almacenar nuestros gastos e inicialízala en el constructor.

>**Nota**: Los desarrolladores que utilizan Visual Studio pueden notar que los namespaces se escriben según la heurística de ProjectName.FolderName. Spent puede requerir que agruegue mas namespaces debido a esto. Por ejemplo, cuando hace referencia al modelo `Expense` en` ExpensesViewModel`, puede que tengas que agregar un using adicional para `Spent.Models`.

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
Ahora que tenemos una colección para almacenar los gastos de nuestros usuarios, vamos a añadir un método para obtener los gastos del usuario.


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

El método `GetExpensesAsync` es un gran ejemplo de boilerplate para todos los métodos del modelo de vista. Primero, comprobamos si `IsBusy` es `true` (en cuyo caso no necesitamos repetir la operación). Si no es así, establece `IsBusy` en `true` y después ejecuta la lógica de nuestro método. Si hay algún problema, reporta esa excepción al usuario a través del `MessengingCenter` (que cubriremos más adelante en este módulo). Por último, asegúrate que `IsBusy` está configurado como ` false` de nuevo para que pueda continuar con nuevas operaciones en el modelo de vista.

Vamos a añadir algunos datos de usuario simulados a nuestro método dentro del bloque `try` de `GetExpensesAsync`.

```csharp
Expenses.Clear();
Expenses.Add(new Expense { Company = "Walmart", Description = "Always low prices.", Amount = "$14.99", Date = DateTime.Now });
Expenses.Add(new Expense { Company = "Apple", Description = "New iPhone came out - irresistable.", Amount = "$999", Date = DateTime.Now.AddDays(-7) });
Expenses.Add(new Expense { Company = "Amazon", Description = "Case to protect my new iPhone.", Amount = "$50", Date = DateTime.Now.AddDays(-2) });
```

Let's call `GetExpensesAsync` in the constructor to ensure that data is added to our `Expenses` property.

Genial! Ahora tenemos datos cargándose en nuestro modelo de vista. Pero ¿y si el usuario quiere actualizar la vista? En este momento, la única forma de cargar más datos es reiniciar la aplicación, ya que el único lugar en el que cargamos los gastos está en el constructor del modelo de vista. Si el enlace de datos es el flujo de datos entre el modelo de vista y de vista, **Los Commandos** son el flujo de eventos. Añadamos un nuevo `Command` que podemos llamar desde nuestra lógica de interfaz de usuario para actualizar los datos.

```csharp
public Command GetExpensesCommand { get; set; }
```

Vamos a inicializar el comando en nuestro constructor.

```csharp
GetExpensesCommand = new Command(
    async () => await GetExpensesAsync());
```

Ahora tenemos un `ExpensesViewModel` completo que carga una lista de `Expenses` y también permite al usuario actualizar la lista llamando a nuestro `GetExpensesCommand`. Vamos a construir nuestra interfaz de usuario!

##### 4. Agregar página de gastos.
Las interfaces de usuario en Xamarin.Forms se construyen utilizando C# o XAML. Aunque hay ventajas e inconvenientes para cada enfoque, XAML nos ayuda a implementar mejor el patrón MVVM y mantener una separación de nuestro modelo de vista y lógica de vista. También ayuda en la visualización del árbol visual que puede ser un poco más difícil de hacer al definir nuestras interfaces de usuario en C#.

Vamos a agregar una página para mostrar nuestros gastos. Haz clic con el botón derecho sobre la carpeta `Views`, haz clic sobre `Add -> New File`, añade `Forms -> Forms ContentPage Xaml` si utilizas Xamarin Studio o `Cross-Platform -> Forms Xaml Page` si utilizas Visual Studio llámalo `ExpensesPage`. Se añadirán dos archivos: `ExpensesPage.xaml` para definir nuestra interfaz de usuario y` ExpensesPage.xaml.cs` (nuestro "codebehind") para conectar nuestra vista a nuestro modelo de vista.

Debido a que estamos mostrando una lista de elementos, el control que tiene más sentido aquí es el control [`ListView`] (https://developer.xamarin.com/guides/xamarin-forms/user-interface/listview/). Añade un `ListView` entre los elementos` ContentPage.Content` en XAML.

```xml
<?xml version="1.0" encoding="UTF-8"?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" x:Class="Spent.ExpensesPage">
	<ContentPage.Content>
		<ListView>

		</ListView>
	</ContentPage.Content>
</ContentPage>
```

A continuación, vamos a configurar nuestro `ListView` para usar el `ObservableCollection<Expense>` que definimos en nuestro `ExpensesViewModel`. Para ello, necesitamos definir la propiedad `ItemsSource` del `ListView`.

```xml
<ListView ItemsSource="{Binding Expenses}">

</ListView>
```

Esto significa que todos los datos mostrados en el `ListView` están "enlazados" al objeto `Expenses` en nuestro modelo de vista. Ahora que nuestro `ListView` sabe qué datos mostrar, necesitamos decirle cómo mostrar los `Expenses` individuales. Podemos hacerlo definiendo la propiedad `ListView.ItemTemplate`.

```xml
<ListView ItemsSource="{Binding Expenses}">
	<ListView.ItemTemplate>
		<DataTemplate>

		</DataTemplate>
	</ListView.ItemTemplate>
</ListView>
```

El código anterior sirve para definir las celdas individuales dentro de un `ListView`. Xamarin.Forms viene con varias [celdas pre-construidas] (https://developer.xamarin.com/guides/xamarin-forms/controls/cells/) para que puedas aprovecharlas (`TextCell`, `ImageCell` y `SwitchCell`), pero también puedes crear tus propias celdas. Aprovechemos el `TextCell`preconfigurado para mostrar nuestros datos de `Expense`.

```csharp
<ListView ItemsSource="{Binding Expenses}">
	<ListView.ItemTemplate>
		<DataTemplate>
			<TextCell Text="{Binding Company}" Detail="{Binding Amount}" />
		</DataTemplate>
	</ListView.ItemTemplate>
</ListView>
```

Esto define un `ListView` relleno de datos del objeto `Expenses`. Cada celda es un "gasto" individual, por lo que podemos enlazar datos directamente a las propiedades de ese gasto (`Empresa`,` Descripción`, `Cantidad`, etc.). Pero esto plantea una pregunta importante - ¿cómo sabe la vista a qué objetos enlazarse? Para que nuestra vista sepa dónde se pueden encontrar los objetos vinculados, necesitamos establecer el `BindingContext` para la página. El `BindingContext` es donde Xamarin.Forms buscará los objetos que están enlazados desde la interfaz de usuario. Para configurar esto, todo lo que necesitamos hacer es ir a `ExpensesPage.xaml.cs` y agregar la siguiente línea a nuestro constructor.


```csharp
public ExpensesPage()
{
	InitializeComponent();

	BindingContext = new ExpensesViewModel();
}
``` 

Idealmente, la única lógica escrita en el archivo codebehind debería ser la configuración del `BindingContext`. Lo último que tenemos que hacer es actualizar `App.xaml.cs` para cambiar la propiedad `MainPage` a nuestra nueva `ExpensesPage`.

```csharp
MainPage = new ExpensesPage();
```

Ejecuta la aplicación, y ahora deberías ver una lista de los gastos que se muestran.

 ![](/modules/module-1/images/expenses-list-view.png)

¡Genial! Ahora tenemos un `ListView` con nuestros gastos que recogemos en nuestro modelo de vista. ¿Pero qué pasa si el usuario quiere actualizar estos datos? Un patrón común en el desarrollo móvil cuando se trabaja con `ListView`s es el [pull-to-refresh pattern] (https://developer.xamarin.com/guides/xamarin-forms/user-interface/listview/interactivity/#Pull_to_Refresh). Por suerte para nosotros, esto ya esta construido en Xamarin.Forms; Lo único que tenemos que hacer es configurar algunas propiedades en nuestro `ListView`!

Volviendo a `ExpensesPage.xaml`, añadamos los siguientes atributos a nuestro` ListView` para activar el patrón de pull-to-refresh.

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

Vamos detenernos para ver las propiedades que acabamos de añadir:

* `IsPullToRefreshEnabled`: Define si permitimos al usuario refrescar el `ListView`. Si tu contenido requiere actualización, debes establecer este valor en `true`.
* `IsRefreshing`: Define si el `ListView` está en proceso de actualización. Podemos enlazar datos a la propiedad `IsBusy`. Establezcamos `Mode` a `OneWay`, lo que garantiza que este valor sólo se puede actualizar desde nuestro modelo de vista a nuestra vista, no de vista a modelo de vista (es decir, `TwoWay`).
* `RefreshCommand`: Define el `Command` a ser utilizado para realizar la lógica de la actualización. Cuando se produce la acción pull-to-refresh, este `Command` se ejecuta.

Vuelve a ejecutar Spent, y ahora deberás ser capaz de actualizar para cargar datos!

##### 5. Añadir página de detalles de gastos.
La mayoría de `ListView`s también permiten a los usuarios hacer clic en las celdas individuales para ver una pantalla de detalle con más información sobre el objeto particular de la celda. Esto se conoce como **Navegación Maestro / Detalle** y es un patrón muy común en el desarrollo móvil. Replicar este tipo de comportamiento es extremadamente fácil con Xamarin.Forms.

Comenzamos agregando un nuevo XAML de Xamarin.Forms `ContentPage` a la carpeta `Views` llamado `ExpenseDetailPage`. Dentro de esta página, mostraremos todas las propiedades del objeto `Expense`, incluyendo la foto del recibo. Ya que usaremos más de una vista en esta página (a diferencia de ExpensesPage), tendremos que usar uno de los diseños de Xamarin.Forms para diseñar nuestros controles. El diseño más sencillo y más común es el [`StackLayout`] (https://developer.xamarin.com/api/type/Xamarin.Forms.StackLayout/), que define una pila de controles en orientación vertical u horizontal. Añadamos un nuevo `StackLayout` con un 'Padding` de `20` para asegurar que nuestras vistas dentro del diseño no estén demasiado cerca de la izquierda, arriba, derecha o abajo de la pantalla.

```csharp
<?xml version="1.0" encoding="UTF-8"?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" x:Class="Spent.ExpenseDetailPage">
	<ContentPage.Content>
		<StackLayout Padding="20">

		</StackLayout>
	</ContentPage.Content>
</ContentPage>
```

A continuación, usemos los controles `Label` e `Image` en Xamarin.Forms para mostrar al usuario información detallada de su gasto.

```xml
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

Ten en cuenta que estamos realizando enlaces de datos aquí para mostrar los datos de los gastos. Podemos acceder a las propiedades de los objetos en nuestro `BindingContext` utilizando la sintaxis de punto, como si queriéramos acceder a los valores de estas propiedades en C#. Al igual que con nuestro `ExpensesPage`, necesitamos proporcionar un `BindingContext` para que nuestro enlazado de datos funcione correctamente. Abre `ExpenseDetailPage.xaml.cs`, añade una propiedad llamada `Expense` y actualiza el constructor para que pase un `Expense` como parámetro, establezca la propiedad `Expense` en este parametro y establezca `BindingContext` en `this`.

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

¡Genial! Ahora es el momento de implementar la navegación desde nuestro `ListView` a nuestra página de detalles. Podemos hacerlo utilizando la propiedad `SelectedItem` del `ListView`. Actualiza `ExpensesPage.xaml` enlazar los datos de la propiedad `ListView.SelectedItem` a `SelectedExpenseItem`.

```xml
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

Cuando cambia el `SelectedItem`, podemos navegar hasta la vista de detalle del `SelectedItem`. Abre `ExpensesViewModel` y agrega una propiedad llamada `SelectedExpenseItem`.

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

En este código, estamos actualizando el `SelectedExpenseItem`, y disparando `OnPropertyChanged`. Si el elemento seleccionado no es nulo, queremos navegar a la página de detalles. Finalmente, establezcamos el valor `null` para eliminar cualquier resaltado de celda que suceda cuando un usuario presiona la celda.

Para manejar la navegación, necesitaremos una referencia a `Page`. Añadamos un campo de nivel de clase llamado `ExpensesPage`, añadamos un parámetro al constructor de `ExpensesViewModel` para obtener un `ExpensesPage`, y este lo igualamos a nuestro nuevo campo `ExpensesPage`.

```csharp
ExpensesPage page;
public ExpensesViewModel(ExpensesPage expensesPage)
{
	page = expensesPage;
	...
}
```

A continuación, actualicemos nuestra propiedad `SelectedExpenseItem` para navegar a ella cuando cambien el valor.

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

También tenemos que actualizar `Expenses.xaml.cs` para pasarle `this` como un valor al constructor de `ExpensesViewModel`.

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

Es posible que hayas notado que estamos utilizando una propiedad `Navigation` de la página para redirigir a una nueva página de detalles en la pila de navegación. En este momento, `ExpensesPage` es un `ContentPage`, que no contiene ninguna capacidad de navegación. Para obtener esta habilidad, debemos envolver nuestro `ExpensesPage` en un [`NavigationPage`] (https://developer.xamarin.com/api/type/Xamarin.Forms.NavigationPage/) para obtener la capacidad de hacer push / pop y la navegación modal. Ve a `App.xaml.cs` y actualiza el `MainPage`:

```csharp
MainPage = new NavigationPage(new ExpensesPage());
```

Al hacer esto, nuestra propiedad `ExpensesPage.Navigation` ya está disponible para su uso. Además, usando `NavigationPage`, obtendremos una bonita barra de navegación en la parte superior de todas nuestras páginas para informar a los usuarios sobre la página en la que se encuentran y retroceder (con un botón "Atrás" ). Para ver el título en la barra de navegación, podemos actualizar la propiedad `Title` de cada página al valor correspondiente.

`**ExpensesPage**`
```xml
<?xml version="1.0" encoding="UTF-8"?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" x:Class="Spent.ExpensesPage"
		Title="Expenses">
...
</ContentPage>

```

`**ExpenseDetailPage**`
```xml
<?xml version="1.0" encoding="UTF-8"?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" x:Class="Spent.ExpenseDetailPage"
		Title="Expense Detail">
...
</ContentPage>
```

Ahora, ejecuta la aplicación, haz clic sobre una celda de gastos y te llevará a una página de detalles con más información sobre el gasto.

 ![](/modules/module-1/images/expenses-detail-view.png)

##### 6. Navegación con el Centro de mensajería.
En este momento, tenemos un flujo de navegación de maestro-detalle que muestra una lista de gastos, así como información detallada sobre cada gasto. Podemos limpiar esto para mejorarlo y reducir el acoplamiento entre nuestras vistas y modelos de vista.

Xamarin.Forms [`MessagingCenter`] (https://developer.xamarin.com/guides/xamarin-forms/messaging-center/) permite a los modelos de vista y otros componentes comunicarse sin tener que saber nada acerca el uno del otro aparte de un simple mensaje contrato. El `MessagingCenter` tiene dos partes principales:

1. **Subscribe**: Escucha los mensajes y realiza alguna acción cuando los reciba. Muchos suscriptores pueden estar escuchando el mismo mensaje.
2. **Send**: Publica un mensaje para que los oyentes puedan actuar. Si no hay suscriptores suscritos, entonces el mensaje será ignorado.

En lugar de pasar un `Page` para manejar la navegación, ¿qué pasa si usamos el ` MessagingCenter` de Xamarin.Forms para manejar esto? Actualizemos nuestra aplicación actual para utilizar este enfoque.

Comencemos por deshacer algunos de los "daños" que hemos hecho! Abre `ExpensesViewModel` y elimina el parámetro del constructor, así como el campo `ExpensesPage` y todas las referencias a él en `SelectedExpenseItem`. Ve a `ExpensesPage.xaml.cs` y actualiza la inicialización de `ExpensesViewModel` para que no tenga parámetros.

A continuación, abramos de nuevo `ExpensesViewModel` y enviemos nuestro primer mensaje! Enviemos un mensaje usando la siguiente firma de método `MessagingCenter.Send (TSender sender, string message, TArgs args)`. 

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

Ten en cuenta que pasamos `this` como el remitente, "NavigateToDetail" como el mensaje, y nuestro objeto `Expense` seleccionado como un argumento. Enviar un mensaje es genial, pero si nadie está escuchando los mensajes, no pasará nada. Vuelve a `ExpensesPage.xaml.cs` y añade dos nuevos métodos: `SubscribeToMessages` y `UnsubscribeFromMessages`. Además, reemplaza los métodos de ciclo de vida `OnAppearing` y `OnDisappearing` de nuestro `ContentPage` para llamar a `SubscribeToMessages` y `UnsubscribeFromMessages`.

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

Es importante que nos suscribamos y demos de baja de los mensajes del `MessagingCenter` para evitar referencias nulas y posibles pérdidas de memoria. Suscribámonos al mensaje enviado desde nuestro modelo de vista en `SubscribeToMessages`.

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

Esto nos suscribe a los mensajes con la cadena "NavigateToDetail" proviniente de `ExpensesViewModel` con argumento(s) `Expense`. Si recibimos un mensaje, primero verificamos que el argumento no es nulo, luego navegamos a nuestra página de detalles.

Podemos darnos de baja fácilmente en `UnsubscribeFromMessages`.

```csharp
MessagingCenter.Unsubscribe<ExpensesViewModel, Expense>(this, "NavigateToDetail");
MessagingCenter.Unsubscribe<ExpensesViewModel, string>(this, "Error");
```

Ejecuta la aplicación, la navegación seguirá funcionando según lo previsto, pero esta vez estamos evitando adecuadamente el acoplamiento estrecho entre nuestro modelo de vista y nuestra vista.

#### Closing Remarks
En este módulo, has aprendido sobre los conceptos básicos de la creación de aplicaciones con Xamarin.Forms, incluyendo la creación de interfaces de usuario en XAML, navegación, MVVM, enlace de datos y comandos, así como el uso del `MessagingCenter`. En el siguiente módulo, incluiremos los últimos detalles a nuestra aplicación Xamarin.Forms antes de conectarla a la nube en los módulos 3 y 4.