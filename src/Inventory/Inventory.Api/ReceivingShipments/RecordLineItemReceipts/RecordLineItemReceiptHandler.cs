using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.ReceivingShipments.RecordLineItemReceipts;

public sealed record RecordLineItemReceipt(Guid ReceivedShipmentId, string Sku, int ReceivedQuantity);

public sealed class RecordLineItemReceiptValidator : AbstractValidator<RecordLineItemReceipt>
{
    public RecordLineItemReceiptValidator()
    {
        RuleFor(x => x.ReceivedShipmentId).NotEmpty();
        RuleFor(x => x.Sku).NotEmpty();
        RuleFor(x => x.Sku).MaximumLength(32);
        RuleFor(x => x.Sku).Matches(@"^[A-Z0-9\-]+$");
        RuleFor(x => x.ReceivedQuantity).NotEmpty();
    }
}

public static class RecordLineItemReceiptHandler
{
    public static ProblemDetails Validate(RecordLineItemReceipt command, ReceivedShipment shipment)
    {
        var (_, sku, receivedQuantity) = command;

        if (shipment.Status != ReceivingShipmentStatus.Received)
            return new ProblemDetails
            {
                Detail = "Can only record quantities for received shipments",
                Status = StatusCodes.Status412PreconditionFailed
            };

        if (shipment.LineItems.All(li => li.Sku != sku))
            return new ProblemDetails
            {
                Detail = $"SKU {sku} is not part of this shipment",
                Status = StatusCodes.Status412PreconditionFailed
            };

        return WolverineContinue.NoProblems;
    }

    [Tags(Tags.ReceivingShipments)]
    [WolverinePost("/api/receiving-shipments/{shipmentId}/line-items/record")]
    [AggregateHandler]
    public static object Handle(RecordLineItemReceipt command)
    {
        // To be implemented
        return new { Success = true };
    }
}
