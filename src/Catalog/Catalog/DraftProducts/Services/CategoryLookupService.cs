namespace Catalog.DraftProducts.Services;

public interface ICategoryLookupService
{
    bool DoesCategoryExist(Guid categoryId);
}

public class CategoryLookupService : ICategoryLookupService
{
    public bool DoesCategoryExist(Guid categoryId)
    {
        return true;
    }
}
