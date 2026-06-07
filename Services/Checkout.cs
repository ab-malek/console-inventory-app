using GroceryInventoryApp.Discounts;
using GroceryInventoryApp.Models;

namespace GroceryInventoryApp.Services;

public sealed class Checkout
{
    private readonly IReadOnlyList<DiscountCampaign> _activeDiscounts;
    private readonly Inventory _inventory;

    public Checkout(IReadOnlyList<DiscountCampaign> activeDiscounts, Inventory inventory)
    {
        _activeDiscounts = activeDiscounts;
        _inventory = inventory;
        CurrentOrder = new Order();
    }

    public Order CurrentOrder { get; private set; }

    public void StartNewOrder()
    {
        CurrentOrder = new Order();
    }

    public void AddItemToOrder(Item item, int quantity)
    {
        var availableAfterCart = _inventory.GetStock(item.Barcode) - CurrentOrder.GetQuantityForBarcode(item.Barcode);

        if (quantity > availableAfterCart)
        {
            throw new InvalidOperationException($"Insufficient inventory. Available for this order: {availableAfterCart}.");
        }

        var orderItem = new OrderItem(item, quantity);
        CurrentOrder.AddItem(orderItem);
        ApplyBestDiscount(orderItem);
    }

    public Receipt ProcessPayment(decimal paymentAmount)
    {
        CurrentOrder.SetPayment(paymentAmount);

        if (CurrentOrder.CalculateChange() < 0)
        {
            throw new InvalidOperationException("Payment is less than the order total.");
        }

        foreach (var orderItem in CurrentOrder.Items)
        {
            _inventory.ReduceStock(orderItem.Item.Barcode, orderItem.Quantity);
        }

        return new Receipt(CurrentOrder);
    }

    private void ApplyBestDiscount(OrderItem orderItem)
    {
        var basePrice = orderItem.CalculatePrice();
        DiscountCampaign? bestDiscount = null;
        var bestPrice = basePrice;

        foreach (var discount in _activeDiscounts.Where(discount => discount.IsApplicable(orderItem.Item)))
        {
            var discountedPrice = orderItem.CalculatePriceWithDiscount(discount);

            if (discountedPrice < bestPrice)
            {
                bestPrice = discountedPrice;
                bestDiscount = discount;
            }
        }

        if (bestDiscount is not null)
        {
            CurrentOrder.ApplyDiscount(orderItem, bestDiscount);
        }
    }
}
