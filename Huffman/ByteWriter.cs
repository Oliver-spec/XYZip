namespace Huffman;

public class ByteWriter
{
  private List<byte> _byteList;
  private int _bufferSize;
  private FileStream _stream;

  public ByteWriter(FileStream stream, int bufferSize)
  {
    _stream = stream;
    _bufferSize = bufferSize;
    _byteList = [];
  }

  public void WriteByte(int byteToWrite, bool isFinalByte = false)
  {
    _byteList.Add((byte)byteToWrite);

    if (_byteList.Count == _bufferSize)
    {
      _stream.Write(_byteList.ToArray(), 0, _bufferSize);
    }
    else if (isFinalByte)
    {
      _stream.Write(_byteList.ToArray(), 0, _byteList.Count);
    }
  }
}
