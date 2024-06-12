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

    validInput = false;

    do
    {
      Console.WriteLine("Enter the File Path:");

      filePath = Console.ReadLine();

      if (filePath != null && File.Exists(filePath))
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

        ByteReader byteReader = new ByteReader(fs, 1_000_000_000);

        while (true)
        {
          int byteReadAsInt = byteReader.GetByte();

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
      encoder.GenerateCompressedFile(filePath, 1_000_000_000);
    }
    else
    {
      Decoder decoder = new Decoder();
      decoder.Decode(filePath ?? throw new Exception("Invalid File Path"));
    }
  }
}