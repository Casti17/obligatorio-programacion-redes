namespace Domain
{
    public class Message
    {
        public Message(string receptor, string sender, string msg)
        {
            this.Receptor = receptor;
            this.Sender = sender;
            this.MessageBody = msg;
        }

        public string MessageBody { get; set; }
        public string Sender { get; set; }
        public string Receptor { get; set; }
        public bool Read { get; set; }
    }
}