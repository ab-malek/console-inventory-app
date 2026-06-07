using GroceryInventoryApp.Services;
using GroceryInventoryApp.UI;

var system = GroceryStoreSystem.CreateWithSampleData();
var app = new TerminalApp(system);

app.Run();
