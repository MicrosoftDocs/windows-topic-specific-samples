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
using System.Threading.Tasks;

namespace QuizGame.Model
{
    public class MockHostCommunicator : IHostCommunicator
    {
        internal MockClientCommunicator Client1 { get; set; }
		internal MockClientCommunicator Client2 { get; set; }

        public async Task EnterLobby()
        {
            // Simulate a periodic broadcast. Loops until the ClientViewModel adds a handler to the GameAvailable event. 
			while (!this.Client1.OnGameAvailable() || !this.Client2.OnGameAvailable()) { await Task.Delay(100); }
        }

        public void LeaveLobby()
        {
            // No need to do anything in the mock version.
        }

        public async Task SendQuestion(Question question)
        {
            this.Client1.OnNewQuestionAvailable(question);
			this.Client2.OnNewQuestionAvailable(question);
		}

        public event EventHandler<PlayerEventArgs> PlayerJoined = delegate { };

        internal void OnPlayerJoined(string playerName)
        {
            this.PlayerJoined(this, new PlayerEventArgs { PlayerName = playerName });
        }

        public event EventHandler<PlayerEventArgs> PlayerDeparted = delegate { };

        internal void OnPlayerDeparted(string playerName)
        {
            this.PlayerDeparted(this, new PlayerEventArgs { PlayerName = playerName });
        }

        public event EventHandler<AnswerReceivedEventArgs> AnswerReceived = delegate { };

        internal void OnAnswerReceived(string playerName, int answerIndex)
        {
            this.AnswerReceived(this, new AnswerReceivedEventArgs { 
                PlayerName = playerName, AnswerIndex = answerIndex });
        }

    }
}
