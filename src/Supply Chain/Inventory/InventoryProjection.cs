using Marten.Events.Aggregation;

namespace Inventory;

public class InventoryReadModel
{
    public Guid Id { get; set; }
    public string Sku { get; set; }
    public int QuantityOnHand { get; set; }
}

public class InventoryProjection : SingleStreamProjection<InventoryReadModel, Guid>
{
    public InventoryProjection()
    {
        ProjectEvent<InventoryInitialized>(Apply);
        ProjectEvent<InventoryIncremented>(Apply);
        ProjectEvent<InventoryDecremented>(Apply);
    }

    public void Apply(InventoryReadModel readModel, InventoryInitialized evnt)
    {
        readModel.Id =  evnt.Id;
        readModel.Sku = evnt.Sku;
    }

    public void Apply(InventoryReadModel readModel, InventoryIncremented evnt)
    {
        readModel.QuantityOnHand += evnt.Quantity;
    }

    public void Apply(InventoryReadModel readModel, InventoryDecremented evnt)
    {
        readModel.QuantityOnHand -= evnt.Quantity;
    }
}
