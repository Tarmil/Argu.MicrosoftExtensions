# Argu.MicrosoftExtensions

This library helps integrate [Argu](http://fsprojects.github.io/Argu/), the F# argument and configuration parsing library, into Microsoft.Extensions hosted applications.
It provides:

* Injecting `ParseResults` into the dependency injection;
* Parsing configuration items (from eg `appSettings.json`) with Argu.

## Injecting ParseResults

Add Argu parse results to the dependency injection using `services.AddArgu<Args>()`:

```fsharp
type Args =
    | Username of string
    | Password of string

type Startup() =

    member this.ConfigureServices(services: IServiceCollection) =
        services.AddArgu<Args>()
```

This method takes most of the same optional arguments as [`ArgumentParser.Parse()`](http://fsprojects.github.io/Argu/reference/argu-argumentparser-1.html#Parse):

```fsharp
        services.AddArgu<Args>(ignoreMissing = true, raiseOnUsage = false)
```

Alternately, it can take a function of type `IConfigurationReader -> ParseResults<Args>`:

```fsharp
        services.AddArgu<Args>(fun config ->
            ArgumentParser.Create<Args>().Parse(configurationReader = config))
```

You can now use the injected parse results:

```fsharp
type MyService(args: ParseResults<Args>) =

    let username = args.GetResult Username
    let password = args.GetResult Password
```

## Parsing configuration

The argument parser will also parse values from the injected configuration.

```json
// appSettings.json
{
    "username": "johndoe",
    "password": "p455w0rd"
}
```

```fsharp
type Args =
    | Username of string
    | Password of string
```

To use nested keys, use `CustomAppSettings` with colons as separators.

```json
// appSettings.json
{
    "credentials": {
        "username": "johndoe",
        "password": "p455w0rd"
    }
}
```

```fsharp
type Args =
    | [<CustomAppSettings "credentials:username">] Username of string
    | [<CustomAppSettings "credentials:password">] Password of string
```

To disable reading arguments from the configuration, pass `useConfiguration = false` to `services.AddArgu()`.
