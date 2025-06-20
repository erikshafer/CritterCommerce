namespace Inventory;

public sealed record SkuNameQuantity
{
    public SkuNameQuantity(string Sku, string Name, int Quantity)
    {
        this.Sku = Sku;
        this.Name = Name;
        this.Quantity = Quantity;
    }

    public SkuNameQuantity(Sku Sku, Name Name, Quantity Quantity)
    {
        this.Sku = Sku;
        this.Name = Name;
        this.Quantity = Quantity;
    }

    public string Sku { get; init; }
    public string Name { get; init; }
    public int Quantity { get; init; }

    public void Deconstruct(out string Sku, out string Name, out int Quantity)
    {
        Sku = this.Sku;
        Name = this.Name;
        Quantity = this.Quantity;
    }
}
