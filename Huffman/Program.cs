using Huffman;

public static class Program
{
  private static void Main(string[] args)
  {
    if (args.Length == 0 || args.Length > 1)
    {
      Console.WriteLine("Invalid File Path");
      Environment.Exit(0);
    }

    string path = args[0];

    if (!File.Exists(path))
    {
      Console.WriteLine("Invalid File Path");
      Environment.Exit(0);
    }

    Queue queue = new Queue();

    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
    {
      Console.WriteLine($"\nUncompressed Size: {fs.Length} Byte(s)");

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
    encoder.GenerateCompressedFile(path);

    // debug
    Console.WriteLine("\nCanonical Codes:");
    foreach (Tuple<byte, string> tuple in encoder.CanonicalCodes)
    {
      Console.WriteLine($"{tuple.Item1}\t{encoder.CanonicalCodesDict[tuple.Item1].Item2}\t{tuple.Item2}");
    }

    // debug
    using (FileStream fs = new FileStream("compressed.xyz", FileMode.Open, FileAccess.Read))
    {
      Console.WriteLine($"\nCompressed Size: {fs.Length} Byte(s)");

      while (true)
      {
        int byteReadAsInt = fs.ReadByte();

        if (byteReadAsInt == -1)
        {
          break;
        }

        byte byteRead = (byte)byteReadAsInt;

        // debug
        Console.WriteLine(Convert.ToString(byteRead, 2).PadLeft(8, '0'));

        queue.CountFrequency(byteRead);
      }
    }

    // Console.WriteLine("\nString Output:");
    // Console.WriteLine(encoder.StringOutput);

    // Decoder decoder = new(encoder.StringOutput);

    // Console.WriteLine("\nDecoded Canonical Codes:");
    // foreach (KeyValuePair<string, string> entry in decoder.CodesDict)
    // {
    //   Console.WriteLine($"{entry.Key}\t{entry.Value}");
    // }

    // Console.WriteLine("\nDecoded String Output:");
    // Console.WriteLine(decoder.Output);
  }
}