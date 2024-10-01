using System.Collections.Generic;
using System.Linq;
using KeycloakTesting.Models;

namespace KeycloakTesting.Services
{
    public class MappingService
    {
        private readonly List<NumberColorMapping> _mappings;
        private int _nextId = 1;

        public MappingService()
        {
            // Initialize with default mappings
            _mappings = new List<NumberColorMapping>
            {
                new NumberColorMapping { Id = _nextId++, Number = 1, Color = "green" },
                new NumberColorMapping { Id = _nextId++, Number = 2, Color = "yellow" },
                new NumberColorMapping { Id = _nextId++, Number = 3, Color = "red" }
            };
        }

        public string GetColorByNumber(int number)
        {
            var mapping = _mappings.FirstOrDefault(m => m.Number == number);
            return mapping != null ? mapping.Color : "unknown";
        }

        public List<NumberColorMapping> GetAllMappings()
        {
            return _mappings;
        }

        public void UpdateMapping(int id, int number, string color)
        {
            var mapping = _mappings.FirstOrDefault(m => m.Id == id);
            if (mapping != null)
            {
                mapping.Number = number;
                mapping.Color = color;
            }
        }

        public void AddMapping(int number, string color)
        {
            _mappings.Add(new NumberColorMapping { Id = _nextId++, Number = number, Color = color });
        }

        public void DeleteMapping(int id)
        {
            var mapping = _mappings.FirstOrDefault(m => m.Id == id);
            if (mapping != null)
            {
                _mappings.Remove(mapping);
            }
        }
    }
}
