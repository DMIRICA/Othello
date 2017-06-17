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
            Connect("192.168.0.4", 2017);
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
                _Socket.BeginConnect(new IPEndPoint(IPAddress.Parse(ipAddress), port), ConnectCallback, _Socket);
                _Socket.BeginReceive(_buffer, 0, 1024, SocketFlags.None, new AsyncCallback(ReceiveCallback), _Socket);
            }
            catch (SocketException e)
            {
                Debug.Log("Connect exception " + e.Message);
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                _Socket.EndConnect(ar);
                
            }
            catch (SocketException e)
            {
                Debug.Log("Connect callback exception " + e.Message);
            }

        }

        private void ReceiveCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            try
            {
                int buffLength = _Socket.EndReceive(AR);
                byte[] packet = new byte[buffLength];
                Array.Copy(_buffer, packet, packet.Length);
                
                PacketHandler.Handle(packet, _Socket);
                
                _buffer = new byte[1024];
                _Socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            }
            catch (Exception e)
            {
                Debug.Log("ReceiveCallback exception " + e.Message);
            }
        }

        public void SendPacket(byte[] packet)
        {

            if (_Socket.Connected)
            {
                try
                {
                    _Socket.BeginSend(packet, 0, packet.Length, SocketFlags.None, new AsyncCallback(SendCallBack), _Socket);

                }
                catch (SocketException e)
                {
                    Debug.Log("Sendpacket exception " + e.Message);
                }
            }

        }

        private void SendCallBack(IAsyncResult AR)
        {
            try
            {
                Socket resceiver = (Socket)AR.AsyncState;
                resceiver.EndSend(AR);
            }
            catch (SocketException e)
            {
                Debug.Log("Sendpacket callback exception " + e.Message);
            }

        }

    }
}
