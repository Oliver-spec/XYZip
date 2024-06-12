namespace Huffman;

public class ByteReader
{
  private byte[] _byteArray;
  private FileStream Stream { get; }
  private int BytesToRead { get; set; }
  private int TotalBytesRead { get; set; } = 0;
  private int Index { get; set; } = 0;

  public ByteReader(FileStream fileStreamToRead, int bytesToReadAtOnce)
  {
    Stream = fileStreamToRead;
    BytesToRead = bytesToReadAtOnce;
    _byteArray = new byte[BytesToRead];
  }

  public int GetByte()
  {
    if (Index > _byteArray.Length - 1 || TotalBytesRead == 0)
    {
      int bytesReadThisIteration = Stream.Read(_byteArray, 0, BytesToRead);

      if (bytesReadThisIteration < BytesToRead)
      {
        BytesToRead = bytesReadThisIteration;
      }

      if (bytesReadThisIteration == 0)
      {
        return -1;
      }

      Array.Resize(ref _byteArray, bytesReadThisIteration);

      TotalBytesRead += bytesReadThisIteration;
      Stream.Seek(TotalBytesRead, SeekOrigin.Begin);

      Index = 0;
    }

    byte byteToReturn = _byteArray[Index];
    Index++;

    return byteToReturn;
  }
}
