using GroceryInventoryApp.Models;

namespace GroceryInventoryApp.Services;

public sealed class Catalog
{
    private readonly Dictionary<string, Item> _items = new(StringComparer.OrdinalIgnoreCase);

    public void UpdateItem(Item item)
    {
        _items[item.Barcode] = item;
    }

    public bool RemoveItem(string barcode)
    {
        return _items.Remove(barcode);
    }

    public Item? GetItem(string barcode)
    {
        return _items.GetValueOrDefault(barcode);
    }

    public IReadOnlyCollection<Item> GetItems()
    {
        return _items.Values.OrderBy(item => item.Name).ToList();
    }
}
