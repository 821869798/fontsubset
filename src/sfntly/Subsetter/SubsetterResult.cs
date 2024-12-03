namespace sfntly.Subsetter;

public class SubsetterResult
{
    public Exception Exception { get; internal set; }

    public int OriginCharactersCount { get; internal set; }

    public int SubsetCharactersCount { get; internal set; }
}
