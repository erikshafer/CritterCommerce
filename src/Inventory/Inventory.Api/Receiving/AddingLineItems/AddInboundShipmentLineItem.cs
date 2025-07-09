using FluentValidation;
using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.Receiving.AddingLineItems;

public record AddInboundShipmentLineItem(Guid ReceivedShipmentId, string Sku, int ExpectedQuantity);

public sealed class AddInboundShipmentLineItemValidator : AbstractValidator<AddInboundShipmentLineItem>
{
    public AddInboundShipmentLineItemValidator()
    {
        RuleFor(x => x.ReceivedShipmentId).NotEmpty();
        RuleFor(x => x.Sku).NotEmpty();
        RuleFor(x => x.Sku).MaximumLength(32);
        RuleFor(x => x.Sku).Matches(@"^[A-Z0-9\-]+$");
        RuleFor(x => x.ExpectedQuantity).NotEmpty();
    }
}

public static class AddInboundShipmentLineItemHandler
{

    [Tags(Tags.ReceivingShipments)]
    [WolverinePost("/api/receiving-shipments/{receivedShipmentId}/line-items/add")]
    [AggregateHandler]
    public static Events Handle(AddInboundShipmentLineItem command, ReceivedShipment shipment)
    {
        var events = new Events();


        events += new ReceivedShipmentLineItemAdded(command.Sku, command.ExpectedQuantity);
        return events;
    }
}
