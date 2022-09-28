using Protocolo;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AppServidor
{
    internal class Servidor
    {
        private static readonly SettingsManager settingsMng = new SettingsManager();

        public static void Main(string[] args)
        {
            Console.WriteLine("Iniciando Aplicacion Servidor....!!!");

            var socketServer = new Socket(
            AddressFamily.InterNetwork,
                SocketType.Stream,
                    ProtocolType.Tcp);

            string ipServidor = settingsMng.ReadSettings(ServerConfig.ServerIPConfigKey);
            int puerto = int.Parse(settingsMng.ReadSettings(ServerConfig.ServerPortConfigKey));

            var localEndpoint = new IPEndPoint(IPAddress.Parse(ipServidor), puerto);
            // puertos 0 a 65535   pero del 1 al 1024 estan reservados

            socketServer.Bind(localEndpoint); // vinculo el socket al EndPoint
            socketServer.Listen(100); // Pongo al Servidor en modo escucha
            int clientes = 0;
            bool salir = false;

            while (!salir)
            {
                var socketClient = socketServer.Accept();
                clientes++;
                int nro = clientes;
                Console.WriteLine("Acepte un nuevo pedido de Conexion");
                new Thread(() => ManejarCliente(socketClient, nro)).Start();
            }

            Console.ReadLine();

            // Como hacemos para aceptar N clientes.
            // Como hacemos para enviar y recibir mensajes.

            // Cierro el socket
            socketServer.Shutdown(SocketShutdown.Both);
            socketServer.Close();
        }

        private static void ManejarCliente(Socket socketCliente, int nro)
        {
            try
            {
                Console.WriteLine("Cliente {0} conectado", nro);
                bool clienteConectado = true;
                while (clienteConectado)
                {
                    /// PARA RECIBO DE ARCHIVOS /////

                    /*Console.WriteLine("Antes de recibir el archivo");
                    var fileCommonHandler = new FileCommsHandler(socketCliente);
                    fileCommonHandler.ReceiveFile();
                    Console.WriteLine("Archivo recibido!!");*/

                    // Primero recibo el largo del mensaje en 4 bytes
                    byte[] dataLength = new byte[Constantes.LargoFijo];
                    int offset = 0;
                    int size = Constantes.LargoFijo;

                    while (offset < size)
                    {
                        int recibidos = socketCliente.Receive(dataLength, offset, size - offset, SocketFlags.None);
                        if (recibidos == 0)
                        {
                            throw new SocketException();
                        }
                        offset += recibidos;
                    }

                    // Ahora recibo el mensaje

                    byte[] data = new byte[BitConverter.ToInt32(dataLength, 0)];
                    // en Visual Studio no es necesario el parametro 0, solo con el buffer es suficiente
                    offset = 0;
                    size = BitConverter.ToInt32(dataLength, 0);
                    while (offset < size)
                    {
                        int recibidos = socketCliente.Receive(data, offset, size - offset, SocketFlags.None);
                        if (recibidos == 0)
                        {
                            throw new SocketException();
                        }
                        offset += recibidos;
                    }
                    string mensaje = Encoding.UTF8.GetString(data);
                    Console.WriteLine("El cliente {0} dice: {1}", nro, mensaje);

                    //// Envio una respuesta al cliente
                    string respuesta = "OK";
                    byte[] datarespuesta = Encoding.UTF8.GetBytes(respuesta);
                    byte[] datarespuestaLength = BitConverter.GetBytes(data.Length);
                    //// Mando primero el tamaño
                    socketCliente.Send(dataLength);
                    //// Mando el mensaje
                    socketCliente.Send(datarespuesta);
                }
                Console.WriteLine("Cliente Desconectado");
            }
            catch (SocketException)
            {
                Console.WriteLine("Cliente Desconectado!");
            }
        }
    }
}