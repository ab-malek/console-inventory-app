namespace GroceryInventoryApp.Models;

public sealed class Receipt
{
    public Receipt(Order order)
    {
        ReceiptId = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        Order = order;
        IssueDate = DateTime.Now;
    }

    public string ReceiptId { get; }
    public Order Order { get; }
    public DateTime IssueDate { get; }

    public string PrintReceipt()
    {
        var lines = new List<string>
        {
            "Grocery Store Receipt",
            $"Receipt: {ReceiptId}",
            $"Order:   {Order.OrderId}",
            $"Date:    {IssueDate:g}",
            new string('-', 52)
        };

        foreach (var item in Order.Items)
        {
            var discount = Order.GetAppliedDiscount(item);
            var lineTotal = discount is null
                ? item.CalculatePrice()
                : item.CalculatePriceWithDiscount(discount);

            lines.Add($"{item.Item.Name} x{item.Quantity} @ {item.Item.Price:C} = {lineTotal:C}");

            if (discount is not null)
            {
                lines.Add($"  Discount: {discount.Name}");
            }
        }

        lines.Add(new string('-', 52));
        lines.Add($"Subtotal: {Order.CalculateSubtotal():C}");
        lines.Add($"Total:    {Order.CalculateTotal():C}");
        lines.Add($"Paid:     {Order.PaymentAmount:C}");
        lines.Add($"Change:   {Order.CalculateChange():C}");

        return string.Join(Environment.NewLine, lines);
    }
}
