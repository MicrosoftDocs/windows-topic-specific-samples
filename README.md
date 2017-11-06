---
category: CustomUserInteractions Inking
---

# Ink toolbar location and orientation sample (dynamic)

> The default platform target is ARM, change this to x64 or x86 if you want to test on a non-ARM device.

This sample supports the [Pen interactions and Windows Ink](https://docs.microsoft.com/en-us/windows/uwp/input-and-devices/pen-and-stylus-interactions) topics on the [Windows Dev Center](https://developer.microsoft.com/en-us/windows).

In this sample, we demonstrate how to dynamically set the location and orientation of the ink toolbar in a basic UWP application based on changes to user preferences, device settings, or device states. We use the [Windows.UI.Xaml.Controls.InkToolbar](https://docs.microsoft.com/uwp/api/windows.ui.xaml.controls.inktoolbar) and [Windows.UI.ViewManagement.UISettings](https://docs.microsoft.com/uwp/api/windows.ui.viewmanagement.uisettings) APIs to get the user preferences and device state. 

We focus on the following:
* Adding basic ink support
* Adding an InkToolbar
* Dynamically setting the location and orientation of the ink toolbar based on the device orientation and the left or right-hand writing preferences specified through **Settings > Devices > Pen & Windows Ink > Pen > Choose which hand you write with**

## Contributing

We welcome your input on issues and suggestions for new samples! Please file them as issues on this GitHub repo.  At this time we are not accepting new samples from the public, but please check back as we evolve our contribution model.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
