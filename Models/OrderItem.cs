using GroceryInventoryApp.Discounts;

namespace GroceryInventoryApp.Models;

public sealed class OrderItem
{
    public OrderItem(Item item, int quantity)
    {
        Item = item;
        Quantity = quantity;
    }

    public Item Item { get; }
    public int Quantity { get; }

    public decimal CalculatePrice()
    {
        return Math.Round(Item.Price * Quantity, 2);
    }

    public decimal CalculatePriceWithDiscount(DiscountCampaign discount)
    {
        return discount.CalculateDiscount(this);
    }
}
