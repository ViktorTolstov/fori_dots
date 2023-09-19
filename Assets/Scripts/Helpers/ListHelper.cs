using System.Collections.Generic;

namespace Fori.Helpers
{
    public static class ListHelper
    {
        public static bool ContainsDuplicate<T>(this IEnumerable<T> list)
        {
            var uniqueElements = new HashSet<T>();
        
            foreach (var item in list)
            {
                if (uniqueElements.Contains(item))
                {
                    return true;
                }
                
                uniqueElements.Add(item);
            }
        
            return false;
        }
    }
}