using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Wolverine;
using Wolverine.Attributes;
using Wolverine.Http;
using Wolverine.Http.Marten;
using Wolverine.Marten;

namespace Inventory.Api.WarehouseLevels.MovingLots;

public sealed record MoveInventoryLot(string FacilityLotId);

public sealed class MoveInventoryLotValidator : AbstractValidator<MoveInventoryLot>
{
    public MoveInventoryLotValidator()
    {
        RuleFor(x => x.FacilityLotId).NotEmpty();
        RuleFor(x => x.FacilityLotId).Matches(@"^[A-Z0-9\-]+$");
    }
}

public static class MoveInventoryLotHandler
{
    [WolverineBefore]
    public static ProblemDetails CheckLotSpace(
        MoveInventoryLot command,
        IFacilityLotService service)
    {
        var isLotAvailable = service.IsLotAvailable(command.FacilityLotId);
        if (isLotAvailable is false)
            return new ProblemDetails
            {
                Detail = $"Lot '{command.FacilityLotId}' is not available",
                Status = StatusCodes.Status412PreconditionFailed
            };

        return WolverineContinue.NoProblems;
    }

    [Tags(Tags.WarehouseInventoryLevels)]
    [WolverinePost("/api/inventory-level/{inventoryLevelId}/move")]
    public static (IResult, Events, OutgoingMessages ) Handle(
        MoveInventoryLot command,
        [Aggregate("inventoryLevelId")] InventoryLevel inventoryLevel)
    {
        Events events = [];
        OutgoingMessages messages = [];

        events.Add(new InventoryLotMoved(command.FacilityLotId));
        messages.Add(new InventoryLevelMovedLots(inventoryLevel.Id,
            OldFacilityLotId: inventoryLevel.FacilityLotId,
            NewFacilityLotId: command.FacilityLotId));

        return (Results.Ok(), events, messages);
    }
}
