using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Newtonsoft.Json")] // To be able to deserialize OperationError
[assembly: InternalsVisibleTo("TixFactory.Operations.Tests.Unit")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")] // FakeItEasy
