namespace Huffman;

public class Tree
{
  private Queue QueueToProcess { get; set; } = new Queue();
  public Node Root { get; private set; } = new Node(null, null, 0, 0);

  public void GrowTree(Queue queueToProcess)
  {
    QueueToProcess = queueToProcess;

    if (QueueToProcess.Count == 1)
    {
      Root = QueueToProcess.Dequeue();
      return;
    }

    while (QueueToProcess.Count > 1)
    {
      Node leftNode = QueueToProcess.Dequeue();
      Node rightNode = QueueToProcess.Dequeue();
      int totalFrequency = leftNode.Frequency + rightNode.Frequency;
      QueueToProcess.Enqueue(new(leftNode, rightNode, 0, totalFrequency), totalFrequency);
    }

    Root = QueueToProcess.Dequeue();
  }
}
