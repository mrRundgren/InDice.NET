namespace InDice.NET;

public class DefaultExtractor : IExtractor
{
    public IEncoder Encoder { get; set; } = null!;
    public IEnumerable<string> ExcludedWords { get; set; } = null!;

    public DefaultExtractor()
    {
        Encoder = new DefaultEncoder();
        ExcludedWords = new List<string>();
    }

    public DefaultExtractor(string unsafeChars, IEnumerable<string>? excludedWords = null)
    {
        Encoder = new DefaultEncoder(unsafeChars);
        ExcludedWords = excludedWords?.Select(_ => _.ToUpper()) ?? Enumerable.Empty<string>();
    }

    public IEnumerable<string> ExtractExplicits(string source) =>
        source.QualifiedSplit(' ', '"').Where(_ => _.StartsWith('+')).Select(_ => Encoder.Encode(_[1..]));

    public IEnumerable<string> ExtractImplicits(string source) =>
        source.QualifiedSplit(' ', '"').Where(_ => !ExcludedWords.Contains(_.ToUpper()) && _.StartsWith('-') == false && _.StartsWith('+') == false).Select(_ => Encoder.Encode(_));

    public IEnumerable<string> ExtractExclusions(string source) =>
        source.QualifiedSplit(' ', '"').Where(_ => _.StartsWith('-')).Select(_ => Encoder.Encode(_[1..]));
}
