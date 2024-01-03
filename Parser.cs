
using SyntaxTree;

public class Parser
{
  private List<Token> tokenList;

  private int lengthOfTokens;

  private int position;

  private Dictionary<string, SyntaxNode> vars = new Dictionary<string, SyntaxNode> { };

  public Parser(List<Token> tokens)
  {
    tokenList = tokens;
    position = 0;
    lengthOfTokens = tokens.Count;
  }

  private void Next()
  {
    position++;
  }

  private Token PeekList()
  {
    if (position < lengthOfTokens)
      return tokenList[position];

    throw new NullReferenceException("");
  }

  private bool RequireThis(TokenType type) => PeekList().Type == type;
  private bool RequireThis(string value) => PeekList().Value == value;

  private bool Require(TokenType type)
  {
    Next();
    return RequireThis(type);
  }

  public RootNode ParseCode()
  {
    RootNode root = new RootNode();

    while (position < lengthOfTokens)
      root.AddNode(ParseExpression());

    return root;
  }

  private SyntaxNode ParseExpression()
  {
    SyntaxNode node;

    if (!RequireThis(TokenType.KEY))
      PlpError.Alert("parsing error: \n\t firs token in string require key.");

    if (RequireThis("puts"))
      node = new PutsNode(ParsFormula());
    else
      node = ParsAppointmentVariable(PeekList().Value);

    return node;
  }

  private SyntaxNode ParsAppointmentVariable(string name)
  {
    if (!Require(TokenType.EQU))
      PlpError.Alert("parsing error: \n\t before variable require eq (':') operator.");

    SyntaxNode node = ParsFormula();

    vars[name] = node;

    return node;
  }

  private SyntaxNode ParsFormula()
  {
    Next();
    SyntaxNode node = ParsVariableOrNumber();

    while (!Require(TokenType.END_OF_LINE))
    {
      var type = PeekList().Type; Next();

      node = BinaryNode.FactoryBinOperators(node, ParsVariableOrNumber(), type);
    }

    Next();
    return node;
  }

  private SyntaxNode ParsVariableOrNumber()
  {
    bool isNumber = RequireThis(TokenType.NUMBER);
    bool isKey = RequireThis(TokenType.KEY);

    if (!(isKey || isNumber))
      PlpError.Alert(
        $"parsing error: \n\t in parsing require number or key, but given {PeekList().Type}"
      );

    if (isNumber) return new NumberNode(int.Parse(PeekList().Value));

    return ParsVariable();
  }

  private SyntaxNode ParsVariable()
  {
    string name = PeekList().Value;

    if (!vars.ContainsKey(name))
      PlpError.Alert($"parsing error: \n\t var '{name}' is not defined");

    return vars[name];
  }
};
