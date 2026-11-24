namespace Common.config.modificationRequest;

public sealed class ByteArrayServletInputStream(byte[] input) : MemoryStream(input, writable: false)
{
 public bool IsFinished => Position>=Length;
 public bool IsReady => CanRead;
}
