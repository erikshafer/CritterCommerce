using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.Inbound;

public sealed record DeliverShipment(
    DateTime DeliveredAt
);

public sealed class DeliverShipmentValidator : AbstractValidator<DeliverShipment>
{
    public DeliverShipmentValidator()
    {
        RuleFor(x => x.DeliveredAt).NotEmpty();
    }
}

public static class DeliverShipmentHandler
{
    public static ProblemDetails Validate(FreightShipment shipment)
    {
        if (shipment.Status == FreightShipmentStatus.Delivered)
            return new ProblemDetails
            {
                Detail = $"Shipment is already in '{shipment.Status}' status",
                Status = StatusCodes.Status412PreconditionFailed
            };

        return WolverineContinue.NoProblems;
    }

    [Tags(Tags.InboundShipments)]
    [WolverinePost("/api/freight-shipments/{id}/deliver")]
    [AggregateHandler]
    public static (IResult, Events, OutgoingMessages) Handle(
        DeliverShipment command,
        FreightShipment shipment)
    {
        Events events = [];
        OutgoingMessages messages = [];

        events.Add(new FreightShipmentPickedUp(command.DeliveredAt));
        messages.Add(new InboundShipmentNotification(shipment.Id, "Delivered"));

        return (Results.Ok(), events, messages);
    }
}
