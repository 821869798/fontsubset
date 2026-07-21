global using Integer = System.Int32;
global using Long = System.Int64;
global using boolean = System.Boolean;
global using BigDecimal = System.Decimal;
global using Date = System.DateTime;

global using OutputStream = System.IO.Stream;
global using InputStream = System.IO.Stream;
global using FilterInputStream = System.IO.Stream;
global using FilterOutputStream = System.IO.Stream;
global using BufferedInputStream = System.IO.BufferedStream;
global using BufferedOutputStream = System.IO.BufferedStream;
global using PushbackInputStream = System.IO.BufferedStream;
global using FontInputStream = System.IO.BufferedStream;
global using FontOutputStream = System.IO.BufferedStream;
global using ByteArrayInputStream = System.IO.MemoryStream;
global using ByteArrayOutputStream = System.IO.MemoryStream;
global using FileInputStream = System.IO.FileStream;
global using FileOutputStream = System.IO.FileStream;

global using Charset = System.Text.Encoding;

global using RuntimeException = System.Exception;
global using IndexOutOfBoundsException = System.IndexOutOfRangeException;
global using UnsupportedOperationException = System.NotSupportedException;
global using UnsupportedEncodingException = System.NotSupportedException;
global using NoSuchElementException = System.Collections.Generic.KeyNotFoundException;
global using IllegalArgumentException = System.ArgumentException;
global using IllegalStateException = System.InvalidOperationException;
global using InvalidParameterException = System.ArgumentException;

global using System.Text;
using System.Runtime.CompilerServices;
using com.google.typography.font.sfntly.table.core;
using System;
using System.Collections;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;

static class MapExt
{

    public static void put<TKey, TValue>(this IDictionary<TKey, TValue> map, TKey key, TValue value)
    {
        map[key] = value;
    }

    public static TValue remove<TKey, TValue>(this IDictionary<TKey, TValue> map, TKey key)
    {
        map.Remove(key, out var value);

        return value;
    }

    public static ICollection<TValue> values<TKey, TValue>(this IDictionary<TKey, TValue> map)
    {
        return map.Values;
    }

    public static bool containsKey<TKey, TValue>(this IDictionary<TKey, TValue> map, TKey key)
    {
        return map.ContainsKey(key);
    }

    public static TValue get<TKey, TValue>(this IDictionary<TKey, TValue> map, TKey key)
    {
        map.TryGetValue(key, out var value);

        return value;
    }

    public static ICollection<TKey> keySet<TKey, TValue>(this IDictionary<TKey, TValue> map)
    {
        return map.Keys;
    }

    public static IDictionary<TKey, TValue> entrySet<TKey, TValue>(this IDictionary<TKey, TValue> map)
    {
        return map;
    }
}

static class KeyValuePairExt
{

    public static TKey getKey<TKey, TValue>(this KeyValuePair<TKey, TValue> keyValuePair)
    {
        return keyValuePair.Key;
    }

    public static TValue getValue<TKey, TValue>(this KeyValuePair<TKey, TValue> keyValuePair)
    {
        return keyValuePair.Value;
    }
}

static class ListExt
{

    public static T get<T>(this IList<T> list, int index)
    {
        return list[index];
    }

    public static void set<T>(this IList<T> list, int index, T value)
    {
        list[index] = value;
    }

    public static IList<T> subList<T>(this IList<T> list, int index, int length)
    {
        throw new NotImplementedException();
    }
}

static class CollectionExt
{
    public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            list.Add(item);
        }
    }
}

static class Collections
{
    public static IList<T> unmodifiableList<T>(IList<T> items)
    {
        return items.ToImmutableList();
    }

    public static IDictionary<TKey, TValue> unmodifiableMap<TKey, TValue>(IDictionary<TKey, TValue> map)
    {
        return map.ToImmutableDictionary();
    }

    public static ISet<TValue> unmodifiableSet<TValue>(ISet<TValue> items)
    {
        return items.ToImmutableHashSet();
    }
}

