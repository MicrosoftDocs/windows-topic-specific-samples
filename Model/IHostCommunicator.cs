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
    public interface IHostCommunicator
    {
        // start broadcasting, accepting players
        Task EnterLobby();

        // stop broadcasting, stop accepting players
        void LeaveLobby();

        Task SendQuestion(Question question);

        // sender = this, args = PlayerEventArgs
        event EventHandler<PlayerEventArgs> PlayerJoined;

        // sender = this, args = PlayerEventArgs
        event EventHandler<PlayerEventArgs> PlayerDeparted;

        // sender = this, args = AnswerReceivedEventArgs
        event EventHandler<AnswerReceivedEventArgs> AnswerReceived;
    }

    public class PlayerEventArgs : EventArgs { public string PlayerName { get; set; } }
    public class AnswerReceivedEventArgs : PlayerEventArgs { public int AnswerIndex { get; set; } }
}
