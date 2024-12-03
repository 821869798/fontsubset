using com.google.typography.font.sfntly;

namespace com.google.typography.font.tools.sfnttool;

public class SfntInfo
{
    public Font OriginFont { get; internal set; }

    /// <summary>
    ///  修改后的字体
    /// </summary>
    public Font ModifiedFont { get; internal set; }

}
