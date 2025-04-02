using Wolverine.Http;

namespace Catalog.Api;

public class HelloEndpoint
{
    [WolverineGet("/")]
    public string Get() => "Hello.";
}
