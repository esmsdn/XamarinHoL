# Módulo 3: Desarrollando aplicaciones conectadas con Azure Mobile Apps
**Objetivo**: Construye tu primera aplicación móvil conectada con Azure Mobile App y Xamarin.

##### Prerequisitos
Asegúrate de que tienes el siguiente software instalado

* Visual Studio 2015 edición Community (o superior) o Xamarin Studio edición Community (o superior)
* [Xamarin](xamarin.com/download)
* **Opcional**: [SQLite para Universal Windows Platform](https://visualstudiogallery.msdn.microsoft.com/4913e7d5-96c9-4dde-a1a1-69820d615936) - Requerido para el uso de las Azure Mobile Apps en aplicaciones Universal Windows Platform (UWP).

Este módulo usa Azure, así que asegúrate antes de empezarlo de que o bien tienes una [free trial (con 170€ en créditos)](https://www.visualstudio.com/dev-essentials/) o cuentas ya con una cuenta de Azure. [Aquí](https://github.com/esmsdn/Workshops/blob/master/MicrosoftAzure/Microsoft%20Azure%20con%20Dev%20Essentials.md) tienes una guía de activación para tu cuenta Dev Essentials.

Bájate el código de inicio para empezar este módulo, o continúa trabajando con el código ya completado del Módulo 2.

### Instrucciones del módulo
En este módulo aprenderás lo básico para desarrollar aplicaciones móviles conectadas con Azure Mobile Apps y Xamarin y configuración del backend, para habilitar la sincronización online/offline de nuestras aplicaciones móviles.

##### 1. Crear una nueva Azure Mobile App.
##### Crear un Grupo de Recursos
Para empezar, visita [portal.azure.com](https://portal.azure.com). El [Portal de Azure](https://azure.microsoft.com/es-es/features/azure-portal/) proporciona una interfaz para crear nuevos servicios cloud, desde backends para móviles, análisis de big data y machine learning hasta manejo de identidades con Azure Active Directory. El portal se abre en una página llamada **Dashboard**, que es como el "Home" para todos tus servicios de Azure. Para facilitar el acceso, puedes crear accesos directos a tus servicios en el dashboard. En la parte izquierda del portal vas a encontrar una barra que contiene links a varios servicios cloud, y también un botón de "Nuevo", que permite crear nuevos recursos.

Para ayudar a organizar los recursos (como por ejemplo el backend de la app móvil), nos podemos aprovechar de los **[Grupos de recursos](https://docs.microsoft.com/es-es/azure/azure-resource-manager/resource-group-portal)** de Azure. Un grupo de recursos es como una carpeta, en la cual ponemos los recursos cloud que se tienen que agrupar en el mismo directorio para facilitar el acceso y manejo de los mismos. Lo normal es crear un nuevo grupo de recursos para cada aplicación móvil que hagas.

En la parte izquierda del portal, haz click en el botón `Nuevo`. Una nueva **[sección](https://docs.microsoft.com/es-es/azure/app-service-web/app-service-web-app-azure-portal)**, o ventana de navegación, aparecerán los servicios del `Azure Marketplace`. En la ventana de búsqueda, escribe `Grupo de recursos` y presiona `Enter`. En la lista de resultados que aparecen, selecciona `Grupo de recursos`.

 ![](/modules/module-3/images/create-new-resource-group.png)

Aparecerá una sección de `Grupo de recursos` con los detalles de lo que es un grupo de recursos, junto con el botón de publicar y un link a la documentación. Haz click en el botón `Crear`. Te aparecerá otro menú, para crear un nuevo grupo de recursos. Introduce un nombre en `Nombre del Grupo de recursos`, como por ejemplo "SpentApp". Selecciona una suscripción (aunque normalmente la que viene por defecto es totalmente válida). Finalmente, selecciona una `Ubicación`. Aquí es donde residen todos tus servicios, así que es importante que selecciones la locaclización más cercana a tus clientes. Para un proyecto de prueba como este, selecciona la localización más cercana a tí. Haz click en `Anclar al panel` para crear un acceso directo a este grupo de recursos desde el panel de Azure, y después presiona `Crear`.


 ![](/modules/module-3/images/create-empty-resource-group.png)

Se creará un grupo de recursos y podrás consultar su contenido desde la sección `Grupo de recursos`.

 ![](/modules/module-3/images/resource-group.png)

##### Crea una Azure Mobile App
Ahora que tenemos un contenedor cloud para nuestros servicios, vamos a crear una **Azure Mobile App*.  [Azure Mobile Apps](https://azure.microsoft.com/es-es/services/app-service/mobile/) ofrece escalabilidad, una plataforma de desarrollo para móviles con alta disponibilidad y que con grandes posibilidades para los desarrolladores de aplicaciones móviles, como poder alojar datos en la nube, autenticación de usuarios (tanto en local, redes sociales como de empresa), así como enviar notificaciones push y crear API endpoints. También cuenta con otras características muy interesantes, como la sincronización automática online y offline, escalado automático, entornos de prueba, despliegue contínuo, y redes virtuales.

Para crear una nueva Azure Mobile App, haz click en el botón de `Añadir` desde la parte de arriba del grupo de recursos que acabas de crear y busca `Mobile Apps`.

 ![](/modules/module-3/images/create-azure-mobile-app.png)

Encontrarás varias opciones tras la búsqueda. Las dos más relevantes son `Mobile App` y `Mobile Apps Quickstart`. Para aplicaciones en producción, tendrás que crear una `Mobile App`. Esta opción te provee de todas las funcionalidades que ofrece el recurso Azure Mobile App, incluyendo almacenamiento de datos con SQLServer y la posibilidad para construir backends en Node.js o .NET. Para probar aplicaciones e ir empezando, podemos usar la [`Mobile Apps Quickstart`](https://azure.microsoft.com/es-es/marketplace/partners/microsoft/trymobileappsnode/), que crea un backend en Node.js con un almacenamiento SQLite que no requiere de configuración adicional.

> Para más detalle acerca de las Azure Mobile Apps, incluyendo cómo construir backends en .NET para aplicaciones móviles, revisa el apartado "Aprender de Azure" del curso de GitHub.

Haz click en `Mobile Apps Quickstart`, y selecciona `Crear`. Aparecerá la ventana de creación de `Mobile Apps Quickstart`. Ponle un nombre en `Nombre de la aplicación`. Ten en cuenta que este valor tiene que ser único, ya que es una URL. Para el `Plan de App Service/Ubicación`, haz click en `Crear nuevo`. Aparecerá una nueva ventana llamada `Nuevo plan de App Service`. Introduce un nombre para tu plan, una localización, y selecciona un `Plan de tarifa`. Cualquier plan de tarifa funciona bien (incluido el Gratis), pero mi recomendación es que selecciones el `Básico`. Para aplicaciones de producción, deberías elegir al menos la tarifa `Estándar`, que contiene características muy útiles que te servirán para el backend de la aplicación móvil, incluyendo herramientas de puesta en marcha para probar los cambios del backend, escalado automático hasta 10 instancias, backups diarios y georeplicación para tener backends más rápidos.

Selecciona el plan que más te convenga y haz click en el botón `Create`. De esta forma empezará el **despliegue** 

 ![](/modules/module-3/images/create-new-mobile-app.png)

Puedes seguir el progreso del despliegue haciendo click en la campana de notificación en la esquina superior derecha. Date cuenta que este proceso puede llevar desde cualquier sitio, entre 2 y 5 minutos, ya que están ocurriendo muchos eventos de background (despliegue de un website, configuración de la base de datos, etc.).

 ![](/modules/module-3/images/deployment-progress.png)

Cuando se despliegue con éxito, vuelve al grupo de recursos, haciendo click en `Grupos de recursos` y después selecciona tu grupo de recursos dentro de la barra del lateral. Se han desplegado dos ítems en tu grupo de recursos, un plan de App Service y un App Service.

##### 2. Configura las Azure Easy Tables.
Haz click en la App Service para ver los detalles de tu Azure Mobile App. Desde esta sección podrás configurar los parámetros de la Azure Mobile App, como el almacenamiento de los datos, las notificaciones push y la autenticación. También puedes monitorizar el uso de la aplicación móvil para investigar posibles fallos en el backend.

 ![](/modules/module-3/images/azure-mobile-app.png)

En la barra de búsqueda, escribe `Tablas fáciles`. Las tablas fáciles son una característica de `Azure Mobile App`, que  nos permite crear un backend sin escribir código alguno. Podemos proveer de un esquema a la tabla y la Tabla Fácil generará automáticamente un API endpoint y manejará todos los datos por nosotros.

> Las tablas fáciles son una buena forma de almacenar datos no relacionales. Si tu aplicación móvil necesita datos relacionales, lo mejor es crear una Azure Mobile App en .NET. Para aprender cómo hacer esto, visita el apartado "Learn Azure".

Para crear una tabla, haz click en el botón `Añadir`. En el `Nombre`de la tabla introduce `Expense`. Si configuras tu autenticación para tu `Mobile App`, puedes también manejar permisos. Para este taller vamos a habilitar la opción de crear-leer-actualizar-eliminar(CRUD en inglés) el acceso a todo el mundo, dejando por defecto el permiso `Permitir acceso anónimo`. Después de que se haya creado la tabla, seleciona la tabla `Expese`. Desde esta sección, puedes ver los datos ya creados, actualizar los permisos de la tabla, añadir scripts a tu API endpoint, así como manejar el esquema de la tabla. Haz click en `Manejar esquema`, seguido de `Añadir columna`.

 ![](/modules/module-3/images/manage-schema.png)

Para `Nombre de columna`, introduce `company`. Para `Tipo de datos`, selecciona `String`. Repite este proceso para las siguientes propiedades en nuestro modelo `Expense`:

 * `description`: String
 * `amount`: String
 * `receipt`: String
 * `date` : Date

 ![](/modules/module-3/images/schema.png)

¡Acabamos de terminar con la configuración del backend de la Azure Mobile App, sin escribir código, gracias a las tablas fáciles!.

##### 3. Conecta Spent a la nube.
Ahora que tenemos una funcionalidad en el backend, conectemos el Spent a la nube.

Si te diste cuenta antes, teníamos varias columnas adicionales que se añadieron automáticamente cuando creamos una tabla fácil. Estas columnas son usadas para ayudar a la sincronización offline de la Azure Mobile App y la resolución de conflictos. Vamos a crear una nueva clase llamada `EntityData`en la carpeta de modelos para poder visualizar estas columnas.

```csharp

using System;

using Microsoft.WindowsAzure.MobileServices;

namespace Spent
{
	public class EntityData
	{
		public EntityData()
		{
			Id = Guid.NewGuid().ToString();
		}

		public string Id { get; set; }

		[CreatedAt]
		public DateTimeOffset CreatedAt { get; set; }

		[UpdatedAt]
		public DateTimeOffset UpdatedAt { get; set; }

		[Version]
		public string AzureVersion { get; set; }
	}
}
```

Lo siguiente es actualizar nuestro modelo `Expense` a la subclase `EntityData`. Todos los modelos que queremos conectar a la nube deben tener `EntityData`como clase base.

Ahora que nuestros modelos están configurados, haz click derecho en `Servicios`, y añade un nueva clase vacía en C# llamada `AzureDataService`. Implementaremos la interfaz `IDataService`. Con esta acción escribirás los mínimos cambios de código posible en la aplicación para proveerla del nuevo servicio de almacenamiento de datos en la nube. Implementa la interfaz `IDataService`y añade un método llamado `Initialize`.

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
...
public class AzureDataService : IDataService
{
	public AzureDataService()
	{

	}

	async Task Initialize()
	{

	}

	public async Task AddExpenseAsync(Expense ex)
	{
		throw new NotImplementedException();
	}

	public async Task<IEnumerable<Expense>> GetExpensesAsync()
	{
		throw new NotImplementedException();
	}
}
```
Implementaremos este servicio usando el **[Azure Mobile Client SDK](https://github.com/Azure/azure-mobile-apps-net-client)**. Este SDK es extremadamente potente; tiene mucha "magia" por detrás. No nos tenemos que preocupar de formar las peticiones HTTP, configurando las cabeceras de autenticación o recogiendo nuestros datos - el SDK maneja todos eso por nosotros. El Azure Mobile Client SDK se ha escrito por y para desarrolladores .NET, y por tanto la API es sencilla de usar y se aprovecha de las características que tiene C# como async/wait. El SDK se distribuye mediante dos paquetes NuGet: el [Azure Mobile Client SDK](https://www.nuget.org/packages/Microsoft.Azure.Mobile.Client/) y el [Azure Mobile Client SDK SQLiteStore](https://www.nuget.org/packages/Microsoft.Azure.Mobile.Client.SQLiteStore/). El paquete SQLiteStore nos permite tener una aplicación funcional incluso si el usuario pierde conectividad con la sincronización online/offline. Empecemos trayendo estos namespaces dentro del `AzureDataService`.


```csharp
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
```

La mayor parte de la comunicación se lleva a cabo mediante la clase `MobileServiceClient`, que permite manejar la comunicación con nuestra Azure Mobile App. Crea una nueva propiedad pública que se llame `MobileService` de tipo `MobileServiceClient`.

```csharp
public MobileServiceClient MobileService { get; set; }
```

Inicialicemos esto en el constructor de nuestro `AzureDataService` pasándole la URL del taller. Este código le permite al `MobileServiceClient` saber la URL base para todas las peticiones de la API que se tengan que crear.

```csharp
public AzureDataService()
{
    MobileService = new MobileServiceClient("http://spendapplab.azurewebsites.net/", null);
}
```

Al igual que lo hicimos en `MockDataService`, nuestro `AzureDataService` necesitará alguna lógica de inicialización. Empecemos añadiendo dos campos a nivel de clase para hacer un seguimiento del estado de inicialiazación y almacenar una referencia para nuestra tabla de gastos.

```csharp
bool isInitialized;
IMobileServiceSyncTable<Expense> expensesTable;
```

Añade el siguiente código al método `Initialize` para configurar nuestro almacenamiento local.

```csharp
async Task Initialize()
{
	if (isInitialized)
		return;
			
	var store = new MobileServiceSQLiteStore("app.db");
	store.DefineTable<Expense>();
	await MobileService.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());
	expensesTable = MobileService.GetSyncTable<Expense>();

	isInitialized = true;
}
```

Este código comprueba si el `AzureDataService` ya se ha inicializado. Si no fuese así, creamos un nuevo `MobileServiceSQLiteStore`, que es una base de datos local para manejar la sincronización online/offline. Lo siguiente será definir la tabla `Expense`. Finalmente, inicializamos los datos del almacenamiento con un handler síncrono por defecto que maneja por nosotros la resolución de conflictos de forma automática.

La sincronización online/offline necesita un enlace entre los almacenamientos locales y remotos para estar en sincronía. Añade un nuevo método llamado `SyncExpenses`.

```csharp
async Task SyncExpenses()
{
	try
	{
		await MobileService.SyncContext.PushAsync();
		await expensesTable.PullAsync($"all{typeof(Expense).Name}", expensesTable.CreateQuery());
	}
	catch (Exception)
	{
		System.Diagnostics.Debug.WriteLine("An error syncing occurred. That is OK, as we have offline sync.");
	}
}
```

Con este código lo que hacemos primero es subir todos los cambios locales al servidor si todavía no se hubiesen hecho. Lo siguiente que hace es bajarse todos los cambios del servidor que no se encuentran disponibles localmente en el dispositivo. Podemos hacer una query mediante el método `PullAsync`, que debido a que es sync, se bajarán únicamente los últimos cambios y no la tabla entera. ¡Ya estamos listos para escribir código que añada o elimine gastos!

Añade el siguiente código en el método `AddExpenseAsync`. Primero arrancaremos el servicio, después usaremos el campo `expensesTable`, que ya habíamos creado antes para insertar un ítem en el almacenamiento local y finalmente se sincronizarán los datos con el servidor.

```csharp
public async Task AddExpenseAsync(Expense ex)
{
	await Initialize();

	await expensesTable.InsertAsync(ex);
    await SyncExpenses();
}
```

Por último, tenemos que añadir más código al `GetExenseAsync`para sacar todos los gastos tanto de los datos locales como de los almacenados remotamente

```csharp
public async Task<IEnumerable<Expense>> GetExpensesAsync()
{
	await Initialize();

	await SyncExpenses();

	return await expensesTable.ToEnumerableAsync();
}
```

Debido a que estamos usando el `DependencyService`, tenemos que asegurarnos de que sólo está disponible una dependencia para el `IdataService` de manera que encuentre el tipo correcto. Elimina el atributo `MockDataService`y añade el siguiente código en la parte de arriba de `AzureDataService`.

```csharp
using Xamarin.Forms;

[assembly: Dependency(typeof(Spent.AzureDataService))]
```

Ejecuta la aplicación, añade un ítem, y deberías ver el item subido al almacenamiento de la nube en la Azure Mobile App.

¡Baaang! Ya tenemos escrita toda la lógica de nuestro `AzureDataService`. En solo 50 lineas de código estamos guardando y recogiendo los gastos de ambos almacenamientos (local y remoto) y guardándolos de manera síncrona. Corre la aplicación y serás capaz de añadir nuevos items y que estos parezcan en las tablas fáciles del portal.

 ![](/modules/module-3/images/ExpensesApp.png)
