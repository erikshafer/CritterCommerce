using FluentValidation;
using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.ReceivingShipments.PutAwayShipments;

public sealed record PutAwayShipment(Guid ReceivedShipmentId, Guid LocationId, string PutawayBy);

public sealed class PutAwayShipmentValidator : AbstractValidator<PutAwayShipment>
{
    public PutAwayShipmentValidator()
    {
        RuleFor(x => x.ReceivedShipmentId).NotEmpty();
        RuleFor(x => x.LocationId).NotEmpty();
        RuleFor(x => x.PutawayBy).NotEmpty();
        RuleFor(x => x.PutawayBy).MaximumLength(32);
    }
}

public static class PutAwayShipmentHandler
{
    [Tags(Tags.ReceivingShipments)]
    [WolverinePost("/api/receiving-shipments/{shipmentId}/put-away")]
    [AggregateHandler]
    public static Events Handle(PutAwayShipment command)
    {
        var events = new Events();
        var now = DateTime.UtcNow;

        events += new ReceivedShipmentPutAway(command.LocationId, now);

        return events;
    }
}
