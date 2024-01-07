
using SyntaxTree;

namespace plp;

public class Parser(List<Token> tokens)
{
  private readonly List<Token> tokenList = tokens;

  private readonly int lengthOfTokens = tokens.Count;

  private int position = 0;

  private string line = "1";

  private readonly Dictionary<string, VariableNode> vars = [];

  private void Next()
  {
    position++;
  }

  private void Next(int number)
  {
    position += number;
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
    RootNode root = new();

    while (position < lengthOfTokens)
      root.AddNode(ParseExpression());


    return root;
  }

  private RootNode ParseFunction()
  {
    RootNode root = new();

    while (!RequireThis(TokenType.CLOSE_FUNCTION))
      root.AddNode(ParseExpression());

    Next();

    return root;
  }

  private SyntaxNode ParseExpression()
  {
    SyntaxNode node = SyntaxNode.NullReferens;

    if (RequireThis(TokenType.KEY))
      if (RequireThis("puts"))
        node = new PutsNode(ParsFormula());
      else
        node = ParsAppointmentVariable(PeekList().Value);
    else if (RequireThis(TokenType.CALL))
      node = ParsCallFunction();
    else
      ParserError($"firs token in line require key", true);

    return node;
  }

  private VariableNode ParsCallFunction()
  {
    Next();
    if (!RequireThis(TokenType.KEY))
      ParserError($"in parsing require function name", true);

    string name = PeekList().Value;

    if (!vars.ContainsKey(name))
      ParserError($"function '{name}' is not defined", false);

    if (!Require(TokenType.END_OF_LINE))
      ParserError($"in parsing require END_OF_LINE (;)", true);

    Next();

    return vars[name];
  }

  private SyntaxNode ParsAppointmentVariable(string name)
  {
    SyntaxNode node = SyntaxNode.NullReferens;

    if (Require(TokenType.EQU))
      node = ParsFormula();
    else if (RequireThis(TokenType.OPEN_FUNCTION))
    {
      Next();
      node = ParseFunction();
    }
    else
      ParserError($"before variable '{name}' require EQU (':') operator", true);

    vars[name] = new(node);

    return SyntaxNode.NullReferens;
  }

  private SyntaxNode ParsFormula()
  {
    SyntaxNode node = MathPars(TokenType.END_OF_LINE);

    line = PeekList().Value;

    Next();
    return node;
  }

  private SyntaxNode ParsPriorityFromula() => MathPars(TokenType.CLOSE_EXSPRESSION);

  private SyntaxNode MathPars(TokenType endType)
  {
    Next();
    SyntaxNode node = ParsVariableOrNumber();

    while (!Require(endType)) // -todo pars priority formula
    {
      TokenType type = PeekList().Type;
      Next();

      node = BinaryNode.FactoryBinOperators(node, ParsVariableOrNumber(), type);
    }

    return node;
  }

  private SyntaxNode ParsVariableOrNumber()
  {
    bool isNumber = RequireThis(TokenType.NUMBER);
    bool isKey = RequireThis(TokenType.KEY);
    bool isPriority = RequireThis(TokenType.OPEN_EXPRESSION);

    if (!(isKey || isNumber || isPriority))
      ParserError($"in parsing require number or key or priority expression", true);

    if (isNumber) return new NumberNode(int.Parse(PeekList().Value));
    if (isPriority) return ParsPriorityFromula();

    return ParsVariable();
  }

  private VariableNode ParsVariable()
  {
    string name = PeekList().Value;

    if (!vars.ContainsKey(name))
      ParserError($"var '{name}' is not defined", false);

    return vars[name];
  }

  private SyntaxNode ParserError(string alert, bool canButGivenRferens)
  {
    string butGivenReferens = canButGivenRferens ? $", but given {PeekList().Type}" : "";

    PlpError.Alert(
      $"parsing error: \n\t {alert}{butGivenReferens}\n on line ~{line}"
    );

    return SyntaxNode.NullReferens;
  }
}
