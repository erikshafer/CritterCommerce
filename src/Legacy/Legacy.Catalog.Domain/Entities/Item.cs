namespace Legacy.Catalog.Domain.Entities;

public class Item
{
    public int Id { get; set; }

    public string Sku() => Id.ToString();

    public string Name { get; set; } = null!;
    public int? BrandId { get; set; }
    public int? CategoryId { get; set; }
    public bool IsVariant { get; set; } = false;
    public int? IsVariantOf { get; set; }
    public bool Discontinued { get; set; }
    public string Description { get; set; } = null!;
    public string? WeightUnit { get; set; } = "lbs";
    public decimal? Weight { get; set; }
    public string? MeasureUnit { get; set; } = "in";
    public decimal? Height { get; set; }
    public decimal? Length { get; set; }
    public decimal? Width { get; set; }
    public string? Color { get; set; }
    public string? Color2 { get; set; }
    public string? BulletPoint1 { get; set; } = null!;
    public string? BulletPoint2 { get; set; } = null!;
    public string? BulletPoint3 { get; set; } = null!;
    public string? WarningCode1 { get; set; }
    public string? WarningCode2 { get; set; }
    public string? WarningCode3 { get; set; }
    public bool ChildCouldChokeWarning { get; set; }
    public string? Picture1Url { get; set; } = null!;
    public string? Picture2Url { get; set; } = null!;
    public string? Picture3Url { get; set; } = null!;
}
