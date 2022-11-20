namespace InDice.NET;
public class IndexEncoder : IEncoder
{
    public string UnsafeChars { get; set; } = "AOUÅEIYÄÖ";

    public IndexEncoder() { }

    public IndexEncoder(string unsafeChars)
    {
        UnsafeChars = unsafeChars;
    }

    public string Encode(string strToEncode) => 
        new string(strToEncode.Where(c => !UnsafeChars.Contains(c, StringComparison.InvariantCultureIgnoreCase) && Char.IsLetterOrDigit(c)).ToArray()).ToUpper().Normalize();
}