using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace InputSanitizer.Infrastructure
{
    internal class Shared
    {
        public static readonly IDictionary<Type, List<PropertyInfo>> _ObjectPropertiesCache = new Dictionary<Type, List<PropertyInfo>>();
    }
}
