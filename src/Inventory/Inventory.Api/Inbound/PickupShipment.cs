using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.Inbound;

public sealed record PickupShipment(
    Guid FreightShipmentId,
    DateTime PickedupAt
);

public sealed class PickupShipmentValidator : AbstractValidator<PickupShipment>
{
    public PickupShipmentValidator()
    {
        RuleFor(x => x.FreightShipmentId).NotEmpty();
        RuleFor(x => x.PickedupAt).NotEmpty();
    }
}

public static class PickupShipmentHandler
{
    public static ProblemDetails Validate(FreightShipment shipment)
    {
        if (shipment.Status == FreightShipmentStatus.Scheduled)
            return new ProblemDetails
            {
                Detail = "Shipment has been picked up and is in transit",
                Status = StatusCodes.Status412PreconditionFailed
            };

        return WolverineContinue.NoProblems;
    }

    [Tags(Tags.InboundShipments)]
    [WolverinePost("/api/freight-shipments/{freightShipmentId}/pickup")]
    [AggregateHandler]
    public static (IResult, Events, OutgoingMessages) Handle(
        PickupShipment command,
        FreightShipment shipment)
    {
        Events events = [];
        OutgoingMessages messages = [];

        events.Add(new FreightShipmentPickedUp(command.PickedupAt));
        messages.Add(new InboundShipmentNotification(shipment.Id, "PickedUp"));

        return (Results.Ok(), events, messages);
    }
}
