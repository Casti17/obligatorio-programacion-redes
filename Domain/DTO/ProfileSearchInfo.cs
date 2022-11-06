using System.Text;

namespace Domain.DTO
{
    public class ProfileSearchInfo
    {
        public string Username { get; set; }

        public string Code()
        {
            return $"{this.Username}";
        }

        public void Decode(byte[] data)
        {
            string attributes = Encoding.UTF8.GetString(data);

            this.Username = attributes;
        }
    }
}