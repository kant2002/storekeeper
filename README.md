Store Keeper
============

Nice store keeper, keeps all you classes in the store. All dependencies would be stored there. Also AOT friendly.

To try it out, look at the `StoreKeeper.CompilationTests` project.
To run test suite

    dotnet run --project StoreKeeper.CompilationTests/StoreKeeper.CompilationTests.csproj 

# Implementation plan

- [x] Scoped registration using class.
- [x] Scoped registration using interface.
- [x] Scoped registration using `Func<T>`.
- [x] Scoped registration using instance object.
- [x] Implement disposing scope services.
- [ ] Implement asyncronous disposing scope services.
- [x] Implement singleton services.
- [ ] Implement transient services.
- [ ] Dependency resolution.
- [ ] Support derived from ServiceCollection classes.
- [ ] Support for `BuildServiceProvider`.
- [ ] Support for `IEnumerable<T>` services.
- [ ] Dynamic registration of services.
- [ ] Registrations across assemblies.
- [ ] Generic Host support.
- [ ] Services with `internal` visibility.

# Usage example

Almost identical with ServiceCollection usage except `BuildServiceProviderAot` method produce statically constructed service provider.

```csharp
var serviceContainer = new ServiceCollection();
serviceContainer.AddScoped<EnglishGreeting>();
serviceContainer.AddScoped<IGreeting, EnglishGreeting>();
var serviceProvider = serviceContainer.BuildServiceProviderAot();
```
