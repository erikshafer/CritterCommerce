using JasperFx.Events;
using Marten.Events.Aggregation;

namespace Inventory.ReceivingShipments.Views;

public record OutstandingReceivedShipmentsView
{
    public Guid Id { get; set; }
    public string ShippingNumber { get; set; } = default!;
    public string Facility { get; set; } = default!;
    public DateTime DeliveredAt { get; set; }
    public string Status { get; set; } = default!;
    // Key: SKU
    public Dictionary<string, SkuOutstandingLine> OutstandingSkus { get; init; } = new();

    public int OutstandingItems => OutstandingSkus.Values.Sum(x => x.OutstandingQuantity);
    public int TotalItems => OutstandingSkus.Values.Sum(x => x.ExpectedQuantity);
    public int UniqueSkuCount => OutstandingSkus.Count;
    public int AgeInDays => (int)(DateTime.UtcNow.Date - DeliveredAt.Date).TotalDays;
}

public record SkuOutstandingLine
{
    public string Sku { get; set; } = default!;
    public int ExpectedQuantity { get; set; }
    public int ReceivedQuantity { get; set; }
    public int OutstandingQuantity => Math.Max(ExpectedQuantity - ReceivedQuantity, 0);
}

public class OutstandingReceivedShipmentsProjection : SingleStreamProjection<OutstandingReceivedShipmentsView, Guid>
{
    public static OutstandingReceivedShipmentsView Create(IEvent<ReceivedShipmentCreated> @event) =>
        new()
        {
            Id = @event.StreamId,
            ShippingNumber = @event.Data.ShippingNumber,
            Facility = @event.Data.Facility,
            DeliveredAt = @event.Data.DeliveredAt,
            Status = ReceivingShipmentStatus.Created.ToString(),
            OutstandingSkus = new Dictionary<string, SkuOutstandingLine>()
        };

    public static OutstandingReceivedShipmentsView Apply(ReceivedShipmentLineItemAdded @event, OutstandingReceivedShipmentsView view)
    {
        var skus = new Dictionary<string, SkuOutstandingLine>(view.OutstandingSkus);
        if (!skus.ContainsKey(@event.Sku))
        {
            skus[@event.Sku] = new SkuOutstandingLine
            {
                Sku = @event.Sku,
                ExpectedQuantity = @event.ExpectedQuantity,
                ReceivedQuantity = 0
            };
        }
        else
        {
            // If SKU already exists, sum extra quantities
            var curr = skus[@event.Sku];
            skus[@event.Sku] = curr with { ExpectedQuantity = curr.ExpectedQuantity + @event.ExpectedQuantity };
        }
        return view with
        {
            OutstandingSkus = skus,
            Status = ReceivingShipmentStatus.Receiving.ToString()
        };
    }

    public static OutstandingReceivedShipmentsView Apply(ReceivedShipmentLineItemQuantityRecorded @event, OutstandingReceivedShipmentsView view)
    {
        var skus = new Dictionary<string, SkuOutstandingLine>(view.OutstandingSkus);
        if (skus.TryGetValue(@event.Sku, out var skuLine))
        {
            skus[@event.Sku] = skuLine with
            {
                ReceivedQuantity = skuLine.ReceivedQuantity + @event.ReceivedQuantity
            };
        }
        // Determine if fully received
        var stillOutstanding = skus.Values.Any(x => x.OutstandingQuantity > 0);
        var newStatus = stillOutstanding ? view.Status : ReceivingShipmentStatus.Received.ToString();

        return view with
        {
            OutstandingSkus = skus,
            Status = newStatus
        };
    }

    public static OutstandingReceivedShipmentsView Apply(ReceivedShipmentMarkedAsReceived @event, OutstandingReceivedShipmentsView view)
    {
        // Mark all SKUs as fully received
        var allReceived = new Dictionary<string, SkuOutstandingLine>();
        foreach (var kv in view.OutstandingSkus)
        {
            var l = kv.Value;
            allReceived[kv.Key] = l with { ReceivedQuantity = l.ExpectedQuantity };
        }

        return view with
        {
            OutstandingSkus = allReceived,
            Status = ReceivingShipmentStatus.Received.ToString()
        };
    }

    public static OutstandingReceivedShipmentsView Apply(ReceivedShipmentPutAway @event, OutstandingReceivedShipmentsView view) =>
        view with
        {
            Status = ReceivingShipmentStatus.PutAway.ToString()
        };
}
