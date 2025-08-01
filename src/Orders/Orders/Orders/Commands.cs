namespace Orders.Orders;

public record PlaceOrder(
    Guid CustomerId,
    IEnumerable<OrderLine> Lines,
    ShippingInfo ShippingDetails
);


public record CancelOrder(
    Guid OrderId,
    Guid RequestedBy,
    string Reason);

public record ReturnLine(
    Guid SkuId,
    int Quantity);
public record ReturnOrder(
    Guid OrderId,
    IEnumerable<ReturnLine> ReturnLines,
    string Reason);
