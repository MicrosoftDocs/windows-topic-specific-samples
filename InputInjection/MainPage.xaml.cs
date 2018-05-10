//  ---------------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// 
//  The MIT License (MIT)
// 
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//  ---------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Input.Preview.Injection;
using Windows.UI.Input;
using Windows.UI.ViewManagement;

namespace InputInjection
{
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// The virtual input device.
        /// </summary>
        InputInjector _inputInjector;

        /// <summary>
        /// Initialize the app, set the window size, 
        /// and add pointer input handlers for the container.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();

            ApplicationView.PreferredLaunchViewSize =
                new Size(600, 200);
            ApplicationView.PreferredLaunchWindowingMode =
                ApplicationViewWindowingMode.PreferredLaunchViewSize;

            // Button handles PointerPressed/PointerReleased in 
            // the Tapped routed event, but we need the container Grid 
            // to handle them also. Add a handler for both 
            // PointerPressedEvent and PointerReleasedEvent on the input Grid 
            // and set handledEventsToo to true.
            ContainerInput.AddHandler(PointerPressedEvent,
                new PointerEventHandler(ContainerInput_PointerPressed), true);
            ContainerInput.AddHandler(PointerReleasedEvent,
                new PointerEventHandler(ContainerInput_PointerReleased), true);
        }

        /// <summary>
        /// PointerReleased handler for all pointer conclusion events.
        /// PointerPressed and PointerReleased events do not always occur 
        /// in pairs, so your app should listen for and handle any event that 
        /// might conclude a pointer down (such as PointerExited, PointerCanceled, 
        /// and PointerCaptureLost).  
        /// </summary>
        /// <param name="sender">Source of the click event</param>
        /// <param name="e">Event args for the button click routed event</param>
        private void ContainerInput_PointerReleased(
            object sender, PointerRoutedEventArgs e)
        {
            // Prevent most handlers along the event route from handling event again.
            e.Handled = true;

            // Shut down the virtual input device.
            _inputInjector.UninitializeTouchInjection();
        }

        /// <summary>
        /// PointerPressed handler.
        /// PointerPressed and PointerReleased events do not always occur 
        /// in pairs. Your app should listen for and handle any event that 
        /// might conclude a pointer down (such as PointerExited, 
        /// PointerCanceled, and PointerCaptureLost).  
        /// </summary>
        /// <param name="sender">Source of the click event</param>
        /// <param name="e">Event args for the button click routed event</param>
        private void ContainerInput_PointerPressed(
            object sender, PointerRoutedEventArgs e)
        {
            // Prevent most handlers along the event route from 
            // handling the same event again.
            e.Handled = true;

            InjectTouchForMouse(e.GetCurrentPoint(ContainerInput));

        }

        /// <summary>
        /// Inject touch input on injection target corresponding 
        /// to mouse click on input target.
        /// </summary>
        /// <param name="pointerPoint">The mouse click pointer.</param>
        private void InjectTouchForMouse(PointerPoint pointerPoint)
        {
            // Create the touch injection object.
            _inputInjector = InputInjector.TryCreate();

            if (_inputInjector != null)
            {
                _inputInjector.InitializeTouchInjection(
                    InjectedInputVisualizationMode.Default);

                // Create a unique pointer ID for the injected touch pointer.
                // Multiple input pointers would require more robust handling.
                uint pointerId = pointerPoint.PointerId + 1;

                // Get the bounding rectangle of the app window.
                Rect appBounds =
                    Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().VisibleBounds;

                // Get the top left screen coordinates of the app window rect.
                Point appBoundsTopLeft = new Point(appBounds.Left, appBounds.Top);

                // Get a reference to the input injection area.
                GeneralTransform injectArea =
                    ContainerInject.TransformToVisual(Window.Current.Content);

                // Get the top left screen coordinates of the input injection area.
                Point injectAreaTopLeft = injectArea.TransformPoint(new Point(0, 0));

                // Get the screen coordinates (relative to the input area) 
                // of the input pointer.
                int pointerPointX = (int)pointerPoint.Position.X;
                int pointerPointY = (int)pointerPoint.Position.Y;

                // Create the point for input injection and calculate its screen location.
                Point injectionPoint =
                    new Point(
                        appBoundsTopLeft.X + injectAreaTopLeft.X + pointerPointX,
                        appBoundsTopLeft.Y + injectAreaTopLeft.Y + pointerPointY);

                // Create a touch data point for pointer down.
                // Each element in the touch data list represents a single touch contact. 
                // For this example, we're mirroring a single mouse pointer.
                List<InjectedInputTouchInfo> touchData =
                    new List<InjectedInputTouchInfo>
                    {
                        new InjectedInputTouchInfo
                        {
                            Contact = new InjectedInputRectangle
                            {
                                Left = 30, Top = 30, Bottom = 30, Right = 30
                            },
                            PointerInfo = new InjectedInputPointerInfo
                            {
                                PointerId = pointerId,
                                PointerOptions =
                                InjectedInputPointerOptions.PointerDown |
                                InjectedInputPointerOptions.InContact |
                                InjectedInputPointerOptions.New,
                                TimeOffsetInMilliseconds = 0,
                                PixelLocation = new InjectedInputPoint
                                {
                                    PositionX = (int)injectionPoint.X ,
                                    PositionY = (int)injectionPoint.Y
                                }
                        },
                        Pressure = 1.0,
                        TouchParameters =
                            InjectedInputTouchParameters.Pressure |
                            InjectedInputTouchParameters.Contact
                    }
                };

                // Inject the touch input. 
                _inputInjector.InjectTouchInput(touchData);

                // Create a touch data point for pointer up.
                touchData = new List<InjectedInputTouchInfo>
                {
                    new InjectedInputTouchInfo
                    {
                        PointerInfo = new InjectedInputPointerInfo
                        {
                            PointerId = pointerId,
                            PointerOptions = InjectedInputPointerOptions.PointerUp
                        }
                    }
                };

                // Inject the touch input. 
                _inputInjector.InjectTouchInput(touchData);
            }
        }

        /// <summary>
        /// Echo injected click target.
        /// </summary>
        /// <param name="sender">Source of the click event</param>
        /// <param name="e">Event args for the button click routed event</param>
        private async void Button_Click_Injected(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                statusText.Text = "Click injected on " + btn.Name;
            });
        }
    }
}
