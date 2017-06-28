using Server.Game;
using Server.Packets;
using Server.Protocol;
using Server.Singleton;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Othello
{
    public class Server
    {
        private static Socket _ServerSocket;
        public static readonly List<Socket> _ClientSockets = new List<Socket>();
        private const int _BUFFER_SIZE = 1024;
        private const int _PORT = 2017;
        private static readonly byte[] _Buffer = new byte[_BUFFER_SIZE];


        public void StartServer()
        {
            Console.WriteLine("Setting up server...");
            _ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _ServerSocket.Bind(new IPEndPoint(IPAddress.Any, _PORT));
            _ServerSocket.Listen(50);
            _ServerSocket.BeginAccept(AcceptCallback, null);
            Console.WriteLine("Server setup complete");
        }


        /// Close all connected client (we do not need to shutdown the server socket as its connections
        /// are already closed with the clients)
        public static void CloseAllSockets()
        {
            foreach (Socket socket in _ClientSockets)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }

            _ServerSocket.Close();
        }

        private static void AcceptCallback(IAsyncResult AR)
        {
            Socket socket;

            try
            {
                socket = _ServerSocket.EndAccept(AR);
            }
            catch (ObjectDisposedException) // I cannot seem to avoid this (on exit when properly closing sockets)
            {
                return;
            }
            Console.WriteLine("Client connected, waiting for request...");
            _ClientSockets.Add(socket);
            Singleton.Instance.ListOfPlayers.Insert(0,new Player(socket));
            /*
            if (Singleton.Instance.ListOfPlayers.Count % 2 == 0)
            {
                Singleton.Instance.addNewRoom();
            }
            socket.BeginReceive(_Buffer, 0, _BUFFER_SIZE, SocketFlags.None, ReceiveCallback, socket);
            _ServerSocket.BeginAccept(AcceptCallback, null);
            */
            socket.BeginReceive(_Buffer, 0, _BUFFER_SIZE, SocketFlags.None, ReceiveCallback, socket);
            _ServerSocket.BeginAccept(AcceptCallback, null);
        }

        private static void ReceiveCallback(IAsyncResult AR)
        {
            Socket current = (Socket)AR.AsyncState;
            
            int received;

            try
            {
                received = current.EndReceive(AR);
                if (received > 0)
                {
                    byte[] recBuf = new byte[received];
                    Array.Copy(_Buffer, recBuf, received);
                    PacketHandler.Handle(recBuf, current);
                    if (current.Connected)
                    {
                        current.BeginReceive(_Buffer, 0, _BUFFER_SIZE, SocketFlags.None, ReceiveCallback, current);
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("Client forcefully disconnected" + e.Message);
                current.Close(); // Dont shutdown because the socket may be disposed and its disconnected anyway
                _ClientSockets.Remove(current);
                return;
            }
            
        }

        public static void SendPacket(Socket clientSocket,byte[] packet)
        {
            try
            {
                clientSocket.BeginSend(packet, 0, packet.Length, SocketFlags.None, SendCallBack, clientSocket);
            }
            catch (SocketException e)
            {
                Console.WriteLine("(Server) Send packet exception ->" + e);
            }

        }

        private static void SendCallBack(IAsyncResult AR)
        {
            try
            {
                Socket socket = AR.AsyncState as Socket;
                socket.EndSend(AR);
            }
            catch (SocketException e)
            {
                Console.WriteLine("(Server) Send packet callback exception ->" + e);
            }

        }

    }
}