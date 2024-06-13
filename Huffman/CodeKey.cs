namespace Huffman;

public class CodeKey
{
  public int Code { get; }
  public int Length { get; }

  public CodeKey(int code, int length)
  {
    Code = code;
    Length = length;
  }
  public CodeKey((int, int) tuple)
  {
    Code = tuple.Item1;
    Length = tuple.Item2;
  }

  public override int GetHashCode()
  {
    return Code;
  }

  public override bool Equals(object? obj)
  {
    if (obj == null || obj is not CodeKey)
    {
      return false;
    }
    else
    {
      return ((CodeKey)obj).Code == Code && ((CodeKey)obj).Length == Length;
    }
  }
}
