using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.ReceivingShipments.RecordLineItemReceipts;

public sealed record RecordLineItemReceipt(Guid ShipmentId, string Sku, int ReceivedQuantity);

public static class RecordLineItemReceiptHandler
{
    [WolverinePost("/api/receiving-shipments/{shipmentId}/line-items/record")]
    [AggregateHandler]
    public static object Handle(RecordLineItemReceipt command)
    {
        // To be implemented
        return new { Success = true };
    }
}
