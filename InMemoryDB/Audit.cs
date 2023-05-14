using System;

namespace InMemoryDB
{
    public enum MessageType{Info, Warning, Error}

    public class Audit
    {
        private int _ID;
        private DateTime _TimeStamp;
        private MessageType _MessageType;
        private string _Message;

        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        public DateTime Timestap
        {
            get { return _TimeStamp; }
            set { _TimeStamp = value; }
        }
        public MessageType MessageType
        {
            get { return _MessageType; }
            set { _MessageType = value; }
        } 

        public string Message
        {
            get { return _Message; }
            set { _Message = value; }
        }

        public Audit(DateTime timeStamp, MessageType messageType, string message)
        {
            _TimeStamp = timeStamp;
            _MessageType = messageType;
            _Message = message;
        }
    }
}
