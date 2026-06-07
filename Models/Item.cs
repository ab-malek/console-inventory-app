namespace GroceryInventoryApp.Models;

public sealed class Item
{
    public Item(string name, string barcode, string category, decimal price)
    {
        Name = name;
        Barcode = barcode;
        Category = category;
        Price = Math.Round(price, 2);
    }

    public string Name { get; }
    public string Barcode { get; }
    public string Category { get; }
    public decimal Price { get; }
}
