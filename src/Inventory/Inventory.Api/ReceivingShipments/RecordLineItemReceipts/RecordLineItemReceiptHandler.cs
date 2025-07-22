using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Attributes;
using Wolverine.Http;
using Wolverine.Http.Marten;
using Wolverine.Marten;

namespace Inventory.Api.ReceivingShipments.RecordLineItemReceipts;

public sealed record RecordLineItemReceipt(string Sku, int ReceivedQuantity);

public sealed class RecordLineItemReceiptValidator : AbstractValidator<RecordLineItemReceipt>
{
    public RecordLineItemReceiptValidator()
    {
        RuleFor(x => x.Sku).NotEmpty();
        RuleFor(x => x.Sku).MaximumLength(32);
        RuleFor(x => x.Sku).Matches(@"^[A-Z0-9\-]+$");
        RuleFor(x => x.ReceivedQuantity).NotEmpty();
    }
}

public static class RecordLineItemReceiptHandler
{
    [WolverineBefore]
    public static ProblemDetails Validate(
        RecordLineItemReceipt command,
        ReceivedShipment shipment)
    {
        if (shipment.Status != ReceivingShipmentStatus.Receiving)
            return new ProblemDetails
            {
                Detail = "Can only record line item quantities for shipments in Receiving status",
                Status = StatusCodes.Status412PreconditionFailed
            };

        return WolverineContinue.NoProblems;
    }

    [Tags(Tags.ReceivingShipments)]
    [WolverinePost("/api/receiving-shipments/{receivedShipmentId}/line-items/record")]
    public static Events Handle(
        RecordLineItemReceipt command,
        [Aggregate("receivedShipmentId")] ReceivedShipment receivedShipment)
    {
        return new Events { new ReceivedShipmentLineItemQuantityRecorded(command.Sku, command.ReceivedQuantity) };
    }
}
