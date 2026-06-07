using System.Globalization;
using GroceryInventoryApp.Discounts;
using GroceryInventoryApp.Models;
using GroceryInventoryApp.Services;

namespace GroceryInventoryApp.UI;

public sealed class TerminalApp
{
    private readonly GroceryStoreSystem _system;

    public TerminalApp(GroceryStoreSystem system)
    {
        _system = system;
    }

    public void Run()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Grocery Inventory Terminal");
            Console.WriteLine("1. Catalog");
            Console.WriteLine("2. Inventory");
            Console.WriteLine("3. Discounts");
            Console.WriteLine("4. Checkout");
            Console.WriteLine("5. Exit");

            switch (ReadRequired("Choose an option: "))
            {
                case "1":
                    CatalogMenu();
                    break;
                case "2":
                    InventoryMenu();
                    break;
                case "3":
                    DiscountMenu();
                    break;
                case "4":
                    CheckoutMenu();
                    break;
                case "5":
                    return;
                default:
                    WriteError("Invalid option.");
                    break;
            }
        }
    }

    private void CatalogMenu()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Catalog");
            Console.WriteLine("1. List items");
            Console.WriteLine("2. Add or update item");
            Console.WriteLine("3. Remove item");
            Console.WriteLine("4. Back");

            switch (ReadRequired("Choose an option: "))
            {
                case "1":
                    PrintCatalog();
                    break;
                case "2":
                    AddOrUpdateItem();
                    break;
                case "3":
                    RemoveItem();
                    break;
                case "4":
                    return;
                default:
                    WriteError("Invalid option.");
                    break;
            }
        }
    }

    private void InventoryMenu()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Inventory");
            Console.WriteLine("1. View stock");
            Console.WriteLine("2. Add shipment stock");
            Console.WriteLine("3. Back");

            switch (ReadRequired("Choose an option: "))
            {
                case "1":
                    PrintCatalog();
                    break;
                case "2":
                    AddShipmentStock();
                    break;
                case "3":
                    return;
                default:
                    WriteError("Invalid option.");
                    break;
            }
        }
    }

    private void DiscountMenu()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Discounts");
            Console.WriteLine("1. List discounts");
            Console.WriteLine("2. Add category percentage discount");
            Console.WriteLine("3. Add item fixed discount");
            Console.WriteLine("4. Back");

            switch (ReadRequired("Choose an option: "))
            {
                case "1":
                    PrintDiscounts();
                    break;
                case "2":
                    AddCategoryDiscount();
                    break;
                case "3":
                    AddItemDiscount();
                    break;
                case "4":
                    return;
                default:
                    WriteError("Invalid option.");
                    break;
            }
        }
    }

    private void CheckoutMenu()
    {
        _system.StartNewOrder();

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Checkout");
            Console.WriteLine("1. Scan/add item");
            Console.WriteLine("2. View current order");
            Console.WriteLine("3. Take payment and print receipt");
            Console.WriteLine("4. Cancel order");

            switch (ReadRequired("Choose an option: "))
            {
                case "1":
                    AddItemToOrder();
                    break;
                case "2":
                    PrintCurrentOrder();
                    break;
                case "3":
                    if (TakePayment())
                    {
                        return;
                    }
                    break;
                case "4":
                    _system.StartNewOrder();
                    Console.WriteLine("Order cancelled.");
                    return;
                default:
                    WriteError("Invalid option.");
                    break;
            }
        }
    }

    private void PrintCatalog()
    {
        Console.WriteLine();
        Console.WriteLine($"{"Barcode",-12} {"Name",-20} {"Category",-14} {"Price",10} {"Stock",8}");
        Console.WriteLine(new string('-', 70));

        foreach (var item in _system.GetCatalogItems())
        {
            Console.WriteLine($"{item.Barcode,-12} {Trim(item.Name, 20),-20} {Trim(item.Category, 14),-14} {item.Price,10:C} {_system.GetStock(item.Barcode),8}");
        }
    }

    private void AddOrUpdateItem()
    {
        var barcode = ReadRequired("Barcode: ");
        var name = ReadRequired("Name: ");
        var category = ReadRequired("Category: ");
        var price = ReadMoney("Price: ");

        _system.AddOrUpdateItem(new Item(name, barcode, category, price));
        Console.WriteLine("Item saved.");
    }

    private void RemoveItem()
    {
        var barcode = ReadRequired("Barcode to remove: ");

        if (_system.RemoveItem(barcode))
        {
            Console.WriteLine("Item removed.");
            return;
        }

        WriteError("No item exists with that barcode.");
    }

    private void AddShipmentStock()
    {
        var barcode = ReadRequired("Barcode: ");
        var count = ReadPositiveInt("Shipment quantity: ");

        try
        {
            _system.UpdateInventory(barcode, count);
            Console.WriteLine("Inventory updated.");
        }
        catch (InvalidOperationException ex)
        {
            WriteError(ex.Message);
        }
    }

    private void PrintDiscounts()
    {
        Console.WriteLine();

        if (!_system.ActiveDiscounts.Any())
        {
            Console.WriteLine("No active discounts.");
            return;
        }

        foreach (var discount in _system.ActiveDiscounts)
        {
            Console.WriteLine($"{discount.DiscountId}: {discount.Name}");
        }
    }

    private void AddCategoryDiscount()
    {
        var category = ReadRequired("Category: ");
        var percentage = ReadMoney("Percentage (example: 15 for 15%): ");
        var discount = new DiscountCampaign(
            Guid.NewGuid().ToString("N")[..8],
            $"{percentage:0.##}% off {category}",
            new CategoryBasedCriteria(category),
            new PercentageBasedStrategy(percentage));

        _system.AddDiscountCampaign(discount);
        Console.WriteLine("Discount campaign added.");
    }

    private void AddItemDiscount()
    {
        var barcode = ReadRequired("Item barcode: ");
        var amount = ReadMoney("Fixed amount off each matching line: ");
        var item = _system.GetItemByBarcode(barcode);

        if (item is null)
        {
            WriteError("No item exists with that barcode.");
            return;
        }

        var discount = new DiscountCampaign(
            Guid.NewGuid().ToString("N")[..8],
            $"{amount:C} off {item.Name}",
            new ItemBasedCriteria(barcode),
            new AmountBasedStrategy(amount));

        _system.AddDiscountCampaign(discount);
        Console.WriteLine("Discount campaign added.");
    }

    private void AddItemToOrder()
    {
        var barcode = ReadRequired("Barcode: ");
        var quantity = ReadPositiveInt("Quantity: ");

        try
        {
            _system.AddItemToCurrentOrder(barcode, quantity);
            Console.WriteLine("Item added.");
        }
        catch (InvalidOperationException ex)
        {
            WriteError(ex.Message);
        }
    }

    private void PrintCurrentOrder()
    {
        var order = _system.CurrentOrder;

        if (!order.Items.Any())
        {
            Console.WriteLine("The order is empty.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine($"Order: {order.OrderId}");
        Console.WriteLine($"{"Item",-20} {"Qty",5} {"Base",10} {"Discount",20} {"Line Total",12}");
        Console.WriteLine(new string('-', 75));

        foreach (var orderItem in order.Items)
        {
            var discount = order.GetAppliedDiscount(orderItem);
            var discountName = discount?.Name ?? "None";
            var lineTotal = discount is null
                ? orderItem.CalculatePrice()
                : orderItem.CalculatePriceWithDiscount(discount);

            Console.WriteLine($"{Trim(orderItem.Item.Name, 20),-20} {orderItem.Quantity,5} {orderItem.CalculatePrice(),10:C} {Trim(discountName, 20),-20} {lineTotal,12:C}");
        }

        Console.WriteLine(new string('-', 75));
        Console.WriteLine($"Subtotal: {order.CalculateSubtotal():C}");
        Console.WriteLine($"Total:    {order.CalculateTotal():C}");
    }

    private bool TakePayment()
    {
        var order = _system.CurrentOrder;

        if (!order.Items.Any())
        {
            WriteError("Cannot finish an empty order.");
            return false;
        }

        PrintCurrentOrder();
        var total = order.CalculateTotal();
        var payment = ReadMoney("Payment amount: ");

        try
        {
            var receipt = _system.ProcessPayment(payment);
            Console.WriteLine();
            Console.WriteLine(receipt.PrintReceipt());
            _system.StartNewOrder();
            return true;
        }
        catch (InvalidOperationException ex)
        {
            WriteError(ex.Message);
            Console.WriteLine($"Amount still due: {(total - payment):C}");
            return false;
        }
    }

    private static string ReadRequired(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var value = Console.ReadLine()?.Trim();

            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            WriteError("Please enter a value.");
        }
    }

    private static int ReadPositiveInt(string prompt)
    {
        while (true)
        {
            var value = ReadRequired(prompt);

            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.CurrentCulture, out var number) && number > 0)
            {
                return number;
            }

            WriteError("Please enter a positive whole number.");
        }
    }

    private static decimal ReadMoney(string prompt)
    {
        while (true)
        {
            var value = ReadRequired(prompt);

            if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.CurrentCulture, out var amount) && amount >= 0)
            {
                return Math.Round(amount, 2);
            }

            WriteError("Please enter a valid non-negative amount.");
        }
    }

    private static string Trim(string value, int length)
    {
        return value.Length <= length ? value : value[..(length - 1)] + ".";
    }

    private static void WriteError(string message)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ForegroundColor = originalColor;
    }
}
