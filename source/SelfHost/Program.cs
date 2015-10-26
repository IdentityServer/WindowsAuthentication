using Microsoft.Owin.Hosting;
using Serilog;
using System;

namespace SelfHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Windows Authentication Service";

            Log.Logger = new LoggerConfiguration()
                .WriteTo.LiterateConsole()
                .CreateLogger();

            using (WebApp.Start<Startup>("https://localhost:44350"))
            {
                Console.WriteLine("running...");
                Console.ReadLine();
            }
        }
    }
}