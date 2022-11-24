using System.Text;

namespace Domain.DTO
{
    public class SendMessageInfo
    {
        public string Sender { get; set; }

        public string Receiver { get; set; }

        public string Message { get; set; }

        //public Message ToEntity()
        //{
        //    return new Domain.Message()
        //    {
        //        Sender = Sender,
        //        Receptor = Receiver,
        //        MessageBody = Message,
        //    };
        //}

        public string Code()
        {
            return $"{this.Sender}#{this.Receiver}#{this.Message}";
        }

        public void Decode(byte[] data)
        {
            string[] attributes = Encoding.UTF8.GetString(data).Split("#");

            this.Sender = attributes[0];
            this.Receiver = attributes[1];
            this.Message = attributes[2];
        }
    }
}