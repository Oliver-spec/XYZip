using Huffman;

public static class Program
{
  private static void Main()
  {
    bool validInput = false;
    string? compressOrDecompress;
    string? filePath;

    do
    {
      Console.WriteLine("To Compress a File, Enter 0");
      Console.WriteLine("To Decompress a File, Enter 1");

      compressOrDecompress = Console.ReadLine();

      if (compressOrDecompress == "0" || compressOrDecompress == "1")
      {
        validInput = true;
      }

    } while (validInput == false);

    do
    {
      Console.WriteLine("Enter the File Path:");

      filePath = Console.ReadLine();

      if (filePath != null || File.Exists(filePath))
      {
        validInput = true;
      }

    } while (validInput == false);

    if (compressOrDecompress == "0")
    {
      Queue queue = new Queue();

      using (FileStream fs = new FileStream(filePath ?? throw new Exception("Invalid File Path"), FileMode.Open, FileAccess.Read))
      {
        Console.WriteLine($"Uncompressed Size: {fs.Length} Byte(s)");

        while (true)
        {
          int byteReadAsInt = fs.ReadByte();

          if (byteReadAsInt == -1)
          {
            break;
          }

          byte byteRead = (byte)byteReadAsInt;
          queue.CountFrequency(byteRead);
        }
      }

      queue.FinaliseQueue();

      Tree tree = new Tree();
      tree.GrowTree(queue);

      Encoder encoder = new Encoder();
      encoder.Encode(tree.Root, "");
      encoder.CanonicalEncode();
      encoder.GenerateCompressedFile(filePath);
    }
    else
    {
      Decoder decoder = new Decoder();
      decoder.Decode(filePath ?? throw new Exception("Invalid File Path"));
    }

    // // debug
    // Console.WriteLine("\nCanonical Codes:");
    // foreach (Tuple<byte, string> tuple in encoder.CanonicalCodes)
    // {
    //   Console.WriteLine($"{tuple.Item1}\t{encoder.CanonicalCodesDict[tuple.Item1].Item2}\t{tuple.Item2}");
    // }

    // // debug
    // using (FileStream fs = new FileStream("compressed.xyz", FileMode.Open, FileAccess.Read))
    // {
    //   Console.WriteLine($"\nCompressed Size: {fs.Length} Byte(s)");

    //   for (long i = 0; i < fs.Length; i++)
    //   {
    //     int byteRead = fs.ReadByte();

    //     if (i > 255)
    //     {
    //       Console.WriteLine(Convert.ToString(byteRead, 2).PadLeft(8, '0'));
    //     }
    //   }
    // }

    // // debug
    // Console.WriteLine("\nDecoded Header: Value - Length");
    // foreach (Tuple<byte, int> tuple in decoder.CodesLengthList)
    // {
    //   Console.WriteLine($"Character Value: {tuple.Item1} Code Length: {tuple.Item2}");
    // }

    // Console.WriteLine("\nDecoded Header: Value - Code");
    // foreach (Tuple<byte, string> tuple in decoder.CodesList)
    // {
    //   Console.WriteLine($"Character Value: {tuple.Item1} Code: {tuple.Item2}");
    // }

    // Console.WriteLine("\nDecoded Dict: Code - Value");
    // foreach (KeyValuePair<string, byte> tuple in decoder.CodesDict)
    // {
    //   Console.WriteLine($"Code: {tuple.Key} Character Value: {tuple.Value}");
    // }
  }
}