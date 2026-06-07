using GroceryInventoryApp.Models;

namespace GroceryInventoryApp.Discounts;

public sealed class CategoryBasedCriteria : IDiscountCriteria
{
    private readonly string _category;

    public CategoryBasedCriteria(string category)
    {
        _category = category;
    }

    public bool IsApplicable(Item item)
    {
        return string.Equals(item.Category, _category, StringComparison.OrdinalIgnoreCase);
    }
}
