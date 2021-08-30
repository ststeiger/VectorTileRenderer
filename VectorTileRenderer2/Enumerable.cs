
namespace VectorTileRenderer
{


	internal static class Enumerable
    {


        internal delegate bool FilterCallback_t<TSource>(TSource s);
        internal delegate TResult MapCallback_t<TSource, TResult>(TSource s);


        internal static double Min(this System.Collections.Generic.IEnumerable<double> source)
        {
            if (source == null)
                throw new System.ArgumentNullException("source");

            bool empty = true;
            double min = double.MaxValue;
            foreach (double element in source)
            {
                min = System.Math.Min(element, min);
                empty = false;
            }
            if (empty)
                throw new System.InvalidOperationException();
            return min;
        }


        internal static double Max(this System.Collections.Generic.IEnumerable<double> source)
        {
            if (source == null)
                throw new System.ArgumentNullException("source");

            bool empty = true;
            double max = double.MinValue;
            foreach (double element in source)
            {
                max = System.Math.Max(element, max);
                empty = false;
            }

            if (empty)
                throw new System.InvalidOperationException();

            return max;
        }


        internal static double Min<TSource>(this System.Collections.Generic.IEnumerable<TSource> source
            , MapCallback_t<TSource, double> selector)
        {
            if (source == null)
                throw new System.ArgumentNullException("source");

            if (selector == null)
                throw new System.ArgumentNullException("selector");

            bool empty = true;
            double min = double.MaxValue;
            foreach (TSource element in source)
            {
                min = System.Math.Min(selector(element), min);
                empty = false;
            }
            if (empty)
                throw new System.InvalidOperationException();
            return min;
        }


        internal static double Max<TSource>(this System.Collections.Generic.IEnumerable<TSource> source
            , MapCallback_t<TSource, double> selector)
        {
            if (source == null)
                throw new System.ArgumentNullException("source");

            if (selector == null)
                throw new System.ArgumentNullException("selector");

            bool empty = true;
            double max = double.MinValue;
            foreach (TSource element in source)
            {
                max = System.Math.Max(selector(element), max);
                empty = false;
            }
            if (empty)
                throw new System.InvalidOperationException();
            return max;
        }


        internal static System.Collections.Generic.IEnumerable<TSource> Skip<TSource>(
            this System.Collections.Generic.IEnumerable<TSource> source, int count)
        {
            if (source == null)
                throw new System.ArgumentNullException("source");

            return CreateSkipIterator(source, count);
        }

        private static System.Collections.Generic.IEnumerable<TSource> CreateSkipIterator<TSource>(
            System.Collections.Generic.IEnumerable<TSource> source, int count)
        {
            System.Collections.Generic.IEnumerator<TSource> enumerator = source.GetEnumerator();
            try
            {
                while (count-- > 0)
                    if (!enumerator.MoveNext())
                        yield break;

                while (enumerator.MoveNext())
                    yield return enumerator.Current;

            }
            finally
            {
                enumerator.Dispose();
            }
        }


        // Select 
        internal static System.Collections.Generic.List<TResult> Map<TSource, TResult>(
              this System.Collections.Generic.IEnumerable<TSource> source
            , MapCallback_t<TSource, TResult> selector)
        {
            if (source == null)
                throw new System.ArgumentNullException("source");

            if (selector == null)
                throw new System.ArgumentNullException("selector");

            System.Collections.Generic.List<TResult> ls = new System.Collections.Generic.List<TResult>();

            foreach (TSource element in source)
                ls.Add(selector(element));

            return ls;
        } // End Function Map 


        // Where
        internal static System.Collections.Generic.List<TSource> Filter<TSource>(
              this System.Collections.Generic.IEnumerable<TSource> source
            , FilterCallback_t<TSource> predicate)
        {
            if (source == null)
                throw new System.ArgumentNullException("source");
            if (predicate == null)
                throw new System.ArgumentNullException("predicate");

            System.Collections.Generic.List<TSource> ls = new System.Collections.Generic.List<TSource>();

            foreach (TSource element in source)
            {
                if (predicate(element))
                    ls.Add(element);
            } // Next element 

            return ls;
        } // End Function Filter 


        internal static System.Collections.Generic.IEnumerable<TSource> Reverse<TSource>(
            this System.Collections.Generic.IEnumerable<TSource> source)
        {
            if (source == null)
                throw new System.ArgumentNullException("source");

            return CreateReverseIterator(source);
        }

        private static System.Collections.Generic.IEnumerable<TSource> CreateReverseIterator<TSource>(
            System.Collections.Generic.IEnumerable<TSource> source)
        {
            System.Collections.Generic.IList<TSource> list = source as System.Collections.Generic.IList<TSource>;
            if (list == null)
                list = new System.Collections.Generic.List<TSource>(source);

            for (int i = list.Count - 1; i >= 0; i--)
                yield return list[i];
        }



        internal static System.Collections.Generic.List<TSource> OrderByDescending<TSource, TKey>(
            this System.Collections.Generic.IEnumerable<TSource> source,
            MapCallback_t<TSource, TKey> keySelector) where TKey : System.IComparable<TKey>
        {
            System.Collections.Generic.List<TSource> ls = new System.Collections.Generic.List<TSource>(source);

            ls.Sort(
                delegate (TSource t1, TSource t2)
                {
                    return keySelector(t2).CompareTo(keySelector(t1));
                }
            ); // Asc 

            return ls;
        }


