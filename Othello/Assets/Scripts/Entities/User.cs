using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Entities
{
    class User
    {

        private string _Username;
        private bool _InGame;
        private bool _AlreadyChallenged;

        public User() { }

        public User(string Username, bool InGame, bool AlreadyChallenged)
        {
            _Username           = Username;
            _InGame             = InGame;
            _AlreadyChallenged  = AlreadyChallenged;
        }

        public string Username
        {
            get {   return  _Username;  }
            set {   _Username = value;  }
        }

        public bool InGame
        {
            get {   return _InGame; }
            set {   _InGame = value; }
        }

        public bool AlreadyChallenged
        {
            get {   return _AlreadyChallenged; }
            set {   _AlreadyChallenged = value; }
        }
    }
}
