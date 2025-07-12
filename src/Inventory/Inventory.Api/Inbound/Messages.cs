namespace Inventory.Api.Inbound;

public record InboundShipmentMessage(Guid FreightShipmentId, string Status);
