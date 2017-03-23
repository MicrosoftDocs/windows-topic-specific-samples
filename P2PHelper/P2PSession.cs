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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

// TODO is there any multicast stuff still in this base class?

namespace P2PHelper
{
    public abstract class P2PSession
    {
        public class MessageEventArgs : EventArgs
        {
            public byte[] Message { get; set; }
            public T DeserializedMessage<T>() where T : class { return Deserialize(this.Message, typeof(T)) as T; }
        }

        public struct P2PSessionConfigurationData
        {
            // Specifies the IP address to use for multicast send/receive.
            public string multicastIP;

            // Specifies the port to use for multicast send/receive.
            public string multicastPort;

            // Specifies the port to use for TCP communication.
            public string tcpPort;
        }

        public struct P2PClient
        {
            // TODO what's the default for structs?
            public string name;
            public string clientTcpIP;
        }

        public struct P2PHost
        {
            public string hostTcpIP;
        }

        public enum SessionType
        {
            LocalNetwork // No other session types are currently supported.
        }

        public event EventHandler ConnectionComplete = delegate { };
        public event EventHandler<MessageEventArgs> MessageReceived = delegate { };

        protected bool SessionHost { get; set; }
        protected ConnectionProfile SessionProfile { get; set; }
        protected P2PSessionConfigurationData Settings { get; set; }
        protected DatagramSocket MulticastSocket { get; set; }

        public P2PSession(P2PSessionConfigurationData config) 
        { 
            this.Settings = config; 
        }

        protected void InitializeNetworkInfo()
        {
            // Get all profiles available for connection, active or otherwise, on the local machine.
            var connectionProfiles = Windows.Networking.Connectivity.NetworkInformation.GetConnectionProfiles();

            // Try to get an active Ethernet profile.
            this.SessionProfile = connectionProfiles.FirstOrDefault(profile =>
                !profile.IsWlanConnectionProfile && !profile.IsWwanConnectionProfile && 
                profile.GetNetworkConnectivityLevel() != NetworkConnectivityLevel.None);

            // If there is no active Ethernet profile, try to get an active WLAN profile.
            if (this.SessionProfile == null) this.SessionProfile = connectionProfiles.FirstOrDefault(profile =>
                profile.IsWlanConnectionProfile && profile.GetNetworkConnectivityLevel() != NetworkConnectivityLevel.None);

            if (this.SessionProfile == null) throw new Exception("Ethernet or WLAN connection must be present.");
        }

        protected async Task<bool> InitializeMulticast(Action<string> onMessageReceived)
        {
            this.MulticastSocket = new DatagramSocket();

            if (onMessageReceived != null) this.MulticastSocket.MessageReceived += 
                (s, e) => onMessageReceived(e.RemoteAddress.ToString());

            bool success = false;
            try
            {
                // Bind to an available UDP port on a specific adapter.
                await this.MulticastSocket.BindServiceNameAsync(this.Settings.multicastPort, this.SessionProfile.NetworkAdapter); 

                this.MulticastSocket.JoinMulticastGroup(new Windows.Networking.HostName(this.Settings.multicastIP));
                success = true;
            }
            catch (Exception) { }

            return success;
        }

        // TODO implement IDisposable instead.
        protected void DisposeMulticast()
        {
            this.MulticastSocket.Dispose();
        }

        protected async Task<bool> SendMessage(object message, string host, string port, Type type)
        {
            bool success = false;
            using (var socket = new StreamSocket())
            {
                try
                {
                    // Establish a connection with the host; verification is not necessary.
                    await socket.ConnectAsync(new Windows.Networking.HostName(host), port);

                    byte[] data = P2PSession.Serialize(message, type);

                    // Send the size of the data first.
                    int size = data.Length;
                    byte[] sizeBuffer = BitConverter.GetBytes(size);
                    await P2PSession.SendDataTCP(socket, sizeBuffer);

                    // Send the serialized data.
                    await P2PSession.SendDataTCP(socket, data);
                    success = true;
                }
                catch (Exception) { }

                return success;
            }
        }

        protected async Task<byte[]> RetrieveMessage(StreamSocket connection)
        {
            // Retrieve the length of the data that is about to be sent.
            int size = 0;
            byte[] sizeBuffer = BitConverter.GetBytes(size);
            await P2PSession.ReceiveDataTCP(connection, sizeBuffer, sizeBuffer.Length);
            size = BitConverter.ToInt32(sizeBuffer, 0);

            // Retrieve the actual data.
            byte[] data = new byte[size];
            await P2PSession.ReceiveDataTCP(connection, data, data.Length);

            return data;
        }

        private static async Task<bool> SendDataTCP(StreamSocket socketConnection, byte[] data)
        {
            bool isSuccessful = false;
            try
            {
               // using (var writer = new DataWriter(socketConnection.OutputStream))
               // { 
                DataWriter writer = new DataWriter(socketConnection.OutputStream);
                    writer.WriteBytes(data);
                    await writer.StoreAsync();
                    await writer.FlushAsync();
                    isSuccessful = true;                    
               // }
            }
            catch (Exception) { }

            return isSuccessful;
        }

        private static async Task<bool> ReceiveDataTCP(StreamSocket socketConnection, byte[] data, int length)
        {
            bool isSuccessful = false;
            try
            {
                //using (var reader = new DataReader(socketConnection.InputStream))
                //{
                DataReader reader = new DataReader(socketConnection.InputStream);
                    // Set inputstream options so that we don't have to know the data size.
                    reader.InputStreamOptions = InputStreamOptions.Partial;
                    await reader.LoadAsync((uint)length);
                    reader.ReadBytes(data);
                    isSuccessful = true;
                //}
            }
            catch (Exception) { }

            return isSuccessful;
        }

        protected static byte[] Serialize(object obj, Type type)
        {
            using (var stream = new MemoryStream())
            {
                new DataContractSerializer(type).WriteObject(stream, obj);
                return stream.ToArray();                
            }
        }

        protected static object Deserialize(byte[] buffer, Type type)
        {
            using (var stream = new MemoryStream(buffer))
            {
                return new DataContractSerializer(type).ReadObject(stream);
            }
        }

        protected void OnConnectionComplete() 
        { 
            this.ConnectionComplete(this, EventArgs.Empty); 
        }

        protected void OnMessageReceived(byte[] message)
        {
            this.MessageReceived(this, new MessageEventArgs { Message = message });
        }

    }
}
