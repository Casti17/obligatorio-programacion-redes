using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Domain
{
    public class Message
    {
        public Message(string receptor, string sender, string msg)
        {
            Receptor = receptor;
            Sender = sender;
            MessageBody = msg;
        }
        public string MessageBody { get; set; }
        public string Sender { get; set; }
        public string Receptor { get; set; }
        public bool Read { get; set; }
    }
}