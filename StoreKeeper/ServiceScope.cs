// -----------------------------------------------------------------------
// <copyright file="ServiceScope.cs" company="Andrii Kurdiumov">
// Copyright (c) Andrii Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace StoreKeeper;

internal enum ServiceScope
{
    Singleton,
    Scoped,
    Transient,
}