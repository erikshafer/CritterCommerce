namespace Inventory.Api.Inbound;

public record InboundShipmentNotification(Guid FreightShipmentId, string Status);
