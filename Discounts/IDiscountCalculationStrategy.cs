namespace GroceryInventoryApp.Discounts;

public interface IDiscountCalculationStrategy
{
    decimal CalculateDiscountedPrice(decimal originalPrice);
}
