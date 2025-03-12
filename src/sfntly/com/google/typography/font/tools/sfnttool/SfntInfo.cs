using com.google.typography.font.sfntly;

namespace com.google.typography.font.tools.sfnttool;

public class SfntInfo
{
    /// <summary>
    /// 原始字体
    /// </summary>
    public Font OriginFont { get; internal set; }

    /// <summary>
    ///  修改后的字体
    /// </summary>
    public Font ModifiedFont { get; internal set; }

    /// <summary>
    /// 不支持的字体
    /// </summary>
    public bool UnSupportedFont { get; internal set; }

}
