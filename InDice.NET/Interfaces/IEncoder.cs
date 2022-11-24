namespace InDice.NET;
public interface IEncoder
{
    string Encode(string value);
    string UnsafeChars { get; set; }
}
