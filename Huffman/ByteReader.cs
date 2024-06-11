namespace Huffman;

public class ByteReader
{
  private FileStream FileStream { get; }
  private int BytesToRead { get; }
  private int BytesRead { get; set; }
  private int Index { get; set; }
  private byte[] ByteArray { get; set; }

  public ByteReader(FileStream fileStreamToRead, int bytesToReadAtOnce)
  {
    FileStream = fileStreamToRead;
    BytesToRead = bytesToReadAtOnce;
    ByteArray = new byte[BytesToRead];
    BytesRead = 0;
    Index = 0;
  }

  public int GetByte()
  {
    if (Index > ByteArray.Length - 1)
    {
      int bytesReadThisIteration = FileStream.Read(ByteArray, BytesRead, BytesToRead);

      if (bytesReadThisIteration == 0)
      {
        return -1;
      }

      BytesRead += bytesReadThisIteration;
    }

    byte byteToReturn = ByteArray[Index];
    Index++;

    return byteToReturn;
  }
}
