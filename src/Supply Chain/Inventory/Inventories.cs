using Marten.Schema;
using Wolverine;
using Wolverine.Marten;

namespace Inventory;

public record InventoryInitialized(Guid Id, string Sku); // evt

public record ValidateInventoryBeforeLaunch(Guid InventoryId); // cmd

public record InventoryValidatedForUse; // evt

public record InventoryIncremented(int Quantity); // evt

public record InventoryDecremented(int Quantity); // evt

public record PhysicalInventoryCountCorrection(int Quantity); // evt

public enum InventoryStatus
{
    Unset = 0,
    Initialized = 1,
    Validated = 2,
    Launched = 4,
    Inactive = 8
}

public class Inventory
{
    public Inventory() { }

    public Inventory(InventoryInitialized initialized)
    {
        Sku = new(initialized.Sku);
        Quantity = 0;
        Status = InventoryStatus.Initialized;
        Version = 1;
    }

    [Identity]
    public Guid Id { get; set; }
    public int Version { get; set; }
    public string Sku { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public InventoryStatus Status { get; set; }

    public bool IsUsable() => Status == InventoryStatus.Validated;

    public void Apply(InventoryValidatedForUse validated)
    {
        Status = InventoryStatus.Validated;
    }

    public void Apply(InventoryIncremented incremented)
    {
        Quantity += incremented.Quantity;
    }

    public void Apply(InventoryDecremented decremented)
    {
        Quantity -= decremented.Quantity;
    }

    public void Apply(PhysicalInventoryCountCorrection correction)
    {
        Quantity = correction.Quantity;
    }
}

public record InventoryReadyForInitialQuantity(Guid InventoryId);

public static class MarkInventoryAsUsableHandler
{
    [AggregateHandler]
    public static async Task<(Events, OutgoingMessages)> Handle(
        ValidateInventoryBeforeLaunch command,
        Inventory inventory,
        IVerifySkuService service)
    {
        var skuIsNotInUse = await service.VerifySkuIsNotInUse(inventory.Sku);

        var messages = new OutgoingMessages();
        var events = new Events();

        if (inventory.IsUsable())
        {
            events += new InventoryValidatedForUse();
            messages.Add(new InventoryReadyForInitialQuantity(inventory.Id));
        }

        // This results in both* new events being captured
        // and the SomeInventoryMessage message going out
        return (events, messages);
    }
}
