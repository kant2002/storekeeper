// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Andrii Kurdiumov">
// Copyright (c) Andrii Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace StoreKeeper.CompilationTests;

using Microsoft.Extensions.DependencyInjection;
using static System.Console;

internal class Program
{
    public static void Main()
    {
        var serviceContainer = new ServiceCollection();
        serviceContainer.AddScoped<EnglishGreeting>();

        serviceContainer.AddScoped<DisposableService>();
        serviceContainer.AddScoped<IGreeting, EnglishGreeting>();
        var serviceProvider = serviceContainer.BuildServiceProviderAot();
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

        WriteLine("OK");
    }
}
