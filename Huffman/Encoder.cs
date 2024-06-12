namespace Huffman;

public class Encoder
{
  private List<Tuple<byte, string>> Codes { get; } = [];
  private List<Tuple<byte, string>> CanonicalCodes { get; } = [];
  private Dictionary<byte, Tuple<string, int>> CanonicalCodesDict { get; } = [];

  private int Compare(Tuple<byte, string> x, Tuple<byte, string> y)
  {
    int comparisonResult = x.Item2.Length.CompareTo(y.Item2.Length);

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
      if (CanonicalCodesDict.TryGetValue((byte)i, out Tuple<string, int>? tuple))
      {
        fileToWrite.WriteByte((byte)tuple.Item2);
      }
      else
      {
        fileToWrite.WriteByte(0);
      }
    }
  }
  public void Encode(Node? currentNode, string code)
  {
    if (currentNode == null)
    {
      return;
    }

    if (currentNode.IsLeafNode())
    {
      Codes.Add(new Tuple<byte, string>(currentNode.Value, code));
      return;
    }

    Encode(currentNode.LeftNode, code + "0");
    Encode(currentNode.RightNode, code + "1");
  }
  public void CanonicalEncode()
  {
    if (Codes.Count == 1)
    {
      CanonicalCodes.Add(new Tuple<byte, string>(Codes[0].Item1, "0"));
      CanonicalCodesDict.Add(Codes[0].Item1, new Tuple<string, int>("0", 1));
      return;
    }

    Codes.Sort(Compare);

    for (int i = 0; i < Codes.Count; i++)
    {
      if (i == 0)
      {
        Tuple<byte, string> tuple = new(Codes[i].Item1, new string('0', Codes[i].Item2.Length));
        CanonicalCodes.Add(tuple);
        CanonicalCodesDict.Add(tuple.Item1, new Tuple<string, int>(tuple.Item2, tuple.Item2.Length));
      }
      else
      {
        string newCode = "";

        string previouscode = CanonicalCodes[i - 1].Item2;
        string currentCode = Codes[i].Item2;

        int previousLength = CanonicalCodes[i - 1].Item2.Length;
        int currentLength = Codes[i].Item2.Length;

        if (currentLength == previousLength)
        {
          int currentCodeBase10 = Convert.ToInt32(previouscode, 2) + 1;
          newCode = Convert.ToString(currentCodeBase10, 2).PadLeft(currentLength, '0');
        }
        else
        {
          int currentCodeBase10 = Convert.ToInt32(previouscode, 2) + 1;
          newCode = Convert.ToString(currentCodeBase10, 2).PadLeft(previousLength, '0').PadRight(currentLength, '0');
        }

        Tuple<byte, string> tuple = new(Codes[i].Item1, newCode);
        CanonicalCodes.Add(tuple);
        CanonicalCodesDict.Add(tuple.Item1, new Tuple<string, int>(tuple.Item2, tuple.Item2.Length));
      }
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

      int byteReadAsInt = byteReader.GetByte();
      string bits = CanonicalCodesDict[(byte)byteReadAsInt].Item1;

      int byteBuffer = 0;
      int count = 7;
      while (true)
      {
        for (int i = 0; i < bits.Length; i++)
        {
          int bit = (int)char.GetNumericValue(bits[i]);
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

        byteReadAsInt = byteReader.GetByte();

        if (byteReadAsInt != -1)
        {
          bits = CanonicalCodesDict[(byte)byteReadAsInt].Item1;
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