public abstract class ClassEnumBase<TClassEnum> : IComparable<TClassEnum> where TClassEnum: ClassEnumBase<TClassEnum>
{
    static FieldInfo[] _fields = typeof(TClassEnum)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(x => x.FieldType == typeof(TClassEnum))
        .ToArray();
    static int init_ordinal = 0;

    readonly int _ordinal;

    public ClassEnumBase()
    {
        _ordinal = init_ordinal;

        ++init_ordinal;
    }

    public static TClassEnum[] values()
    {
        return _fields.Select(x => (TClassEnum)x.GetValue(null)).ToArray();
    }

    public int CompareTo([AllowNull] TClassEnum other)
    {
        return _ordinal - other._ordinal;
    }

    public string name()
    {
        return _fields[_ordinal].Name;
    }

    public int ordinal()
    {
        return _ordinal;
    }
}

static class StreamExt
{
    public static void close(this Stream stream)
    {
        stream.Close();
    }

    public static void write(this Stream stream, byte[] buffer, int offset, int count)
    {
        stream.Write(buffer, offset, count);
    }

    public static void write(this Stream stream, byte[] buffer)
    {
        stream.Write(buffer);
    }

    public static int read(this Stream stream, byte[] buffer, int offset, int count)
    {
        return stream.Read(buffer, offset, count);
    }

    public static int read(this Stream stream, byte[] buffer)
    {
        return stream.Read(buffer);
    }

    public static int unread(this Stream stream, byte[] buffer)
    {
        throw new NotImplementedException();
    }

    public static int readFixed(this Stream stream)
    {
        throw new NotImplementedException();
    }

    public static int readUShort(this Stream stream)
    {
        throw new NotImplementedException();
    }

    public static long readULong(this Stream stream)
    {
        throw new NotImplementedException();
    }

    public static int readULongAsInt(this Stream stream)
    {
        throw new NotImplementedException();
    }

    public static void writeFixed(this Stream stream, int f)
    {
        stream.writeULong(f);
    }

    public static void writeUShort(this Stream stream, int us)
    {
        stream.write((byte)((us >> 8) & 0xff));
        stream.write((byte)(us & 0xff));
    }

    public static void writeULong(this Stream stream, long ul)
    {
        stream.write((byte)((ul >> 24) & 0xff));
        stream.write((byte)((ul >> 16) & 0xff));
        stream.write((byte)((ul >> 8) & 0xff));
        stream.write((byte)(ul & 0xff));
    }

    public static void write(this Stream stream, int value)
    {
        stream.WriteByte((byte)value);
    }

    public static long position(this Stream stream)
    {
        return stream.Position;
    }

    public static void skip(this Stream stream, long offset)
    {
        stream.Seek(offset, SeekOrigin.Current);
    }

    public static int available(this Stream stream)
    {
        return checked((int)(stream.Length - stream.Position));
    }

    public static void writeTo(this Stream stream, Stream destStream)
    {
        stream.CopyTo(destStream);
    }
}


static class Arrays
{
    public static T[] copyOf<T>(T[] array, int length)
    {
        return array.AsSpan().Slice(0, length).ToArray();
    }

    public static T[] copyOfRange<T>(T[] array, int offset, int length)
    {
        throw new NotImplementedException();
    }

    public static void arraycopy<T>(T[] s, int sOffset, T[] d, int dOffset, int length)
    {
        Array.Copy(s, sOffset, d, dOffset, length);

    }

    public static IList<T> asList<T>(this IEnumerable<T> e)
    {
        throw new NotImplementedException();
    }
}

static class NumberHelper
{
    public static string toHexString(int value)
    {
        return Convert.ToString(value, 16);
    }

    public static string toHexString(long value)
    {
        return Convert.ToString(value, 16);
    }

