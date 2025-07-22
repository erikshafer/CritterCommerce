using FluentValidation;
using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.ReceivingShipments.PutAwayShipments;

public sealed record PutAwayShipment(Guid LocationId, string PutawayLotId);

public sealed class PutAwayShipmentValidator : AbstractValidator<PutAwayShipment>
{
    public PutAwayShipmentValidator()
    {
        RuleFor(x => x.LocationId).NotEmpty();
        RuleFor(x => x.PutawayLotId).NotEmpty();
        RuleFor(x => x.PutawayLotId).MaximumLength(32);
    }
}

public static class PutAwayShipmentHandler
{
    [Tags(Tags.ReceivingShipments)]
    [WolverinePost("/api/receiving-shipments/{receivedShipmentId}/put-away")]
    public static Events Handle(PutAwayShipment command)
    {
        var events = new Events();
        var now = DateTime.UtcNow;

        events += new ReceivedShipmentPutAway(command.PutawayLotId, now);

        return events;
    }
}
