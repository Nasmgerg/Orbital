using Microsoft.Extensions.DependencyInjection;
using Orbital.Context;
using Orbital.View;

namespace Orbital
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            var services = new ServiceCollection();
            services.AddSingleton<CrewContext>();
            services.AddTransient<StartUp>();
            services.AddTransient<Add>();
            var serviceProvider = services.BuildServiceProvider();

            Application.Run(serviceProvider.GetRequiredService<StartUp>());
        }
    }
}
