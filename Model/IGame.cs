/* 
    Copyright (c) Microsoft Corporation. All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all code Samples for Windows Store apps and Windows Phone Store apps, visit http://code.msdn.microsoft.com/windowsapps
  
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace QuizGame.Model
{
    public interface IGame : INotifyPropertyChanged
    {
        event EventHandler<QuestionEventArgs> NewQuestionAvailable;
        void AddPlayer(string playerName);
        Question CurrentQuestion { get; }
        GameState GameState { get; set; }
        Dictionary<string, int> GetResults();
        bool IsGameOver { get; }
        void NextQuestion();
        ObservableCollection<string> PlayerNames { get; set; }
        List<Question> Questions { get; }
        void RemovePlayer(string playerName);
        void StartGame();
        bool SubmitAnswer(string playerName, int answerIndex);
        Dictionary<string, Dictionary<Question, int?>> SubmittedAnswers { get; }
        string Winner { get; }
    }

    public class QuestionEventArgs : EventArgs { public Question Question { get; set; } }

}
