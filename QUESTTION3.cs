using System;
using System.Collections.Generic;

public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}

public class ElectronicItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public string Brand { get; }
    public int WarrantyMonths { get; }

    public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        Brand = brand;
        WarrantyMonths = warrantyMonths;
    }
}

public class GroceryItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; }

    public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        ExpiryDate = expiryDate;
    }
}

public class DuplicateItemException : Exception
{
    public DuplicateItemException(string message) : base(message) { }
}

public class ItemNotFoundException : Exception
{
    public ItemNotFoundException(string message) : base(message) { }
}

public class InvalidQuantityException : Exception
{
    public InvalidQuantityException(string message) : base(message) { }
}

public class InventoryRepository<T> where T : IInventoryItem
{
    private Dictionary<int, T> _items = new Dictionary<int, T>();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
            throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
        _items.Add(item.Id, item);
    }

    public T GetItemById(int id)
    {
        if (!_items.TryGetValue(id, out T item))
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        return item;
    }

    public void RemoveItem(int id)
    {
        if (!_items.Remove(id))
            throw new ItemNotFoundException($"Item with ID {id} not found.");
    }

    public List<T> GetAllItems()
    {
        return new List<T>(_items.Values);
    }

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0)
            throw new InvalidQuantityException("Quantity cannot be negative.");
        T item = GetItemById(id);
        item.Quantity = newQuantity;
    }
}

public class WareHouseManager
{
    public InventoryRepository<ElectronicItem> Electronics = new InventoryRepository<ElectronicItem>();
    public InventoryRepository<GroceryItem> Groceries = new InventoryRepository<GroceryItem>();

    public void SeedData()
    {
        // Electronics
        Electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 12));
        Electronics.AddItem(new ElectronicItem(2, "Phone", 20, "Apple", 24));
        Electronics.AddItem(new ElectronicItem(3, "Tablet", 15, "Samsung", 6));

        // Groceries
        Groceries.AddItem(new GroceryItem(1, "Milk", 50, new DateTime(2025, 8, 30)));
        Groceries.AddItem(new GroceryItem(2, "Bread", 30, new DateTime(2025, 8, 20)));
        Groceries.AddItem(new GroceryItem(3, "Eggs", 100, new DateTime(2025, 8, 25)));
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        var items = repo.GetAllItems();
        Console.WriteLine($"Printing {typeof(T).Name}s:");
        foreach (var item in items)
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}");
            if (item is ElectronicItem elec)
            {
                Console.WriteLine($"  Brand: {elec.Brand}, Warranty: {elec.WarrantyMonths} months");
            }
            else if (item is GroceryItem groc)
            {
                Console.WriteLine($"  Expiry: {groc.ExpiryDate.ToShortDateString()}");
            }
        }
        Console.WriteLine();
    }

    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            if (quantity < 0)
                throw new InvalidQuantityException("Quantity to add cannot be negative.");
            var item = repo.GetItemById(id);
            repo.UpdateQuantity(id, item.Quantity + quantity);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error increasing stock: {ex.Message}");
        }
    }

    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing item: {ex.Message}");
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        WareHouseManager manager = new WareHouseManager();
        manager.SeedData();
        manager.PrintAllItems(manager.Groceries);
        manager.PrintAllItems(manager.Electronics);

        // Try to add a duplicate item
        try
        {
            manager.Groceries.AddItem(new GroceryItem(1, "Duplicate Milk", 10, new DateTime(2025, 9, 1)));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding duplicate item: {ex.Message}");
        }

        // Try to remove a non-existent item
        try
        {
            manager.Electronics.RemoveItem(999);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing non-existent item: {ex.Message}");
        }

        // Try to update with invalid quantity
        try
        {
            manager.Groceries.UpdateQuantity(1, -5);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating with invalid quantity: {ex.Message}");
        }
    }
}
