// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Andrii Kurdiumov">
// Copyright (c) Andrii Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StoreKeeper.GenericHost;

var host = new HostBuilder()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddSingleton<TestService>();
    })
    .UseServiceProviderFactory(new AotServiceProviderFactory())
    .Build();

await host.RunAsync();

internal class TestService
{
    public void Method() => Console.WriteLine("Hello Host");
}