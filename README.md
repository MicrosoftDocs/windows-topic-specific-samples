---
category: CustomUserInteractions Inking
---

# Ink recognition sample (basic)

> The default platform target is ARM, change this to x64 or x86 if you want to test on a non-ARM device.

This sample supports the [Pen interactions and Windows Ink](https://docs.microsoft.com/en-us/windows/uwp/input-and-devices/pen-and-stylus-interactions) topics on the [Windows Dev Center](https://developer.microsoft.com/en-us/windows).

In this sample, we demonstrate how to support Windows Ink in a basic UWP application and use the [Windows.UI.Input.Inking.Analysis](https://docs.microsoft.com/uwp/api/windows.ui.input.inking.analysis) APIs to recognize text and shapes. 

We focus on the following:
* Adding basic ink support
* Supporting handwriting recognition
* Supporting shape recognition

> **Note:** The basic ink recognition shown in this sample is best suited for richer recognition scenarios that include analysis and interpretation of document structure, list items, shapes, and drawings (in addition to text recognition). For straightforward, single-line, plain text scenarios such as form input, see the constrained recognition sample that uses just the [Windows.UI.Input.Inking](https://docs.microsoft.com/uwp/api/windows.ui.input.inking) APIs. 

## Contributing

We welcome your input on issues and suggestions for new samples! Please file them as issues on this GitHub repo.  At this time we are not accepting new samples from the public, but please check back as we evolve our contribution model.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.