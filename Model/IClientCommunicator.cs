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
    public interface IClientCommunicator
    {
        void Initialize();
        
        // Adds the specified player to the current game.
        void JoinGame(string playerName);

        // Removes the specified player from the current game.
        void LeaveGame(string playerName);

        // Submits an answer to the current question.
        void AnswerQuestion(string playerName, int option);

        // Occurs when a game is available for joining. 
        event EventHandler GameAvailable;

        // Occurs when new question data has arrived.
        event EventHandler<QuestionEventArgs> NewQuestionAvailable;
    }
}
