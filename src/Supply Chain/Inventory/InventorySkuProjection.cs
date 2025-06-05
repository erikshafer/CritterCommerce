using JasperFx.Events;
using Marten.Events.Aggregation;

namespace Inventory;

public class InventorySku
{
    public Guid Id { get; set; }
    public string Sku { get; set; }
}

public class InventorySkuProjection : SingleStreamProjection<InventorySku, Guid>
{
    public InventorySkuProjection()
    {
        // nothing
    }

    public InventorySku Create(IEvent<InventoryInitialized> evnt)
    {
        return new InventorySku { Id = evnt.Id, Sku = evnt.Data.Sku };
    }
}
