# Modulo 4: Guardando Blobs en el Azure Storage
**Objetivo**: Aprende lo básico del almacenamiento de Azure Storage , de las Azure Functions y cómo están relacionados con el desarrollo de aplicaciones móviles.

##### Prerequisitos
Asegúrate de que tienes el siguiente software instalado:

* Visual Studio 2015 Edición Community (o superior) o Xamarin Studio Edición Community (o superior)
* [Xamarin](xamarin.com/download)
* **Opcional**: [SQLite para Universal Windows Platform](https://visualstudiogallery.msdn.microsoft.com/4913e7d5-96c9-4dde-a1a1-69820d615936) - Es necesario para usar Azure Mobile Apps en aplicaciones Universal Windows Platform (UWP).

Este módulo usa Azure, así que asegúrate antes de empezarlo de que o bien tienes una [free trial (con 170€ en créditos)](https://azure.microsoft.com/es-es/offers/ms-azr-0044p/) o cuentas ya con una cuenta de Azure.

Bájate el código de inicio para empezar este módulo, o continúa trabajando con el código ya completado del Módulo 3.

### Instrucciones del Módulo
En este módulo trataremos con el Azure Storage, que se usará para almacenar imágenes de recibos en Spent.

##### 1. Crea un Azure Storage bucket.
[Azure Storage](https://azure.microsoft.com/es-es/services/storage/) es un servicio ofrecido por Microsoft que proporciona almacenamiento cloud escalable para datos desestructurados (por ejemplo blobs), archivos, tablas NoSQL y colas. El almacenamiento de tipo Blob es un concepto importante para los desarrolladores de aplicaciones móviles, ya que la mayor parte de las apps móviles presentan algún tipo de datos que deben ser almacenados fuera de la típica tabla de almacenamiento (como una imagen o un archivo).

Vamos a usar el Azure Storage para almacenar las imágenes de recibos, subidas por los usuarios. Al igual que con cualquier otro servicio cloud de Microsoft, crearemos un nuevo Azure Storage desde[portal.azure.com](https://portal.azure.com). Haz click en `Nuevo` en la barra lateral, busca `Azure Storage`, selecciona la opción `Storage Account` y presiona `Crear`.

Te aparecerá el menú de `Crear una cuenta de almacenamiento`.  Configura tu cuenta Azure Storage completando el menú con la siguiente información:

* `Nombre`: Debe ser único ya que es una URL.
* `Modelo de implementación`, selecciona `Resource manager`.
* `Tipo de cuenta`, selecciona `Blob storage`. Aquí creamos una cuenta de almacenamiento especializada sólo para blobs que nos permita seleccionar más opciones de almacenamiento que aplicables únicamente a blobs.
* `Rendimiento`, selecciona `Estándar`.
* `Replicación`, selecciona `Almacenamiento con redundancia geográfica con acceso de lectura (RA-GRS)`. De esta forma replicamos nuestro almacenamiento en varias regiones que no están cerca de tu región más próxima. Esto es ideal cuando tienes usuarios que no se centran en una localización en particular.
* `Cifrado del Servicio Storage`, selecciona `Deshabilitado`. Deberías seleccionar `Habilitado` si tienes documentos comprometidos o información personal.
* `Grupo de recursos`, presiona `Usar existente`, y selecciona el grupo de recursos creados en el Módulo 3.
* `Ubicación`, selecciona el que tengas más cerca. Lo normal es seleccionar el mismo valor que en el Módulo 3.

 ![](/modules/module-4/images/create-storage-account.png)

Haz click en `Crear`para crear una cuenta de Azure Storage especializada en blobs. El despliegue debería llevar un minuto o dos.

Una vez desplegado, haz click en `Claves de Acceso`, que encontrarás en la barra lateral de la cuenta de almacenamiento. Copia el `Nombre de la cuenta de almacenamiento`, así como el valor de `key1`. Guarda esto para después, ya que lo usaremos para conectar nuestra app a la cuenta de almacenamiento.

 ![](/modules/module-4/images/access-keys.png)

##### 2. Crea un contenedor de almacenamiento.
Ahora que tenemos una cuenta de almacenamiento, necesitamos una manera de almacenar blobs en la cuenta. Los blobs tienen que esar almacenados en **contenedores**, que son como carpetas para los blobs. Vamos a crear un contenedor. Ve a la cuenta de almacenamiento dentro del grupo de recursos creado en el Módulo 3. Haz click en cuenta de almacenamiento, seguido de `+ Contenedor`. Te aparecerá el menú de `Nuevo Contenedor`.

 ![](/modules/module-4/images/create-container.png)

Nombra al contenedor `receipts`, selecciona el tipo de acceso `Blob`que permita acceso de lectura a nuestros blobs, y haz click en `Crear`. Aparecerá un nuevo container llamado `receipts`en la lista de contenedores, junto con una URL para acceder al contenedor.

 ![](/modules/module-4/images/created-container.png)

##### 3. Almacena recibos en el Azure Storage.
Ahora que hemos establecido la parte de servidor para el almacenamiento de blobs, vamos a actualizar nuestra aplicación Spent para usar Azure Storage en el almacenamiento de fotos recibidas.

Al igual que las Azure Mobile Apps, el Azure Storage tiene un poderoso SDK llamado **[Windows Azure Storage SDK](https://www.nuget.org/packages/WindowsAzure.Storage/)**. Esto nos permitirá subir blobs u otro tipo de datos al Azure Storage de una manera muy sencilla. Si tu aplicación gira entorno al almacenamiento, gracias al SDK puedes aumentar la funcionalidad de sincronizar offline que hay en el SDK del Azure Mobile Client, de manera que puedas almacenar datos en el Azure Storage. Por conveniencia, este NuGet ya se ha añadido a proyectos iOS, Android y Universal Windows Platform.

Abre el `NewExpenseViewModel`, y ve al método `SaveExpenseAsync`. Ahora mismo estamos pasando el `Expense`a nuestro `MessagingCenter`, el cual está suscrito a nuestro `ExpensesViewModel`para añadir nuevos gastos. El problema está en que apenas le pasamos un path de una imagen local al view model; idealmente, le pasaríamos la propia imagen para subirla de forma más fácil. Añade un nuevo campo a nivel de clase para almacenar el `MediaFile`hasta que el usuario quiera guardarlo.


```csharp
MediaFile receiptPhoto;
```

Después, pon el valor en el método `AttachReceiptAsync`.

```csharp
receiptPhoto = photo;
```

Modifiquemos ahora el método `SaveExpenseAsync` para enviar un `object[]` con dos items en el array: el `Expense`y el `MediaFile`, y enviar el array desde el `MessagingCenter`al `ExpensesViewModel`.

```csharp
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

Saltemos ahora al `ExpensesViewModel`. En el constructor del `ExpensesViewModel`, nos suscribimos al mensaje enviado desde el `NewExpenseViewModel`para añadir gastos a nuestro almacenamiento. Aquí es donde añadiremos el código necesario para subir una foto al Azure Storage antes de guardarla en nuestro almacenamiento.

Actualiza la suscripción con el mensaje, para aceptar dos parámetros y lanzar los objetos de vuelta al formato original.


```csharp
using Plugin.Media.Abstraction;
...
MessagingCenter.Subscribe<NewExpenseViewModel, object[]>(this, "AddExpense", async (obj, expenseData) =>
{
    var expense = expenseData[0] as Expense;
	var photo = expenseData[1] as MediaFile;
	Expenses.Add(expense);

    // TODO: Upload photo to Azure Storage.

    await DependencyService.Get<IDataService>().AddExpenseAsync(expense);
});
```

Debemos comprobar que la foto no es un null antes de subirla: quizás el usuario no ha elegido una foto del recibo para subir. En tal caso, no deberíamos subir la foto al almacenamiento. Si la foto no es null, subimos la imagen.


```csharp
...
if (photo != null)
{

}
...
```

> Nota: Hay muchas maneras de usar el Windows Azure Storage SDK para subir blobs. La siguiente es una de las diferentes formas que podrías implementar.

Para empezar, crearemos un nuevo `CloudStorageAccount` parseando un string de conexión a nuestra cuenta de almacenamiento. Es importante que te des cuenta de que **no** debes usar strings de conexión en aplicaciones para producción, sólo cuando se testea localmente. Para aplicaciones listas para producción debes crear un API endpoint que nos proporcione Shared Access Signature tokens para usuarios bajo demanda. No solo esta opción es más segura, sino que puedes hacer cambios de permisos en el almacenamiento sin necesidad de actualizar tu aplicación - deberías actualizar el API endpoint. Para más información sobre esta acción visita el apartado del curso "Learn Azure".

La conexión por string se forma sustituyendo en tu `Nombre de cuenta` y `Clave de cuenta` del paso #2 en los lugares apropiados en el siguiente string.

```csharp
DefaultEndpointsProtocol=https;AccountName={INSERT-ACCOUNT-NAME-HERE};AccountKey={INSERT-ACCOUNT-KEY-HERE}
```

Cuando ya lo hayamos construido, añade la siguiente linea de código para crear un objeto `CoudStorageAccount` y un `CloudBlobClient` para subir los blobs. (Asegúrate de meter tu propio string de conexión compuesto por tu nombre y clave de cuenta). 

```csharp
using Microsoft.WindowsAzure.Storage;
...
// Connect to the Azure Storage account.
// NOTE: You should use SAS tokens instead of Shared Keys in production applications.
var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=spentappstorage;AccountKey=n/o1HFFL3VqDI9RweXG/xnKvWMFiaq1Giw/htpCESjHILPoBx2VAvf/iAcGDh5D/+0GIaSTT9TT9OxWTIlshYA==");
var blobClient = storageAccount.CreateCloudBlobClient();
```

A continuación sacaremos una referencia del contenedor `receipts` creado anteriormente.

```csharp
// Create the blob container if it doesn't already exist.
var container = blobClient.GetContainerReference("receipts");
await container.CreateIfNotExistsAsync();
```

¡Es el momento de subir nuestro blob! Crearemos un nuevo `BlockBlob` con un nombre aleatorio generado desde un `Guid`. Después convertiremos el `MediaFile` a un `System.IO.Stream`, y subiremos la imagen. Por último, podemos anidar la propiedad `Expense.Receipt` en la URL de tu recibo subido al Azure Storage.


```csharp
// Upload the blob to Azure Storage.
var blockBlob = container.GetBlockBlobReference(Guid.NewGuid().ToString());
await blockBlob.UploadFromStreamAsync(photo.GetStream());
expense.Receipt = blockBlob.Uri.ToString();
```

Ahora la suscripción al mensaje "AddExpense"  debe parecer algo parecido a lo siguiente.

```csharp
MessagingCenter.Subscribe<NewExpenseViewModel, object[]>(this, "AddExpense", async (obj, expenseData) =>
{
	var expense = expenseData[0] as Expense;
	var photo = expenseData[1] as MediaFile;
	Expenses.Add(expense);

	// TODO: Upload photo to Azure Storage.
	if (photo != null)
	{
		// Connect to the Azure Storage account.
		// NOTE: You should use SAS tokens instead of Shared Keys in production applications.
		var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=spentappstorage;AccountKey=n/o1HFFL3VqDI9RweXG/xnKvWMFiaq1Giw/htpCESjHILPoBx2VAvf/iAcGDh5D/+0GIaSTT9TT9OxWTIlshYA==");
		var blobClient = storageAccount.CreateCloudBlobClient();

		// Create the blob container if it doesn't already exist.
		var container = blobClient.GetContainerReference("receipts");
		await container.CreateIfNotExistsAsync();

		// Upload the blob to Azure Storage.
		var blockBlob = container.GetBlockBlobReference(Guid.NewGuid().ToString());
		await blockBlob.UploadFromStreamAsync(photo.GetStream());
		expense.Receipt = blockBlob.Uri.ToString();
	}

	await DependencyService.Get<IDataService>().AddExpenseAsync(expense);
});
```

¡Genial! Ahora ejecuta la app, crea un nuevo gasto, adjunta un recibo y guarda la foto. Si quieres, abre la aplicación en otro simulador, guarda los cambios y observa que la imagen se baja y se muestra en la pagina de detalles de gastos.
