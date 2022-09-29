using Communication;
using System;
using System.Net.Sockets;
using System.Text;

namespace AppCliente
{
    public class SocketHelper
    {
        private readonly Socket _socket;

        public SocketHelper(Socket socket)
        {
            this._socket = socket;
        }

        public void SendRequest(Request protocol)
        {
            this.Send(protocol.Header.GetRequest()); //agarro command y datalength y envio
            this.Send(Encoding.UTF8.GetBytes(protocol.Body)); //agarro la data y envio
        }

        public void Send(byte[] data)
        {
            int offset = 0;
            while (offset < data.Length)
            {
                var sent = this._socket.Send(
                    data,
                    offset,
                    data.Length - offset,
                    SocketFlags.None);
                if (sent == 0)
                {
                    throw new Exception("Connection lost");
                }

                offset += sent;
            }
        }

        public byte[] Receive(int length)
        {
            int offset = 0;
            var data = new byte[length];
            while (offset < length)
            {
                var received = this._socket.Receive(
                    data,
                    offset,
                    length - offset,
                    SocketFlags.None);
                if (received == 0)
                {
                    throw new Exception("Connection lost");
                }

                offset += received;
            }

            return data;
        }
    }
}