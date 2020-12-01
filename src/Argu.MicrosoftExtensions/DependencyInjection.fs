namespace Microsoft.Extensions.DependencyInjection

open System
open System.Runtime.CompilerServices
open Microsoft.Extensions.DependencyInjection.Extensions
open Argu
open Argu.MicrosoftExtensions

[<Extension>]
type ArguExtensions =

    [<Extension>]
    static member AddArguConfigurationReader(this: IServiceCollection) =
        this.TryAddSingleton<IConfigurationReader, ConfigurationReader>()

    [<Extension>]
    static member AddArgu<'T when 'T : not struct and 'T :> IArgParserTemplate>
        (
            this: IServiceCollection,
            configure: Func<IConfigurationReader, ParseResults<'T>>
        ) =
        this.AddSingleton<ParseResults<'T>>(fun services ->
                configure.Invoke(services.GetRequiredService<IConfigurationReader>())
            )
            .AddArguConfigurationReader()

    [<Extension>]
    static member AddArgu<'T when 'T : not struct and 'T :> IArgParserTemplate>
        (
            this: IServiceCollection,
            ?inputs: string[],
            ?ignoreMissing: bool,
            ?ignoreUnrecognized: bool,
            ?raiseOnUsage: bool,
            ?useConfiguration: bool
        ) =
        let create configurationReader =
            ArgumentParser.Create<'T>().Parse(
                ?inputs = inputs,
                ?configurationReader = configurationReader,
                ?ignoreMissing = ignoreMissing,
                ?ignoreUnrecognized = ignoreUnrecognized,
                ?raiseOnUsage = raiseOnUsage
            )

        let useConfiguration = useConfiguration |> Option.defaultValue true
        if useConfiguration then
            this.AddArgu<'T>(create << Some)
        else
            this.AddSingleton<ParseResults<'T>>(create None) |> ignore
