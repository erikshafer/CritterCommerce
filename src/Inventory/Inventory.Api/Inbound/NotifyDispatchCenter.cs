namespace Inventory.Api.Inbound;

/* event notification that may double as a message -- TBD */

public record NotifyDispatchCenter(Guid ShipmentId, string Pickedup);
