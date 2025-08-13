using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

interface IInventoryEntity
{
    int Id { get; }
}

record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

class InventoryLogger<T> where T : IInventoryEntity
{
    private List<T> _log = new List<T>();
    private readonly string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Add(T item)
    {
        _log.Add(item);
    }

    public List<T> GetAll()
    {
        return new List<T>(_log);
    }

    public void SaveToFile()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(_filePath))
            {
                string json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
                writer.Write(json);
            }
        }
        catch (IOException ex)
        {
            throw new Exception($"Error saving to file: {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        try
        {
            using (StreamReader reader = new StreamReader(_filePath))
            {
                string json = reader.ReadToEnd();
                if (!string.IsNullOrEmpty(json))
                {
                    _log = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
                }
            }
        }
        catch (FileNotFoundException)
        {
            _log = new List<T>(); // Initialize empty list if file doesn't exist
        }
        catch (IOException ex)
        {
            throw new Exception($"Error loading from file: {ex.Message}");
        }
        catch (JsonException ex)
        {
            throw new Exception($"Error deserializing file: {ex.Message}");
        }
    }
}

class InventoryApp
{
    private readonly InventoryLogger<InventoryItem> _logger;

    public InventoryApp()
    {
        _logger = new InventoryLogger<InventoryItem>("inventory.json");
    }

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Laptop", 10, DateTime.Now.AddDays(-5)));
        _logger.Add(new InventoryItem(2, "Mouse", 50, DateTime.Now.AddDays(-3)));
        _logger.Add(new InventoryItem(3, "Keyboard", 30, DateTime.Now.AddDays(-2)));
        _logger.Add(new InventoryItem(4, "Monitor", 15, DateTime.Now.AddDays(-1)));
        _logger.Add(new InventoryItem(5, "Printer", 8, DateTime.Now));
    }

    public void SaveData()
    {
        try
        {
            _logger.SaveToFile();
            Console.WriteLine("Data saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public void LoadData()
    {
        try
        {
            _logger.LoadFromFile();
            Console.WriteLine("Data loaded successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public void PrintAllItems()
    {
        Console.WriteLine("Inventory Items:");
        foreach (var item in _logger.GetAll())
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Date Added: {item.DateAdded.ToShortDateString()}");
        }
    }
}

class Program
{
    static void Main()
    {
        InventoryApp app = new InventoryApp();
        app.SeedSampleData();
        app.SaveData();

        // Simulate new session by creating a new instance
        app = new InventoryApp();
        app.LoadData();
        app.PrintAllItems();
    }
}