using GroceryInventoryApp.Models;

namespace GroceryInventoryApp.Discounts;

public sealed class DiscountCampaign
{
    private readonly IDiscountCriteria _criteria;
    private readonly IDiscountCalculationStrategy _calculationStrategy;

    public DiscountCampaign(
        string discountId,
        string name,
        IDiscountCriteria criteria,
        IDiscountCalculationStrategy calculationStrategy)
    {
        DiscountId = discountId;
        Name = name;
        _criteria = criteria;
        _calculationStrategy = calculationStrategy;
    }

    public string DiscountId { get; }
    public string Name { get; }

    public bool IsApplicable(Item item)
    {
        return _criteria.IsApplicable(item);
    }

    public decimal CalculateDiscount(OrderItem item)
    {
        return _calculationStrategy.CalculateDiscountedPrice(item.CalculatePrice());
    }
}
