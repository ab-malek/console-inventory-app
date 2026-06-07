using GroceryInventoryApp.Discounts;

namespace GroceryInventoryApp.Models;

public sealed class Order
{
    private readonly List<OrderItem> _items = new();
    private readonly Dictionary<OrderItem, DiscountCampaign> _appliedDiscounts = new();

    public Order()
    {
        OrderId = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
    }

    public string OrderId { get; }
    public IReadOnlyList<OrderItem> Items => _items;
    public decimal PaymentAmount { get; private set; }

    public void AddItem(OrderItem item)
    {
        _items.Add(item);
    }

    public decimal CalculateSubtotal()
    {
        return _items.Sum(item => item.CalculatePrice());
    }

    public decimal CalculateTotal()
    {
        return _items.Sum(item =>
            _appliedDiscounts.TryGetValue(item, out var discount)
                ? item.CalculatePriceWithDiscount(discount)
                : item.CalculatePrice());
    }

    public decimal CalculateChange()
    {
        return PaymentAmount - CalculateTotal();
    }

    public void ApplyDiscount(OrderItem item, DiscountCampaign discount)
    {
        _appliedDiscounts[item] = discount;
    }

    public DiscountCampaign? GetAppliedDiscount(OrderItem item)
    {
        return _appliedDiscounts.GetValueOrDefault(item);
    }

    public void SetPayment(decimal paymentAmount)
    {
        PaymentAmount = Math.Round(paymentAmount, 2);
    }

    public int GetQuantityForBarcode(string barcode)
    {
        return _items
            .Where(item => string.Equals(item.Item.Barcode, barcode, StringComparison.OrdinalIgnoreCase))
            .Sum(item => item.Quantity);
    }
}
