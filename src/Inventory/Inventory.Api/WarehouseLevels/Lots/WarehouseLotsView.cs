using JasperFx.Events;
using Marten.Events.Projections;

namespace Inventory.Api.WarehouseLevels.Lots;

public sealed record WarehouseLotsView
{
    public Guid Id { get; set; }
    public string Warehouse { get; set; } = null!;
    public string Lot { get; set; } = null!;
}

public class WarehouseLotsProjection : MultiStreamProjection<WarehouseLotsView, Guid>
{
    public WarehouseLotsProjection()
    {
        Identity<IEvent<InventoryInitialized>>(e => e.StreamId);
        Identity<IEvent<InventoryLotMoved>>(e => e.StreamId);
    }

    public WarehouseLotsView Create(IEvent<InventoryInitialized> @event) => new()
    {
        Id = @event.StreamId,
        Warehouse = @event.Data.Facility,
        Lot = @event.Data.FacilityLotId
    };

    public void Apply(WarehouseLotsView view, InventoryLotMoved @event) =>
        view.Lot = @event.FacilityLotId;
}
