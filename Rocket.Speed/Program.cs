using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace Rocket.Speed
{
    internal static class Program
    {
        private static readonly string uri = Properties.Resources.Uri;
        private static readonly string endPoint = Properties.Resources.EndPoint;
        private static readonly string user = Properties.Resources.User;
        private static readonly string password = Properties.Resources.Password;
        private static readonly string fieldUser = Properties.Resources.FieldUser;
        private static readonly string fieldPassword = Properties.Resources.FieldPassword;
        private static readonly string fieldTerms = Properties.Resources.FieldTerms;
        private static readonly string terms = Properties.Resources.TermsUse;
        private static readonly int interval = 60 * Convert.ToInt32(Properties.Resources.Interval) * 1000;

        private static List<string> history = new List<string>();

        private static void Main()
        {
            Console.Clear();

            Console.WriteLine($"Rocket Speed{Environment.NewLine}");
            Console.Write("Senha: ");

            var senha = string.Empty;
            ConsoleKey key;

            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && senha.Length > 0)
                {
                    Console.Write("\b \b");
                    senha = senha[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    senha += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);

            if (senha != "SPEED#720")
            {
                Console.Clear();
                Console.WriteLine("Senha incorreta!");
                Console.WriteLine("\nPressione Enter para sair...");
                Console.ReadLine();

                Environment.Exit(0);
            }

            var timer = new Timer(
            callback: new TimerCallback(Autenticar),
            state: 0,
            dueTime: 0,
            period: interval);

            int opcao = 0;

            do
            {
                Console.Clear();
                MontarTelaPrincipal();

                var isInt = int.TryParse(Console.ReadLine(), out opcao);

                if (!isInt)
                    continue;

                switch (opcao)
                {
                    case 1:
                        StatusConexao();
                        break;

                    case 2:
                        Autenticar();
                        break;
                }
            } while
            (opcao != 3);
        }

        private static void MontarTelaPrincipal()
        {
            Console.WriteLine($"Rocket Speed{Environment.NewLine}");
            Console.WriteLine("[1] - Status Conexão");
            Console.WriteLine("[2] - Autenticação");
            Console.WriteLine("[3] - Fechar");

            Console.Write($"{Environment.NewLine}Opção: ");
        }

        private static void StatusConexao()
        {
            Ping pingSender = new();
            PingOptions options = new()
            {
                DontFragment = true
            };

            const string data = "";

            byte[] buffer = Encoding.ASCII.GetBytes(data);
            const int timeout = 20;
            PingReply reply = pingSender.Send("google.com", timeout, buffer, options);

            Console.Clear();

            if (reply.Status == IPStatus.Success)
                Console.WriteLine("Está conectado!");
            else
                Console.WriteLine("Não está conectado!");

            foreach (string tentativa in history)
                Console.WriteLine(tentativa);

            history.Clear();

            Console.WriteLine("\nPressione Enter para voltar...");
            Console.ReadLine();
        }

        private static void Autenticar(object timerState = null)
        {
            var baseAddress = new Uri(uri);
            var cookieContainer = new CookieContainer();

            using var handler = new HttpClientHandler() { CookieContainer = cookieContainer };

            using var client = new HttpClient(handler) { BaseAddress = baseAddress };
            var homePageResult = client.GetAsync("/");

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>(fieldUser, user),
                new KeyValuePair<string, string>(fieldPassword, password),
                new KeyValuePair<string, string>(fieldTerms, terms),
            });

            var loginResult = client.PostAsync(endPoint, content).Result;

            if (!loginResult.IsSuccessStatusCode && loginResult.StatusCode != HttpStatusCode.Redirect)
                history.Add($"{DateTime.Now} | {loginResult.StatusCode} - {(int)loginResult.StatusCode}");
        }
    }
}