namespace GroceryInventoryApp.Discounts;

public sealed class AmountBasedStrategy : IDiscountCalculationStrategy
{
    private readonly decimal _discountAmount;

    public AmountBasedStrategy(decimal discountAmount)
    {
        _discountAmount = Math.Round(discountAmount, 2);
    }

    public decimal CalculateDiscountedPrice(decimal originalPrice)
    {
        return Math.Max(0, Math.Round(originalPrice - _discountAmount, 2));
    }
}
