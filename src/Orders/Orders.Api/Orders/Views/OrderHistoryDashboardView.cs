using Marten.Events.Aggregation;

namespace Orders.Api.Orders.Views;

public class OrderFulfillmentDashboardView
{
    public Guid Id { get; set; }
    public int Placed { get; set; }
    public int Confirmed { get; set; }
    public int Canceled { get; set; }
    public int Fulfilled { get; set; }
}

public class OrderFulfillmentDashboardProjection : SingleStreamProjection<OrderFulfillmentDashboardView, Guid>
{
    public void Apply(OrderPlaced evt, OrderFulfillmentDashboardView view)
    {
        view.Placed += 1;
    }

    public void Apply(OrderConfirmed evt, OrderFulfillmentDashboardView view)
    {
        view.Confirmed += 1;
        view.Placed -= 1; // move from Placed to Confirmed
    }

    public void Apply(OrderCanceled evt, OrderFulfillmentDashboardView view)
    {
        view.Canceled += 1;

        // Decrement from either Confirmed or Placed, depending on state.
        if (view.Confirmed > 0)
            view.Confirmed -= 1;
        else if (view.Placed > 0)
            view.Placed -= 1;
    }

    // This event would come from Fulfillment stream, illustrating cross-stream projections:
    public void Apply(OrderFulfilled evt, OrderFulfillmentDashboardView view)
    {
        view.Fulfilled += 1;
        view.Confirmed -= 1;
    }
}
