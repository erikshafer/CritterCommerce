using Wolverine.Marten;

namespace Orders.Orders;

[AggregateHandler]
public static class OrderHandler
{
    public static OrderPlaced Handle(PlaceOrder cmd)
    {
        return new OrderPlaced(
            OrderId: Guid.NewGuid(),
            CustomerId: cmd.CustomerId,
            Lines: cmd.Lines,
            ShippingDetails: cmd.ShippingDetails,
            PlacedAt: DateTimeOffset.UtcNow
        );
    }

    public static IEnumerable<object> Handle(CancelOrder cmd, Order order)
    {
        if (order.Status is OrderStatus.Canceled or OrderStatus.Returned)
            yield break; // Already canceled or returned

        yield return new OrderCanceled(
            OrderId: cmd.OrderId,
            RequestedBy: cmd.RequestedBy,
            CanceledAt: DateTimeOffset.UtcNow,
            Reason: cmd.Reason
        );
    }

    public static IEnumerable<object> Handle(ReturnOrder cmd, Order order)
    {
        if (order.Status != OrderStatus.Confirmed)
            yield break; // Cannot return unfulfilled orders

        yield return new OrderReturned(
            OrderId: cmd.OrderId,
            ReturnLines: cmd.ReturnLines,
            ReturnedAt: DateTimeOffset.UtcNow,
            Reason: cmd.Reason
        );
    }

    // Example reaction to external event (Payments)
    public static OrderConfirmed Handle(Payments.Events.PaymentCaptured evt, Order order)
    {
        if (order.Status != OrderStatus.Created)
            return null!; // Ignore if already confirmed/canceled

        return new OrderConfirmed(evt.OrderId, DateTimeOffset.UtcNow);
    }
}

public class Payments
{
    public class Events
    {
        public record PaymentCaptured(Guid OrderId);
    }
}
