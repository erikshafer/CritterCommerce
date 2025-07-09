using FluentValidation;
using JasperFx.Core;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Attributes;
using Wolverine.Http;
using Wolverine.Marten;

namespace Inventory.Api.Receiving.ReceivingInbound;

public sealed record ReceiveInboundShipment(string ShipmentNumber, Guid InboundShipmentId, string ReceivedBy);

public sealed class ReceiveInboundShipmentValidator : AbstractValidator<ReceiveInboundShipment>
{
    public ReceiveInboundShipmentValidator()
    {
        RuleFor(x => x.ShipmentNumber).NotEmpty();
        RuleFor(x => x.InboundShipmentId).NotEmpty();
        RuleFor(x => x.ReceivedBy).NotEmpty();
    }
}

public static class ReceiveInboundShipmentHandler
{
    [WolverineBefore]
    public static ProblemDetails CheckOnSkuUsage(ReceiveInboundShipment command)
    {
        // Perhaps some business logic like querying a service to
        // check against the Inbound Shipment perhaps!

        return WolverineContinue.NoProblems;
    }

    [Tags(Tags.ReceivingShipments)]
    [WolverinePost("/api/receiving-shipments")]
    public static (IResult, IStartStream) Handle(ReceiveInboundShipment command, ReceivedShipment shipment)
    {
        var (shipmentNumber, inboundShipmentId, receivedBy) = command;

        var id = CombGuidIdGeneration.NewGuid();
        var receivedAt = DateTime.UtcNow;
        var scheduled = new InboundShipmentReceived(id, shipmentNumber, inboundShipmentId, receivedBy, receivedAt);

        var start = MartenOps.StartStream<ReceivedShipment>(scheduled);

        var location = $"/api/receiving-shipments/{start.StreamId}";
        return (Results.Created(location, start.StreamId), start);
    }
}
