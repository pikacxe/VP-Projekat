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
        public DateTime Timestamp
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

        private string GetType()
        {
            switch (MessageType)
            {
                case MessageType.Info:
                    return "Info";
                case MessageType.Warning:
                    return "Warning";
                case MessageType.Error:
                    return "Error";
                default:
                    return string.Empty;
            }
        }

        public override string ToString()
        {
            return $"{ID}\t{Timestamp}\t{GetType()}\t{Message}";
        }

        public static string FormatHeader()
        {
            return "\n==========AUDITS==========\nID\tTIMESTAMP\t\tTYPE\tMESSAGE";
        }

        

    }
}