        internal static System.Collections.Generic.List<TSource> OrderBy<TSource, TKey>(
            this System.Collections.Generic.IEnumerable<TSource> source,
            MapCallback_t<TSource, TKey> keySelector) where TKey:System.IComparable<TKey>
        {
            System.Collections.Generic.List<TSource> ls = new System.Collections.Generic.List<TSource>(source);

            ls.Sort(
                delegate (TSource t1, TSource t2) 
                {
                    return keySelector(t1).CompareTo(keySelector(t2)); 
                }
            ); // Asc 

            return ls;
        }

        // ls.Sort(delegate (int aa, int bb) { return aa.CompareTo(bb); });

        private enum Fallback
        {
            Default,
            Throw
        }


        private static TSource Last<TSource>(
            this System.Collections.Generic.IEnumerable<TSource> source
            , MapCallback_t<TSource, bool> predicate, Fallback fallback)
        {
            bool empty = true;
            TSource item = default(TSource);

            foreach (TSource element in source)
            {
                if (!predicate(element))
                    continue;

                item = element;
                empty = false;
            }

            if (!empty)
                return item;

            if (fallback == Fallback.Throw)
                throw new System.InvalidOperationException();

            return item;
        }




        private static TSource First<TSource>(this System.Collections.Generic.IEnumerable<TSource> source
            , MapCallback_t<TSource, bool> predicate, Fallback fallback)
        {
            foreach (TSource element in source)
                if (predicate(element))
                    return element;

            if (fallback == Fallback.Throw)
                throw new System.InvalidOperationException();

            return default(TSource);
        }

        internal static TSource First<TSource>(this System.Collections.Generic.IEnumerable<TSource> source)
        {
            if (source == null)
                throw new System.ArgumentNullException("source");

            System.Collections.Generic.IList<TSource> list = source as System.Collections.Generic.IList<TSource>;
            if (list != null)
            {
                if (list.Count != 0)
                    return list[0];
            }
            else
            {
                using (System.Collections.Generic.IEnumerator<TSource> enumerator = source.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                        return enumerator.Current;
                }
            }

            throw new System.InvalidOperationException("The source sequence is empty");
        }

        internal static TSource First<TSource>(
              this System.Collections.Generic.IEnumerable<TSource> source, 
              MapCallback_t<TSource, bool> predicate)
        {

            if (source == null)
                throw new System.ArgumentNullException("source");
            if (predicate == null)
                throw new System.ArgumentNullException("predicate");

            return source.First(predicate, Fallback.Throw);
        }




        static class PredicateOf<T>
        {
            internal static readonly MapCallback_t<T, bool> Always = delegate(T t) { return true; };
        }


        internal static TSource Last<TSource>(this System.Collections.Generic.IEnumerable<TSource> source)
        {
            if (source == null)
                throw new System.ArgumentNullException("source");

            System.Collections.Generic.ICollection<TSource> collection = source as System.Collections.Generic.ICollection<TSource>;
            if (collection != null && collection.Count == 0)
                throw new System.InvalidOperationException();

            System.Collections.Generic.IList<TSource> list = source as System.Collections.Generic.IList<TSource>;
            if (list != null)
                return list[list.Count - 1];

            return source.Last(PredicateOf<TSource>.Always, Fallback.Throw);
        }

        internal static TSource Last<TSource>(this System.Collections.Generic.IEnumerable<TSource> source
            , MapCallback_t<TSource, bool> predicate)
        {
            if (source == null)
                throw new System.ArgumentNullException("source");
            if (predicate == null)
                throw new System.ArgumentNullException("predicate");

            return source.Last(predicate, Fallback.Throw);
        }


        internal static System.Collections.Generic.Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(
            this System.Collections.Generic.IEnumerable<TSource> source,
            MapCallback_t<TSource, TKey> keySelector, MapCallback_t<TSource, TElement> elementSelector)
        {
            return ToDictionary<TSource, TKey, TElement>(source, keySelector, elementSelector, null);
        }

        internal static System.Collections.Generic.Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(
            this System.Collections.Generic.IEnumerable<TSource> source,
                MapCallback_t<TSource, TKey> keySelector, 
                MapCallback_t<TSource, TElement> elementSelector, 
                System.Collections.Generic.IEqualityComparer<TKey> comparer)
        {
            if (source == null)
                throw new System.ArgumentNullException("source");
            if (keySelector == null)
                throw new System.ArgumentNullException("keySelector");
            if (elementSelector == null)
                throw new System.ArgumentNullException("elementSelector");

            if (comparer == null)
                comparer = System.Collections.Generic.EqualityComparer<TKey>.Default;

            System.Collections.Generic.Dictionary<TKey, TElement> dict = new System.Collections.Generic.Dictionary<TKey, TElement>(comparer);
            foreach (TSource e in source)
                dict.Add(keySelector(e), elementSelector(e));

            return dict;
        }

        internal static System.Collections.Generic.Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this System.Collections.Generic.IEnumerable<TSource> source,
                MapCallback_t<TSource, TKey> keySelector)
        {
            return ToDictionary(source, keySelector, null);
        }

        internal static System.Collections.Generic.Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(
            this System.Collections.Generic.IEnumerable<TSource> source,
                MapCallback_t<TSource, TKey> keySelector, System.Collections.Generic.IEqualityComparer<TKey> comparer)
        {
            return ToDictionary<TSource, TKey, TSource>(source, keySelector, delegate(TSource x){ return x; }, comparer);
        }


    } // End Class Enumerable 


} // End Namespace UpsertSQL 
