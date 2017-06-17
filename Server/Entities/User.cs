using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Entities
{
    class User
    {
        private int _ID;
        private string _Username;
        private string _Password;
        private string _Email;
        

        public User() { }

        public User(int ID, string Username, string Password, string Email)
        {
            this._ID = ID;
            this._Username = Username;
            this._Password = Password;
            this._Email = Email;
        }

        public int ID { get => _ID; set => _ID = value; }
        public string Username { get => _Username; set => _Username = value; }
        public string Password { get => _Password; set => _Password = value; }
        public string Email { get => _Email; set => _Email = value; }
    }
}
