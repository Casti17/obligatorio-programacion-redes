﻿using Protocolo;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AppCliente
{
    internal class Client
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Iniciando Aplicacion Cliente....!!!");

            var socketCliente = new Socket(
            AddressFamily.InterNetwork,
                SocketType.Stream,
                    ProtocolType.Tcp);

            var localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);
            socketCliente.Bind(localEndPoint);
            var serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000);
            socketCliente.Connect(serverEndpoint);
            Console.WriteLine("Cliente Conectado al Servidor...!!!");
            Console.WriteLine("\nBienvenido al Sistema Client");
            Console.WriteLine("Opciones validas: ");
            Console.WriteLine("Alta de usuario");
            Console.WriteLine("Alta de perfil de trabajo ");
            Console.WriteLine("Asociar foto a perfil de trabajo");
            Console.WriteLine("Consultar perfiles existentes");
            Console.WriteLine("Consultar perfil especifico");
            Console.WriteLine("Mensajes");
            Console.WriteLine("Configuracion");
            Console.WriteLine("Logout");
            Console.WriteLine("Ingrese su opcion: ");
            bool parar = false;
            while (!parar)
            {
                String mensaje = Console.ReadLine();
                switch (mensaje)
                {
                    case "Alta de usuario":
                        _logged = await CaseUpUser();
                        break;

                    case "Alta de perfil de trabajo":
                        if (_logged)
                        {
                            await CaseUpProfile();
                        }
                        else
                        {
                            Console.WriteLine("Tiene que estar logueado para utilizar esta funcionalidad.");
                        }
                        break;

                    case "Asociar foto a perfil de trabajo":
                        if (_logged)
                        {
                        }
                        break;
                }
                if (mensaje.Equals("Exit", StringComparison.InvariantCultureIgnoreCase))
                {
                    parar = true;
                }
                else
                {
                    try
                    {
                        byte[] data = Encoding.UTF8.GetBytes(mensaje);
                        byte[] dataLength = BitConverter.GetBytes(data.Length);

                        // Mando primero el tamaño
                        int offset = 0;
                        int size = Constantes.LargoFijo;
                        while (offset < size)
                        {
                            int enviados = socketCliente.Send(dataLength, offset, size - offset, SocketFlags.None);
                            if (enviados == 0)
                            {
                                throw new SocketException();
                            }
                            offset += enviados;
                        }

                        // Mando el mensaje
                        offset = 0;
                        size = data.Length;
                        while (offset < size)
                        {
                            int enviados = socketCliente.Send(data, offset, size - offset, SocketFlags.None);
                            if (enviados == 0)
                            {
                                throw new SocketException();
                            }
                            offset += enviados;
                        }

                        // El cliente recibe la respuesta
                        byte[] datarespuestaLength = new byte[Constantes.LargoFijo];
                        int recibido = socketCliente.Receive(datarespuestaLength);
                        if (recibido == 0)
                        {
                            throw new SocketException();
                        }

                        byte[] datarespuesta = new byte[BitConverter.ToInt32(dataLength, 0)];
                        // en Visual Studio no es necesario el parametro 0, solo con el buffer es suficiente
                        recibido = socketCliente.Receive(datarespuesta);
                        if (recibido == 0)
                        {
                            throw new SocketException();
                        }
                        string respuesta = Encoding.UTF8.GetString(datarespuesta);
                        Console.WriteLine("El servidor respondio: {0}", respuesta);
                    }
                    catch (SocketException)
                    {
                        Console.WriteLine("Se perdio la conexion con el Servidor");
                        parar = true;
                    }
                }
            }

            Console.WriteLine("Cierro el Cliente");
            // Cerrar la conexion.
            socketCliente.Shutdown(SocketShutdown.Both);
            socketCliente.Close();
        }

        private static void CaseUpUser()
        {
            if (!_logged)
            {
                Console.WriteLine("Ingrese nombre de usuario")
            }
        }
    }
}