Store Keeper
============

Nice store keeper, keeps all you classes in the store. All dependencies would be stored there. Also AOT friendly.

# How to use

Add `SqlMarshal` Nuget package using

```
dotnet add package StoreKeeper
```

To try it out, look at the `StoreKeeper.CompilationTests` project.
To run test suite

    dotnet run --project StoreKeeper.CompilationTests/StoreKeeper.CompilationTests.csproj 

# AOT preparation example

This library provides `UseAotServices` method which replaces all implicit service construction with
explicit factory functions. This makes service collection ready for AOT with reflection-free mode.

```csharp
var serviceContainer = new ServiceCollection();
serviceContainer.AddScoped<EnglishGreeting>();
serviceContainer.AddScoped<IGreeting, EnglishGreeting>();
var serviceProvider = serviceContainer.UseAotServices().BuildServiceProvider();

// Registration
interface IGreeting
{
    string SayHello(string name);
}

internal class EnglishGreeting : IGreeting
{
    public string SayHello(string name)
    {
        return $"Hello {name}!";
    }
}
```

You do not need to do anything except recompilation of the project, after adding library.
Source generator make sure that your code would work in reflection-free mode. 
Regular NativeAOT mode can also use better code generation pattern and do not rely on reflection.

# Experimental statically resolving container

Almost identical with ServiceCollection usage except `BuildServiceProviderAot` method produce statically constructed service provider.

```csharp
var serviceContainer = new ServiceCollection();
serviceContainer.AddScoped<EnglishGreeting>();
serviceContainer.AddScoped<IGreeting, EnglishGreeting>();
var serviceProvider = serviceContainer.BuildServiceProviderAot();
```

# Implementation plan

- [x] Scoped registration using class.
- [x] Scoped registration using interface.
- [x] Scoped registration using `Func<T>`.
- [x] Scoped registration using instance object.
- [x] Implement disposing scope services.
- [ ] Implement asyncronous disposing scope services.
- [x] Implement singleton services.
- [ ] Implement transient services.
- [x] Dependency resolution.
- [ ] Support derived from ServiceCollection classes.
- [ ] Support for `BuildServiceProvider`.
- [ ] Support for `IEnumerable<T>` services.
- [ ] Dynamic registration of services.
- [ ] Registrations across assemblies.
- [ ] Generic Host support.
- [ ] Services with `internal` visibility.
- [x] Add `UseAotServices` method which will replace all service registration within assembly.