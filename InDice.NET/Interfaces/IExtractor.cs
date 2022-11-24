namespace InDice.NET;
public interface IExtractor
{
    IEnumerable<string> ExtractExplicits(string source);
    IEnumerable<string> ExtractImplicits(string source);
    IEnumerable<string> ExtractExclusions(string source);
}
