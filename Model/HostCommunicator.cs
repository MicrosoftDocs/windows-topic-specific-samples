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
using System.Threading.Tasks;


namespace QuizGame.Model
{
    /// <summary>
    /// Provides a game-oriented adapter to the P2PSessionHost class. 
    /// </summary>
    public sealed class HostCommunicator : IHostCommunicator
    {
        public event EventHandler<PlayerEventArgs> PlayerJoined = delegate { };
        public event EventHandler<PlayerEventArgs> PlayerDeparted = delegate { };
        public event EventHandler<AnswerReceivedEventArgs> AnswerReceived = delegate { };

        private P2PSessionHost Host { get; set; }
        private Action<HostCommandData>[] CommandActions { get; set; }

        public HostCommunicator(P2PSessionHost host)
        {
            this.Host = host;
            this.CommandActions = new Action<HostCommandData>[] { 
                commandData => this.PlayerJoined(this, new PlayerEventArgs { PlayerName = commandData.PlayerName }),
                commandData => this.PlayerDeparted(this, new PlayerEventArgs { PlayerName = commandData.PlayerName }),
                commandData => this.AnswerReceived(this, new AnswerReceivedEventArgs { 
                    PlayerName = commandData.PlayerName, AnswerIndex = (int)commandData.Data }) };
        }

        // Start broadcasting, start accepting players.
        public async Task EnterLobby()
        {
            if (await Host.CreateP2PSession(P2PSession.SessionType.LocalNetwork))
            {
                this.Host.MessageReceived += (s, e) => 
                {
                    var commandData = e.DeserializedMessage<HostCommandData>();
                    this.CommandActions[(int)commandData.Command](commandData);
                };
                this.Host.StartAcceptingConnections();
            }
        }

        // Stop broadcasting, stop accepting players.
        public void LeaveLobby()
        {
            this.Host.StopAcceptingConnections();
        }

        public async Task SendQuestion(Question question)
        {
            await this.Host.SendMessageToAll(question, typeof(Question));
        }

    }
}
