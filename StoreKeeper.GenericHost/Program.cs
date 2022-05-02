﻿// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Andrii Kurdiumov">
// Copyright (c) Andrii Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureServices((hostContext, services) =>
    {
    })
    .UseServiceProviderFactory(new AotServiceProviderFactory())
    .Build();

await host.RunAsync();
