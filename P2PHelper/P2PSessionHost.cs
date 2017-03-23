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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Networking.Sockets;

namespace P2PHelper
{
    public class P2PSessionHost : P2PSession
    {
        public List<P2PClient> ClientList { get; set; }

        private StreamSocketListener SessionListener { get; set; }
        private Timer Timer { get; set; }

        public P2PSessionHost(P2PSessionConfigurationData config) : base(config)
        {
            this.SessionListener = new StreamSocketListener();
            this.ClientList = new List<P2PClient>();
        }

        public async Task<bool> CreateP2PSession(SessionType type)
        {
            if (type != SessionType.LocalNetwork) throw new NotSupportedException(
                "SessionType.LocalNetwork is the only SessionType supported.");

            this.SessionHost = true;
            this.SessionListener.ConnectionReceived += SessionListener_ConnectionReceived;  
            await this.SessionListener.BindEndpointAsync(null, Settings.tcpPort);
            this.InitializeNetworkInfo();
            return await this.InitializeMulticast(null);
        }

        private bool AcceptingConnections { get; set; }
        public void StartAcceptingConnections()
        {
            // TODO replace "state" with the right thing
            AcceptingConnections = true;
            this.Timer = new Timer(async state => await SendMulticastMessage(""), null, 0, 500);   
        }

        // TODO don't dispose timer here. implement IDisposable
        public void StopAcceptingConnections()
        {
            AcceptingConnections = false;
            this.Timer.Dispose();// TODO Timer.Stop instead
            //this.SessionListener.ConnectionReceived -= SessionListener_ConnectionReceived;
        }

        private async void SessionListener_ConnectionReceived(StreamSocketListener sender, 
            StreamSocketListenerConnectionReceivedEventArgs args)
        {
            byte[] message = await RetrieveMessage(args.Socket);
            if(AcceptingConnections)
            { 
                var newClient = new P2PClient { clientTcpIP = args.Socket.Information.RemoteAddress.ToString() };
                if (!this.ClientList.Any(client => client.clientTcpIP == newClient.clientTcpIP))
                {
                    this.ClientList.Add(newClient);
                    this.OnConnectionComplete();
                }
            }

            OnMessageReceived(message);
        }

        protected async Task SendMulticastMessage(string output)
        {
            using (var multicastOutput = await this.MulticastSocket.GetOutputStreamAsync(
                new Windows.Networking.HostName(Settings.multicastIP), 
                this.MulticastSocket.Information.LocalPort))
            {
                await multicastOutput.WriteAsync(Encoding.UTF8.GetBytes(output).AsBuffer());
            }
        }

        public async Task<bool> SendMessage(P2PClient client, object message, Type type = null)
        {
            return await base.SendMessage(message, client.clientTcpIP, Settings.tcpPort, type ?? typeof(object));
        }

        public async Task<bool> SendMessageToAll(object message, Type type = null)
        {
            var messageTasks = this.ClientList.Select(client => this.SendMessage(client, message, type));

            // When all the tasks complete, return true if they all succeeded. 
            return (await Task.WhenAll(messageTasks)).All(value => { return value; });
        }
    }
}