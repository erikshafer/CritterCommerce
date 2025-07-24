using Marten.Events.Projections;

namespace Orders.Api.Orders.Views;

public class CustomerOrderHistoryView
{
    public Guid Id { get; set; } // CustomerId
    public List<OrderSummary> Orders { get; set; } = new();
}

public class OrderSummary
{
    public Guid OrderId { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = default!;
    public DateTimeOffset PlacedAt { get; set; }
    public DateTimeOffset? ConfirmedAt { get; set; }
    public DateTimeOffset? CanceledAt { get; set; }
}

public class CustomerOrderHistoryProjection : MultiStreamProjection<CustomerOrderHistoryView, Guid>
{
    public CustomerOrderHistoryProjection()
    {
        Identity<OrderPlaced>(e => e.CustomerId);
        Identity<OrderConfirmed>(e => e.OrderId);
        Identity<OrderCanceled>(e => e.OrderId);
    }

    public void Apply(OrderPlaced evt, CustomerOrderHistoryView view)
    {
        view.Id = evt.CustomerId;
        view.Orders.Add(new OrderSummary
        {
            OrderId = evt.OrderId,
            TotalPrice = evt.Lines.Sum(x => x.Quantity * x.UnitPrice),
            Status = "Placed",
            PlacedAt = evt.PlacedAt
        });
    }

    public void Apply(OrderConfirmed evt, CustomerOrderHistoryView view)
    {
        var order = view.Orders.FirstOrDefault(x => x.OrderId == evt.OrderId);
        if (order is not null)
        {
            order.Status = "Confirmed";
            order.ConfirmedAt = evt.ConfirmedAt;
        }
    }

    public void Apply(OrderCanceled evt, CustomerOrderHistoryView view)
    {
        var order = view.Orders.FirstOrDefault(x => x.OrderId == evt.OrderId);
        if (order is not null)
        {
            order.Status = "Canceled";
            order.CanceledAt = evt.CanceledAt;
        }
    }
}
