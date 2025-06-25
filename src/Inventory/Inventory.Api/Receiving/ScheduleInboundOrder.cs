using Wolverine.Marten;

namespace Inventory.Api.Receiving;

public record ScheduleInboundOrder(Guid ShipmentId, string ShipmentNumber, DateTime ExpectedArrival);

public static class ScheduleInboundOrderHandler
{
    [AggregateHandler]
    public static Events Handle(ScheduleInboundOrder command)
    {
        var events = new Events();
        events += new InboundOrderScheduled(command.ShipmentId, command.ShipmentNumber, command.ExpectedArrival);
        return events;
    }
}