    public static int numberOfLeadingZeros(int i)
    {        // HD, Count leading 0's
        if (i <= 0)
            return i == 0 ? 32 : 0;
        int n = 31;
        if (i >= 1 << 16) { n -= 16; i >>>= 16; }
        if (i >= 1 << 8) { n -= 8; i >>>= 8; }
        if (i >= 1 << 4) { n -= 4; i >>>= 4; }
        if (i >= 1 << 2) { n -= 2; i >>>= 2; }
        return n - (i >>> 1);
    }

    public static int highestOneBit(int x)
    {
        throw new NotImplementedException();
    }
}

sealed class FuncComparer<T> : Comparer<T>
{
    readonly Func<T, T, int> compareFunc;

    public FuncComparer(Func<T,T, int> compareFunc)
    {
        this.compareFunc = compareFunc;
    }

    public override int Compare(T x, T y) 
    {
        return compareFunc(x, y);
    }
}

static class Helper
{
    public static IEnumerable<T> BoxEnumerator<T>(this IEnumerator<T> enumerator)
    {
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }

    public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, bool condition, Func<T, bool> predicate)
    {
        if (condition) source = source.Where(predicate);

        return source;
    }
    
    public static T As<T>(this object obj) where T : class
    {
        return Unsafe.As<T>(obj);
    }
}

sealed class AtomicReference<T> where T : class
{
    T value;

    public T get()
    {
        return value;
    }

    public void compareAndSet(T comparand, T value)
    {
        Interlocked.CompareExchange(ref this.value, value, comparand);
    }
}

public static class EnumSet
{
    internal static EnumSet<TEnum> copyOf<TEnum>(EnumSet<TEnum> set)
    {
        throw new NotImplementedException();
    }

    internal static EnumSet<TEnum> noneOf<TEnum>()
    {
        return new EnumSet<TEnum>();

    }

    internal static EnumSet<TEnum> allOf<TEnum>()
    {
        if (typeof(TEnum).IsEnum)
        {
            return new EnumSet<TEnum>((TEnum[])Enum.GetValues(typeof(TEnum)));
        }

        return (EnumSet<TEnum>)typeof(EnumSet)
            .GetMethod(nameof(InternalAllOf), BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
            .MakeGenericMethod(typeof(TEnum))
            .Invoke(null, null);
    }

    internal static EnumSet<TClassEnum> InternalAllOf<TClassEnum>() where TClassEnum : ClassEnumBase<TClassEnum>
    {
        return new EnumSet<TClassEnum>(ClassEnumBase<TClassEnum>.values());
    }

    internal static EnumSet<TEnum> range<TEnum>(TEnum start, TEnum end)
    {
        var res = allOf<TEnum>();

        res.RemoveWhere(x => !IsRange(x));

        return res;

        bool IsRange(TEnum item)
        {
            var s = Comparer<TEnum>.Default.Compare(item, start);
            var e = Comparer<TEnum>.Default.Compare(item, end);

            return s >= 0 && e <= 0;
        }
    }
}

public sealed class EnumSet<TEnum> : HashSet<TEnum>
{
    public EnumSet()
    {

    }

    public EnumSet(IEnumerable<TEnum> collection):base(collection)
    {

    }

    public void RemoveAll(EnumSet<TEnum> enumSet)
    {
        throw new NotImplementedException();
    }


    public bool isEmpty()
    {
        return Count is 0;
    }
}

public class BitSet
{
    public int cardinality()
    {
        throw new NotImplementedException();
    }
    public void set(int index)
    {


        throw new NotImplementedException();
    }
    public void or(BitSet value)
    {
        throw new NotImplementedException();

    }
    public void andNot(BitSet value)
    {
        throw new NotImplementedException();

    }
    public void and(BitSet value)
    {

        throw new NotImplementedException();
    }
    public bool get(int index)
    {
        throw new NotImplementedException();
    }

    public virtual int size()
    {
        throw new NotImplementedException();
    }

    public int nextSetBit(int b)
    {
        throw new NotImplementedException();
    }

    public bool isEmpty()
    {
        throw new NotImplementedException();
    }

    public bool intersects(BitSet v)
    {
        throw new NotImplementedException();
    }
}