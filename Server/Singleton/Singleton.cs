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
    public class Singleton
    {
        private static Singleton _Instance;
        public List<Room> ListOfRooms;
        public List<Player> ListOfPlayers;
        public ushort RoomIDHelper;
             
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


    }
}
