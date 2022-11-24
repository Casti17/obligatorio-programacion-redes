using System;
using System.Text;

namespace Communication
{
    public class Header
    {
        private readonly byte[] _command;
        private readonly byte[] _dataLength;

        private int _iCommand;
        private int _iDataLength;

        public int ICommand
        {
            get => this._iCommand;
            set => this._iCommand = value;
        }

        public int IDataLength
        {
            get => this._iDataLength;
            set => this._iDataLength = value;
        }

        public Header()
        {
        }

        public Header(int command, int datalength)
        {
            string commandToString = command.ToString("D2"); // padding de 2 0s
            this._command = Encoding.UTF8.GetBytes(commandToString);
            string dataToString = datalength.ToString("D4"); // padding de 4 0s
            this._dataLength = Encoding.UTF8.GetBytes(dataToString);
        }

        public Header(byte[] buffer)
        {
            this.SplitHeaderProtocol(buffer);
        }

        public byte[] GetRequest()
        {
            var header = new byte[ProtocolConstants.CommandLength + ProtocolConstants.DataLength];
            Array.Copy(this._command, 0, header, 0, 2); // array source, indexSource, array destination, indexDestination length. Obtencion del comando
            Array.Copy(this._dataLength, 0, header, ProtocolConstants.CommandLength, 4); // obtencion de la data.
            return header;
        }

        public bool SplitHeaderProtocol(byte[] data)
        {
            var command = Encoding.UTF8.GetString(data, 0, ProtocolConstants.CommandLength);
            this._iCommand = int.Parse(command);
            var dataLength = Encoding.UTF8.GetString(data, ProtocolConstants.CommandLength, ProtocolConstants.DataLength);
            this._iDataLength = int.Parse(dataLength);
            return true;
        }
    }
}