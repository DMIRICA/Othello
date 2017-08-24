using Server.database;
using Server.Entities;
using Server.Game;
using Server.Packets;
using Server.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Singleton
{
   class Singleton
   {

        private DatabaseConnection _DatabaseConnection;
        private static Singleton _Instance;
        private  ushort _RoomIDHelper;
        public List<Room> ListOfRooms;
        public List<User> ListOfUsersLogged;

        public List<Player> ListOfPlayers;
        
             
        public static Singleton Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new Singleton();
                }
                return _Instance;
            }
        }

        private Singleton()
        {
            ListOfRooms = new List<Room>();
            ListOfPlayers = new List<Player>();
            ListOfUsersLogged = new List<User>();
            _DatabaseConnection = new DatabaseConnection();
            RoomIDHelper = 0;
        }

        
        public Room GetRoomByID(ushort id)
        {
            foreach(Room r in ListOfRooms)
            {
                if(r.ID == id)
                {
                    return r;
                }
            }
            return null;
        }

        public User GetUserLoogedByUsername(string username)
        {
            foreach (User u in ListOfUsersLogged)
            {
                if (u.Username == username)
                {
                    return u;
                }
            }
            return null;
        }

        public DatabaseConnection DatabaseConnection
        {
            get { return _DatabaseConnection; }
            set { _DatabaseConnection = value; }
        }

        public ushort RoomIDHelper
        {
            get { return _RoomIDHelper; }
            set { _RoomIDHelper = value; }
        }

        public bool isUserLogged(string username)
        {
            bool online = false;
            foreach(User user in ListOfUsersLogged)
            {
                if (user.Username == username)
                {
                    online = true;
                    break;
                }
                
            }
            return online;
        }
    }
}
