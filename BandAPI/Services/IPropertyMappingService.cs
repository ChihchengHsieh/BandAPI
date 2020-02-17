using System.Collections.Generic;

namespace BandAPI.Services
{
    public interface IPropertyMappingService
    {
        Dictionary<string, PropertyMappingValue> GetPropertyMappig<TSource, TDestination>();
        public bool ValidMappingExists<TSource, TDestination>(string fields);
    }
}