using System;
using System.Configuration;

namespace Protocolo
{
    public class SettingsManager
    {
        public string ReadSettings(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                return appSettings[key] ?? string.Empty;
            }
            catch
            {
                Console.WriteLine("Error al leer el archivo.");
                return string.Empty;
            }
        }
    }
}