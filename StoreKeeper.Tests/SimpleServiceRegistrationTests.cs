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

public sealed class ServiceProviderAot : IServiceProvider, System.IDisposable
{
    internal ServiceProviderAot(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
    {
        this.serviceScopeFactory = new ServiceScopeFactory(services);
        this.implicitScope = serviceScopeFactory.CreateScope();
    }

    private sealed class ServiceScopeFactory : IServiceScopeFactory
    {
        private Microsoft.Extensions.DependencyInjection.IServiceCollection services;

        public ServiceScopeFactory(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
        {
            this.services = services;
        }

        public IServiceScope CreateScope()
        {
            var result = new ScopedServices();
            return result;
        }
    }

    private ServiceScopeFactory serviceScopeFactory;

    private Microsoft.Extensions.DependencyInjection.IServiceScope implicitScope;

    public void Dispose()
    {
        implicitScope.Dispose();
    }

    internal class ScopedServices : IServiceProvider, Microsoft.Extensions.DependencyInjection.IServiceScope
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
        if (t == typeof(Microsoft.Extensions.DependencyInjection.IServiceScopeFactory))
        {
            return serviceScopeFactory;
        }

        return implicitScope.ServiceProvider.GetService(t);
    }
}

public static class StoreKeeperExtensions
{
    public static ServiceProviderAot BuildServiceProviderAot(this Microsoft.Extensions.DependencyInjection.IServiceCollection services, Microsoft.Extensions.DependencyInjection.ServiceProviderOptions options)
    {
        return new ServiceProviderAot(services);
    }
    public static ServiceProviderAot BuildServiceProviderAot(this Microsoft.Extensions.DependencyInjection.IServiceCollection services)
    {
        return new ServiceProviderAot(services);
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

public sealed class ServiceProviderAot : IServiceProvider, System.IDisposable
{
    internal ServiceProviderAot(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
    {
        this.serviceScopeFactory = new ServiceScopeFactory(services);
        this.implicitScope = serviceScopeFactory.CreateScope();
    }

    private sealed class ServiceScopeFactory : IServiceScopeFactory
    {
        private Microsoft.Extensions.DependencyInjection.IServiceCollection services;

        public ServiceScopeFactory(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
        {
            this.services = services;
        }

        public IServiceScope CreateScope()
        {
            var result = new ScopedServices();
            return result;
        }
    }

    private ServiceScopeFactory serviceScopeFactory;

    private Microsoft.Extensions.DependencyInjection.IServiceScope implicitScope;

    public void Dispose()
    {
        implicitScope.Dispose();
    }

    internal class ScopedServices : IServiceProvider, Microsoft.Extensions.DependencyInjection.IServiceScope
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
        if (t == typeof(Microsoft.Extensions.DependencyInjection.IServiceScopeFactory))
        {
            return serviceScopeFactory;
        }

        return implicitScope.ServiceProvider.GetService(t);
    }
}

public static class StoreKeeperExtensions
{
    public static ServiceProviderAot BuildServiceProviderAot(this Microsoft.Extensions.DependencyInjection.IServiceCollection services, Microsoft.Extensions.DependencyInjection.ServiceProviderOptions options)
    {
        return new ServiceProviderAot(services);
    }
    public static ServiceProviderAot BuildServiceProviderAot(this Microsoft.Extensions.DependencyInjection.IServiceCollection services)
    {
        return new ServiceProviderAot(services);
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

public sealed class ServiceProviderAot : IServiceProvider, System.IDisposable
{
    internal ServiceProviderAot(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
    {
        this.serviceScopeFactory = new ServiceScopeFactory(services);
        this.implicitScope = serviceScopeFactory.CreateScope();
    }

    private sealed class ServiceScopeFactory : IServiceScopeFactory
    {
        private Microsoft.Extensions.DependencyInjection.IServiceCollection services;

        public ServiceScopeFactory(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
        {
            this.services = services;
        }

        public IServiceScope CreateScope()
        {
            var result = new ScopedServices();
            return result;
        }
    }

    private ServiceScopeFactory serviceScopeFactory;

    private Microsoft.Extensions.DependencyInjection.IServiceScope implicitScope;

    public void Dispose()
    {
        implicitScope.Dispose();
    }

    internal class ScopedServices : IServiceProvider, Microsoft.Extensions.DependencyInjection.IServiceScope
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
        if (t == typeof(Microsoft.Extensions.DependencyInjection.IServiceScopeFactory))
        {
            return serviceScopeFactory;
        }

        return implicitScope.ServiceProvider.GetService(t);
    }
}

public static class StoreKeeperExtensions
{
    public static ServiceProviderAot BuildServiceProviderAot(this Microsoft.Extensions.DependencyInjection.IServiceCollection services, Microsoft.Extensions.DependencyInjection.ServiceProviderOptions options)
    {
        return new ServiceProviderAot(services);
    }
    public static ServiceProviderAot BuildServiceProviderAot(this Microsoft.Extensions.DependencyInjection.IServiceCollection services)
    {
        return new ServiceProviderAot(services);
    }
}
";
        Assert.AreEqual(expectedOutput, output);
    }

    [TestMethod]
    public void ScopedRegistrationUsingInstance()
    {
        string source = @"
using Microsoft.Extensions.DependencyInjection;

class TestService {}

class Test
{
    void Method()
    {
        var services = new ServiceCollection();
        services.AddScoped(new TestService());
    }
}";
        string output = this.GetGeneratedOutput(source, NullableContextOptions.Disable);

        Assert.IsNotNull(output);

        var expectedOutput = @"using System;
using Microsoft.Extensions.DependencyInjection;

public sealed class ServiceProviderAot : IServiceProvider, System.IDisposable
{
    internal ServiceProviderAot(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
    {
        this.serviceScopeFactory = new ServiceScopeFactory(services);
        this.implicitScope = serviceScopeFactory.CreateScope();
    }

    private sealed class ServiceScopeFactory : IServiceScopeFactory
    {
        private Microsoft.Extensions.DependencyInjection.IServiceCollection services;

        public ServiceScopeFactory(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
        {
            this.services = services;
        }

        public IServiceScope CreateScope()
        {
            var result = new ScopedServices();
            foreach (var serviceDescriptor in this.services)
            {
                if (serviceDescriptor.ServiceType == typeof(global::TestService))
                {
                    result._TestService = (global::TestService)serviceDescriptor.ImplementationInstance;
                }
            }

            return result;
        }
    }

    private ServiceScopeFactory serviceScopeFactory;

    private Microsoft.Extensions.DependencyInjection.IServiceScope implicitScope;

    public void Dispose()
    {
        implicitScope.Dispose();
    }

    internal class ScopedServices : IServiceProvider, Microsoft.Extensions.DependencyInjection.IServiceScope
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
                return _TestService;
            }

            return null;
        }
    }

    public object GetService(Type t)
    {
        if (t == typeof(Microsoft.Extensions.DependencyInjection.IServiceScopeFactory))
        {
            return serviceScopeFactory;
        }

        return implicitScope.ServiceProvider.GetService(t);
    }
}

public static class StoreKeeperExtensions
{
    public static ServiceProviderAot BuildServiceProviderAot(this Microsoft.Extensions.DependencyInjection.IServiceCollection services, Microsoft.Extensions.DependencyInjection.ServiceProviderOptions options)
    {
        return new ServiceProviderAot(services);
    }
    public static ServiceProviderAot BuildServiceProviderAot(this Microsoft.Extensions.DependencyInjection.IServiceCollection services)
    {
        return new ServiceProviderAot(services);
    }
}
";
        Assert.AreEqual(expectedOutput, output);
    }
}
