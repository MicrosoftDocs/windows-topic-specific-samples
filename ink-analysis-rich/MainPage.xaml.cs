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
using Windows.UI.Xaml.Media;

using Analysis.Models;
using Windows.UI.Input.Inking.Analysis;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;
using Windows.UI;

namespace Analysis 
{
    /// <summary>
    /// The application's single UI page.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Reference to our application model (with store and revert functions).
        private AppModel appModel = new AppModel();

        // Reference to our shape model (with rendering attributes).
        //private ShapeModel shape;

        // The object that performs ink analysis.
        private Ink​Analyzer inkAnalyzer;

        public bool boolHighlightWritingRegions = false;
        public bool boolHighlightParagraphs = false;
        public bool boolHighlightTextLines = false;
        public bool boolHighlightWords = false;
        
        /// <summary>
        /// Initialize the UI page.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();

            // Set supported inking device types.
            inkCanvas.InkPresenter.InputDeviceTypes =
                Windows.UI.Core.CoreInputDeviceTypes.Touch | 
                Windows.UI.Core.CoreInputDeviceTypes.Mouse |
                Windows.UI.Core.CoreInputDeviceTypes.Pen;

            inkAnalyzer = new InkAnalyzer();
        }

        /// <summary>
        /// Convert ink strokes to text and shape objects.
        /// </summary>
        /// <param name="sender">Source of the click event</param>
        /// <param name="e">Event args for the button click routed event</param>
        private async void AnalyzeAllStrokes_Click(object sender, RoutedEventArgs e)
        {
            // Initialize the ink analyzer.
            inkAnalyzer.ClearDataForAllStrokes();
            // Retrieve all ink strokes on the ink canvas.
            IReadOnlyList<InkStroke> inkStrokes = inkCanvas.InkPresenter.StrokeContainer.GetStrokes();

            // Ensure an ink stroke is present.
            if (inkStrokes.Count > 0)
            {
                // Store the ink strokes to support revert.
                //appModel.CopyInk(inkCanvas.InkPresenter.StrokeContainer);

                // Add the ink stroke data for analysis.
                inkAnalyzer.AddDataForStrokes(inkStrokes);

                // If you're only interested in a specific type of recognition,
                // such as writing or drawing, you can constrain recognition 
                // using the SetStrokDataKind method as follows:
                // foreach (var stroke in strokesText)
                // {
                //     inkAnalyzer.SetStrokeDataKind(stroke.Id, InkAnalysisStrokeKind.Writing);
                // }
                // This can improve both efficiency and recognition results.
                // In this example, we try to recognizing both, so the platform default 
                // of "InkAnalysisStrokeKind.Auto" is used.
                InkAnalysisResult inkAnalysisResults = await inkAnalyzer.AnalyzeAsync();

                // Have ink strokes on the canvas changed?
                if (inkAnalysisResults.Status == InkAnalysisStatus.Updated)
                {
                    if (null != inkAnalyzer.AnalysisRoot.Children
                    && inkAnalyzer.AnalysisRoot.Children.Count > 0)
                    {
                        foreach (IInkAnalysisNode node in inkAnalyzer.AnalysisRoot.Children)
                        {
                            switch (node.Kind)
                            {
                                // Strokes recognized as writing.
                                case InkAnalysisNodeKind.WritingRegion:
                                    // Iterate through the writing nodes to get each InkWord.
                                    IterateText(node);
                                    break;

                                // Strokes recognized as drawing.
                                case InkAnalysisNodeKind.InkDrawing:
                                    InkAnalysisInkDrawing drawing = node as InkAnalysisInkDrawing;
                                    // Create stroke ID store.
                                    IReadOnlyList<uint> strokeIds = drawing.GetStrokeIds();

                                    // Retrieve the stroke for the current drawing node.
                                    InkStroke drawingStroke = 
                                        inkCanvas.InkPresenter.StrokeContainer.GetStrokeById(strokeIds[0]);

                                    // Create a new ShapeModel object.
                                    //shape = new ShapeModel(drawing, drawingStroke.DrawingAttributes);

                                    ShapeModel shape = 
                                        (drawing.DrawingKind == InkAnalysisDrawingKind.Drawing) ? 
                                        null : new ShapeModel(drawing, drawingStroke.DrawingAttributes);

                                    if (null != shape)
                                    {
                                        // Draw a shape object on the recognition canvas.
                                        DrawShape(shape);

                                        // Select shape ink strokes for subsequent deletion from the ink canvas.
                                        // A recognized shape has a bounding rect with four points.
                                        if (null != strokeIds && shape.RotatedBoundingRect.Count > 0)
                                        {
                                            foreach (uint strokeId in strokeIds)
                                            {
                                                InkStroke s = 
                                                    inkCanvas.InkPresenter.StrokeContainer.GetStrokeById(strokeId);
                                                s.Selected = true;
                                                // Delete selected ink strokes from the ink canvas.
                                                inkCanvas.InkPresenter.StrokeContainer.DeleteSelected();
                                                appModel.CopyInkStroke(s);
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Iterate through all writing ink analysis nodes until InkWord node.
        /// </summary>
        /// <param name="writingNode">The InkAnalysisNode</param>
        private void IterateText(IInkAnalysisNode writingNode)
        {
            if (null != writingNode.Children)
            {
                foreach (IInkAnalysisNode child in writingNode.Children)
                {
                    // If an InkWord node, begin analysis.
                    if (child.Kind == InkAnalysisNodeKind.InkWord)
                    {
                        InkAnalysisInkWord inkWord = (InkAnalysisInkWord)child;
                        // Draw a TextBlock object on the recognitionCanvas.
                        DrawText(inkWord.RecognizedText, inkWord.BoundingRect);

                        // Select all ink strokes on the ink canvas for subsequent deletion.
                        foreach (var strokeId in writingNode.GetStrokeIds())
                        {
                            var textStroke =
                                inkCanvas.InkPresenter.StrokeContainer.GetStrokeById(strokeId);
                            textStroke.Selected = true;
                            // Delete selected ink strokes from the ink canvas.
                            inkCanvas.InkPresenter.StrokeContainer.DeleteSelected();
                            appModel.CopyInkStroke(textStroke);
                        }
                    }
                    else
                        IterateText(child);
                }
            }
        }

        /// <summary>
        /// Draw ink recognition text string on the recognitionCanvas.
        /// </summary>
        /// <param name="recognizedText">The string returned by text recognition.</param>
        /// <param name="boundingRect">The bounding rect of the original ink writing.</param>
        private void DrawText(string recognizedText, Rect boundingRect)
        {
            TextBlock text = new TextBlock();
            TranslateTransform translateTransform = new TranslateTransform();
            TransformGroup transformGroup = new TransformGroup();

            translateTransform.X = boundingRect.Left;
            translateTransform.Y = boundingRect.Top;
            transformGroup.Children.Add(translateTransform);
            text.RenderTransform = transformGroup;

            text.Text = recognizedText;
            text.FontSize = boundingRect.Height;

            recognitionCanvas.Children.Add(text);
        }

        /// <summary>
        /// Draw corresponding shape object on the recognitionCanvas.
        /// </summary>
        private void DrawShape(ShapeModel shape)
        {
            switch (shape.DrawingKind)
            {
                case InkAnalysisDrawingKind.Ellipse:
                case InkAnalysisDrawingKind.Circle:
                    {
                        Ellipse ellipse = new Ellipse();
                        var brush = new SolidColorBrush(Windows.UI.ColorHelper.FromArgb(255, 0, 0, 255));
                        ellipse.Stroke = brush;
                        ellipse.StrokeThickness = 2;

                        // Ellipses are constructed from four points, clockwise.
                        // Points 0 and 2 define one axis, while points 1 and 3 
                        // define the other axis.
                        IReadOnlyList<Point> points = shape.Source.Points;

                        // Calculate the geometric center of the ellipse.
                        Point center = 
                            new Point((points[0].X + points[2].X) / 2.0, 
                            (points[0].Y + points[2].Y) / 2.0);

                        // Assign the length of one axis to one of the dimensions of the ellipse (Width).
                        ellipse.Width = AxisLength(points[0], points[2]);

                        // Assign the length of the other axis to the other ellipse dimension (Height).
                        ellipse.Height = AxisLength(points[1], points[3]);

                        // Apply multiple transforms (rotate and translate).
                        // A composite transform applies scale, skew, rotate, then translate, in order.
                        // Use TransformGroup, if you want the transforms applied in a different order.
                        var compositeTransform = new CompositeTransform();

                        // Use the angle of the Width axis to calculate the 
                        // clockwise angle of rotation to draw our ellipse.
                        double rotationAngle = 
                            Math.Atan2(points[2].Y - points[0].Y, 
                            points[2].X - points[0].X);

                        // Convert radians to degrees.
                        compositeTransform.Rotation = rotationAngle * 180.0 / Math.PI;

                        // Set the center of rotation to the center of our ellipse.
                        compositeTransform.CenterX = ellipse.Width / 2.0;
                        compositeTransform.CenterY = ellipse.Height / 2.0;

                        // Set the location of the center of our ellipse.
                        compositeTransform.TranslateX = center.X - ellipse.Width / 2.0;
                        compositeTransform.TranslateY = center.Y - ellipse.Height / 2.0;

                        ellipse.RenderTransform = compositeTransform;

                        recognitionCanvas.Children.Add(ellipse);
                    }
                    break;
                case InkAnalysisDrawingKind.Rectangle:
                case InkAnalysisDrawingKind.Triangle:
                case InkAnalysisDrawingKind.Diamond:
                case InkAnalysisDrawingKind.EquilateralTriangle:
                case InkAnalysisDrawingKind.Hexagon:
                case InkAnalysisDrawingKind.IsoscelesTriangle:
                case InkAnalysisDrawingKind.Parallelogram:
                case InkAnalysisDrawingKind.Pentagon:
                case InkAnalysisDrawingKind.Quadrilateral:
                case InkAnalysisDrawingKind.RightTriangle:
                case InkAnalysisDrawingKind.Square:
                case InkAnalysisDrawingKind.Trapezoid:
                    {
                        Polygon polygon = new Polygon();
                        foreach (Point p in shape.RotatedBoundingRect)
                        {
                            polygon.Points.Add(p);
                        }

                        polygon.Stroke = new SolidColorBrush(shape.DrawingAttributes.Color);
                        polygon.StrokeThickness = shape.DrawingAttributes.Size.Height;

                        recognitionCanvas.Children.Add(polygon);
                    }
                    break;
                // Ink stroke is not recognized as a supported shape.
                case InkAnalysisDrawingKind.Drawing:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Get the distance between the axis points of an ellipse.
        /// </summary>
        /// <param name="p0">Starting point</param>
        /// <param name="p1">Ending point</param>
        /// <returns></returns>
        static double AxisLength(Point p0, Point p1)
        {
            double dX = p1.X - p0.X;
            double dY = p1.Y - p0.Y;
            return Math.Sqrt(dX * dX + dY * dY);
        }

        /// <summary>
        /// Revert text and shape objects back to ink strokes.
        /// </summary>
        /// <param name="sender">Source of the click event</param>
        /// <param name="e">Event args for the button click routed event</param>
        private void RevertAnalysis_Click(object sender, RoutedEventArgs e)
        {
            // Clear content from recognition canvas.
            recognitionCanvas.Children.Clear();

            appModel.RevertAnalysis(inkCanvas.InkPresenter.StrokeContainer);
        }

        /// <summary>
        /// Handle button clicks for highlighting ink writing.
        /// </summary>
        /// <param name="sender">Source of the click event</param>
        /// <param name="e">Event args for the button click routed event</param>
        private async void HighlightTextStrokes_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;

            // Clear the recognition canvas and reset view model.
            recognitionCanvas.Children.Clear();
            boolHighlightWritingRegions = false;
            boolHighlightParagraphs = false;
            boolHighlightTextLines = false;
            boolHighlightWords = false;

            // Set the highlight type based on the button clicked.
            switch (b.Name)
            {
                case "highlightWritingRegions":
                    boolHighlightWritingRegions = true;
                    await HighlightText();
                    break;
                case "highlightParagraphs":
                    boolHighlightParagraphs = true;
                    await HighlightText();
                    break;
                case "highlightTextLines":
                    boolHighlightTextLines = true;
                    await HighlightText();
                    break;
                case "highlightWords":
                    boolHighlightWords = true;
                    await HighlightText();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Retrieve and highlight the ink analysis nodes.
        /// </summary>
        /// <returns></returns>
        private async Task HighlightText()
        {
            // Initialize the ink analyzer.
            inkAnalyzer.ClearDataForAllStrokes();
            // Add the ink stroke data for analysis.
            // If you're only interested in a specific type of 
            // recognition, such as writing or drawing, constrain 
            // recognition using the SetStrokDataKind method.
            // This can improve both efficiency and recognition results.
            foreach(InkStroke stroke in inkCanvas.InkPresenter.StrokeContainer.GetStrokes())
            {
                inkAnalyzer.AddDataForStroke(stroke);
                inkAnalyzer.SetStrokeDataKind(stroke.Id, InkAnalysisStrokeKind.Writing);
            }
            InkAnalysisResult result = await inkAnalyzer.AnalyzeAsync();
            // Have ink strokes on the canvas changed?
            if (result.Status == InkAnalysisStatus.Updated)
            {
                if (null != inkAnalyzer.AnalysisRoot.Children
                    && inkAnalyzer.AnalysisRoot.Children.Count > 0)
                {
                    foreach (IInkAnalysisNode node in inkAnalyzer.AnalysisRoot.Children)
                    {
                        // Highlight text ink strokes.
                        await DrawHighlight(node);
                    }
                }
            }
        }

        /// <summary>
        /// Iterate through the ink analysis nodes (to each InkWord leaf node).
        /// Draw highlight for specified ink writing node type.
        /// </summary>
        /// <param name="node">The ink analysis node being processed.</param>
        /// <returns></returns>
        private async Task DrawHighlight(IInkAnalysisNode node)
        {
            bool complete = false;
            // Set the stroke and fill attributes of the highlight.
            switch (node.Kind)
            {
                case InkAnalysisNodeKind.WritingRegion:
                    if (boolHighlightWritingRegions)
                    {
                        // Add a polygon for the highlight (outlined or filled box).
                        CreateHighlightPolygon(node, Colors.Red, 2, Colors.Transparent);
                        complete = true;
                    }
                    break;
                case InkAnalysisNodeKind.Paragraph:
                    if (boolHighlightParagraphs)
                    {
                        // Add a polygon for the highlight (outlined or filled box).
                        CreateHighlightPolygon(node, Colors.Blue, 2, Colors.Transparent);
                        complete = true;
                    }
                    break;
                case InkAnalysisNodeKind.Line:
                    if (boolHighlightTextLines)
                    {
                        // Add a polygon for the highlight (outlined or filled box).
                        CreateHighlightPolygon(node, Colors.Green, 2, Colors.Transparent);
                        complete = true;
                    }
                    break;
                case InkAnalysisNodeKind.InkWord:
                    if (boolHighlightWords)
                    {
                        // Add a polygon for the highlight (outlined or filled box).
                        CreateHighlightPolygon(node, Colors.Yellow, 0, Color.FromArgb(0x55, 0xCf, 0xf2, 0x04));
                        complete = true;
                    }
                    break;
            }

            if (null != node.Children && complete == false)
            {
                foreach (IInkAnalysisNode child in node.Children)
                {
                    await DrawHighlight(child);
                }
            }
        }

        /// <summary>
        /// Draw highlight polygon on recognition canvas.
        /// </summary>
        /// <param name="node">The ink analysis node being processed.</param>
        /// <param name="strokecolor">The stroke color of the highlight polygon.</param>
        /// <param name="thickness">The stroke thickness of the highlight polygon.</param>
        /// <param name="fillcolor">The fill color of the highlight polygon.</param>
        private void CreateHighlightPolygon(
            IInkAnalysisNode node, Color strokecolor, int thickness, Color fillcolor)
        {
            // Create a polygon for the highlight (outlined or filled box).
            Polygon polygon = new Polygon();
            // Duplicate the points of the ink analysis node for the polygon.
            foreach (Point p in node.RotatedBoundingRect)
            {
                polygon.Points.Add(p);
            }
            polygon.Stroke = new SolidColorBrush(strokecolor);
            polygon.StrokeThickness = thickness;
            polygon.Fill = new SolidColorBrush(fillcolor);
            if (null != polygon)
            {
                recognitionCanvas.Children.Add(polygon);
            }
        }
    }
}
