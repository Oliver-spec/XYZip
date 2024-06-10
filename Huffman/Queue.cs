namespace Huffman;

public class Queue : PriorityQueue<Node, int>
{
  private byte ByteToAdd { get; set; } = 0;
  private Dictionary<byte, int> FrequencyDict { get; } = [];

  private void PopulateFrequencyDict()
  {
    if (FrequencyDict.TryGetValue(ByteToAdd, out int value))
    {
      FrequencyDict[ByteToAdd] = ++value;
    }
    else
    {
      FrequencyDict[ByteToAdd] = 1;
    }
  }
  public void FinaliseQueue()
  {
    foreach (KeyValuePair<byte, int> entry in FrequencyDict)
    {
      Node node = new(null, null, entry.Key, entry.Value);
      Enqueue(node, entry.Value);
    }
  }
  public void CountFrequency(byte byteToAdd)
  {
    ByteToAdd = byteToAdd;
    PopulateFrequencyDict();
  }
}
