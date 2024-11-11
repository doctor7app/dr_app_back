using Microsoft.AspNetCore.OData.Deltas;
using Newtonsoft.Json.Linq;

namespace Common.Helpers
{
    /// <summary>
    /// This class is used to be able to update collection of navigation properties.
    /// Fix the issue of the not being able to detect the navigation properties when patching using Delta.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class DeltaWithCollectionsSupport<T> : Delta<T> where T : class
    {
        public override bool TrySetPropertyValue(string name, object value)
        {
            var propertyInfo = typeof(T).GetProperty(name);

            return propertyInfo != null && value is JArray array
                ? base.TrySetPropertyValue(name, array.ToObject(propertyInfo.PropertyType))
                : base.TrySetPropertyValue(name, value);
        }
    }
}