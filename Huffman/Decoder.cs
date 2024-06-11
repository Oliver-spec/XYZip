namespace Huffman;

public class Decoder
{
  public List<Tuple<byte, int>> CodesLengthList { get; } = [];
  public List<Tuple<byte, string>> CodesList { get; } = [];
  public Dictionary<string, byte> CodesDict { get; } = [];

  private int Compare(Tuple<byte, int> x, Tuple<byte, int> y)
  {
    int comparisonResult = x.Item2.CompareTo(y.Item2);

    if (comparisonResult == 0)
    {
      return x.Item1.CompareTo(y.Item1);
    }
    else
    {
      return comparisonResult;
    }
  }
  private void GetCodesDict()
  {
    for (int i = 0; i < CodesLengthList.Count; i++)
    {
      if (i == 0)
      {
        Tuple<byte, string> tuple = new Tuple<byte, string>(CodesLengthList[i].Item1, new string('0', CodesLengthList[i].Item2));
        CodesList.Add(tuple);
      }
      else
      {
        string newCode = "";

        string previouscode = CodesList[i - 1].Item2;

        int previousLength = CodesLengthList[i - 1].Item2;
        int currentLength = CodesLengthList[i].Item2;

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

        Tuple<byte, string> tuple = new(CodesLengthList[i].Item1, newCode);
        CodesList.Add(tuple);
      }
    }

    foreach (Tuple<byte, string> tuple in CodesList)
    {
      CodesDict.Add(tuple.Item2, tuple.Item1);
    }
  }
  public void Decode(string path)
  {
    using (FileStream fileToRead = new FileStream(path, FileMode.Open, FileAccess.Read))
    using (FileStream fileToWrite = File.Create("decompressed.txt"))
    {
      for (int i = 0; i < 256; i++)
      {
        byte codeLength = (byte)fileToRead.ReadByte();

        if (codeLength == 0)
        {
          continue;
        }

        CodesLengthList.Add(new Tuple<byte, int>((byte)i, codeLength));
      }

      CodesLengthList.Sort(Compare);
      GetCodesDict();

      string possibleCode = "";

      for (long i = 256; i < fileToRead.Length - 2; i++)
      {
        string readingFrame = Convert.ToString(fileToRead.ReadByte(), 2).PadLeft(8, '0');

        foreach (char bit in readingFrame)
        {
          possibleCode += bit;

          if (CodesDict.TryGetValue(possibleCode, out byte actualValue))
          {
            fileToWrite.WriteByte(actualValue);
            possibleCode = "";
          }
        }
      }

      string secondLastByte = Convert.ToString(fileToRead.ReadByte(), 2).PadLeft(8, '0');
      int lastByte = fileToRead.ReadByte();
      string usefulBits = secondLastByte.Substring(0, lastByte);

      foreach (char bit in usefulBits)
      {
        possibleCode += bit;

        if (CodesDict.TryGetValue(possibleCode, out byte actualValue))
        {
          fileToWrite.WriteByte(actualValue);
          possibleCode = "";
        }
      }
    }
  }
}
