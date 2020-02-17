using BandAPI.Entities;
using BandAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BandAPI.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> _bandPropertyMapping = new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase) {
            {"Id", new PropertyMappingValue( new List<string>() { "Id"})  },
            { "Name",new PropertyMappingValue( new List<string>() { "Name"}) },
            { "MainGenre",new PropertyMappingValue( new List<string>() { "MainGenre"}) },
            { "FoundedYearsAgo",new PropertyMappingValue( new List<string>() { "Founded"}, true)},
        };

        private IList<IPropertyMappingMarker> _propertyMappings = new List<IPropertyMappingMarker>();


        public PropertyMappingService()
        {
            // Q: Why BandDto to Band?
            _propertyMappings.Add(new PropertyMapping<BandDto, Band>(_bandPropertyMapping));  // Adding the band PropertyMapping
        }


        public Dictionary<string, PropertyMappingValue> GetPropertyMappig<TSource, TDestination>()
        {
            var matchingMapping = _propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
                return matchingMapping.First().MappingDictionary;


            throw new Exception("No mapping was found");
        }


        public bool ValidMappingExists<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMappig<TSource, TDestination>();
            if (string.IsNullOrWhiteSpace(fields))
                return true;

            var fieldsAfterSplit = fields.Split(",");

            foreach (var field in fieldsAfterSplit)
            {

                var trimField = field.Trim();
                var indexOfSpace = trimField.IndexOf(" ");
                var propertyName = indexOfSpace == -1 ? trimField : trimField.Remove(indexOfSpace);
                if (!propertyMapping.ContainsKey(propertyName))
                    return false;
            }


            return true;
        }
    }
}
