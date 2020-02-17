using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BandAPI.Services
{
    public class PropertyMappingValue
    {
        // This class is the condition of Ordering
        public IEnumerable<string> DestinationProperties { get; set; }
        public bool Revert { get; set; }

        public PropertyMappingValue(IEnumerable<string> destinationProperties, bool revert = false)
        {
            // Q: Why do we nned a DestinationPreperties here? We already have one in _bandPropertyMapping, it's a Dictionary<string, PropertyMappingValue>
            // Q: Where to store the name of the field that I want to sort?
            DestinationProperties = destinationProperties ?? throw new ArgumentNullException(nameof(destinationProperties));
            Revert = revert;
        }
    }
}
