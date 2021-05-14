using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace Rocket.Speed
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            const int intervalo = 3 * 60 * 1000;

            var timer = new Timer(
            callback: new TimerCallback(Autenticar),
            state: 0,
            dueTime: intervalo,
            period: intervalo);

            Console.WriteLine("Iniciado!");

            Console.ReadLine();
            Console.ReadLine();
            Console.ReadLine();
            Console.ReadLine();
        }

        private static void Autenticar(object timerState)
        {
            var baseAddress = new Uri("");
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })

            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                var homePageResult = client.GetAsync("/");

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("", ""),
                    new KeyValuePair<string, string>("", ""),
                    new KeyValuePair<string, string>("accept", "true"),
                });
                var loginResult = client.PostAsync("", content).Result;

                Console.WriteLine($"Tentou conectar: {DateTime.Now} | {loginResult.StatusCode} - {(int)loginResult.StatusCode}");
            }
        }

        private void FormtarTela()
        {
            Console.WriteLine("( 1 ) - Autenticar");
        }
    }
}