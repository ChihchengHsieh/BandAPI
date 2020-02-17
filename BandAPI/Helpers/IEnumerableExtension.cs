using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BandAPI.Helpers
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<ExpandoObject> ShapeData<TSource>(this IEnumerable<TSource> source, string fields)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));


            var propertyInfoList = new List<PropertyInfo>();

            if (string.IsNullOrWhiteSpace(fields)) // Checking if the fields provided
            {
                var propertyInfos = typeof(TSource).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance); // Get info of all the properties 

                propertyInfoList.AddRange(propertyInfos); // Adding all the properties to the list
            }
            else
            {
                var fieldsAfterSplit = fields.Split(",");
                foreach (var filed in fieldsAfterSplit) // Adding the info of the required properties to the list
                {
                    var propertyName = filed.Trim();

                    // Get the info of the property 
                    var propertyInfos = typeof(TSource).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance); // Q; What's the Pipe mean?

                    // If the info is null, then throw Exception
                    if (propertyInfos == null)
                        throw new Exception($"{propertyName.ToString()} was not found");

                    // Adding the property to the list
                    propertyInfoList.Add(propertyInfos);
                }
            }

            // The ObjectList will the returning object
            var objectList = new List<ExpandoObject>();

            foreach (TSource sourceObject in source) // For every returning element in the source, filtering the fields
            {
                var dataShapeObject = new ExpandoObject();

                foreach (var propertyInfo in propertyInfoList)
                {
                    var propertyValue = propertyInfo.GetValue(sourceObject);

                    ((IDictionary<string, object>)dataShapeObject).Add(propertyInfo.Name, propertyValue);
                }

                objectList.Add(dataShapeObject);
            }

            return objectList;
        }
    }
}
