# Módulo 5: Mobile DevOps con Visual Studio Team Services y HockeyApp
**Objetivo**: Aprende el conocimiento necesario para usar Mobile DevOps, incluyendo integración contínua (con Visual Studio Team Services) y despliegue (con HockeyApp), así como testing con Xamarin Test Cloud.

##### Prerrequisitos
Asegúrate de que tienes el siguiente software instalado:

* Visual Studio 2015 Edición Community (o superior) o Xamarin Studio Edición Community (o superior)
* [Xamarin](xamarin.com/download)
* **Opcional**: [SQLite para Universal Windows Platform](https://visualstudiogallery.msdn.microsoft.com/4913e7d5-96c9-4dde-a1a1-69820d615936) - Es necesario para usar Azure Mobile Apps en aplicaciones Universal Windows Platform (UWP).

Bájate el código de inicio para empezar este módulo, o continúa trabajando con el código ya completado del Módulo 4.

### Instrucciones del módulo
En este módulo vas a crear y configurar tu cuenta de Visual Studio Team Services (VSTS) para integración contínua, despliegue contínuo mediante usar HockeyApp desde el Visual Studio, y testeo de tus apps móviles con Xamarin Test Cloud.

##### 1. Crea un repositorio GitHub
GitHub te proporciona un servicio host Git para control de versiones, muy popular entre nosotros, los desarrolladores (Xamarin es open-source en GitHub). Hacen que sea muy fácil alojar mi código, tener un seguimiento de los cambios y establecer revisiones de código. Usaremos GitHub para alojar nuestra app Spent.

Lo primero, [créate una cuenta en GitHub](https://github.com/join) si todavía no tienes una. También bájate la aplicación para escritorio [GitHub para Mac o Windows](https://desktop.github.com/). Esto hará tan fácil como sea posible revisar el código sin tener que cerrar la consola, proporcionando una interfaz de usuario para manejar los repositorios Git.

Crea un nuevo repositorio haciendo click en `+ ->  New respository`. A mano derecha tendrás la barra de navegación de GitHub. Te aparecerá la página `Create a new repository`.

* `Repository name`: Escoge cualquier nombre que te guste, siempre que no sea un conflicto con otros repositorios previos que ya hayas creado.
* `Public / Private`: Lo normal será tener el repositorio `Public`, pero también está la opción de hacerlo `Private`.
* `Initialize this repository with a README` / `Add .gitignore` / `Add a license`: Todos estos son campos opcionales.

 ![](/XmarinSpent/modules/module-5/images/create-repository.png)

Haz click en `Create repository` y se te creará el repositorio. Selecciona `Clone or download -> Open in Desktop` para abrir el repositorio en el cliente GitHub de escritorio.

Arrastra el código desde la carpeta del módulo a la del repopsitorio. Probablemente esté en `Documentos/GitHub/NOMBRE-DE-TU-REPOSITORIO`. Después de que se acabe de copiar, abre el cliente GitHub de escritorio, selecciona tu repositorio e introduce un `Resumen` en el cliente GitHub y haz click en `Commit to master`. Para sincronizar tus cambios con el GitHub, presiona el botón `Sync`.


 ![](/modules/module-5/images/spent-github-client.png)

¡Bien! Ahora que nuestro código está alojado en GitHub, vamos a configurar una cuenta VSTS para proporcionar integración contínua.

##### 2. Crea un acuenta de Visual Studio Team Services.
[Visual Studio Team Services](https://www.visualstudio.com/es/team-services) ofrece un kit de herramientas colaborativas para la nube, como repositorios de código, integración contínua, detección de bugs y planificación. En este módulo usaremos integración contínua. Si ya tienes cuenta de VSTS te puedes saltar el paso #2. 

Visita la [página Visual Studio Team Services](https://www.visualstudio.com/es/team-services) y haz click en `Comience a usarlo de manera gratuita` para empezar.

Empieza intruciendo un nombre de cuenta, y seleccionando cómo te gustaría manejar el control de versiones. Una de las características más usadas de VSTS es la integración con GitHub, si no elegiste usar el control de versión built-in de VSTS. Haz click en el botón `Continue`. Llevará un par de minutos suministrar una cuenta VSTS.


 ![](/modules/module-5/images/create-account.png)

`MyFirstProject` aparecerá en Visual Studio Team Services para explorar todo lo que ofrece VSTS. Ahora crearemos un nuevo proyecto para Spent. Lo podemos hacer presionando el logo de `Team Services` en la esquina superior izquierda. Después, haz click en el botón `New`, debajo de `Recent projects and teams`. Debería aparecer el diálogo `Create team project`.

* `Project name`: Introduce `Spent`.
* `Description`, `Process template`, `Version control`: No se necesita para este módulo, pero si quieres puedes seleccionar la opción que mejor te encaje.

 ![](/modules/module-5/images/create-project.png)

Haz click en `Create project`, deja que se cargue el proyecto, y haz click en `Navigate to project`. Éste es el dashboard de tu proyecto, desde el que tendrás acceso al código, seguimiento de problemas, información de compilación y más.

##### 3. Conecta el Visual Studio Team Services al GitHub.
Visual Studio Team Services trabaja bien con GitHub. Para conectar nuestro código Spent al proyecto VSTS, haz click en el icono de configuración, que se encuentra en la esquina superior derecha de la página.

 ![](/modules/module-5/images/vsts-settings.png)

Podemos añadir GitHub como un nuevo endpoint de servicio bajo la pestaña de los servicios (junto con otra integración de servicios). Haz click en la pestaña `Services` y selecciona `New Service Endpoint -> GitHub`. 

 ![](/modules/module-5/images/add-service-endpoint.png)

Haz click en el botón `Authorize` y autoriza el Visual Studio Team Services para que acceda a tu cuenta. Una vez se hayamos hecho la autenticación con éxito, haz click en `OK`.

Añadiendo este endpoint de servicio, ahora podemos usar GitHub en Visual Studio Team Service.

##### 4. Crea una build definition.
Para este ejemplo configuraremos la integración contínua para nuestra app Spent de Android. Para empezar, vamos a crear un **build definition**. Cn esto le decimos al Visual Studio Team Services cómo proceder con la compilación de un proyecto en particular, desde el reestablecimiento de los NuGets al control de versiones de la app.

Empezamos haciendo click en la pestaña `Build` de la barra de navegación, y después presionamos el botón `New definition`. Nos aparecerá la pantalla de `Create new build definition`. Selecciona la compilación `Xamarin.android` y haz click en `Next`.

 ![](/modules/module-5/images/new-build-definition.png)

Para el `Repository source`, haz click en `GitHub` - `Continuous integration`. Presiona `Create`para crear un build definition.

Vamos ahora a configurar nuestro build definition para bajarnos el código `Spent`de nuestro repositorio en GitHub. Haz click en la pestaña `Repository`. Selecciona el repositorio que acabas de crear desde el desplegable, pon el `Default branch` a `master` y el `Clean` a `true`.

 ![](/modules/module-5/images/configure-repository.png)

Ahora volvemos a la pestaña `Build` desde la que podremos editar, añadir o eliminar pasos en nuestro build definition. Puedes ver que nuestro template añade unos pocos pasos por nosotros, como el reestablecimiento de los paquetes NuGet, compilar la solución y registrarse en los archivos APK. De momento vamos ahora a desactivar el `Xamarin Test Cloud` simplemente haciendo click derecho en ese botón y seleccionando `Disable selected tasks`.

¡Baaang! Ya hemos terminado configurando nuestro build definition.

##### 5. Encolar tu primer build con integración contínua.
Ahora que tenemos VSTS y el GitHub configurado, vamos a empezar el building. Selecciona `Save` en el build definition final y dale un nombre único. Podemos hacer click en el botón `Queue new build...` en la parte de arriba a la derecha de la página.

 ![](/modules/module-5/images/queue-build-button.png)

Los valores por defecto no suelen funcionar, así que simplemente puedes hacer click en `OK`para empezar el build. Nuestro primer build ahora está encolado en el hosted agent, y en breves empezará. Cuando esté completado, podremos ver un log con el build completo, así como una lista de los ítems que ha necesitado descargarse (como el Android APK).

 ![](/modules/module-5/images/build-succeeded.png)

¡Baaang! ¡Nuestro build se ha ejecutado con éxito! ¿Pero y si ahora queremos compilar automáticamente nuestra aplicación con cada commit? Podemos hacerlo creando un **trigger** en el build definition. Haciendo click en el botón `Triggers` en la barra de navegación del build definition, haz click en `Continuous Integration` y después en `Save`. ¡Ya tenemos integración contínua para nuestra aplicación de Android, con todo configurado en apenas unos minutos!

> También puedes crear builds para ramas específicas y no para otras. Es normal tener ramas para cada nueva característica, así que asegúrate que cada característica esté apropiadamente testeada antes de juntar ramas en la rama master.

##### 6. Crear la cuenta de HockeyApp.
HockeyApp te permite distribuir fácilmente tu aplicación para el testeo y también te aporta información útil de diagnóstico como el reporte de errores, uso de métricas y feedback de usuarios. Vamos a usar HockeyApp para distribuir builds a los encargados de testear via VSTS y HockeyApp.

Ve a [HockeyApp.net](https://rink.hockeyapp.net/users/sign_up) para registrarte. Si ya tienes una cuenta HockeyApp, sáltate este paso. Introduce tu `Nombre`, `Email` y `Contraseña`, y asegúrate de revisar el apartado `Im a developer`. Haz click en `Register` para crear tu cuenta.

##### 7. Crea un proyecto HockeyApp.
Lo primero, crea un proyecto HockeyApp. Para ello, haz click en el botón `New App` del [HockeyApp dashboard](https://rink.hockeyapp.net/manage/dashboard). Selecciona la opción `Create an app manually instead`.

 ![](/modules/module-5/images/create-app.png)

Se te cargará la página de `Create App`.

* `Platform`: `Android`
* `Title`: `Spent`
* `Package Name`: `com.pierceboggan.spent`

Haz click en `Save` para crear la aplicación. Asegúrate de tomar nota de tu `App ID`. A continuación vete a las opciones de `Create API Token` en `Settings` y crea un nuevo API token de VSTS para usar después.

##### 8. Despliegue contínuo a la HockeyApp desde el Visual Studio Team Services.
El Visual Studio Team Services tiene muchísimas funcionalidades que se pueden extender incluso mucho más a través de extensiones gratuitas o de pago en el [Visual Studio Team Services Marketplace](https://marketplace.visualstudio.com/VSTS). Para conseguir un despliegue contínuo con HockeyApp, [instala HockeyApp desde el VSTS Marketplace](https://marketplace.visualstudio.com/items?itemName=ms.hockeyapp). Selecciona la cuenta de VSTS que desees instalar la extensión y haz click en `Confirmar`.

 ![](/modules/module-5/images/install_hockeyapp.png)

Vamos a conectar nuestra cuenta HockeyApp al equipo `Spent`. Vuelve al menu de configuración del proyecto en el Visual Studio Team Services y selecciona la pestaña `Services`. Haz click en `New Service Endpoint -> HockeyApp`. Para `Connection Name`, introduce `HockeyApp`. Para `API Token` y mete el API token generado en el paso #7. 

Vuelve al build definition creado en el paso #4 y haz click en `Add build step -> Deploy -> HockeyApp -> Add`. Te aparecerá un nuevo paso para la HockeyApp en el build definition y después presiona `Close`. 

 ![](/modules/module-5/images/add-deploy-step.png)

Haz click en el paso de build definition para HockeyApp. Selecciona la `HockeyApp Connection` que acabas de crear. Para `App Id`, copia el `App Id` desde el HockeyApp del proyecto que creamos antes en el paso #7. Para `Binary File Path`, introduce `$(build.binariesdirectory)/$(BuildConfiguration)/*.apk`. Haz click en `Save` para subir el build definition. ¡Encola un nuevo build y verás tu aplicación compilada y desplegada en tu HockeyApp!

 ![](/modules/module-5/images/hockey-app-success.png)

En este taller has creado una app para iOS, Android y Windows usando Xamarin.Forms para seguir los gastos. Después hemos conectado la aplicación a la nube usando las tablas fáciles del backend mediante Azure Mobile Apps y hemos guardado archivos en nuestro almacenamiento de la nube con Azure Storage. Finalmente, vimos cómo configurar DevOps para tus apps con Visual Studio Team Services y HockeyApp.

