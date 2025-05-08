namespace Catalog.Api.Items;

public sealed record AssignSkuToDraftItem(string Sku, Guid ItemId);

public sealed record AssignedSkuToDraftItem();
