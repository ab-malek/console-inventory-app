using GroceryInventoryApp.Models;

namespace GroceryInventoryApp.Discounts;

public sealed class ItemBasedCriteria : IDiscountCriteria
{
    private readonly string _barcode;

    public ItemBasedCriteria(string barcode)
    {
        _barcode = barcode;
    }

    public bool IsApplicable(Item item)
    {
        return string.Equals(item.Barcode, _barcode, StringComparison.OrdinalIgnoreCase);
    }
}
