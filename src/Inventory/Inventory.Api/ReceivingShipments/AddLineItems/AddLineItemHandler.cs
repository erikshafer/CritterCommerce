using FluentValidation;
using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.ReceivingShipments.AddLineItems;

public sealed record AddLineItem(Guid ReceivedShipmentId, string Sku, int ExpectedQuantity);

public sealed class AddLineItemValidator : AbstractValidator<AddLineItem>
{
    public AddLineItemValidator()
    {
        RuleFor(x => x.ReceivedShipmentId).NotEmpty();
        RuleFor(x => x.Sku).NotEmpty();
        RuleFor(x => x.Sku).MaximumLength(32);
        RuleFor(x => x.Sku).Matches(@"^[A-Z0-9\-]+$");
        RuleFor(x => x.ExpectedQuantity).NotEmpty();
    }
}

public static class AddLineItemHandler
{
    [WolverinePost("/api/receiving-shipments/{shipmentId}/line-items/add")]
    [AggregateHandler]
    public static object Handle(AddLineItem command)
    {
        // Business logic for adding a line item.
        // To be implemented.
        return new { Success = true };
    }
}
