using FluentValidation;
using Wolverine.Http;
using Wolverine.Http.Marten;
using Wolverine.Marten;

namespace Inventory.Api.ReceivingShipments.PutAwayShipments;

public sealed record PutAwayShipment(string PutawayLotId, DateTime PutawayAt);

public sealed class PutAwayShipmentValidator : AbstractValidator<PutAwayShipment>
{
    public PutAwayShipmentValidator()
    {
        RuleFor(x => x.PutawayLotId).NotEmpty();
        RuleFor(x => x.PutawayLotId).MaximumLength(32);
        RuleFor(x => x.PutawayAt).NotEmpty();
    }
}

public static class PutAwayShipmentHandler
{
    [Tags(Tags.ReceivingShipments)]
    [WolverinePost("/api/receiving-shipments/{receivedShipmentId}/putaway")]
    public static Events Handle(
        PutAwayShipment command,
        [Aggregate("receivedShipmentId")] ReceivedShipment receivedShipment)
    {
        var events = new Events();

        events += new ReceivedShipmentPutAway(command.PutawayLotId, command.PutawayAt);

        return events;
    }
}
