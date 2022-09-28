using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Message
    {
        public string MessageBody { get; set; }
        public string Sender { get; set; }
        public string Receptor { get; set; }
    }
}
