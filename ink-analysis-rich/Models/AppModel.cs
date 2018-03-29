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
using System.Linq;

using Windows.UI.Input.Inking;

namespace Analysis.Models
{
    class AppModel
    {
        public InkStrokeContainer StrokeContainer { get; } = new InkStrokeContainer();
        //public InkStrokeContainer StrokeContainer { get; }

        //public List<RecognizedShape> Drawings { get; set; }

        public AppModel()
        {
            //StrokeContainer = new InkStrokeContainer();
            //Drawings = new List<RecognizedShape>();
        }

        //public void CopyInk(InkStrokeContainer inkStrokeContainer)
        //{
        //    List<InkStroke> inkStrokes = inkStrokeContainer.GetStrokes().ToList();
        //    foreach (InkStroke stroke in inkStrokes)
        //    {
        //        StrokeContainer.AddStroke(stroke.Clone());
        //    }
        //    //this.StrokeContainer.AddStrokes(inkStrokes);
        //}

        //public void CopyInk(InkStrokeContainer inkStrokeContainer) =>
        //    StrokeContainer.AddStrokes(inkStrokeContainer.GetStrokes().Select(stroke => stroke.Clone()));

        public void CopyInkStroke(InkStroke inkStroke) =>
            StrokeContainer.AddStroke(inkStroke.Clone());


        public void RevertAnalysis(InkStrokeContainer inkStrokeContainer)
        {
            List<InkStroke> inkStrokes = StrokeContainer.GetStrokes().ToList();
            foreach (InkStroke stroke in inkStrokes)
            {
                inkStrokeContainer.AddStroke(stroke.Clone());
            }
            StrokeContainer.Clear();
        }



        //public void RevertAnalysis()
        //{
        //    MainPage.inkStrokeContainer.AddStrokes(StrokeContainer.GetStrokes().Select(stroke => stroke.Clone()));
        //    StrokeContainer.Clear();
        //}

    }
}
