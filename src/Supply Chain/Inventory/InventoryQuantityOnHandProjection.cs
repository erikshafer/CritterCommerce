using JasperFx.Events;
using Marten.Events.Aggregation;

namespace Inventory;

public class InventoryQuantityOnHand
{
    public Guid Id { get; set; }
    public string Sku { get; set; }
    public int? QuantityOnHand { get; set; }
}

public class InventoryQuantityOnHandProjection : SingleStreamProjection<InventoryQuantityOnHand, Guid>
{
    public InventoryQuantityOnHandProjection()
    {
        // Notice there's no starting/creation/init event here. Read below at Create().
        ProjectEvent<InventoryIncremented>(Apply);
        ProjectEvent<InventoryDecremented>(Apply);
    }

    /// <summary>
    /// Note that Apply() methods are used for all the other events, but Marten
    /// has a convention it introduced a few versions back to handle the creation
    /// of projections better. Thus, the Create() method.
    /// Below you can see we're using IEvent<TEvent>, which will also include
    /// meta-data and other goodies, which perhaps you'll want during a projection's
    /// creation. Otherwise if you just want your standard event data, just call upon
    /// `.Data` on the IEvent.
    /// </summary>
    public InventoryQuantityOnHand Create(IEvent<InventoryInitialized> evnt)
    {
        return new InventoryQuantityOnHand { Id = evnt.Id, Sku = evnt.Data.Sku, QuantityOnHand = 0 };
    }

    public void Apply(InventoryQuantityOnHand readModel, InventoryIncremented evnt)
    {
        readModel.QuantityOnHand += evnt.Quantity;
    }

    public void Apply(InventoryQuantityOnHand readModel, InventoryDecremented evnt)
    {
        readModel.QuantityOnHand -= evnt.Quantity;
    }
}
