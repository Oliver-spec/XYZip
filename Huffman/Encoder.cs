namespace Huffman;

public class Encoder
{
  private List<Tuple<byte, string>> Codes { get; } = [];
  public List<Tuple<byte, string>> CanonicalCodes { get; } = [];
  public Dictionary<byte, Tuple<string, int>> CanonicalCodesDict { get; } = [];

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
  public void GenerateCompressedFile(string path)
  {
    using (FileStream fileToRead = new FileStream(path, FileMode.Open, FileAccess.Read))
    using (FileStream fileToWrite = File.Create("compressed.xyz"))
    {
      WriteHeader(fileToWrite);

      long filesize = fileToRead.Length;
      string currentByteToWrite = "";
      string excessCode = "";

      while (filesize > 0)
      {
        byte byteRead = Convert.ToByte(fileToRead.ReadByte());
        filesize--;

        string code = excessCode + CanonicalCodesDict[byteRead].Item1;
        excessCode = "";

        if (currentByteToWrite.Length + code.Length <= 8)
        {
          currentByteToWrite += code;
        }
        else
        {
          int lengthToWrite = 8 - currentByteToWrite.Length;
          currentByteToWrite += code.Substring(0, lengthToWrite);
          excessCode = code.Substring(lengthToWrite);
        }

        if (currentByteToWrite.Length == 8)
        {
          fileToWrite.WriteByte(Convert.ToByte(currentByteToWrite, 2));

          if (filesize == 0)
          {
            fileToWrite.WriteByte(Convert.ToByte(currentByteToWrite.Length));
          }

          currentByteToWrite = "";
        }
        else if (filesize == 0)
        {
          fileToWrite.WriteByte(Convert.ToByte(currentByteToWrite.PadRight(8, '0'), 2));
          fileToWrite.WriteByte(Convert.ToByte(currentByteToWrite.Length));
          currentByteToWrite = "";
        }
      }

      if (excessCode.Length > 0)
      {
        fileToWrite.WriteByte(Convert.ToByte(excessCode.PadRight(8, '0'), 2));
        fileToWrite.WriteByte(Convert.ToByte(excessCode.Length));
      }
    }
  }
}
