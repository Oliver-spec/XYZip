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

      for (long i = 0; i < fs.Length; i++)
      {
        int byteRead = fs.ReadByte();

        if (i > 255)
        {
          Console.WriteLine(Convert.ToString(byteRead, 2).PadLeft(8, '0'));
        }
      }
    }

    Decoder decoder = new Decoder();
    decoder.Decode("compressed.xyz");

    // debug
    Console.WriteLine("\nDecoded Header: Value - Length");
    foreach (Tuple<byte, int> tuple in decoder.CodesLengthList)
    {
      Console.WriteLine($"Character Value: {tuple.Item1} Code Length: {tuple.Item2}");
    }

    Console.WriteLine("\nDecoded Header: Value - Code");
    foreach (Tuple<byte, string> tuple in decoder.CodesList)
    {
      Console.WriteLine($"Character Value: {tuple.Item1} Code: {tuple.Item2}");
    }

    Console.WriteLine("\nDecoded Dict: Code - Value");
    foreach (KeyValuePair<string, byte> tuple in decoder.CodesDict)
    {
      Console.WriteLine($"Code: {tuple.Key} Character Value: {tuple.Value}");
    }
  }
}