namespace Huffman;

public class Decoder
{
  private List<(int, int)> CodesLengthList { get; } = []; // (Value, Length)
  private List<(int, (int, int))> CodesList { get; } = []; // (Value, (Code, Length))
  private Dictionary<CodeKey, int> CodesDict { get; } = []; // ((Code, Length), Value)

  private int Compare((int, int) x, (int, int) y)
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
        (int, (int, int)) tuple = (CodesLengthList[i].Item1, (0, CodesLengthList[i].Item2));
        CodesList.Add(tuple);
      }
      else
      {
        int code;

        int previousCode = CodesList[i - 1].Item2.Item1;
        int previousLength = CodesLengthList[i - 1].Item2;
        int currentLength = CodesLengthList[i].Item2;

        if (currentLength == previousLength)
        {
          code = previousCode + 1;
        }
        else
        {
          code = (previousCode + 1) << (currentLength - previousLength);
        }

        (int, (int, int)) tuple = (CodesLengthList[i].Item1, (code, CodesLengthList[i].Item2));
        CodesList.Add(tuple);
      }
    }

    foreach ((int, (int, int)) tuple in CodesList)
    {
      CodesDict.Add(new CodeKey(tuple.Item2), tuple.Item1);
    }
  }
  public void Decode(string path, int bufferSize)
  {
    string fileName = path.Split('.')[0];
    string extension = path.Split('.')[1];

    using (FileStream fileToRead = new FileStream(path, FileMode.Open, FileAccess.Read))
    using (FileStream fileToWrite = File.Create($"{fileName}_decompressed.{extension}"))
    {
      ByteReader byteReader = new ByteReader(fileToRead, bufferSize);
      ByteWriter byteWriter = new ByteWriter(fileToWrite, bufferSize);

      Console.WriteLine($"\rCompressed Size: {fileToRead.Length} Byte(s)");

      // Read header
      for (int i = 0; i < 256; i++)
      {
        int length = byteReader.GetByte();

        if (length == 0)
        {
          continue;
        }

        CodesLengthList.Add((i, length));
      }

      CodesLengthList.Sort(Compare);
      GetCodesDict();

      long bytesDecompressed = 256;

      // Read body but stop before 2nd last byte
      int codeLength = 0;
      int codeBuffer = 0;
      int byteGet = 0;
      int minimumLength = CodesLengthList[0].Item2;

      for (long i = 256; i < fileToRead.Length - 2; i++)
      {
        byteGet = byteReader.GetByte();
        CodeKey key;
        for (int j = 8; j > 0; j--)
        {
          codeBuffer |= BitGetter.GetBit(byteGet, j);
          codeLength++;

          if (codeLength >= minimumLength)
          {
            key = new CodeKey(codeBuffer, codeLength);

            if (CodesDict.TryGetValue(key, out int value))
            {
              byteWriter.WriteByte(value);
              codeBuffer = 0;
              codeLength = 0;
              bytesDecompressed++;
            }
          }

          codeBuffer <<= 1;
        }

        if (bytesDecompressed % 1_000_000 == 0)
        {
          Console.Write($"\rDecompressing... {bytesDecompressed / 1_000_000} / {fileToRead.Length / 1_000_000} MB");
        }
      }

      // Read the last 2 bytes
      int secondLastByte = byteReader.GetByte();
      int lastByte = byteReader.GetByte();
      CodeKey lastTwoKeys;
      for (int i = 8; i > 8 - lastByte; i--)
      {
        codeBuffer |= BitGetter.GetBit(secondLastByte, i);
        codeLength++;

        if (codeLength >= minimumLength)
        {
          lastTwoKeys = new CodeKey(codeBuffer, codeLength);

          if (CodesDict.TryGetValue(lastTwoKeys, out int value))
          {
            if (i == 8 - lastByte + 1)
            {
              byteWriter.WriteByte(value, true);
            }
            else
            {
              byteWriter.WriteByte(value);
            }

            codeBuffer = 0;
            codeLength = 0;
            bytesDecompressed++;
          }
        }

        codeBuffer <<= 1;
      }

      Console.WriteLine($"\rDecompressed Size: {fileToWrite.Length} Byte(s)");
    }
  }
}
