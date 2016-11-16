using Server.Game;
using Server.Packets;
using Server.Singleton;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ReversiServer
{
    public class Server
    {
        private static Socket _ServerSocket;
        public static readonly List<Socket> _ClientSockets = new List<Socket>();
        private const int _BUFFER_SIZE = 1024;
        private const int _PORT = 2016;
        private static readonly byte[] _buffer = new byte[_BUFFER_SIZE];



        public void StartServer()
        {
            Console.WriteLine("Setting up server...");
            _ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _ServerSocket.Bind(new IPEndPoint(IPAddress.Any, _PORT));
            _ServerSocket.Listen(50);
            _ServerSocket.BeginAccept(AcceptCallback, null);
            Console.WriteLine("Server setup complete");
        }

        /// <summary>
        /// Close all connected client (we do not need to shutdown the server socket as its connections
        /// are already closed with the clients)
        /// </summary>
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
            if (Singleton.Instance.ListOfPlayers.Count % 2 == 0)
            {
                InitRoom();
            }
            socket.BeginReceive(_buffer, 0, _BUFFER_SIZE, SocketFlags.None, ReceiveCallback, socket);
            _ServerSocket.BeginAccept(AcceptCallback, null);
        }

        private static void InitRoom()
        {
            Room room = new Room(Singleton.Instance.ListOfPlayers[0], Singleton.Instance.ListOfPlayers[1],Singleton.Instance.RoomIDHelper);
            Singleton.Instance.ListOfRooms.Add(room);
            Singleton.Instance.RoomIDHelper++;
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
                    Array.Copy(_buffer, recBuf, received);
                    PacketHandler.Handle(recBuf, current);
                    if (current.Connected)
                    {
                        current.BeginReceive(_buffer, 0, _BUFFER_SIZE, SocketFlags.None, ReceiveCallback, current);
                    }
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Client forcefully disconnected");
                current.Close(); // Dont shutdown because the socket may be disposed and its disconnected anyway
                _ClientSockets.Remove(current);
                return;
            }
            
        }

        public static void SendPacket(Socket playerSocket,byte[] packet)
        {
            try
            {
                if(playerSocket.Connected)
                playerSocket.BeginSend(packet, 0, packet.Length, SocketFlags.None, SendCallBack, playerSocket);

            }
            catch (Exception e) { }

        }

        private static void SendCallBack(IAsyncResult AR)
        {
            try
            {
                Socket socket = AR.AsyncState as Socket;
                socket.EndSend(AR);
            }
            catch (Exception e) { }

        }

    }
}
