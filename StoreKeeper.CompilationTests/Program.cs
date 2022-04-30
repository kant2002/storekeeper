// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Andrii Kurdiumov">
// Copyright (c) Andrii Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;

internal class Program
{
    public static void Main()
    {
        var serviceContainer = new ServiceCollection();
        serviceContainer.AddScoped<EnglishGreeting>();
        var serviceProvider = serviceContainer.BuildServiceProviderAot();
        Console.WriteLine($"Type of Service Provider: {serviceProvider.GetType().FullName}");
        var greeting = serviceProvider.GetRequiredService<EnglishGreeting>();
        greeting.SayHello("Andrii");
    }
}
