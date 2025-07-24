namespace Orders.Api.Orders;

using Wolverine;
using Marten.Schema;

public record OrderLine(Guid SkuId, int Quantity, decimal UnitPrice);
public record ShippingInfo(string Address, string City, string PostalCode, string Country);

public class Order
{
    [Identity] public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Created;
    public List<OrderLine> Lines { get; set; } = new();
    public decimal TotalPrice { get; set; }
    public ShippingInfo ShippingDetails { get; set; } = default!;

    public void Apply(OrderPlaced evt)
    {
        Id = evt.OrderId;
        CustomerId = evt.CustomerId;
        Lines = evt.Lines.ToList();
        TotalPrice = Lines.Sum(x => x.UnitPrice * x.Quantity);
        ShippingDetails = evt.ShippingDetails;
        Status = OrderStatus.Created;
    }

    public void Apply(OrderConfirmed _) => Status = OrderStatus.Confirmed;
    public void Apply(OrderCanceled _) => Status = OrderStatus.Canceled;
    public void Apply(OrderReturned _) => Status = OrderStatus.Returned;
}
