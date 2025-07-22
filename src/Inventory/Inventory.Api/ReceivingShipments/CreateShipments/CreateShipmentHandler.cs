using FluentValidation;
using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.ReceivingShipments.CreateShipments;

public sealed record CreateShipment(string ShippingNumber, string Facility, DateTime DeliveredAt);

public sealed class CreateShipmentValidator : AbstractValidator<CreateShipment>
{
    public CreateShipmentValidator()
    {
        RuleFor(x => x.ShippingNumber).NotEmpty();
        RuleFor(x => x.Facility).NotEmpty();
        RuleFor(x => x.DeliveredAt).NotEmpty();
    }
}

public static class CreateShipmentHandler
{
    [Tags(Tags.ReceivingShipments)]
    [WolverinePost("/api/receiving-shipments")]
    public static (IResult, IStartStream) Handle(CreateShipment command)
    {
        var created = new ReceivedShipmentCreated(command.ShippingNumber, command.Facility, command.DeliveredAt);
        var start = MartenOps.StartStream<ReceivedShipment>(created);

        var location = $"/api/receiving-shipments/{start.StreamId}";
        return (Results.Created(location, start.StreamId), start);
    }
}
