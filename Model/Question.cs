/* 
    Copyright (c) Microsoft Corporation. All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all code Samples for Windows Store apps and Windows Phone Store apps, visit http://code.msdn.microsoft.com/windowsapps
  
*/

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace QuizGame.Model
{
    [DataContract]
    public class Question
    {
        [DataMember] public string Text { get; set; }
        [DataMember] public List<string> Options { get; set; }
        [DataMember] public int CorrectAnswerIndex { get; set; }
    }
}
