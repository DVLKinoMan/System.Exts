using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System.Exts;

public static partial class Extensions
{
    public static Array CreateArray(this IEnumerable<object> source, Type type) =>
        source.ToArray().CreateArray(type);

    public static Array CreateArray(this object[] arr, Type type)
    {
        var result = Array.CreateInstance(type, arr.Length);
        for (int i = 0; i < arr.Length; i++)
            result.SetValue(arr[i], i);
        return result;
    }

    public static bool IsEqual(this byte[] byteArr1, byte[] byteArr2) =>
        byteArr1.Length == byteArr2.Length && byteArr1.All((d, i) => d == byteArr2[i]);

    public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T val)
    {
        foreach (var s in source)
            yield return s;

        yield return val;
    }

    public static IEnumerable<T> Concat<T>(this T val, IEnumerable<T> source)
    {
        yield return val;

        foreach (var s in source)
            yield return s;
    }

    public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable) => enumerable == null || !enumerable.Any();

    public static IEnumerable<T> AsEnumerable<T>(this T val)
    {
        yield return val;
    }

    public static IEnumerable<(TResult1, TResult2)> SelectManyWithItem<T, TResult1, TResult2>(
        this IEnumerable<T> source, Func<T, IEnumerable<TResult1>> func, Func<T, TResult2> itemSelector) =>
        from s in source
        from result1 in func(s)
        select (result1, itemSelector(s));

    public static T AddAndReturn<T>(this List<T> list, T t)
    {
        list.Add(t);
        return t;
    }

    //public static bool ContainsRange<T>(this IEnumerable<T> source, IEnumerable<T> range) =>
    //    throw new NotImplementedException();

    //public static bool ContainsRange<T, TResult>(this IEnumerable<T> source, Expression<Func<T, TResult>> selector, IEnumerable<TResult> range) =>
    //    throw new NotImplementedException();

    public static IEnumerable<T> Concat<T>(params IEnumerable<T>[] sources)
    {
        IEnumerable<T> result = [];
        foreach (var source in sources)
        {
            result = result.Concat(source);
        }

        return result;
    }

    /// <summary>
    /// If source every element is true for index and element
    /// </summary>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool All<T>(this IEnumerable<T> source, Func<T, int, bool> predicate)
    {
        int i = 0;
        foreach (var item in source)
            if (!predicate(item, i++))
                return false;

        return true;
    }

    /// <summary>
    /// Partition source with following predicate and return both partitions
    /// </summary>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static (List<T> Filtered, List<T> OutsideOfFilter) PartitionByPredicate<T>(this IEnumerable<T> source,
        Func<T, bool> predicate)
    {
        List<T> filtered = [], outsideOfFilter = [];
        foreach (var s in source)
        {
            if (predicate(s))
                filtered.Add(s);
            else outsideOfFilter.Add(s);
        }

        return (filtered, outsideOfFilter);
    }

    /// <summary>
    /// Partition source with following predicate and return both partitions with selector
    /// </summary>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <param name="valueSelector"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static (List<TResult> Filtered, List<TResult> OutsideOfFilter) PartitionByPredicate<T, TResult>(
        this IEnumerable<T> source,
        Func<T, bool> predicate, Func<T, TResult> valueSelector)
    {
        List<TResult> filtered = [], outsideOfFilter = [];
        foreach (var s in source)
        {
            if (predicate(s))
                filtered.Add(valueSelector.Invoke(s));
            else outsideOfFilter.Add(valueSelector.Invoke(s));
        }

        return (filtered, outsideOfFilter);
    }

    /// <summary>
    /// Converts IEnumerable source to concrete type list
    /// </summary>
    /// <param name="source"></param>
    /// <param name="elementType"></param>
    /// <returns></returns>
    public static IList ConvertToConcreteList(this IEnumerable source, Type elementType)
    {
        var targetListType = typeof(List<>).MakeGenericType(elementType);
        var convertedList = (IList)Activator.CreateInstance(targetListType)!;
        foreach (var obj in source)
            if (obj != null)
                convertedList.Add(obj);

        return convertedList;
    }

    public static IList ConvertToConcreteNullableList(this IEnumerable source, Type elementType)
    {
        var targetListType = typeof(List<>).MakeGenericType(elementType);
        var convertedList = (IList)Activator.CreateInstance(targetListType)!;
        foreach (var obj in source)
            convertedList.Add(obj);

        return convertedList;
    }

    /// <summary>
    /// Gets first element from source and creates concrete object
    /// </summary>
    /// <param name="source"></param>
    /// <param name="elementType"></param>
    /// <returns></returns>
    public static object ConvertToConcreteObject(this IEnumerable source, Type elementType)
    {
        var instance = Activator.CreateInstance(elementType)!;
        foreach (var obj in source)
            if (obj != null)
            {
                instance = obj;
                break;
            }

        return instance;
    }

    /// <summary>
    /// Converts IEnumerable generic source to object list
    /// </summary>
    /// <param name="genericList"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<object> ConvertToObjectList<T>(this IEnumerable<T> genericList)
    {
        List<object> objectList = [];
        foreach (var item in genericList)
            if (item != null)
                objectList.Add(item);

        return objectList;
    }

    /// <summary>
    /// Converts IEnumerable generic source to object list
    /// </summary>
    /// <param name="genericList"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<object?> ConvertToNullableObjectsList<T>(this IEnumerable<T> genericList)
    {
        List<object?> objectList = [];
        foreach (var item in genericList)
            objectList.Add(item);

        return objectList;
    }

    public static List<object> ConvertToObjectList(this IEnumerable enumerable) =>
        enumerable.ConvertToIEnumerable().ToList();

    public static IEnumerable<object> ConvertToIEnumerable(this IEnumerable enumerable)
    {
        foreach (var item in enumerable)
            if (item != null)
                yield return item;
    }

    public static IEnumerable<T> ConvertToConcrete<T>(this IEnumerable enumerable, Func<object, T> converter)
    {
        foreach (var item in enumerable)
            yield return converter(item);
    }

    public static Dictionary<TKey, List<TValue>> ToDictionaryList<TKey, TValue>(this IEnumerable<TValue> source,
        Func<TValue, TKey?> keySelector) where TKey : notnull
    {
        var dict = new Dictionary<TKey, List<TValue>>();

        foreach (var item in source)
        {
            var key = keySelector(item);
            if (key != null && !dict.TryAdd(key, [item]))
                dict[key].Add(item);
        }

        return dict;
    }
}