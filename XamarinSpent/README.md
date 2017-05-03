# Module 5: Mobile DevOps with Visual Studio Team Services and HockeyApp
**Objective**: Learn the basics of a solid Mobile DevOps strategy, including continuous integration (with Visual Studio Team Services) and deployment (with HockeyApp), as well as acceptance testing with Xamarin Test Cloud.

##### Prerequisites
Ensure you have the following software installed:

* Visual Studio 2015 Community Edition (or higher) or Xamarin Studio Community Edition (or higher)
* [Xamarin](xamarin.com/download)

Download the starter code for this module to begin, or continue working with the completed code from Module 3.

### Module Instructions
In this module, you will create and configure a Visual Studio Team Services (VSTS) account for continuous integration, use HockeyApp to continuously deploy apps from VSTS, and test your mobile apps with Xamarin Test Cloud.

##### 1. Create a GitHub respository.
GitHub provides a hosted Git service for version control that's extremely popular with developers (Xamarin is open-sourced on GitHub). They make it an absolute delight to host my code, track changes, and perform code reviews. We will be using GitHub to host our Spent app.

To begin, [create a GitHub account](https://github.com/join) if you don't already have one. Also, download the [GitHub for Mac or Windows](https://desktop.github.com/) desktop applications. These will make it as easy as possible to check in code without having to drop down to the console by providing a user interface for managing Git repositories.

Create a new repository by clicking `+ ->  New respository` in the upper-righthand portion of the GitHub navigation bar. The `Create a new repository` page will appear.

* `Repository name`: Any name you would like, it just cannot conflict with previous repositories you have created.
* `Public / Private`: Feel free to make the repository `Public`, but this will also work with `Private`.
* `Initialize this repository with a README` / `Add .gitignore` / `Add a license`: These are all optional fields.

 ![](/modules/module-5/images/create-repository.png)

Click `Create repository`, and a GitHub respository will be created. Click `Clone or download -> Open in Desktop` to open the respository in the GitHub desktop client.

Drag the code from this module's folder to the respository folder. This is most likely in `Documents/GitHub/YOUR-REPOSITORY-NAME`. After this finishes copying, open the GitHub desktop client, select your respository, and enter a `Summary` in the GitHub client and click `Commit to master`. To sync the changes to GitHub, click the `Sync` button.

 ![](/modules/module-5/images/spent-github-client.png)

Awesome! Now that our code is hosted in GitHub, let's configure a VSTS account to provide continuous integration.

##### 2. Create a Visual Studio Team Services account.
[Visual Studio Team Services](https://www.visualstudio.com/en-us/products/visual-studio-team-services-vs.aspx) provides a set of cloud-powered collaboration tools, from code respositories, to continuous integration, to bug tracking and agile planning tools. In this module, we will be using it for continuous integration. If you already have a VSTS account, feel free to skip to Step #2.

Visit the [Visual Studio Team Services landing page](https://www.visualstudio.com/en-us/products/visual-studio-team-services-vs.aspx) and click `Get started for FREE` to begin.

Start by entering an account name, and selecting how you would like to handle version control. One of my favorite features of VSTS is that it integrates nicely with GitHub as well, if you elect not to use the built-in version control in VSTS. Click the `Continue` button. It may take a minute or two to provision a VSTS account.

 ![](/modules/module-5/images/create-account.png)

`MyFirstProject` will be provisioned in Visual Studio Team Services for you to explore everything VSTS has to offer. We will want to create a new project for Spent. We can do that by clicking the `Team Services` logo in the upper-lefthand corner. Next, click the `New` button under `Recent projects and teams`. The `Create team project` dialog will appear.

* `Project name`: Enter `Spent`.
* `Description`, `Process template`, `Version control`: Not required for this module, but feel free to select the option that works best for you.

 ![](/modules/module-5/images/create-project.png)

Click `Create Project`, allow the project time to spin up, and click `Navigate to project`. This is your project dashboard, where you have access to code, issue tracking, build information, and more.

##### 3. Connect Visual Studio Team Services to GitHub.
Visual Studio Team Services works great with GitHub. To connect our code for Spent to the VSTS project, click the project settings icon in the top right of the page.

 ![](/modules/module-5/images/vsts-settings.png)

We can add GitHub as a new service endpoint under the services tab (along with many other service integrations). Click on the `Services` tab, click `New Service Endpoint -> GitHub`.

 ![](/modules/module-5/images/add-service-endpoint.png)

Click the `Authorize` button and authorize Visual Studio Team Services to access your account. Once the authentication is successful, click `OK`.

By adding this service endpoint, we can now use GitHub throughout Visual Studio Team Services.

##### 4. Create a build definition.
For this module, we will configure continuous integration for our Android Spent app. To get started, we are going to create a **build definition**. This tells Visual Studio Team Services how to go about building a particular project, from restoring NuGets to versioning the app.

To get started, click on the `Build` tab on the navigation bar, followed by the `New definition` button. The `Create new build definition` screen will appear. Click on the `Xamarin.Android` build definition, and click `Next`.

 ![](/modules/module-5/images/new-build-definition.png)

For `Repository source`, click `GitHub`, and check the `Continuous integration` button. Click `Create` to create the build definition.

Let's configure our build definition to pull code from our `Spent` repository on GitHub. Click the `Repository` tab. Select the repository you just crerated from the dropdown, set the `Default branch` to `master`, and set `Clean` to `true`.

 ![](/modules/module-5/images/configure-repository.png)

Next, jump back over to the `Build` tab where we can add, edit, and remove steps in our build definition. You can see that our template added a few steps for us, such as restoring NuGet packages, building the solution, and signing the APK files. Let's disable the `Xamarin Test Cloud` and unit testing build steps for now by right-clicking it and selecting `Disable selected tasks`. 

Boom! We're done configuring our build definition.

##### 5. Queue your first continuous integration build.
Now that we have VSTS and GitHub configured, let's start building. Tap `Save` on the final build definition and give it a unique name. We can now click the `Queue new build...` button on the upper-righthand side of the page.

 ![](/modules/module-5/images/queue-build-button.png)

The defaults should work great, so we can just click `OK` to start the build. Our first build is now queued in the hosted agent, and we will soon be abl to see our build begin. When completed, we can view a full build log, as well as a list of artifacts to download (such as the Android APK).

 ![](/modules/module-5/images/build-succeeded.png)

Boom! Our build succeeded! But what if we wanted to automatically build our app with every commit? We can do that by creating a **trigger** in our build definition. Click the `Triggers` button in the build definition navigation bar, click `Continuous Integration` and click `Save`. We now have continuous integration for our Android app fully configured in just a few minutes!

> You can also create builds for specific branches, and not for others. It's common to have branches for each new feature or bug fix when using Git/GitHub, so we want to ensure those are properly tested before merging into our master branch.

##### 6. Create HockeyApp account.
HockeyApp allows you to easily distribute your app to testers, and also provides useful diagnostic information such as crash reporting, user metrics, and user feedback. Let's use HockeyApp to distribute successful builds to our testers via VSTS and HockeyApp.

Go to [HockeyApp.net](https://rink.hockeyapp.net/users/sign_up) to sign up. If you already have a HockeyApp account, feel free to skip this step. Enter your `Name`, `Email`, and `Password`, and be sure to check the `I'm a developer` box. Click `Register` to create your account.

##### 7. Create HockeyApp project.
To get started, let's create a HockeyApp project. Most likely, you will want to do this on a per app, per platform basis. To get started, click the `New App` button on the [HockeyApp dashboard](https://rink.hockeyapp.net/manage/dashboard). Select the `Create an app manually instead` option.

 ![](/modules/module-5/images/create-app.png)

The `Create App` page will load.

* `Platform`: `Android`
* `Title`: `Spent`
* `Package Name`: `com.pierceboggan.spent`

Click `Save` to creat the application. Be sure to take note of your `App ID`. Next, visit the `Create API Token` setting in Settings and generate a new API token for VSTS to use later.

##### 8. Continuously deploy to HockeyApp from Visual Studio Team Services.
Visual Studio Team Services has tons of built-in functionality that can be extended even further via free and paid extensions in the [Visual Studio Team Services Marketplace](https://marketplace.visualstudio.com/VSTS). To gain continuous deployment with HockeyApp, [install HockeyApp from the VSTS Marketplace](https://marketplace.visualstudio.com/items?itemName=ms.hockeyapp). Select the VSTS account you wish to install the extension into, and click `Confirm`.

 ![](/modules/module-5/images/install_hockeyapp.png)

Let's connect our HockeyApp account to the `Spent` team. Navigate back to your project settings in Visual Studio Team Services, and select the `Services` tab. Click `New Service Endpoint -> HockeyApp`. For `Connection Name`, enter `HockeyApp`. For `API Token`, enter the API token generated in Step #7. 

Jump back the build definition created in Step #4 and click `Add build step -> Deploy -> HockeyApp -> Add`. A build step will appear for HockeyApp in the build definition, then click `Close`. 

 ![](/modules/module-5/images/add-deploy-step.png)

Click on the build definition step for HockeyApp. Select the `HockeyApp Connection` just created. For `App Id`, copy the `App Id` from HockeyApp for the project we created earlier in Step #7. For `Binary File Path`, enter `$(build.binariesdirectory)/$(BuildConfiguration)/*.apk`. Click `Save` to update the build definition. Queue a new build, and you will see your app build and deploy to HockeyApp!

 ![](/modules/module-5/images/hockey-app-success.png)

In this workshop, you created your first mobile app for iOS, Android, and Windows using Xamarin.Forms to track expenses. We then connected our app to the cloud using the no-code Easy Tables backend in Azure Mobile Apps, and saved files to cloud storage using Azure Storage. Finally, we took a look at how to configure mobile DevOps for your apps with Visual Studio Team Services and HockeyApp.
