namespace Huffman;

public static class BitGetter
{
  public static int GetBit(int bits, int bitNumber)
  {
    bool isSet = (bits & (1 << bitNumber - 1)) != 0;
    return isSet ? 1 : 0;
  }
}
