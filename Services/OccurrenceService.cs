using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using TechForClimate.Models;

namespace TechForClimate.Services
{
    public class OccurrenceService
    {
        private readonly string _filePath = "ocorrencias.json";

        public List<Occurrence> GetAll()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Occurrence>();
            }

            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<Occurrence>>(json) ?? new List<Occurrence>();
        }

        public void Add(Occurrence occurrence)
        {
            var list = GetAll();
            occurrence.Id = list.Count > 0 ? list[^1].Id + 1 : 1;
            list.Add(occurrence);

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(list, options);
            File.WriteAllText(_filePath, json);
        }

        public bool Delete(int id)
        {
            var list = GetAll();
            var itemToRemove = list.Find(o => o.Id == id);

            if (itemToRemove == null) return false;

            list.Remove(itemToRemove);

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(list, options);
            File.WriteAllText(_filePath, json);

            return true;
        }
    }
}