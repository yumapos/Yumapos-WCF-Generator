using System;
using System.Collections.Generic;
using System.Linq;

namespace WCFGenerator.Common
{
    public class CollectionComparisonResult<T>
    {
        public CollectionComparisonResult()
        {
            Updated = new List<T>();
            Added = new List<T>();
            Deleted = new List<T>();
        }

        public IList<T> Updated { get; private set; }

        public IList<T> Added { get; private set; }

        public IList<T> Deleted { get; private set; }

        public bool WasChanged => Updated.Any() || Added.Any() || Deleted.Any();
    }

    public static class CollectionComparator
    {
        public static CollectionComparisonResult<T> Compare<T>(
            IList<T> toCompare,
            IList<T> oldValues,
            Func<T, T, bool> sameItem,
            Func<T, T, bool> completelyIdentical)
        {
            var ret = new CollectionComparisonResult<T>();

            var toCompareSorter = new List<T>(toCompare);
            var oldValuesSorter = new List<T>(oldValues);

            foreach (var t in toCompareSorter)
            {
                var found = default(T);
                foreach (var oldValue in oldValuesSorter)
                {
                    if (sameItem(t, oldValue))
                    {
                        found = oldValue;
                        if (!completelyIdentical(t, oldValue))
                        {
                            ret.Updated.Add(t);
                        }
                        break;
                    }
                }

                if (found != null)
                {
                    oldValuesSorter.Remove(found);
                }
                else
                {
                    ret.Added.Add(t);
                }
            }

            foreach (var a in oldValuesSorter)
            {
                ret.Deleted.Add(a);
            }

            return ret;
        }
    }
}
