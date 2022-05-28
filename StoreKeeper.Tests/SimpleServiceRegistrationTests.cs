// -----------------------------------------------------------------------
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
        this.serviceScopeFactory = new ServiceScopeFactory(services, null);
        this.implicitScope = serviceScopeFactory.CreateScope();
    }

    private sealed class ServiceScopeFactory : IServiceScopeFactory
    {
        private Microsoft.Extensions.DependencyInjection.IServiceCollection services;

        private IServiceProvider singletonScope;

        public ServiceScopeFactory(Microsoft.Extensions.DependencyInjection.IServiceCollection services, IServiceProvider singletonScope)
        {
            this.services = services;
            this.singletonScope = singletonScope;
        }

        public IServiceScope CreateScope()
        {
            var result = new ScopedServices(this.singletonScope);
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
        private IServiceProvider singletonScope;

        public ScopedServices(IServiceProvider singletonScope)
        {
            this.singletonScope = singletonScope;
        }

        public IServiceProvider ServiceProvider => this;

        public void Dispose()
        {
        }

        internal global::TestService _TestService;

        public object GetService(Type t)
        {
            if (t == typeof(global::TestService))
            {
                _TestService = _TestService ?? new global::TestService();
                return _TestService;
            }

            return singletonScope?.GetService(t);
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

internal static class ServicesReplacementExtensions
{
    public static object Build_global_TestService(IServiceProvider serviceProvider)
    {
        return new global::TestService();
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
        this.serviceScopeFactory = new ServiceScopeFactory(services, null);
        this.implicitScope = serviceScopeFactory.CreateScope();
    }

    private sealed class ServiceScopeFactory : IServiceScopeFactory
    {
        private Microsoft.Extensions.DependencyInjection.IServiceCollection services;

        private IServiceProvider singletonScope;

        public ServiceScopeFactory(Microsoft.Extensions.DependencyInjection.IServiceCollection services, IServiceProvider singletonScope)
        {
            this.services = services;
            this.singletonScope = singletonScope;
        }

        public IServiceScope CreateScope()
        {
            var result = new ScopedServices(this.singletonScope);
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
        private IServiceProvider singletonScope;

        public ScopedServices(IServiceProvider singletonScope)
        {
            this.singletonScope = singletonScope;
        }

        public IServiceProvider ServiceProvider => this;

        public void Dispose()
        {
        }

        internal global::TestService _ITestService;

        public object GetService(Type t)
        {
            if (t == typeof(global::ITestService))
            {
                _ITestService = _ITestService ?? new global::TestService();
                return _ITestService;
            }

            return singletonScope?.GetService(t);
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

internal static class ServicesReplacementExtensions
{
    public static object Build_global_ITestService_global_TestService(IServiceProvider serviceProvider)
    {
        return new global::TestService();
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
        this.serviceScopeFactory = new ServiceScopeFactory(services, null);
        this.implicitScope = serviceScopeFactory.CreateScope();
    }

    private sealed class ServiceScopeFactory : IServiceScopeFactory
    {
        private Microsoft.Extensions.DependencyInjection.IServiceCollection services;

        private IServiceProvider singletonScope;

        public ServiceScopeFactory(Microsoft.Extensions.DependencyInjection.IServiceCollection services, IServiceProvider singletonScope)
        {
            this.services = services;
            this.singletonScope = singletonScope;
        }

        public IServiceScope CreateScope()
        {
            var result = new ScopedServices(this.singletonScope);
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
        private IServiceProvider singletonScope;

        public ScopedServices(IServiceProvider singletonScope)
        {
            this.singletonScope = singletonScope;
        }

        public IServiceProvider ServiceProvider => this;

        public void Dispose()
        {
            if (_TestService != null)
            {
                ((System.IDisposable)_TestService).Dispose();
                _TestService = null;
            }

        }

        internal global::TestService _TestService;

        public object GetService(Type t)
        {
            if (t == typeof(global::TestService))
            {
                _TestService = _TestService ?? new global::TestService();
                return _TestService;
            }

            return singletonScope?.GetService(t);
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

internal static class ServicesReplacementExtensions
{
    public static object Build_global_TestService(IServiceProvider serviceProvider)
    {
        return new global::TestService();
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
        this.serviceScopeFactory = new ServiceScopeFactory(services, null);
        this.implicitScope = serviceScopeFactory.CreateScope();
    }

    private sealed class ServiceScopeFactory : IServiceScopeFactory
    {
        private Microsoft.Extensions.DependencyInjection.IServiceCollection services;

        private IServiceProvider singletonScope;

        public ServiceScopeFactory(Microsoft.Extensions.DependencyInjection.IServiceCollection services, IServiceProvider singletonScope)
        {
            this.services = services;
            this.singletonScope = singletonScope;
        }

        public IServiceScope CreateScope()
        {
            var result = new ScopedServices(this.singletonScope);
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
        private IServiceProvider singletonScope;

        public ScopedServices(IServiceProvider singletonScope)
        {
            this.singletonScope = singletonScope;
        }

        public IServiceProvider ServiceProvider => this;

        public void Dispose()
        {
        }

        internal global::TestService _TestService;

        public object GetService(Type t)
        {
            if (t == typeof(global::TestService))
            {
                return _TestService;
            }

            return singletonScope?.GetService(t);
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
    public void ScopedRegistrationUsingFactory()
    {
        string source = @"
using Microsoft.Extensions.DependencyInjection;

class TestService {}

class Test
{
    void Method()
    {
        var services = new ServiceCollection();
        services.AddScoped(() => new TestService());
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
        this.serviceScopeFactory = new ServiceScopeFactory(services, null);
        this.implicitScope = serviceScopeFactory.CreateScope();
    }

    private sealed class ServiceScopeFactory : IServiceScopeFactory
    {
        private Microsoft.Extensions.DependencyInjection.IServiceCollection services;

        private IServiceProvider singletonScope;

        public ServiceScopeFactory(Microsoft.Extensions.DependencyInjection.IServiceCollection services, IServiceProvider singletonScope)
        {
            this.services = services;
            this.singletonScope = singletonScope;
        }

        public IServiceScope CreateScope()
        {
            var result = new ScopedServices(this.singletonScope);
            foreach (var serviceDescriptor in this.services)
            {
                if (serviceDescriptor.ServiceType == typeof(global::TestService))
                {
                    result._TestServiceFactory = (Func<IServiceProvider, Object>)serviceDescriptor.ImplementationFactory;
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
        private IServiceProvider singletonScope;

        public ScopedServices(IServiceProvider singletonScope)
        {
            this.singletonScope = singletonScope;
        }

        public IServiceProvider ServiceProvider => this;

        public void Dispose()
        {
        }

        internal global::TestService _TestService;

        internal Func<IServiceProvider, Object> _TestServiceFactory;

        public object GetService(Type t)
        {
            if (t == typeof(global::TestService))
            {
                _TestService = _TestService ?? (global::TestService)_TestServiceFactory(this);
                return _TestService;
            }

            return singletonScope?.GetService(t);
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
    public void SingletonRegistrationUsingInstance()
    {
        string source = @"
using Microsoft.Extensions.DependencyInjection;

class TestService {}

class Test
{
    void Method()
    {
        var services = new ServiceCollection();
        services.AddSingleton(new TestService());
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
        this.singletonScope = new SingletonServices(null);
        foreach (var serviceDescriptor in services)
        {
            if (serviceDescriptor.ServiceType == typeof(global::TestService))
            {
                this.singletonScope._TestService = (global::TestService)serviceDescriptor.ImplementationInstance;
            }
        }

        this.serviceScopeFactory = new ServiceScopeFactory(services, this.singletonScope);
        this.implicitScope = serviceScopeFactory.CreateScope();
    }

    private sealed class ServiceScopeFactory : IServiceScopeFactory
    {
        private Microsoft.Extensions.DependencyInjection.IServiceCollection services;

        private IServiceProvider singletonScope;

        public ServiceScopeFactory(Microsoft.Extensions.DependencyInjection.IServiceCollection services, IServiceProvider singletonScope)
        {
            this.services = services;
            this.singletonScope = singletonScope;
        }

        public IServiceScope CreateScope()
        {
            var result = new ScopedServices(this.singletonScope);
            return result;
        }
    }

    private ServiceScopeFactory serviceScopeFactory;

    private Microsoft.Extensions.DependencyInjection.IServiceScope implicitScope;

    private SingletonServices singletonScope;

    public void Dispose()
    {
        implicitScope.Dispose();
    }

    internal class ScopedServices : IServiceProvider, Microsoft.Extensions.DependencyInjection.IServiceScope
    {
        private IServiceProvider singletonScope;

        public ScopedServices(IServiceProvider singletonScope)
        {
            this.singletonScope = singletonScope;
        }

        public IServiceProvider ServiceProvider => this;

        public void Dispose()
        {
        }

        public object GetService(Type t)
        {
            return singletonScope?.GetService(t);
        }
    }

    internal class SingletonServices : IServiceProvider, Microsoft.Extensions.DependencyInjection.IServiceScope
    {
        private IServiceProvider singletonScope;

        public SingletonServices(IServiceProvider singletonScope)
        {
            this.singletonScope = singletonScope;
        }

        public IServiceProvider ServiceProvider => this;

        public void Dispose()
        {
        }

        internal global::TestService _TestService;

        public object GetService(Type t)
        {
            if (t == typeof(global::TestService))
            {
                return _TestService;
            }

            return singletonScope?.GetService(t);
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
    public void ScopedRegistrationResolveDependency()
    {
        string source = @"
using Microsoft.Extensions.DependencyInjection;

class DependentTestService {}
class TestService
{
    public TestService(DependentTestService dependency) {}
}

class Test
{
    void Method()
    {
        var services = new ServiceCollection();
        services.AddScoped<DependentTestService>();
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
        this.serviceScopeFactory = new ServiceScopeFactory(services, null);
        this.implicitScope = serviceScopeFactory.CreateScope();
    }

    private sealed class ServiceScopeFactory : IServiceScopeFactory
    {
        private Microsoft.Extensions.DependencyInjection.IServiceCollection services;

        private IServiceProvider singletonScope;

        public ServiceScopeFactory(Microsoft.Extensions.DependencyInjection.IServiceCollection services, IServiceProvider singletonScope)
        {
            this.services = services;
            this.singletonScope = singletonScope;
        }

        public IServiceScope CreateScope()
        {
            var result = new ScopedServices(this.singletonScope);
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
        private IServiceProvider singletonScope;

        public ScopedServices(IServiceProvider singletonScope)
        {
            this.singletonScope = singletonScope;
        }

        public IServiceProvider ServiceProvider => this;

        public void Dispose()
        {
        }

        internal global::DependentTestService _DependentTestService;

        internal global::TestService _TestService;

        public object GetService(Type t)
        {
            if (t == typeof(global::DependentTestService))
            {
                _DependentTestService = _DependentTestService ?? new global::DependentTestService();
                return _DependentTestService;
            }

            if (t == typeof(global::TestService))
            {
                _TestService = _TestService ?? new global::TestService((global::DependentTestService)GetService(typeof(global::DependentTestService)));
                return _TestService;
            }

            return singletonScope?.GetService(t);
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

internal static class ServicesReplacementExtensions
{
    public static object Build_global_DependentTestService(IServiceProvider serviceProvider)
    {
        return new global::DependentTestService();
    }

    public static object Build_global_TestService(IServiceProvider serviceProvider)
    {
        var param_0 = (global::DependentTestService)serviceProvider.GetService(typeof(global::DependentTestService));
        return new global::TestService(param_0);
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
