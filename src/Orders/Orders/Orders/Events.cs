namespace Orders.Orders;

public record OrderPlaced(
    Guid OrderId,
    Guid CustomerId,
    IEnumerable<OrderLine> Lines,
    ShippingInfo ShippingDetails,
    DateTimeOffset PlacedAt
);

public record OrderConfirmed(
    Guid OrderId,
    DateTimeOffset ConfirmedAt
);

public record OrderCanceled(
    Guid OrderId,
    Guid RequestedBy,
    DateTimeOffset CanceledAt,
    string Reason
);

public record OrderReturned(
    Guid OrderId,
    IEnumerable<ReturnLine> ReturnLines,
    DateTimeOffset ReturnedAt,
    string Reason
);
