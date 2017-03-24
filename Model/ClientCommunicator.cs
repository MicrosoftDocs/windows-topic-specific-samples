/* 
    Copyright (c) Microsoft Corporation. All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all code Samples for Windows Store apps and Windows Phone Store apps, visit http://code.msdn.microsoft.com/windowsapps
  
*/

using P2PHelper;
using System;

namespace QuizGame.Model
{
    /// <summary>
    /// Provides a game-oriented adapter to the P2PSessionClient class. 
    /// </summary>
    public sealed class ClientCommunicator : IClientCommunicator
    {
        public event EventHandler GameAvailable = delegate { };
        public event EventHandler<QuestionEventArgs> NewQuestionAvailable = delegate { };

        private P2PSessionClient Client { get; set; }

        public ClientCommunicator(P2PSessionClient client)
        {
            this.Client = client;
            this.Client.HostAvailable += (s, e) => this.GameAvailable(this, EventArgs.Empty);
            this.Client.MessageReceived += (s, e) => this.NewQuestionAvailable(this,
                new QuestionEventArgs { Question = e.DeserializedMessage<Question>() });
        }

        public async void Initialize() 
        { 
            await this.Client.ListenForP2PSession(P2PSession.SessionType.LocalNetwork); 
        }

        public async void JoinGame(string playerName)
        {
            await Client.SendMessage(new HostCommandData { 
                PlayerName = playerName, Command = Command.Join }, typeof(HostCommandData));
        }

        public async void LeaveGame(string playerName)
        {
            await Client.SendMessage(new HostCommandData {
                PlayerName = playerName, Command = Command.Leave }, typeof(HostCommandData));
        }

        public async void AnswerQuestion(string playerName, int option)
        {
            await Client.SendMessage(new HostCommandData { 
                PlayerName = playerName, Command = Command.Answer, Data = option }, typeof(HostCommandData));
        }

    }
}
