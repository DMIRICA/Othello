﻿using System;
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
            this.ID = ID;
            this._Username = Username;
            this._Password = Password;
            this._Email = Email;
        }

        public int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
            }
        }
        public string Username
        {
            get
            {
                return _Username;
            }
            set
            {
                _Username = value;
            }
        }

        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                _Password = value;
            }
        }

        public string Email
        {
            get
            {
                return _Email;
            }
            set
            {
                _Email = value;
            }
        }
    }
}