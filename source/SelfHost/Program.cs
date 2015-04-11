using Microsoft.Owin.Hosting;
using Serilog;
using System;

namespace SelfHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .CreateLogger();

            using (WebApp.Start<Startup>("https://localhost:44350"))
            {
                Console.WriteLine("running...");
                Console.ReadLine();
            }
        }
    }
}