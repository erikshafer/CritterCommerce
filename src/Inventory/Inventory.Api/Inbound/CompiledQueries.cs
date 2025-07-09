using System.Linq.Expressions;
using Inventory.Api.Locations;
using Marten.Linq;

namespace Inventory.Api.Inbound;

public class FindLocationByName : ICompiledQuery<Location, Location>
{
    public string Name { get; set; } = null!;

    public Expression<Func<IMartenQueryable<Location>, Location>> QueryIs() =>
        q => q.FirstOrDefault(x => x.Name == Name)!;
}
