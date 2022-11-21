namespace InDice.NET;
public class DefaultEncoder : IEncoder
{
    public string UnsafeChars { get; set; } = "";

    public DefaultEncoder() { }

    public DefaultEncoder(string unsafeChars)
    {
        UnsafeChars = unsafeChars;
    }

    public string Encode(string strToEncode) => 
        new string(strToEncode.Where(c => !UnsafeChars.Contains(c, StringComparison.InvariantCultureIgnoreCase) && Char.IsLetterOrDigit(c)).ToArray()).ToUpper().Normalize();
}