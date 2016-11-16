using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System;

namespace Assets.Scripts.Networking
{
    public class ClientConnection
    {

        private Socket _Socket;
        private byte[] _buffer = new byte[1024];

        public ClientConnection()
        {
            _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Connect("89.136.5.12", 2016);
        }

        public bool isConnected()
        {
            return _Socket.Connected;

        }

        public void CloseSocket()
        {
            if (isConnected())
            {
                _Socket.Shutdown(SocketShutdown.Both);
                _Socket.Close();
            }
        }

        public void Connect(string ipAddress, int port)
        {
            try
            {
                _Socket.BeginConnect(new IPEndPoint(IPAddress.Parse(ipAddress), port), ConnectCallback, null);
                _Socket.BeginReceive(_buffer, 0, 1024, SocketFlags.None, ReceiveCallback, _Socket);
            }
            catch (Exception e)
            {
                
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                _Socket.EndConnect(ar);
                
            }
            catch (Exception e)
            {
            }

        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int buffLength = _Socket.EndReceive(ar);
                byte[] packet = new byte[buffLength];
                Array.Copy(_buffer, packet, packet.Length);

                PacketHandler.Handle(packet, _Socket);

                _buffer = new byte[1024];
                _Socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, null);
            }
            catch (Exception e) {}
        }

        public void SendPacket(byte[] packet)
        {
            try
            {
                _Socket.BeginSend(packet, 0, packet.Length, SocketFlags.None, SendCallBack, _Socket);

            }
            catch (Exception e)
            {

            }

        }

        private void SendCallBack(IAsyncResult AR)
        {
            try { _Socket.EndSend(AR); }
            catch (Exception e) { }

        }

    }
}
