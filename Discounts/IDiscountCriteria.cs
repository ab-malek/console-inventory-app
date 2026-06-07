using GroceryInventoryApp.Models;

namespace GroceryInventoryApp.Discounts;

public interface IDiscountCriteria
{
    bool IsApplicable(Item item);
}
