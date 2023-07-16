using System.Diagnostics;
using System.Reflection;

namespace com.google.typography.font.sfntly.table.opentype.component;

public sealed class GsubLookupType : ClassEnumBase<GsubLookupType>, LookupType {
    public static readonly GsubLookupType
      GSUB_SINGLE = new(1),
      GSUB_MULTIPLE = new(2),
      GSUB_ALTERNATE = new(3),
      GSUB_LIGATURE = new(4),
      GSUB_CONTEXTUAL = new(5),
      GSUB_CHAINING_CONTEXTUAL = new(6),
      GSUB_EXTENSION = new(7),
      GSUB_REVERSE_CHAINING_CONTEXTUAL_SINGLE = new(8);

    readonly int _typeNum;

    private GsubLookupType(int typeNum)
    {
        _typeNum = typeNum;
    }
    public int typeNum()
    {
        return _typeNum;
    }
    public override String ToString()
    {
        return GetType().GetFields(BindingFlags.Public | BindingFlags.Static).FirstOrDefault(x => Equals(x.GetValue(null), this)).Name.ToLower();
    }

    public static GsubLookupType forTypeNum(int typeNum)
    {
        if (typeNum <= 0 || typeNum > _values.Length)
        {
            Debug.WriteLine("unknown gsub lookup typeNum: %d\n", typeNum);
            return null;
        }
        return _values[typeNum - 1];
    }

    private static readonly GsubLookupType[] _values = values();
    /*





  */
}