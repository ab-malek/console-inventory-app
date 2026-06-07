namespace GroceryInventoryApp.Discounts;

public sealed class PercentageBasedStrategy : IDiscountCalculationStrategy
{
    private readonly decimal _discountPercentage;

    public PercentageBasedStrategy(decimal discountPercentage)
    {
        _discountPercentage = discountPercentage;
    }

    public decimal CalculateDiscountedPrice(decimal originalPrice)
    {
        var rate = Math.Clamp(_discountPercentage, 0, 100) / 100m;
        return Math.Round(originalPrice * (1 - rate), 2);
    }
}
