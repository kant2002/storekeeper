﻿// -----------------------------------------------------------------------
// <copyright file="SimpleServiceRegistrationTests.cs" company="Andrii Kurdiumov">
// Copyright (c) Andrii Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace StoreKeeper.Tests;

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class SimpleServiceRegistrationTests : CodeGenerationTestBase
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
        services.AddScoped<TestService>();
    }
}";
        string output = this.GetGeneratedOutput(source, NullableContextOptions.Disable);

        Assert.IsNotNull(output);

        var expectedOutput = @"using System;
using Microsoft.Extensions.DependencyInjection;

internal class CustomServiceProvider : IServiceProvider, IServiceScopeFactory
{
    public IServiceScope CreateScope()
    {
        return new ScopedServices();
    }

    private ScopedServices implicitScope = new ScopedServices();

    internal class ScopedServices : IServiceProvider, IServiceScope
    {
        public IServiceProvider ServiceProvider => this;

        public void Dispose()
        {
        }

        private global::TestService _TestService;

        public object GetService(Type t)
        {
            if (t == typeof(global::TestService))
            {
                _TestService = _TestService ?? new global::TestService();
                return _TestService;
            }

            return null;
        }
    }

    public object GetService(Type t)
    {
        if (t == typeof(Microsoft.Extensions.DependencyInjection.IServiceScope))
        {
            return this;
        }

        return implicitScope.GetService(t);
    }
}

public static class StoreKeeperExtensions
{
    public static IServiceProvider BuildServiceProviderAot(this Microsoft.Extensions.DependencyInjection.IServiceCollection services, Microsoft.Extensions.DependencyInjection.ServiceProviderOptions options)
    {
        return new CustomServiceProvider();
    }
    public static IServiceProvider BuildServiceProviderAot(this Microsoft.Extensions.DependencyInjection.IServiceCollection services)
    {
        return new CustomServiceProvider();
    }
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void ScopedRegistrationUsingInterface()
    {
        string source = @"
using Microsoft.Extensions.DependencyInjection;

interface ITestService {}
class TestService: ITestService {}

class Test
{
    void Method()
    {
        var services = new ServiceCollection();
        services.AddScoped<ITestService, TestService>();
    }
}";
        string output = this.GetGeneratedOutput(source, NullableContextOptions.Disable);

        Assert.IsNotNull(output);

        var expectedOutput = @"using System;
using Microsoft.Extensions.DependencyInjection;

internal class CustomServiceProvider : IServiceProvider, IServiceScopeFactory
{
    public IServiceScope CreateScope()
    {
        return new ScopedServices();
    }

    private ScopedServices implicitScope = new ScopedServices();

    internal class ScopedServices : IServiceProvider, IServiceScope
    {
        public IServiceProvider ServiceProvider => this;

        public void Dispose()
        {
        }

        private global::TestService _ITestService;

        public object GetService(Type t)
        {
            if (t == typeof(global::ITestService))
            {
                _ITestService = _ITestService ?? new global::TestService();
                return _ITestService;
            }

            return null;
        }
    }

    public object GetService(Type t)
    {
        if (t == typeof(Microsoft.Extensions.DependencyInjection.IServiceScope))
        {
            return this;
        }

        return implicitScope.GetService(t);
    }
}

public static class StoreKeeperExtensions
{
    public static IServiceProvider BuildServiceProviderAot(this Microsoft.Extensions.DependencyInjection.IServiceCollection services, Microsoft.Extensions.DependencyInjection.ServiceProviderOptions options)
    {
        return new CustomServiceProvider();
    }
    public static IServiceProvider BuildServiceProviderAot(this Microsoft.Extensions.DependencyInjection.IServiceCollection services)
    {
        return new CustomServiceProvider();
    }
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void ScopedRegistrationWithDisposable()
    {
        string source = @"
using Microsoft.Extensions.DependencyInjection;

class TestService : System.IDisposable {}

class Test
{
    void Method()
    {
        var services = new ServiceCollection();
        services.AddScoped<TestService>();
    }
}";
        string output = this.GetGeneratedOutput(source, NullableContextOptions.Disable);

        Assert.IsNotNull(output);

        var expectedOutput = @"using System;
using Microsoft.Extensions.DependencyInjection;

internal class CustomServiceProvider : IServiceProvider, IServiceScopeFactory
{
    public IServiceScope CreateScope()
    {
        return new ScopedServices();
    }

    private ScopedServices implicitScope = new ScopedServices();

    internal class ScopedServices : IServiceProvider, IServiceScope
    {
        public IServiceProvider ServiceProvider => this;

        public void Dispose()
        {
            if (_TestService != null)
            {
                ((System.IDisposable)_TestService).Dispose();
                _TestService = null;
            }

        }

        private global::TestService _TestService;

        public object GetService(Type t)
        {
            if (t == typeof(global::TestService))
            {
                _TestService = _TestService ?? new global::TestService();
                return _TestService;
            }

            return null;
        }
    }

    public object GetService(Type t)
    {
        if (t == typeof(Microsoft.Extensions.DependencyInjection.IServiceScope))
        {
            return this;
        }

        return implicitScope.GetService(t);
    }
}

public static class StoreKeeperExtensions
{
    public static IServiceProvider BuildServiceProviderAot(this Microsoft.Extensions.DependencyInjection.IServiceCollection services, Microsoft.Extensions.DependencyInjection.ServiceProviderOptions options)
    {
        return new CustomServiceProvider();
    }
    public static IServiceProvider BuildServiceProviderAot(this Microsoft.Extensions.DependencyInjection.IServiceCollection services)
    {
        return new CustomServiceProvider();
    }
}
";
        Assert.AreEqual(expectedOutput, output);
    }
}
