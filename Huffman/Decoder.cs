namespace Huffman;

public class Decoder
{
  private int HeaderLength { get; }
  private string Input { get; }
  public string Output { get; set; } = "";
  private List<Tuple<string, int>> CodeLength { get; } = [];
  private List<Tuple<string, string>> Codes { get; } = [];
  public Dictionary<string, string> CodesDict { get; } = [];

  public Decoder(string input)
  {
    Input = input;
    HeaderLength = int.Parse(Input[0].ToString());
    ProcessInput();
    Decode();
    GenerateOutput();
  }

  private void ProcessInput()
  {
    // int headerLength = 0;
    List<string> characterList = [];
    List<int> codeLengthList = [];

    for (int i = 0; i < Input.Length; i++)
    {
      if (i > 0 && i <= HeaderLength)
      {
        characterList.Add(Input[i].ToString());
      }
      else if (i > HeaderLength && i <= HeaderLength * 2)
      {
        codeLengthList.Add(int.Parse(Input[i].ToString()));
      }
    }

    for (int i = 0; i < characterList.Count; i++)
    {
      Tuple<string, int> tuple = new(characterList[i], codeLengthList[i]);
      CodeLength.Add(tuple);
    }
  }
  private void Decode()
  {
    for (int i = 0; i < CodeLength.Count; i++)
    {
      if (i == 0)
      {
        Tuple<string, string> tuple = new(new string('0', CodeLength[i].Item2), CodeLength[i].Item1);
        Codes.Add(tuple);
        // CanonicalCodesDict.Add(tuple.Item1, new Tuple<string, int>(tuple.Item2, tuple.Item2.Length));
      }
      else
      {
        string newCode = "";

        string previouscode = Codes[i - 1].Item1;
        // string currentCode = CodeLength[i].Item1;

        int previousLength = CodeLength[i - 1].Item2;
        int currentLength = CodeLength[i].Item2;

        if (currentLength == previousLength)
        {
          int currentCodeBase10 = Convert.ToInt32(previouscode, 2) + 1;
          newCode = Convert.ToString(currentCodeBase10, 2).PadLeft(currentLength, '0');
        }
        else
        {
          int currentCodeBase10 = Convert.ToInt32(previouscode, 2) + 1;
          newCode = Convert.ToString(currentCodeBase10, 2).PadLeft(previousLength, '0').PadRight(currentLength, '0');
        }

        Tuple<string, string> tuple = new(newCode, CodeLength[i].Item1);
        Codes.Add(tuple);
        // CanonicalCodesDict.Add(tuple.Item1, new Tuple<string, int>(tuple.Item2, tuple.Item2.Length));
      }
    }

    foreach (Tuple<string, string> tuple in Codes)
    {
      CodesDict.Add(tuple.Item1, tuple.Item2);
    }
  }
  private void GenerateOutput()
  {
    string output = "";
    string currentString = "";

    for (int i = HeaderLength * 2 + 1; i < Input.Length; i++)
    {
      currentString += Input[i].ToString();
      if (CodesDict.TryGetValue(currentString, out string? addToOutput))
      {
        output += addToOutput;
        currentString = "";
      }
    }

    Output = output;
  }
}
