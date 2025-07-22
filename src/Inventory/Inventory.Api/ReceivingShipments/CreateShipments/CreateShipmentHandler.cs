using FluentValidation;
using JasperFx.Core;
using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.ReceivingShipments.CreateShipments;

public sealed record CreateShipment(string ShippingNumber, string Facility, DateTime DeliveredAt, string CreatedBy);

public sealed class CreateShipmentValidator : AbstractValidator<CreateShipment>
{
    public CreateShipmentValidator()
    {
        RuleFor(x => x.ShippingNumber).NotEmpty();
        RuleFor(x => x.Facility).NotEmpty();
        RuleFor(x => x.DeliveredAt).NotEmpty();
        RuleFor(x => x.CreatedBy).NotEmpty();
    }
}

public static class CreateShipmentHandler
{
    [Tags(Tags.ReceivingShipments)]
    [WolverinePost("/api/receiving-shipments")]
    public static (IResult, IStartStream) Handle(CreateShipment command)
    {
        var id = CombGuidIdGeneration.NewGuid();
        var created = new ReceivedShipmentCreated(
            id,
            command.ShippingNumber,
            command.Facility,
            command.DeliveredAt,
            command.CreatedBy);
        var start = MartenOps.StartStream<ReceivedShipment>(created);

        var location = $"/api/receiving-shipments/{start.StreamId}";
        return (Results.Created(location, start.StreamId), start);
    }
}
