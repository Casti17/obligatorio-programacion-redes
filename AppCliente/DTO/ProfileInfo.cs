﻿using System.Collections.Generic;
using System.Text;

namespace Domain.DTO
{
    public class ProfileInfo
    {
        public string Username { get; set; }
        public string Imagen { get; set; }
        public string Skills { get; set; }
        public string Descripcion { get; set; }


        public string Code()
        {
            return $"{this.Username}#{this.Imagen}#{this.Skills}#{this.Descripcion}";
        }

        public void Decode(byte[] data)
        {
            string[] attributes = Encoding.UTF8.GetString(data).Split("#");

            this.Username = attributes[0];
            this.Imagen = attributes[1];
            this.Skills = attributes[2];
            this.Descripcion = attributes[3];
        }
    }
}