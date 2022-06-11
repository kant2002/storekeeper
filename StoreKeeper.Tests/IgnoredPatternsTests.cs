// -----------------------------------------------------------------------
// <copyright file="IgnoredPatternsTests.cs" company="Andrii Kurdiumov">
// Copyright (c) Andrii Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace StoreKeeper.Tests;

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class IgnoredPatternsTests : CodeGenerationTestBase
{
    [TestMethod]
    public void ScopedRegistrationUsingClass()
    {
        string source = @"
using Microsoft.Extensions.DependencyInjection;

class TestService {}

class Test
{
    void Method()
    {
        var services = new ServiceCollection();
        services.AddScoped(typeof(TestService));
    }
}";
        string? output = this.GetGeneratedOutput(source, NullableContextOptions.Disable);

        Assert.IsNull(output);
    }
}
