namespace InDice.NET;

public class DefaultExtractor : IExtractor
{
    public IEncoder Encoder { get; set; } = null!;

    public DefaultExtractor()
    {
        Encoder = new DefaultEncoder();
    }

    public DefaultExtractor(string unsafeChars)
    {
        Encoder = new DefaultEncoder(unsafeChars);
    }

    public IEnumerable<string> ExtractExplicits(string source) =>
        source.QualifiedSplit(' ', '"').Where(_ => _.StartsWith('+')).Select(_ => Encoder.Encode(_[1..]));

    public IEnumerable<string> ExtractImplicits(string source) =>
        source.QualifiedSplit(' ', '"').Where(_ => _.StartsWith('-') == false && _.StartsWith('+') == false).Select(_ => Encoder.Encode(_));

    public IEnumerable<string> ExtractExclusions(string source) =>
        source.QualifiedSplit(' ', '"').Where(_ => _.StartsWith('-')).Select(_ => Encoder.Encode(_[1..]));
}
