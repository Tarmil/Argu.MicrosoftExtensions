namespace Argu.MicrosoftExtensions

open Microsoft.Extensions.Configuration
open Argu


type ConfigurationReader(config: IConfiguration) =
    interface IConfigurationReader with
        member _.GetValue(k) = config.[k]
        member _.Name = "Microsoft.Extensions hosted configuration"