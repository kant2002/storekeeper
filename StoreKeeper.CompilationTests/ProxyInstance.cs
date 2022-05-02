// -----------------------------------------------------------------------
// <copyright file="ProxyInstance.cs" company="Andrii Kurdiumov">
// Copyright (c) Andrii Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace StoreKeeper.CompilationTests;

internal class ProxyInstance
{
    private InstanceService service;

    public ProxyInstance(InstanceService service)
    {
        this.service = service;
    }

    public string GetMessage() => this.service.GetMessage() + "Proxied";
}
