namespace Inventory.Procurements;

public sealed record VendorProcurementOrderReceived(
    Guid Id,
    Guid VendorId,
    Guid FulfillmentCenterId,
    DateTimeOffset SignedAt,
    Guid SignedByVendorUserId,
    string SignedByVendorUserEmail
);

public sealed record VendorProcurementOrderItemsRecorded(
    Guid Id,
    SkuNameQuantity[] Items
);

public sealed record VendorProcurementOrderContractReferenced(
    Guid Id,
    Guid VendorContractId,
    bool VendorContractValidated,
    DateTimeOffset VendorContractValidatedAt
);

public sealed record VendorProcurementOrderShipmentArrived(
    Guid Id,
    DateTimeOffset ArrivedAt,
    Guid ArrivalAcknowledgedByFcUserId
);

public sealed record VendorProcurementOrderCancelled(
    Guid Id,
    DateTimeOffset CancelledAt,
    VendorProcurementOrderCancellationReason Reason
);

public class VendorProcurementOrder
{
    public VendorProcurementOrder() { }

    public VendorProcurementOrder(VendorProcurementOrderReceived received)
    {
        Id = received.Id;
        VendorId = received.VendorId;
        FulfillmentCenterId = received.FulfillmentCenterId;
        SignedAt = received.SignedAt;
        SignedByVendorUserId = received.SignedByVendorUserId;
        SignedByVendorUserEmail = new Email(received.SignedByVendorUserEmail);
        Status = VendorProcurementOrderStatus.Received;
    }

    public Guid Id { get; set; }

    public int Version { get; set; }

    public VendorProcurementOrderStatus Status { get; set; }

    public Guid VendorId { get; set; }

    public Guid FulfillmentCenterId { get; set; }

    public DateTimeOffset SignedAt { get; set; }

    public Guid SignedByVendorUserId { get; set; }

    public Email SignedByVendorUserEmail { get; set; }

    public List<SkuNameQuantity> Items { get; set; } = [];

    public Guid? VendorContractId { get; set; }

    public bool? VendorContractValidated { get; set; }

    public DateTimeOffset? VendorContractValidatedAt { get; set; }

    public DateTimeOffset? ReceivedAt { get; set; }

    public Guid ArrivalAcknowledgedByFcUserId { get; set; }

    public DateTimeOffset? CancelledAt { get; set; }

    public VendorProcurementOrderCancellationReason? CancellationReason { get; set; }

    public void Apply(VendorProcurementOrderItemsRecorded recorded)
    {
        Items = new List<SkuNameQuantity>(recorded.Items);
        Status = VendorProcurementOrderStatus.PendingArrival;
    }

    public void Apply(VendorProcurementOrderContractReferenced referenced)
    {
        VendorContractId = referenced.VendorContractId;
        VendorContractValidated = referenced.VendorContractValidated;
        VendorContractValidatedAt = referenced.VendorContractValidatedAt;
    }

    public void Apply(VendorProcurementOrderShipmentArrived arrived)
    {
        ReceivedAt = arrived.ArrivedAt;
        ArrivalAcknowledgedByFcUserId = arrived.ArrivalAcknowledgedByFcUserId;
        Status = VendorProcurementOrderStatus.Arrived;
    }

    public void Apply(VendorProcurementOrderCancelled cancelled)
    {
        CancelledAt = cancelled.CancelledAt;
        CancellationReason = cancelled.Reason;
        Status = VendorProcurementOrderStatus.Cancelled;
    }

    /// <summary>
    /// A convenience method to determine if there's a vendor contract
    /// that was validated at time of processing and can be referenced.
    /// </summary>
    public bool DoesVendorContractExistAndIsValid() =>
        VendorContractId is not null
        && VendorContractId != Guid.Empty
        && VendorContractValidated is true;

    /// <summary>
    /// A convenience method to determine if this procurement order from
    /// a vendor has reached a completed lifecycle (state).
    /// Either the order Arrived or it was Cancelled.
    /// </summary>
    public bool IsInACompletedState() =>
        Status is VendorProcurementOrderStatus.Arrived
               or VendorProcurementOrderStatus.Cancelled;
}

public enum VendorProcurementOrderStatus
{
    Received = 1,
    PendingArrival = 2,
    Arrived = 4,
    Cancelled = 8
}

public enum VendorProcurementOrderCancellationReason
{
    NotApplicable = 1,
    Unknown = 2
}
