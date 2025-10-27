namespace Catalog.DraftProducts.Services;

public interface ICategoryService
{
    bool DoesCategoryExist(Guid categoryId);
}

public class CategoryService : ICategoryService
{
    public bool DoesCategoryExist(Guid categoryId)
    {
        return true;
    }
}
