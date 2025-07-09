using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.Inbound.Cancelling;

public record CancelShipment(
    Guid FreightShipmentId,
    string Reason,
    DateTime CancelledAt
);

public sealed class CancelShipmentValidator : AbstractValidator<CancelShipment>
{
    public CancelShipmentValidator()
    {
        RuleFor(x => x.Reason).NotEmpty();
        RuleFor(x => x.CancelledAt).NotEmpty();
    }
}

public static class CancelShipmentHandler
{
    public static ProblemDetails Validate(FreightShipment shipment)
    {
        if (shipment.Status == FreightShipmentStatus.Cancelled)
            return new ProblemDetails
            {
                Detail = $"Shipment is already in '{shipment.Status}' status",
                Status = StatusCodes.Status412PreconditionFailed
            };

        return WolverineContinue.NoProblems;
    }

    [Tags(Tags.InboundShipments)]
    [WolverinePost("/api/freight-shipments/{id}/cancel")]
    [AggregateHandler]
    public static (Events, OutgoingMessages) Handle(
        CancelShipment command,
        FreightShipment shipment)
    {
        var events = new Events { new FreightShipmentCancelled(command.Reason, command.CancelledAt) };
        var messages = new OutgoingMessages { new InboundShipmentNotification(shipment.Id, "Cancelled") };

        return (events, messages);
    }
}
