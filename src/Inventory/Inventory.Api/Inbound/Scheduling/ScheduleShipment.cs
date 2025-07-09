using FluentValidation;
using JasperFx.Core;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Attributes;
using Wolverine.Http;
using Wolverine.Marten;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace Inventory.Api.Inbound.Scheduling;

public sealed record ScheduleShipment(string Origin, string Destination);

public sealed class ScheduleShipmentValidator : AbstractValidator<ScheduleShipment>
{
    public ScheduleShipmentValidator()
    {
        RuleFor(x => x.Origin).NotEmpty();
        RuleFor(x => x.Destination).NotEmpty();
    }
}

public static class ScheduleShipmentHandler
{
    [WolverineBefore]
    public static async Task<ProblemDetails> CheckOriginAndDestinationLocations(
        ScheduleShipment command,
        IDocumentSession session)
    {
        var (origin, destination) = command;
        var batch = session.CreateBatchQuery();
        var originTask = batch.Query(new FindLocationByName { Name = origin });
        var destinationTask = batch.Query(new FindLocationByName { Name = destination });
        await batch.Execute();

        var originLocation = await originTask;
        if (originLocation is null)
            return new ProblemDetails
            {
                Detail = $"Cannot locate Origin of '{origin}' in our records",
                Status = StatusCodes.Status412PreconditionFailed
            };

        var destinationLocation = await destinationTask;
        if (destinationLocation is null)
            return new ProblemDetails
            {
                Detail = $"Cannot locate Origin of '{destination}' in our records",
                Status = StatusCodes.Status412PreconditionFailed
            };

        return WolverineContinue.NoProblems;
    }

    [Tags(Tags.InboundShipments)]
    [WolverinePost("/api/freight-shipments")]
    public static (IResult, IStartStream) Post(ScheduleShipment command)
    {
        var (origin, destination) = command;

        var id = CombGuidIdGeneration.NewGuid();
        var scheduledAt = DateTime.UtcNow;
        var scheduled = new FreightShipmentScheduled(id, origin, destination, scheduledAt);

        var start = MartenOps.StartStream<FreightShipment>(id, scheduled);

        var location = $"/api/freight-shipments/{start.StreamId}";
        return (Results.Created(location, start.StreamId), start);
    }
}
