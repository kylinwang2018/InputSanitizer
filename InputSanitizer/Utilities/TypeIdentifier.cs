using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InputSanitizer
{
    internal static class TypeIdentifier
    {
        public static bool IsGenericList(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            var interfaceTest = new Predicate<Type>(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>));

            return interfaceTest(type) || type.GetInterfaces().Any(i => interfaceTest(i));
        }
    }
}
