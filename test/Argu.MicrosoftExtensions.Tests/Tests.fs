module Tests

open System.IO
open System.Text
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Xunit
open Argu

type Args =
    | [<CustomAppSettings "credentials:username">] Username of string
    | [<CustomAppSettings "credentials:password">] Password of string
    | Item_Count of int

    interface Argu.IArgParserTemplate with

        member this.Usage =
            match this with
            | Username _ -> "The username"
            | Password _ -> "The password"
            | Item_Count _ -> "The count"

let configJson = """
{
    "credentials": {
        "username": "hello",
        "password": "world"
    },
    "item count": 42
}
"""

let buildHost argv =
    HostBuilder()
        .ConfigureAppConfiguration(fun config ->
            let stream = new MemoryStream(Encoding.UTF8.GetBytes(configJson))
            config.AddJsonStream(stream)
            |> ignore)
        .ConfigureServices(fun services ->
            services.AddArgu<Args>(inputs = argv))
        .Build()

[<Fact>]
let ``Can parse command line`` () =
    let host = buildHost [|"--username"; "johndoe"; "--password"; "p455w0rd"; "--item-count"; "20"|]
    let args = host.Services.GetService<ParseResults<Args>>()
    Assert.Equal("johndoe", args.GetResult <@ Username @>)
    Assert.Equal("p455w0rd", args.GetResult <@ Password @>)
    Assert.Equal(20, args.GetResult <@ Item_Count @>)

[<Fact>]
let ``Can parse configuration`` () =
    let host = buildHost [||]
    let args = host.Services.GetService<ParseResults<Args>>()
    Assert.Equal("hello", args.GetResult <@ Username @>)
    Assert.Equal("world", args.GetResult <@ Password @>)
    Assert.Equal(42, args.GetResult <@ Item_Count @>)
