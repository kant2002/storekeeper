// -----------------------------------------------------------------------
// <copyright file="ServiceDescriptor.cs" company="Andrii Kurdiumov">
// Copyright (c) Andrii Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace StoreKeeper;
using Microsoft.CodeAnalysis;

internal class ServiceDescriptor
{
    public ServiceDescriptor(ServiceScope scope, ITypeSymbol interfaceType, ITypeSymbol implementationType)
    {
        this.Scope = scope;
        this.InterfaceType = interfaceType;
        this.ImplementationType = implementationType;
    }

    public ServiceScope Scope { get; }

    public ITypeSymbol InterfaceType { get; }

    public ITypeSymbol ImplementationType { get; }

    public bool UseInstance { get; set; }

    public bool UseFactory { get; set; }

    public bool IsDisposable => this.ImplementationType.AllInterfaces.Any(_ => _.ToDisplayString() == "System.IDisposable");
}