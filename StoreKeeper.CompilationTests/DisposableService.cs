// -----------------------------------------------------------------------
// <copyright file="DisposableService.cs" company="Andrii Kurdiumov">
// Copyright (c) Andrii Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace StoreKeeper.CompilationTests;

internal class DisposableService : IDisposable
{
    public void Dispose()
    {
        Console.WriteLine("Dispose DisposableService");
    }

    public void DoWork()
    {
        Console.WriteLine("Disposable service created");
    }
}