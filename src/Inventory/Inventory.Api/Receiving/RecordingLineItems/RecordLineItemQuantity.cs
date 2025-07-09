using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.Receiving.RecordingLineItems;

public record RecordLineItemQuantity(Guid ReceivedShipmentId, string Sku, int ReceivedQuantity);

public sealed class RecordLineItemQuantityValidator : AbstractValidator<RecordLineItemQuantity>
{
    public RecordLineItemQuantityValidator()
    {
        RuleFor(x => x.ReceivedShipmentId).NotEmpty();
        RuleFor(x => x.Sku).NotEmpty();
        RuleFor(x => x.Sku).MaximumLength(32);
        RuleFor(x => x.Sku).Matches(@"^[A-Z0-9\-]+$");
        RuleFor(x => x.ReceivedQuantity).NotEmpty();
    }
}

public static class RecordLineItemQuantityHandler
{
    public static ProblemDetails Validate(RecordLineItemQuantity command, ReceivedShipment shipment)
    {
        var (_, sku, receivedQuantity) = command;

        if (shipment.Status != InboundShipmentStatus.Received)
            return new ProblemDetails
            {
                Detail = "Can only record quantities for received shipments",
                Status = StatusCodes.Status412PreconditionFailed
            };

        if (!shipment.LineItems.ContainsKey(sku))
            return new ProblemDetails
            {
                Detail = $"SKU {sku} is not part of this shipment",
                Status = StatusCodes.Status412PreconditionFailed
            };

        return WolverineContinue.NoProblems;
    }

    [Tags(Tags.ReceivingShipments)]
    [WolverinePost("/api/receiving-shipments/{receivedShipmentId}/line-items/record")]
    [AggregateHandler]
    public static Events Handle(RecordLineItemQuantity command, ReceivedShipment shipment)
    {
        var events = new Events();
        events += new ReceivedLineItemQuantityRecorded(command.Sku, command.ReceivedQuantity);

        return events;
    }
}
