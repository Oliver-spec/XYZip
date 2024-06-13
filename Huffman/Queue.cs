namespace Huffman;

public class Queue : PriorityQueue<Node, int>
{
  private Dictionary<int, int> FrequencyDict { get; } = [];

  public void PopulateFrequencyDict(int byteToAdd)
  {
    if (FrequencyDict.TryGetValue(byteToAdd, out int value))
    {
      FrequencyDict[byteToAdd] = ++value;
    }
    else
    {
      FrequencyDict[byteToAdd] = 1;
    }
  }
  public void FinaliseQueue()
  {
    foreach (KeyValuePair<int, int> entry in FrequencyDict)
    {
      Node node = new(null, null, entry.Key, entry.Value);
      Enqueue(node, entry.Value);
    }
  }
}
