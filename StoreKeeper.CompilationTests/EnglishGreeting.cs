// -----------------------------------------------------------------------
// <copyright file="EnglishGreeting.cs" company="Andrii Kurdiumov">
// Copyright (c) Andrii Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

internal class EnglishGreeting : IGreeting
{
    public string SayHello(string name)
    {
        return $"Hello {name}!";
    }
}