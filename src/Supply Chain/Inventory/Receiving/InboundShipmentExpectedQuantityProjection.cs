using Inventory.Receiving;
using Marten.Events.Projections;

public class InboundShipmentExpectedQuantityView
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public int ExpectedQuantity { get; set; }
}

// TODO: may want to make this a SingleStreamProjection instead of MultiStreamProjection
public class InboundShipmentExpectedQuantityProjection : MultiStreamProjection<InboundShipmentExpectedQuantityView, Guid>
{
    public InboundShipmentExpectedQuantityProjection()
    {
        Identity<InboundOrderScheduled>(x => x.Id);
        // Identity<InboundShipmentLineItemAdded>(x => x.Sku); // Using SKU as identity for line items // TODO
    }

    public void Apply(InboundOrderScheduled @event, InboundShipmentExpectedQuantityView view)
    {
        view.Id = @event.Id;
    }

    public void Apply(InboundShipmentLineItemAdded @event, InboundShipmentExpectedQuantityView view)
    {
        view.Sku = @event.Sku;
        view.ExpectedQuantity = @event.ExpectedQuantity;
    }
}
