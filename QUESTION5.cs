using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace InventorySystem
{
    // Define the marker interface
    public interface IInventoryEntity
    {
        int Id { get; }
    }

    // Define the immutable inventory record
    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    // Generic Inventory Logger
    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private List<T> _log;
        private readonly string _filePath;

        public InventoryLogger()
        {
            _log = new List<T>();
            _filePath = "inventory.json";
        }

        public void Add(T item)
        {
            _log.Add(item);
        }

        public List<T> GetAll()
        {
            return _log;
        }

        public void SaveToFile()
        {
            try
            {
                string json = JsonSerializer.Serialize(_log);
                using (StreamWriter writer = new StreamWriter(_filePath))
                {
                    writer.Write(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
                // Optionally rethrow or handle further
            }
        }

        public void LoadFromFile()
        {
            try
            {
                using (StreamReader reader = new StreamReader(_filePath))
                {
                    string json = reader.ReadToEnd();
                    _log = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
                }
            }
            catch (FileNotFoundException)
            {
                _log = new List<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading from file: {ex.Message}");
                _log = new List<T>();
            }
        }
    }

    // Integration Layer – InventoryApp
    public class InventoryApp
    {
        private InventoryLogger<InventoryItem> _logger = new InventoryLogger<InventoryItem>();

        public void SeedSampleData()
        {
            _logger.Add(new InventoryItem(1, "Laptop", 5, DateTime.Now));
            _logger.Add(new InventoryItem(2, "Mouse", 20, DateTime.Now));
            _logger.Add(new InventoryItem(3, "Keyboard", 15, DateTime.Now));
            _logger.Add(new InventoryItem(4, "Monitor", 8, DateTime.Now));
            _logger.Add(new InventoryItem(5, "Printer", 3, DateTime.Now));
        }

        public void SaveData()
        {
            _logger.SaveToFile();
        }

        public void LoadData()
        {
            _logger.LoadFromFile();
        }

        public void PrintAllItems()
        {
            foreach (var item in _logger.GetAll())
            {
                Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Date Added: {item.DateAdded}");
            }
        }
    }

    // Main Application
    class Program
    {
        static void Main(string[] args)
        {
            InventoryApp app = new InventoryApp();
            app.SeedSampleData();
            app.SaveData();

            // Clear memory and simulate a new session by creating a new instance
            app = new InventoryApp();
            app.LoadData();
            app.PrintAllItems();
        }
    }
}
