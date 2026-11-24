namespace SemBackDotNet10.SecurityDomain.Model.SemUserDetail;

public sealed class Token
{
    public Token(string tokenValue, long? duration, DateTime? expiryDate)
    {
        TokenValue = tokenValue;
        Duration = duration;
        ExpiryDate = expiryDate;
    }

    public string TokenValue { get; set; }
    public long? Duration { get; set; }
    public DateTime? ExpiryDate { get; set; }
}
