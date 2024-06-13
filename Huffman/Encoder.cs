namespace Huffman;

public class Encoder
{
  private List<(int, (int, int))> Codes { get; } = []; // (Value, (Code, Length))
  private List<(int, (int, int))> CanonicalCodes { get; } = []; // (Value, (Code, Length))
  private Dictionary<int, (int, int)> CanonicalCodesDict { get; } = []; // (Value, (Code, Length))

  private int Compare((int, (int, int)) x, (int, (int, int)) y)
  {
    int comparisonResult = x.Item2.Item2.CompareTo(y.Item2.Item2);

    if (comparisonResult == 0)
    {
      return x.Item1.CompareTo(y.Item1);
    }
    else
    {
      return comparisonResult;
    }
  }
  private void WriteHeader(FileStream fileToWrite)
  {
    for (int i = 0; i < 256; i++)
    {
      if (CanonicalCodesDict.TryGetValue(i, out (int, int) tuple))
      {
        fileToWrite.WriteByte((byte)tuple.Item2);
      }
      else
      {
        fileToWrite.WriteByte(0);
      }
    }
  }
  public void Encode(Node? currentNode, int code = 0, int length = 0)
  {
    if (currentNode == null)
    {
      return;
    }

    if (currentNode.IsLeafNode())
    {
      Codes.Add((currentNode.Value, (code, length)));
      return;
    }

    int leftCode = code << 1;
    int rightCode = (code << 1) | 1;
    int codeLength = length + 1;

    Encode(currentNode.LeftNode, leftCode, codeLength);
    Encode(currentNode.RightNode, rightCode, codeLength);
  }
  public void CanonicalEncode()
  {
    if (Codes.Count == 1)
    {
      CanonicalCodes.Add((Codes[0].Item1, (0, 1)));
      CanonicalCodesDict.Add(Codes[0].Item1, (0, 1));
      return;
    }

    Codes.Sort(Compare);

    for (int i = 0; i < Codes.Count; i++)
    {
      if (i == 0)
      {
        (int, (int, int)) tuple = (Codes[i].Item1, (0, Codes[i].Item2.Item2));
        CanonicalCodes.Add(tuple);
        CanonicalCodesDict.Add(tuple.Item1, (tuple.Item2.Item1, tuple.Item2.Item2));
      }
      else
      {
        int code;

        int previouscode = CanonicalCodes[i - 1].Item2.Item1;
        int currentCode = Codes[i].Item2.Item1;

        int previousLength = CanonicalCodes[i - 1].Item2.Item2;
        int currentLength = Codes[i].Item2.Item2;

        if (currentLength == previousLength)
        {
          // int currentCodeBase10 = Convert.ToInt32(previouscode, 2) + 1;
          // newCode = Convert.ToString(currentCodeBase10, 2).PadLeft(currentLength, '0');
          code = previouscode + 1;
        }
        else
        {
          // int currentCodeBase10 = Convert.ToInt32(previouscode, 2) + 1;
          // newCode = Convert.ToString(currentCodeBase10, 2).PadLeft(previousLength, '0').PadRight(currentLength, '0');
          code = (previouscode + 1) << (currentLength - previousLength);
        }

        (int, (int, int)) tuple = (Codes[i].Item1, (code, currentLength));
        CanonicalCodes.Add(tuple);
        CanonicalCodesDict.Add(tuple.Item1, (code, currentLength));
      }
    }

    // debug
    foreach (KeyValuePair<int, (int, int)> item in CanonicalCodesDict)
    {
      Console.WriteLine(item.Key + " " + item.Value.Item1 + " " + item.Value.Item2);
    }
  }
  public void GenerateCompressedFile(string path, int bufferSize)
  {
    string fileName = path.Split('.')[0];
    string extension = path.Split('.')[1];

    using (FileStream fileToRead = new FileStream(path, FileMode.Open, FileAccess.Read))
    using (FileStream fileToWrite = File.Create($"{fileName}.{extension}.xyz"))
    {
      ByteReader byteReader = new ByteReader(fileToRead, bufferSize);
      ByteWriter byteWriter = new ByteWriter(fileToWrite, bufferSize);

      WriteHeader(fileToWrite);

      int bytesCompressed = 256;

      // int byteReadAsInt = byteReader.GetByte();
      // string bits = CanonicalCodesDict[(byte)byteReadAsInt].Item1;


      int byteRead = byteReader.GetByte();

      (int, int) bitTuple = CanonicalCodesDict[byteRead]; // (Code, Length)
      int bits = bitTuple.Item1;
      int bitsLength = bitTuple.Item2;

      int bit = 0;
      int byteBuffer = 0;
      int count = 7;
      while (true)
      {
        for (int i = bitsLength; i > 0; i--)
        {
          bit = BitGetter.GetBit(bits, i);
          byteBuffer |= bit << count--;

          if (count < 0)
          {
            // Write byte here
            byteWriter.WriteByte((byte)byteBuffer);
            bytesCompressed++;

            byteBuffer = 0;
            count = 7;
          }
        }

        if (bytesCompressed % 1_000_000 == 0)
        {
          Console.Write($"\rCompressing... {bytesCompressed / 1_000_000} / {fileToRead.Length / 1_000_000} MB");
        }

        byteRead = byteReader.GetByte();

        if (byteRead != -1)
        {
          bitTuple = CanonicalCodesDict[byteRead];
          bits = bitTuple.Item1;
          bitsLength = bitTuple.Item2;
        }
        else
        {
          break;
        }
      }

      if (count < 7)
      {
        // Write 2nd last byte
        byteWriter.WriteByte((byte)byteBuffer);

        // Write final byte
        byteWriter.WriteByte((byte)(7 - count), true);
      }
      else
      {
        // Write final byte
        byteWriter.WriteByte(8, true);
      }

      Console.WriteLine($"\rCompressed Size: {fileToWrite.Length} Byte(s)");
      Console.WriteLine($"Compression Ratio: {(float)fileToWrite.Length / fileToRead.Length:P1}");
    }
  }
}
