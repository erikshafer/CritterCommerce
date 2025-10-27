namespace Catalog.DraftProducts.Services;

public interface IBrandService
{
    bool DoesBrandExist(Guid brandId);
}

public class BrandService : IBrandService
{
    public bool DoesBrandExist(Guid brandId)
    {
        return true;
    }
}
