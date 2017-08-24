using Server.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Entities
{
    public class User
    {
        private int _ID;
        private string _Username;
        private string _Password;
        private string _Email;
        private bool _InGame;
        private bool _IsChallenged;
        private Socket _Socket;
        private Player _Player;


        public User()
        {
            _Player = new Player();
        }

        public User(int ID, string Username, string Password, string Email)
        {
            this.ID = ID;
            this._Username = Username;
            this._Password = Password;
            this._Email = Email;
            this._Player = new Player();
        }

        public int ID
        {
            get { return _ID;  }
            set { _ID = value; }
        }
        public string Username
        {
            get { return _Username;  }
            set { _Username = value; }
        }

        public string Password
        {
            get { return _Password;  }
            set { _Password = value; }
        }

        public string Email
        {
            get { return _Email;  }
            set { _Email = value; }
        }

        public bool InGame
        {
            get { return _InGame;  }
            set { _InGame = value; }
        }

        public Socket Socket
        {
            get { return _Socket;  }
            set { _Socket = value; }
        }

        public bool IsChallenged
        {
            get { return _IsChallenged;  }
            set { _IsChallenged = value; }
        }

        public Player Player
        {
            get { return _Player;  }
            set { _Player = value; }
        }
    }
}
