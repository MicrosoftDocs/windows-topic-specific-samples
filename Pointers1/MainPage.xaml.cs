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
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// For some devices, once the maximum number of contacts is reached, 
// additional contacts might be ignored (PointerPressed not fired).
namespace Pointers1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Dictionary to maintain information about each active contact. 
        // An entry is added during PointerPressed/PointerEntered events and removed 
        // during PointerReleased/PointerCaptureLost/PointerCanceled/PointerExited events.
        Dictionary<uint, Windows.UI.Xaml.Input.Pointer> pointers;

        public MainPage()
        {
            this.InitializeComponent();

            // Initialize the dictionary.
            pointers = new Dictionary<uint, Windows.UI.Xaml.Input.Pointer>();

            // Declare the pointer event handlers.
            Target.PointerPressed += 
                new PointerEventHandler(Target_PointerPressed);
            Target.PointerEntered += 
                new PointerEventHandler(Target_PointerEntered);
            Target.PointerReleased += 
                new PointerEventHandler(Target_PointerReleased);
            Target.PointerExited += 
                new PointerEventHandler(Target_PointerExited);
            Target.PointerCanceled += 
                new PointerEventHandler(Target_PointerCanceled);
            Target.PointerCaptureLost += 
                new PointerEventHandler(Target_PointerCaptureLost);
            Target.PointerMoved += 
                new PointerEventHandler(Target_PointerMoved);
            Target.PointerWheelChanged += 
                new PointerEventHandler(Target_PointerWheelChanged);

            buttonClear.Click += 
                new RoutedEventHandler(ButtonClear_Click);
        }

        /// <summary>
        /// Clear the content of the pointer event log pane.
        /// </summary>
        /// <param name="sender">Source of the pointer event.</param>
        /// <param name="e">Event args for the button click routed event.</param>
        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            eventLog.Blocks.Clear();
        }

        /// <summary>
        /// Update the content of the pointer event log pane.
        /// </summary>
        /// <param name="s">The string to display.</param>
        private void UpdateEventLog(string s)
        {
            Paragraph paragraph = new Paragraph();
            Run run = new Run();
            eventLog.TextWrapping = TextWrapping.Wrap;
            run.Text = s;
            paragraph.Inlines.Add(run);
            eventLog.Blocks.Insert(0, paragraph);
        }

        /// <summary>
        /// Create the pointer info popup.
        /// </summary>
        /// <param name="ptrPt">Reference to the input pointer.</param>
        void CreateInfoPop(PointerPoint ptrPt)
        {
            TextBlock pointerDetails = new TextBlock();
            pointerDetails.Name = ptrPt.PointerId.ToString();
            pointerDetails.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
            pointerDetails.Text = QueryPointer(ptrPt);

            TranslateTransform x = new TranslateTransform();
            x.X = ptrPt.Position.X + 20;
            x.Y = ptrPt.Position.Y + 20;
            pointerDetails.RenderTransform = x;

            Container.Children.Add(pointerDetails);
        }

        /// <summary>
        /// Update the pointer info popup.
        /// </summary>
        /// <param name="ptrPt">Reference to the input pointer.</param>
        void UpdateInfoPop(PointerPoint ptrPt)
        {
            foreach (var pointerDetails in Container.Children)
            {
                if (pointerDetails.GetType().ToString() == "Windows.UI.Xaml.Controls.TextBlock")
                {
                    TextBlock textBlock = (TextBlock)pointerDetails;
                    if (textBlock.Name == ptrPt.PointerId.ToString())
                    {
                        // To get pointer location details, we need extended pointer info.
                        // We get the pointer info through the getCurrentPoint method
                        // of the event argument. 
                        TranslateTransform x = new TranslateTransform();
                        x.X = ptrPt.Position.X + 20;
                        x.Y = ptrPt.Position.Y + 20;
                        pointerDetails.RenderTransform = x;
                        textBlock.Text = QueryPointer(ptrPt);
                    }
                }
            }
        }

        /// <summary>
        /// Destroy the pointer info popup.
        /// </summary>
        /// <param name="ptrPt">Reference to the input pointer.</param>
        void DestroyInfoPop(PointerPoint ptrPt)
        {
            foreach (var pointerDetails in Container.Children)
            {
                if (pointerDetails.GetType().ToString() == "Windows.UI.Xaml.Controls.TextBlock")
                {
                    TextBlock textBlock = (TextBlock)pointerDetails;
                    if (textBlock.Name == ptrPt.PointerId.ToString())
                    {
                        Container.Children.Remove(pointerDetails);
                    }
                }
            }
        }

        /// <summary>
        /// The pointer pressed event handler.
        /// PointerPressed and PointerReleased don't always occur in pairs. 
        /// Your app should listen for and handle any event that can conclude 
        /// a pointer down (PointerExited, PointerCanceled, PointerCaptureLost).
        /// </summary>
        /// <param name="sender">Source of the pointer event.</param>
        /// <param name="e">Event args for the pointer routed event.</param>
        void Target_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // Prevent most handlers along the event route from handling the same event again.
            e.Handled = true;

            PointerPoint ptrPt = e.GetCurrentPoint(Target);

            // Update event log.
            UpdateEventLog("Down: " + ptrPt.PointerId);

            // Lock the pointer to the target.
            Target.CapturePointer(e.Pointer);

            // Update event log.
            UpdateEventLog("Pointer captured: " + ptrPt.PointerId);

            // Check if pointer exists in dictionary (ie, enter occurred prior to press).
            if (!pointers.ContainsKey(ptrPt.PointerId))
            {
                // Add contact to dictionary.
                pointers[ptrPt.PointerId] = e.Pointer;
            }
 
            // Change background color of target when pointer contact detected.
            Target.Fill = new SolidColorBrush(Windows.UI.Colors.Green);

            // Display pointer details.
            CreateInfoPop(ptrPt);
       }

        /// <summary>
        /// The pointer entered event handler.
        /// We do not capture pointer on this event.
        /// </summary>
        /// <param name="sender">Source of the pointer event.</param>
        /// <param name="e">Event args for the pointer routed event.</param>
        private void Target_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            // Prevent most handlers along the event route from handling the same event again.
            e.Handled = true;

            PointerPoint ptrPt = e.GetCurrentPoint(Target);

            // Update event log.
            UpdateEventLog("Entered: " + ptrPt.PointerId);

            // Check if pointer already exists (if enter occurred prior to down).
            if (!pointers.ContainsKey(ptrPt.PointerId))
            {
                // Add contact to dictionary.
                pointers[ptrPt.PointerId] = e.Pointer;
            }

            if (pointers.Count == 0)
            {
                // Change background color of target when pointer contact detected.
                Target.Fill = new SolidColorBrush(Windows.UI.Colors.Blue);
            }

            // Display pointer details.
            CreateInfoPop(ptrPt);
        }

        /// <summary>
        /// The pointer moved event handler.
        /// </summary>
        /// <param name="sender">Source of the pointer event.</param>
        /// <param name="e">Event args for the pointer routed event.</param>
        private void Target_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            // Prevent most handlers along the event route from handling the same event again.
            e.Handled = true;

            PointerPoint ptrPt = e.GetCurrentPoint(Target);

            // Multiple, simultaneous mouse button inputs are processed here.
            // Mouse input is associated with a single pointer assigned when 
            // mouse input is first detected. 
            // Clicking additional mouse buttons (left, wheel, or right) during 
            // the interaction creates secondary associations between those buttons 
            // and the pointer through the pointer pressed event. 
            // The pointer released event is fired only when the last mouse button 
            // associated with the interaction (not necessarily the initial button) 
            // is released. 
            // Because of this exclusive association, other mouse button clicks are 
            // routed through the pointer move event.          
            if (ptrPt.PointerDevice.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                if (ptrPt.Properties.IsLeftButtonPressed)
                {
                    UpdateEventLog("Left button: " + ptrPt.PointerId);
                }
                if (ptrPt.Properties.IsMiddleButtonPressed)
                {
                    UpdateEventLog("Wheel button: " + ptrPt.PointerId);
                }
                if (ptrPt.Properties.IsRightButtonPressed)
                {
                    UpdateEventLog("Right button: " + ptrPt.PointerId);
                }
            }

            // Display pointer details.
            UpdateInfoPop(ptrPt);
        }

        /// <summary>
        /// The pointer wheel event handler.
        /// </summary>
        /// <param name="sender">Source of the pointer event.</param>
        /// <param name="e">Event args for the pointer routed event.</param>
        private void Target_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            // Prevent most handlers along the event route from handling the same event again.
            e.Handled = true;

            PointerPoint ptrPt = e.GetCurrentPoint(Target);

            // Update event log.
            UpdateEventLog("Mouse wheel: " + ptrPt.PointerId);

            // Check if pointer already exists (for example, enter occurred prior to wheel).
            if (!pointers.ContainsKey(ptrPt.PointerId))
            {
                // Add contact to dictionary.
                pointers[ptrPt.PointerId] = e.Pointer;
            }

            // Display pointer details.
            CreateInfoPop(ptrPt);
        }

        /// <summary>
        /// The pointer released event handler.
        /// PointerPressed and PointerReleased don't always occur in pairs. 
        /// Your app should listen for and handle any event that can conclude 
        /// a pointer down (PointerExited, PointerCanceled, PointerCaptureLost).
        /// </summary>
        /// <param name="sender">Source of the pointer event.</param>
        /// <param name="e">Event args for the pointer routed event.</param>
        void Target_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            // Prevent most handlers along the event route from handling the same event again.
            e.Handled = true;

            PointerPoint ptrPt = e.GetCurrentPoint(Target);

            // Update event log.
            UpdateEventLog("Up: " + ptrPt.PointerId);

            // If event source is mouse or touchpad and the pointer is still 
            // over the target, retain pointer and pointer details.
            // Return without removing pointer from pointers dictionary.
            // For this example, we assume a maximum of one mouse pointer.
            if (ptrPt.PointerDevice.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                // Update target UI.
                Target.Fill = new SolidColorBrush(Windows.UI.Colors.Red);

                DestroyInfoPop(ptrPt);

                // Remove contact from dictionary.
                if (pointers.ContainsKey(ptrPt.PointerId))
                {
                    pointers[ptrPt.PointerId] = null;
                    pointers.Remove(ptrPt.PointerId);
                }

                // Release the pointer from the target.
                Target.ReleasePointerCapture(e.Pointer);

                // Update event log.
                UpdateEventLog("Pointer released: " + ptrPt.PointerId);
            }
            else
            {
                Target.Fill = new SolidColorBrush(Windows.UI.Colors.Blue);
            }
        }

        /// <summary>
        /// The pointer capture lost event handler.
        /// Fires for various reasons, including: 
        ///    - User interactions
        ///    - Programmatic capture of another pointer
        ///    - Captured pointer was deliberately released
        // PointerCaptureLost can fire instead of PointerReleased. 
        /// </summary>
        /// <param name="sender">Source of the pointer event.</param>
        /// <param name="e">Event args for the pointer routed event.</param>
        private void Target_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            // Prevent most handlers along the event route from handling the same event again.
            e.Handled = true;

            PointerPoint ptrPt = e.GetCurrentPoint(Target);

            // Update event log.
            UpdateEventLog("Pointer capture lost: " + ptrPt.PointerId);

            if (pointers.Count == 0)
            {
                Target.Fill = new SolidColorBrush(Windows.UI.Colors.Black);
            }

            // Remove contact from dictionary.
            if (pointers.ContainsKey(ptrPt.PointerId))
            {
                pointers[ptrPt.PointerId] = null;
                pointers.Remove(ptrPt.PointerId);
            }

            DestroyInfoPop(ptrPt);
        }

        /// <summary>
        /// The pointer canceled event handler.
        /// Fires for for various reasons, including: 
        ///    - Touch contact canceled by pen coming into range of the surface.
        ///    - The device doesn't report an active contact for more than 100ms.
        ///    - The desktop is locked or the user logged off. 
        ///    - The number of simultaneous contacts exceeded the number supported by the device.
        /// </summary>
        /// <param name="sender">Source of the pointer event.</param>
        /// <param name="e">Event args for the pointer routed event.</param>
        private void Target_PointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            // Prevent most handlers along the event route from handling the same event again.
            e.Handled = true;

            PointerPoint ptrPt = e.GetCurrentPoint(Target);

            // Update event log.
            UpdateEventLog("Pointer canceled: " + ptrPt.PointerId);

            // Remove contact from dictionary.
            if (pointers.ContainsKey(ptrPt.PointerId))
            {
                pointers[ptrPt.PointerId] = null;
                pointers.Remove(ptrPt.PointerId);
            }

            if (pointers.Count == 0)
            {
                Target.Fill = new SolidColorBrush(Windows.UI.Colors.Black);
            }

            DestroyInfoPop(ptrPt);
        }

        /// <summary>
        /// The pointer exited event handler.
        /// </summary>
        /// <param name="sender">Source of the pointer event.</param>
        /// <param name="e">Event args for the pointer routed event.</param>
        private void Target_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            // Prevent most handlers along the event route from handling the same event again.
            e.Handled = true;

            PointerPoint ptrPt = e.GetCurrentPoint(Target);

            // Update event log.
            UpdateEventLog("Pointer exited: " + ptrPt.PointerId);

            // Remove contact from dictionary.
            if (pointers.ContainsKey(ptrPt.PointerId))
            {
                pointers[ptrPt.PointerId] = null;
                pointers.Remove(ptrPt.PointerId);
            }

            if (pointers.Count == 0)
            {
                Target.Fill = new SolidColorBrush(Windows.UI.Colors.Red);
            }

            // Update the UI and pointer details.
            DestroyInfoPop(ptrPt);
        }

        /// <summary>
        /// Get pointer details.
        /// </summary>
        /// <param name="ptrPt">Reference to the input pointer.</param>
        /// <returns>A string composed of pointer details.</returns>
        String QueryPointer(PointerPoint ptrPt)
        {
            String details = "";

            switch (ptrPt.PointerDevice.PointerDeviceType)
            {
                case Windows.Devices.Input.PointerDeviceType.Mouse:
                    details += "\nPointer type: mouse";
                    break;
                case Windows.Devices.Input.PointerDeviceType.Pen:
                    details += "\nPointer type: pen";
                    if (ptrPt.IsInContact)
                    {
                        details += "\nPressure: " + ptrPt.Properties.Pressure;
                        details += "\nrotation: " + ptrPt.Properties.Orientation;
                        details += "\nTilt X: " + ptrPt.Properties.XTilt;
                        details += "\nTilt Y: " + ptrPt.Properties.YTilt;
                        details += "\nBarrel button pressed: " + ptrPt.Properties.IsBarrelButtonPressed;
                    }
                    break;
                case Windows.Devices.Input.PointerDeviceType.Touch:
                    details += "\nPointer type: touch";
                    details += "\nrotation: " + ptrPt.Properties.Orientation;
                    details += "\nTilt X: " + ptrPt.Properties.XTilt;
                    details += "\nTilt Y: " + ptrPt.Properties.YTilt;
                    break;
                default:
                    details += "\nPointer type: n/a";
                    break;
            }

            GeneralTransform gt = Target.TransformToVisual(this);
            Point screenPoint;

            screenPoint = gt.TransformPoint(new Point(ptrPt.Position.X, ptrPt.Position.Y));
            details += "\nPointer Id: " + ptrPt.PointerId.ToString() +
                "\nPointer location (target): " + Math.Round(ptrPt.Position.X) + ", " + Math.Round(ptrPt.Position.Y) +
                "\nPointer location (container): " + Math.Round(screenPoint.X) + ", " + Math.Round(screenPoint.Y);

            return details;
        }
    }
}