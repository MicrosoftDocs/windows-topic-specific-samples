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

using System.Collections.Generic;
using Windows.UI.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// For some devices, once the maximum number of contacts is reached, 
// additional contacts might be ignored (PointerPressed not fired).
namespace UWP_Pointers
{
    public sealed partial class MainPage : Page
    {
        // Track if a primary pointer exists already.
        bool primaryExists = false;

        // The ellipse UI object.
        PointerEllipse pe;

        // A Dictionary containing an entry for each active contact. An entry 
        // is added during PointerPressed/PointerEntered events and removed during 
        // PointerReleased/PointerCaptureLost/PointerCanceled/PointerExited events.
        Dictionary<uint, PointerEllipse> ellipses = new Dictionary<uint, PointerEllipse>();

        public MainPage()
        {
            this.InitializeComponent();

            this.PointerEntered += MainPage_PointerEntered;
            this.PointerPressed += MainPage_PointerPressed;
            this.PointerMoved += MainPage_PointerMoved;
            this.PointerReleased += MainPage_PointerReleased;
            this.PointerExited += MainPage_PointerExited;
            this.PointerCanceled += MainPage_PointerReleased;
            this.PointerCaptureLost += MainPage_PointerReleased;
        }

        /// <summary>
        /// The pointer entered event handler.
        /// No-op if pointer is in contact already (Entered fires after Pressed).
        /// </summary>
        /// <param name="sender">Source of the pointer event.</param>
        /// <param name="e">Event args for the pointer routed event.</param>
        private void MainPage_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (!e.Pointer.IsInContact)
            {
                PointerPoint pt = e.GetCurrentPoint(pointerCanvas);

                pe = new PointerEllipse(pointerCanvas);

                pe.PointerId = pt.PointerId;
                ellipses[pt.PointerId] = pe;
                PointerCounter.Text = ellipses.Count.ToString();

                if (pt.Properties.IsPrimary == true & primaryExists == false)
                {
                    pe.PrimaryPointer = true;
                    pe.PrimaryEllipse = true;
                    primaryExists = true;
                    PointerPrimary.Text = pt.PointerId.ToString();
                }
                else
                {
                    pe.PrimaryPointer = false;
                    pe.PrimaryEllipse = false;
                }


                TranslateTransform ttpe = new TranslateTransform();
                ttpe.X = pt.Position.X - pe.Diameter / 2;
                ttpe.Y = pt.Position.Y - pe.Diameter / 2;
                pe.RenderTransform = ttpe;

                pointerCanvas.Children.Add(pe);
            }
 
            e.Handled = true;
       }

        /// <summary>
        /// The pointer moved event handler.
        /// </summary>
        /// <param name="sender">Source of the pointer event.</param>
        /// <param name="e">Event args for the pointer routed event.</param>
        private void MainPage_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint pt = e.GetCurrentPoint(pointerCanvas);

            if (ellipses.ContainsKey(pt.PointerId))
            { 
                TranslateTransform translate = new TranslateTransform();
                translate.X = pt.Position.X - pe.Diameter / 2;
                translate.Y = pt.Position.Y - pe.Diameter / 2;

                ellipses[pt.PointerId].RenderTransform = translate;
            }

            e.Handled = true;
        }

        /// <summary>
        /// The pointer released event handler - called on PointerReleased, 
        /// PointerCanceled, and PointerCaptureLost events.
        /// </summary>
        /// <param name="sender">Source of the pointer event.</param>
        /// <param name="e">Event args for the pointer routed event.</param>
        private void MainPage_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            // release the pointer from the target.
            pointerCanvas.ReleasePointerCapture(e.Pointer);

            PointerPoint pt = e.GetCurrentPoint(pointerCanvas);

            if (pt.Properties.IsPrimary == true)
            {
                PointerPrimary.Text = "n/a";
                primaryExists = false;
            }

            if (ellipses.ContainsKey(pt.PointerId))
            {
                pointerCanvas.Children.Remove(ellipses[pt.PointerId]);
                ellipses.Remove(pt.PointerId);
                PointerCounter.Text = ellipses.Count.ToString();
            }

            e.Handled = true;
        }

        /// <summary>
        /// The pointer exited event handler.
        /// No-op if pointer is in contact already.
        /// </summary>
        /// <param name="sender">Source of the pointer event.</param>
        /// <param name="e">Event args for the pointer routed event.</param>
        private void MainPage_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (!e.Pointer.IsInContact)
            {
                // release the pointer from the target.
                pointerCanvas.ReleasePointerCapture(e.Pointer);

                PointerPoint pt = e.GetCurrentPoint(pointerCanvas);

                if (pt.Properties.IsPrimary == true)
                {
                    PointerPrimary.Text = "n/a";
                    primaryExists = false;
                }

                if (ellipses.ContainsKey(pt.PointerId))
                {
                    pointerCanvas.Children.Remove(ellipses[pt.PointerId]);
                    ellipses.Remove(pt.PointerId);
                    PointerCounter.Text = ellipses.Count.ToString();
                }
            }

            e.Handled = true;
        }

        /// <summary>
        /// The pointer pressed event handler.
        /// No-op if pointer exists already.
        /// </summary>
        /// <param name="sender">Source of the pointer event.</param>
        /// <param name="e">Event args for the pointer routed event.</param>
        private void MainPage_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // Lock the pointer to the target.
            pointerCanvas.CapturePointer(e.Pointer);

            bool pointerHandled = ellipses.ContainsKey(e.Pointer.PointerId);

            if (!pointerHandled)
            { 

                PointerPoint pt = e.GetCurrentPoint(pointerCanvas);

                pe = new PointerEllipse(pointerCanvas);
                pe.PointerId = pt.PointerId;
                ellipses[pt.PointerId] = pe;
                PointerCounter.Text = ellipses.Count.ToString();

                if (pt.Properties.IsPrimary == true & primaryExists == false)
                {
                    pe.PrimaryPointer = true;
                    pe.PrimaryEllipse = true;
                    primaryExists = true;
                    PointerPrimary.Text = pt.PointerId.ToString();
                }
                else
                {
                    pe.PrimaryPointer = false;
                    pe.PrimaryEllipse = false;
                }

                TranslateTransform ttpe = new TranslateTransform();
                ttpe.X = pt.Position.X - pe.Diameter / 2;
                ttpe.Y = pt.Position.Y - pe.Diameter / 2;
                pe.RenderTransform = ttpe;

                pointerCanvas.Children.Add(pe);
            }

            e.Handled = true;
        }
    }
}
