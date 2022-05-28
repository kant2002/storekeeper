// -----------------------------------------------------------------------
// <copyright file="AotServiceProviderFactory.cs" company="Andrii Kurdiumov">
// Copyright (c) Andrii Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace StoreKeeper.GenericHost;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// AOT specific service provider factory.
/// </summary>
public sealed class AotServiceProviderFactory : IServiceProviderFactory<object>
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
        var provider = this.services.UseAotServices().BuildServiceProvider();
        return provider;
    }
}
