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
using Windows.Networking.Sockets;

namespace P2PHelper
{
    public class P2PSessionClient : P2PSession
    {
        public event EventHandler HostAvailable = delegate { };

        private P2PHost ConnectedHost { get; set; }

        // TODO implement IDisposable to dispose this.
        // An instance of the TCP listener, kept for cleanup purposes.
        private StreamSocketListener SessionListener { get; set; }

        public P2PSessionClient(P2PSessionConfigurationData config) : base(config)
        {
            this.SessionListener = new StreamSocketListener();
        }

        public async Task ListenForP2PSession(SessionType sessionType)
        {
            if (sessionType != SessionType.LocalNetwork) throw new NotSupportedException(
                "SessionType.LocalNetwork is the only SessionType supported.");

            this.SessionHost = false;
            this.SessionListener.ConnectionReceived += async (s, e) => 
                this.OnMessageReceived(await this.RetrieveMessage(e.Socket));
            await this.SessionListener.BindEndpointAsync(null, this.Settings.tcpPort);
            this.InitializeNetworkInfo();

            await this.InitializeMulticast(remoteAddress =>
            {
                if (this.ConnectedHost.hostTcpIP == null)
                {
                    this.ConnectedHost = new P2PHost { hostTcpIP = remoteAddress };
                    this.OnHostAvailable();
                }
            });
        }

        // TODO can this overload be eliminated in favor of optional param in the other overload?
        // Send an object.
        public async Task<bool> SendMessage(object message)
        {
            // TODO check for null on ConnectedHost etc.
            return await base.SendMessage(message, this.ConnectedHost.hostTcpIP, this.Settings.tcpPort, typeof(object));
        }

        // Send a custom object.
        public async Task<bool> SendMessage(object message, Type type)
        {
            return await base.SendMessage(message, this.ConnectedHost.hostTcpIP, this.Settings.tcpPort, type);
        }

        protected void OnHostAvailable()
        {
            this.HostAvailable(this, EventArgs.Empty);
        }
    }
}