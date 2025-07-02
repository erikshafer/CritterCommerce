using Inventory.Api.Documents;
using Inventory.Api.Vendors;
using Marten;
using Wolverine;
using Wolverine.Marten;

namespace Inventory.Api.Receiving;

public record ScheduleInboundOrder(Guid ShipmentId, string ShipmentNumber, DateTime ExpectedArrival);

public static class ScheduleInboundOrderHandler
{
    public static async Task<(HandlerContinuation, ProcurementOrder?, Vendor?)> LoadAsync(
        ScheduleInboundOrder command,
        IDocumentSession session)
    {
        var order = await session.LoadAsync<ProcurementOrder>(command.ShipmentNumber);
        if (order == null)
            return (HandlerContinuation.Stop, null, null);

        var vendor = await session.LoadAsync<Vendor>(order.VendorId);
        if (vendor == null)
            return (HandlerContinuation.Stop, order, null);

        return (HandlerContinuation.Continue, order, vendor);
    }

    // [AggregateHandler]
    public static Events Handle(ScheduleInboundOrder command, ProcurementOrder order, Vendor vendor)
    {
        var events = new Events();
        events += new InboundOrderScheduled(command.ShipmentId, command.ShipmentNumber, command.ExpectedArrival);
        return events;
    }
}
