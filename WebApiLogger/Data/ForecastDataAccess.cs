using System.Collections.Generic;

namespace WebApiLogger.Data
{
    public class ForecastDataAccess
    {
        private List<WeatherForecast> forecasts;
        private object padlock;
        private static ForecastDataAccess instance;
        private List<string> logs;

        private static object singletonPadlock = new object();
        public static ForecastDataAccess GetInstance()
        {

            lock (singletonPadlock)
            { // bloqueante 
                if (instance == null)
                {
                    instance = new ForecastDataAccess();
                }
            }
            return instance;
        }

        private ForecastDataAccess()
        {
            forecasts = new List<WeatherForecast>();
            logs = new List<string>();
            padlock = new object();
        }

        public void AddLog(string log)
        {
            lock (padlock)
            {
                logs.Add(log);
            }
        }
        public void AddForecast(WeatherForecast forecast)
        {
            lock (padlock)
            {
                forecasts.Add(forecast);
            }
        }

        public string[] GetLogs()
        {
            lock(padlock)
            {
                return logs.ToArray();
            }
        }
        public WeatherForecast[] GetForecasts()
        {
            lock (padlock)
            {
                return forecasts.ToArray();
            }
        }

    }
}
