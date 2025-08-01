using FluentValidation;
using Inventory.ReceivingShipments.Services;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Attributes;
using Wolverine.Http;
using Wolverine.Http.Marten;
using Wolverine.Marten;

namespace Inventory.ReceivingShipments.AddLineItems;

public sealed record AddLineItem(string Sku, int ExpectedQuantity);

public sealed class AddLineItemValidator : AbstractValidator<AddLineItem>
{
    public AddLineItemValidator()
    {
        RuleFor(x => x.Sku).NotEmpty();
        RuleFor(x => x.Sku).MaximumLength(32);
        RuleFor(x => x.Sku).Matches(@"^[A-Z0-9\-]+$");
        RuleFor(x => x.ExpectedQuantity).NotEmpty();
    }
}

public static class AddLineItemHandler
{
    [WolverineBefore]
    public static ProblemDetails CheckSkuAndQuantity(
        AddLineItem command,
        ReceivedShipment shipment,
        [FromServices] ISkuService skuService)
    {
        var (sku, receivedQuantity) = command;

        if (receivedQuantity <= 0)
            return new ProblemDetails
            {
                Detail = "Quantity must be greater than zero",
                Status = StatusCodes.Status412PreconditionFailed
            };

        if (shipment.Status == ReceivingShipmentStatus.PutAway)
            return new ProblemDetails
            {
                Detail = "Can not add line items to a shipment that has been putaway",
                Status = StatusCodes.Status412PreconditionFailed
            };

        var isValidSku = skuService.DoesSkuExist(sku);
        if (isValidSku is false)
            return new ProblemDetails
            {
                Detail = $"SKU '{sku}' could not be located and thus cannot be added",
                Status = StatusCodes.Status412PreconditionFailed
            };

        return WolverineContinue.NoProblems;
    }

    [Tags(Tags.ReceivingShipments)]
    [WolverinePost("/api/receiving-shipments/{receivedShipmentId}/line-items/add")]
    public static Events Handle(
        AddLineItem command,
        [Aggregate("receivedShipmentId")] ReceivedShipment receivedShipment)
    {
        return new Events { new ReceivedShipmentLineItemAdded(command.Sku, command.ExpectedQuantity) };
    }
}
