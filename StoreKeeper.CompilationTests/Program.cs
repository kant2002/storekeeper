// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Andrii Kurdiumov">
// Copyright (c) Andrii Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace StoreKeeper.CompilationTests;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using static System.Console;

internal class Program
{
    public static void Main()
    {
        var serviceContainer = new ServiceCollection();
        serviceContainer.AddScoped<EnglishGreeting>();

        serviceContainer.AddScoped<DisposableService>();
        serviceContainer.TryAddScoped<IGreeting, EnglishGreeting>();

        serviceContainer.AddSingleton(new InstanceService());
        serviceContainer.AddScoped((_) => new InstanceServiceFromFunc());
        serviceContainer.AddScoped<ProxyInstance>();
        using (var serviceProvider = serviceContainer.BuildServiceProviderAot())
        {
            WriteLine($"Type of Service Provider: {serviceProvider.GetType().FullName}");
            var englishGreeting = serviceProvider.GetRequiredService<EnglishGreeting>();
            var andriiMessage = englishGreeting.SayHello("Andrii");
            if (andriiMessage != "Hello Andrii!")
            {
                WriteLine("Failed to get EnglishGreeting");
            }

            var iGreeting = serviceProvider.GetRequiredService<IGreeting>();
            var maratMessage = iGreeting.SayHello("Marat");
            if (maratMessage != "Hello Marat!")
            {
                WriteLine("Failed to get IGreeting");
            }

            var disposableService = serviceProvider.GetRequiredService<DisposableService>();
            disposableService.DoWork();

            var message = serviceProvider.GetRequiredService<InstanceService>().GetMessage();
            WriteLine($"Message from instance service: {message}");
            var factoryMessage = serviceProvider.GetRequiredService<InstanceServiceFromFunc>().GetMessage();
            WriteLine($"Message from factory service: {factoryMessage}");

            var proxyMessage = serviceProvider.GetRequiredService<ProxyInstance>().GetMessage();
            WriteLine($"Message from proxy service: {proxyMessage}");
        }

        WriteLine("OK");
    }
}
