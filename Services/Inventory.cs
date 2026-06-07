namespace GroceryInventoryApp.Services;

public sealed class Inventory
{
    private readonly Dictionary<string, int> _stock = new(StringComparer.OrdinalIgnoreCase);

    public void AddStock(string barcode, int count)
    {
        _stock[barcode] = GetStock(barcode) + count;
    }

    public void ReduceStock(string barcode, int count)
    {
        var currentStock = GetStock(barcode);

        if (count > currentStock)
        {
            throw new InvalidOperationException($"Insufficient inventory for barcode {barcode}. Available: {currentStock}.");
        }

        _stock[barcode] = currentStock - count;
    }

    public int GetStock(string barcode)
    {
        return _stock.GetValueOrDefault(barcode);
    }

    public void RemoveStockRecord(string barcode)
    {
        _stock.Remove(barcode);
    }
}
