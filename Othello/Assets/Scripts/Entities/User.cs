using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Entities
{
    public class User
    {

        private string _Username;
        private bool _InGame;
        private bool _IsChallenged;

        public User() { }

        public User(string Username, bool InGame, bool AlreadyChallenged)
        {
            _Username           = Username;
            _InGame             = InGame;
            _IsChallenged  = AlreadyChallenged;
        }

        public string Username
        {
            get {   return  _Username;  }
            set {   _Username = value;  }
        }

        public bool InGame
        {
            get {   return _InGame; }
            set
            {   _InGame = value;
                if (_InGame == true)
                    _IsChallenged = false;
                        
            }
        }

        public bool IsChallenged
        {
            get {   return _IsChallenged; }
            set
            {
                _IsChallenged = value;
                if (_IsChallenged == true)
                    _InGame = false;
            }
        }
    }
}
