namespace Inventory.Api.Receiving;

public sealed record LineItem
{
    public string Sku { get; init; }
    public int ExpectedQuantity { get; init; }
    public int ReceivedQuantity { get; init; }

    public LineItem(string Sku, int ExpectedQuantity, int ReceivedQuantity)
    {
        this.Sku = Sku;
        this.ExpectedQuantity = ExpectedQuantity;
        this.ReceivedQuantity = ReceivedQuantity;
    }

    public void Deconstruct(out string sku, out int expectedQuantity, out int receivedQuantity)
    {
        sku = Sku;
        expectedQuantity = ExpectedQuantity;
        receivedQuantity = ReceivedQuantity;
    }
}
