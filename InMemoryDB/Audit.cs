using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMemoryDB
{
    public enum Message_type{Info, Warning, Error}

    public class Audit
    {
        private int _ID;
        private DateTime _Timestap;
        private Message_type _MessageType;
        private string Message;

        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        public DateTime Timestap
        {
            get { return _Timestap; }
            set { _Timestap = value; }
        }
        private Message_type MessageType
        {
            get { return _MessageType; }
            set { _MessageType = value; }
        } 

        private string _Message
        {
            get { return _Message; }
            set { _Message = value; }
        }

        public Audit(int iD, DateTime timestap, Message_type messageType, string message)
        {
            _ID = iD;
            _Timestap = timestap;
            _MessageType = messageType;
            Message = message;
        }
    }
}
