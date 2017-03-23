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

namespace QuizGame.Model
{
    public class MockClientCommunicator : IClientCommunicator
    {
        internal MockHostCommunicator Host { get; set; }

        public void JoinGame(string playerName)
        {
            this.Host.OnPlayerJoined(playerName);
        }

        public void Initialize()
        {
            // No need to do anything in the mock version.
        }
        
        public void LeaveGame(string playerName)
        {
            this.Host.OnPlayerDeparted(playerName);
        }

        public void AnswerQuestion(string playerName, int answerIndex)
        {
            this.Host.OnAnswerReceived(playerName, answerIndex);
        }

        public event EventHandler GameAvailable;

        internal bool OnGameAvailable()
        {
            var gameAvailable = this.GameAvailable;
            if (gameAvailable != null)
            {
                gameAvailable(this, EventArgs.Empty);
                return true;
            }
            return false;
        }

        public event EventHandler<QuestionEventArgs> NewQuestionAvailable = delegate { };

        internal void OnNewQuestionAvailable(Question newQuestion)
        {
            this.NewQuestionAvailable(this, new QuestionEventArgs { Question = newQuestion });
        }

    }
}
