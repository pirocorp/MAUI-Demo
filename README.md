# MAUI Playground

## What is .NET MAUI?

<ins>**M**</ins>ulti-platform <ins>**A**</ins>pp <ins>**U**</ins>ser <ins>**I**</ins>nterface - a cross-platform framework for Windows, Android, iOS, macOS (for iOS and macOS requires Mac) for creating **native** mobile and desktop **apps** using C# and XAML(e**X**tensible **A**pplication **M**arkup **L**anguage) evolved from Xamarin.Forms.

### Key Aim of MAUI

To enable you to implement as much of your app logic and UI layout in a single code-base.

### How .NET MAUI works

.NET MAUI unifies Android, iOS, macOS, and Windows APIs into a single API that allows a write-once run-anywhere developer experience, while additionally providing deep access to every aspect of each native platform.

.NET 6 or greater provides a series of platform-specific frameworks for creating apps: .NET for Android, .NET for iOS, .NET for macOS, and Windows UI 3 (WinUI 3) library. These frameworks all have access to the same .NET Base Class Library (BCL). This library abstracts the details of the underlying platform away from your code. The BCL depends on the .NET runtime to provide the execution environment for your code. 

While the BCL enables apps running on different platforms to share common business logic, the various platforms have different ways of defining the user interface for an app, and they provide varying models for specifying how the elements of a user interface communicate and interoperate.

.NET MAUI provides a single framework for building the UIs for mobile and desktop apps. The following diagram shows a high-level view of the architecture of a .NET MAUI app:

![image](https://user-images.githubusercontent.com/34960418/216610329-9fede90a-f24f-482e-9c48-fc2ac04880ec.png)

In a .NET MAUI app, you write code that primarily interacts with the .NET MAUI API. .NET MAUI then directly consumes the native platform APIs. In addition, app code may directly exercise platform APIs, if required.

### What .NET MAUI provides

.NET MAUI provides a collection of controls that can be used to display data, initiate actions, indicate activity, display collections, pick data, and more. In addition to a collection of controls, .NET MAUI also provides:

- An elaborate layout engine for designing pages.
- Multiple page types for creating rich navigation types, like drawers.
- Support for data-binding, for more elegant and maintainable development patterns.
- The ability to customize handlers to enhance the way in which UI elements are presented.
- Cross-platform APIs for accessing native device features. These APIs enable apps to access device features such as the GPS, the accelerometer, and battery and network states. For more information, see Cross-platform APIs for device features.
- Cross-platform graphics functionality, that provides a drawing canvas that supports drawing and painting shapes and images, compositing operations, and graphical object transforms.
- A single project system that uses multi-targeting to target Android, iOS, macOS, and Windows. For more information, see .NET MAUI Single project.
- .NET hot reload, so that you can modify both your XAML and your managed source code while the app is running, then observe the result of your modifications without rebuilding the app. For more information, see .NET hot reload.

### Cross-platform APIs for device features

.NET MAUI provides cross-platform APIs for native device features. Examples of functionality provided by .NET MAUI for accessing device features includes:

- Access to sensors, such as the accelerometer, compass, and gyroscope on devices.
- Ability to check the device's network connectivity state, and detect changes.
- Provide information about the device the app is running on.
- Copy and paste text to the system clipboard, between apps.
- Pick single or multiple files from the device.
- Store data securely as key/value pairs.
- Utilize built-in text-to-speech engines to read text from the device.
- Initiate browser-based authentication flows that listen for a callback to a specific app registered URL.


### Single project

.NET MAUI single project takes the platform-specific development experiences you typically encounter while developing apps and abstracts them into a single shared project that can target Android, iOS, macOS, and Windows.

.NET MAUI single project provides a simplified and consistent cross-platform development experience, regardless of the platforms being targeted. .NET MAUI single project provides the following features:

- A single shared project that can target Android, iOS, macOS, and Windows.
- A simplified debug target selection for running your .NET MAUI apps.
- Shared resource files within the single project.
- A single app manifest that specifies the app title, id, and version.
- Access to platform-specific APIs and tools when required.
- A single cross-platform app entry point.

.NET MAUI single project is enabled using multi-targeting and the use of SDK-style projects. For more information about .NET MAUI single project, see .NET MAUI single project.


## Standard MAUI Project Structure

![image](https://user-images.githubusercontent.com/34960418/216617103-d803735d-6ec1-4481-bd62-6a64de1dcdee.png)

- **Platforms** - platform-native initialization code calls `CreateMauiApp()` in `MauiProgram.cs`.
- **MauiProgram.cs** - application entry point, called by the platform's native code, register services in the DI container.
- **Resources** - contains fonts, images and assets used by all platforms.
- **App.xaml** - defines the resources that the app is going to use.
- **App.xaml.cs** - code behind `App.xaml`, defines the App class representing the app at runtime, creates an initial window and assigns it to the MainPage property.
- **AppShell.xaml** - Defines the main structure of the app. The MAUI Shell provides features like: app styling, url navigation and layout options. 
- **AppShell.xaml.cs** - code behind `AppShell.xaml`, define routes.
- **MainPage.xaml** - default main page, XAML based layout definition.
- **MainPage.xaml.cs** - code behind `MainPage.xaml` contains UI event handlers


## App Start-up

![image](https://user-images.githubusercontent.com/34960418/216621451-490b52f6-5a68-49d8-a32e-347536cc3830.png)


## UI Hierarchy

![image](https://user-images.githubusercontent.com/34960418/216623036-e8cac575-24d6-41c2-ab9a-5d2ef704a33c.png)

![image](https://user-images.githubusercontent.com/34960418/216626196-90fa2094-43c0-4b6e-89f2-3979c9056d8d.png)

### XAML vs C#

The UI is typically defined as a combination of XAML and C#. XAML is used more for rendering visuals (XAML is markup language). C# is usually used for the **code behind**, e.g., event handling code.

```xaml
<Button Text="Save" Grid.Row="1" Grid.Column="0" Clicked="OnSaveButtonClicked" Margin="20,0" />
<Button Text="Delete" Grid.Row="1" Grid.Column="1" Clicked="OnDeleteButtonClicked" />
<Button Text="Cancel" Grid.Row="1" Grid.Column="2" Clicked="OnCancelButtonClicked" Margin="20,0" />
```

```csharp
async void OnDeleteButtonClicked(object sender, EventArgs e)
{
    await _dataService.DeleteToDoAsync(ToDo.Id);
    await Shell.Current.GoToAsync("..");
}
```







