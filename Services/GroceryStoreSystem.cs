using GroceryInventoryApp.Discounts;
using GroceryInventoryApp.Models;

namespace GroceryInventoryApp.Services;

public sealed class GroceryStoreSystem
{
    private readonly Catalog _catalog;
    private readonly Inventory _inventory;
    private readonly Checkout _checkout;

    public GroceryStoreSystem()
    {
        _catalog = new Catalog();
        _inventory = new Inventory();
        ActiveDiscounts = new List<DiscountCampaign>();
        _checkout = new Checkout(ActiveDiscounts, _inventory);
    }

    public IReadOnlyList<DiscountCampaign> ActiveDiscounts { get; }

    public Order CurrentOrder => _checkout.CurrentOrder;

    public static GroceryStoreSystem CreateWithSampleData()
    {
        var system = new GroceryStoreSystem();

        system.AddOrUpdateItem(new Item("Apple", "1001", "Produce", 0.75m));
        system.AddOrUpdateItem(new Item("Milk", "2001", "Dairy", 3.49m));
        system.AddOrUpdateItem(new Item("Bread", "3001", "Bakery", 2.99m));
        system.AddOrUpdateItem(new Item("Rice Bag", "4001", "Pantry", 12.50m));

        system.UpdateInventory("1001", 100);
        system.UpdateInventory("2001", 30);
        system.UpdateInventory("3001", 40);
        system.UpdateInventory("4001", 15);

        system.AddDiscountCampaign(new DiscountCampaign(
            "PRODUCE10",
            "10% off Produce",
            new CategoryBasedCriteria("Produce"),
            new PercentageBasedStrategy(10)));

        system.AddDiscountCampaign(new DiscountCampaign(
            "MILK50",
            "$0.50 off Milk",
            new ItemBasedCriteria("2001"),
            new AmountBasedStrategy(0.50m)));

        return system;
    }

    public void AddOrUpdateItem(Item item)
    {
        _catalog.UpdateItem(item);
    }

    public bool RemoveItem(string barcode)
    {
        var removed = _catalog.RemoveItem(barcode);

        if (removed)
        {
            _inventory.RemoveStockRecord(barcode);
        }

        return removed;
    }

    public Item? GetItemByBarcode(string barcode)
    {
        return _catalog.GetItem(barcode);
    }

    public IReadOnlyCollection<Item> GetCatalogItems()
    {
        return _catalog.GetItems();
    }

    public int GetStock(string barcode)
    {
        return _inventory.GetStock(barcode);
    }

    public void UpdateInventory(string barcode, int count)
    {
        if (_catalog.GetItem(barcode) is null)
        {
            throw new InvalidOperationException("Cannot add stock for an unknown barcode.");
        }

        _inventory.AddStock(barcode, count);
    }

    public void AddDiscountCampaign(DiscountCampaign discount)
    {
        ((List<DiscountCampaign>)ActiveDiscounts).Add(discount);
    }

    public void StartNewOrder()
    {
        _checkout.StartNewOrder();
    }

    public void AddItemToCurrentOrder(string barcode, int quantity)
    {
        var item = _catalog.GetItem(barcode)
            ?? throw new InvalidOperationException("Invalid barcode. No catalog item exists.");

        _checkout.AddItemToOrder(item, quantity);
    }

    public Receipt ProcessPayment(decimal paymentAmount)
    {
        return _checkout.ProcessPayment(paymentAmount);
    }
}
