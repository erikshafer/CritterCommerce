using Marten;
using Marten.Events.Aggregation;
// ReSharper disable UnusedType.Global

namespace Inventory;

[Flags]
public enum FulfillmentCenterLocation
{
    OmahaNeUsa = 1,
    AustinTxUsa = 2,
    BellevueWaUsa = 4,
    PhiladelphiaPaUsa = 8,
    TorontoOnCa = 16,
    DoncasterUk = 32
}

public record ProductShipped(Guid Id, int Quantity, DateTime DateTime);

public record ProductReceived(Guid Id, int Quantity, DateTime DateTime);

public record InventoryAdjusted(Guid Id, int Quantity, string Reason, DateTime DateTime);

public class WarehouseRepository
{
    private readonly IDocumentStore _documentStore;

    public WarehouseRepository(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    public WarehouseProductReadModel Get(Guid id)
    {
        using var session = _documentStore.QuerySession();

        var doc = session
            .Query<WarehouseProductReadModel>()
            .SingleOrDefault(x => x.Id == id);

        return doc;
    }
}

public class WarehouseProductReadModel
{
    public Guid Id { get; set; }
    public int QuantityOnHand { get; set; }
}

public class WarehouseProductProjection : SingleStreamProjection<WarehouseProductReadModel, Guid>
{
    public WarehouseProductProjection()
    {
        ProjectEvent<ProductShipped>(Apply);
        ProjectEvent<ProductReceived>(Apply);
        ProjectEvent<InventoryAdjusted>(Apply);
    }


    public void Apply(WarehouseProductReadModel readModel, ProductShipped evnt)
    {
        readModel.QuantityOnHand -= evnt.Quantity;
    }

    public void Apply(WarehouseProductReadModel readModel, ProductReceived evnt)
    {
        readModel.QuantityOnHand += evnt.Quantity;
    }

    public void Apply(WarehouseProductReadModel readModel, InventoryAdjusted evnt)
    {
        readModel.QuantityOnHand += evnt.Quantity;
    }
}

public class WarehouseProductWriteModel
{
    public Guid Id { get; set; }
    public int QuantityOnHand { get; set; }

    public void Apply(ProductShipped evnt)
    {
        Id = evnt.Id;
        QuantityOnHand -= evnt.Quantity;
    }

    public void Apply(ProductReceived evnt)
    {
        Id = evnt.Id;
        QuantityOnHand += evnt.Quantity;
    }

    public void Apply(InventoryAdjusted evnt)
    {
        Id = evnt.Id;
        QuantityOnHand += evnt.Quantity;
    }
}

public class WarehouseProductHandler
{
    private readonly Guid id;
    private readonly IDocumentStore documentStore;

    public WarehouseProductHandler(Guid id, IDocumentStore documentStore)
    {
        this.id = id;
        this.documentStore = documentStore;
    }

    public async Task ShipProduct(int quantity)
    {
        await using var session = documentStore.LightweightSession();

        var warehouseProduct = await session.Events.AggregateStreamAsync<WarehouseProductWriteModel>(id);

        if (quantity > warehouseProduct?.QuantityOnHand)
        {
            throw new InvalidDomainException("Ah... we don't have enough product to ship?");
        }

        session.Events.Append(id, new ProductShipped(id, quantity, DateTime.UtcNow));
        await session.SaveChangesAsync();
    }

    public async Task ReceiveProduct(int quantity)
    {
        using var session = documentStore.LightweightSession();

        session.Events.Append(id, new ProductReceived(id, quantity, DateTime.UtcNow));
        await session.SaveChangesAsync();
    }

    public async Task AdjustInventory(int quantity, string reason)
    {
        using var session = documentStore.LightweightSession();

        var warehouseProduct = await session.Events.AggregateStreamAsync<WarehouseProductWriteModel>(id);

        if (warehouseProduct?.QuantityOnHand + quantity < 0)
        {
            throw new InvalidDomainException("Cannot adjust to a negative quantity on hand.");
        }

        session.Events.Append(id, new InventoryAdjusted(id, quantity, reason, DateTime.UtcNow));
        await session.SaveChangesAsync();
    }
}

public class InvalidDomainException: Exception
{
    public InvalidDomainException(string message): base(message)
    {
    }
}
