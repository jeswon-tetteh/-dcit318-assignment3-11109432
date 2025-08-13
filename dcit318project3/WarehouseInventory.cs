using System;
using System.Collections.Generic;

interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}

class ElectronicItem : IInventoryItem
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

    public override string ToString()
    {
        return $"Electronic: Id={Id}, Name={Name}, Quantity={Quantity}, Brand={Brand}, Warranty={WarrantyMonths} months";
    }
}

class GroceryItem : IInventoryItem
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

    public override string ToString()
    {
        return $"Grocery: Id={Id}, Name={Name}, Quantity={Quantity}, Expiry={ExpiryDate.ToShortDateString()}";
    }
}

class DuplicateItemException : Exception
{
    public DuplicateItemException(string message) : base(message) { }
}

class ItemNotFoundException : Exception
{
    public ItemNotFoundException(string message) : base(message) { }
}

class InvalidQuantityException : Exception
{
    public InvalidQuantityException(string message) : base(message) { }
}

class InventoryRepository<T> where T : IInventoryItem
{
    private Dictionary<int, T> _items = new Dictionary<int, T>();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
        {
            throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
        }
        _items.Add(item.Id, item);
    }

    public T GetItemById(int id)
    {
        if (!_items.TryGetValue(id, out T item))
        {
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        }
        return item;
    }

    public void RemoveItem(int id)
    {
        if (!_items.Remove(id))
        {
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        }
    }

    public List<T> GetAllItems()
    {
        return new List<T>(_items.Values);
    }

    public void UpdateQuantity(int id, int newQuantity)
    {
        T item = GetItemById(id); // Throws if not found
        if (newQuantity < 0)
        {
            throw new InvalidQuantityException("Quantity cannot be negative.");
        }
        item.Quantity = newQuantity;
    }
}

class WareHouseManager
{
    public InventoryRepository<ElectronicItem> _electronics = new InventoryRepository<ElectronicItem>();
    public InventoryRepository<GroceryItem> _groceries = new InventoryRepository<GroceryItem>();

    public void SeedData()
    {
        _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 12));
        _electronics.AddItem(new ElectronicItem(2, "Smartphone", 20, "Samsung", 6));
        _electronics.AddItem(new ElectronicItem(3, "Headphones", 15, "Sony", 24));

        _groceries.AddItem(new GroceryItem(1, "Milk", 50, DateTime.Now.AddDays(7)));
        _groceries.AddItem(new GroceryItem(2, "Bread", 30, DateTime.Now.AddDays(3)));
        _groceries.AddItem(new GroceryItem(3, "Apples", 40, DateTime.Now.AddDays(10)));
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        Console.WriteLine("All items:");
        foreach (var item in repo.GetAllItems())
        {
            Console.WriteLine(item.ToString());
        }
    }

    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            var item = repo.GetItemById(id);
            repo.UpdateQuantity(id, item.Quantity + quantity);
        }
        catch (ItemNotFoundException e)
        {
            Console.WriteLine(e.Message);
        }
        catch (InvalidQuantityException e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
        }
        catch (ItemNotFoundException e)
        {
            Console.WriteLine(e.Message);
        }
    }
}

class Program
{
    static void Main()
    {
        WareHouseManager manager = new WareHouseManager();
        manager.SeedData();

        Console.WriteLine("Grocery Items:");
        manager.PrintAllItems(manager._groceries);

        Console.WriteLine("\nElectronic Items:");
        manager.PrintAllItems(manager._electronics);

        // Try to add a duplicate item
        Console.WriteLine("\nTrying to add duplicate item:");
        try
        {
            manager._groceries.AddItem(new GroceryItem(1, "Duplicate Milk", 10, DateTime.Now.AddDays(5)));
        }
        catch (DuplicateItemException e)
        {
            Console.WriteLine(e.Message);
        }

        // Try to remove a non-existent item
        Console.WriteLine("\nTrying to remove non-existent item:");
        manager.RemoveItemById(manager._electronics, 999);

        // Try to update with invalid quantity (by decreasing more than available)
        Console.WriteLine("\nTrying to update with invalid quantity:");
        manager.IncreaseStock(manager._electronics, 1, -100);
    }
}