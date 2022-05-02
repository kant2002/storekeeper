// -----------------------------------------------------------------------
// <copyright file="AotServiceProviderFactory.cs" company="Andrii Kurdiumov">
// Copyright (c) Andrii Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// AOT specific service provider factory.
/// </summary>
public class AotServiceProviderFactory : IServiceProviderFactory<object>
{
    private IServiceCollection? services;
    private object? serviceContainer;

    /// <inheritdoc/>
    public object CreateBuilder(IServiceCollection services)
    {
        this.services = services;
        this.serviceContainer = new object();
        return this.serviceContainer;
    }

    /// <inheritdoc/>
    public IServiceProvider CreateServiceProvider(object containerBuilder)
    {
        var provider = this.services.BuildServiceProviderAot();
        return provider;
    }
}
