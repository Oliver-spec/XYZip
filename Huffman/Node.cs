namespace Huffman;

public class Node
{
  public int Frequency { get; }
  public int Value { get; }
  public Node? LeftNode { get; }
  public Node? RightNode { get; }

  public Node(Node? leftNode, Node? rightNode, int value, int frequency)
  {
    Value = value;
    Frequency = frequency;
    LeftNode = leftNode;
    RightNode = rightNode;
  }

  public bool IsLeafNode()
  {
    return LeftNode == null;
  }
}
