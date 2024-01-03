

public class Token
{
  public string Value { set; get; }

  public TokenType Type { set; get; }

  public Token(TokenType type, string value)
  {
    Type = type;
    Value = value;
  }

  public Token(TokenType type)
  {
    Type = type;
    Value = "";
  }

  public bool IsType(TokenType type)
  {
    if (Type == TokenType.UNDEF)
      Type = type;

    if (Type == type)
      return true;

    return false;
  }
}
