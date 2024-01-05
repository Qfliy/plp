using System.Text.RegularExpressions;

public class Lexer
{
  private readonly Regex
    NUMBER_LITERAL = new Regex(@"[0-9]"),
    KEY_LITERAL = new Regex(@"\w"),
    IGNORE_LITERAL = new Regex(@"\s");

  private List<Token> tokenList = new List<Token> { new Token(TokenType.UNDEF) };

  private bool canInComment;

  private uint line = 1;

  public string Input
  {
    set;
    private get;
  }

  public Lexer(string program)
  {
    Input = program;
  }

  public List<Token> Analisis()
  {
    bool ok;

    foreach (char item in Input)
    {
      ok = AnalisisChar(item);
      if (!ok) PlpError.Alert($"lexer error: \n\t '{item}' is not valid character");
    }

    filterTokenListOnUndefined();
    return tokenList;
  }

  public void filterTokenListOnUndefined()
  {
    if (tokenList.Last().Type == TokenType.UNDEF)
      tokenList.RemoveAt(tokenList.Count - 1);
  }

  private bool AnalisisChar(char character)
  {
    if (canInComment)
    {
      if (character == '\n')
        canInComment = false;

      return true;
    }

    return (
      IgnoreAnalisis(character) ||
      AnalisisOperators(character) ||
      IsNumber(character) || IsKey(character)
    );
  }

  private bool IgnoreAnalisis(char character)
  {
    if (IGNORE_LITERAL.IsMatch(character.ToString()))
    {
      if (tokenList[tokenList.Count - 1].Type != TokenType.UNDEF)
        tokenList.Add(new Token(TokenType.UNDEF));

      if (character == '\n') line ++;

      return true;
    }

    return false;
  }

  private bool AnalisisOperators(char character)
  {
    return character switch
    {
      '%' => canInComment = true,
      '+' => TokenListAppendOperator(TokenType.ADD),
      '-' => TokenListAppendOperator(TokenType.SUB),
      ':' => TokenListAppendOperator(TokenType.EQU),
      ';' => TokenListAppendOperator(TokenType.END_OF_LINE),
      '!' => TokenListAppendOperator(TokenType.CALL),
      '(' => TokenListAppendOperator(TokenType.OPEN_FUNCTION),
      ')' => TokenListAppendOperator(TokenType.CLOSE_FUNCTION),
      _ => false,
    };
  }

  private bool TokenListAppendOperator(TokenType type)
  {
    if (tokenList[tokenList.Count - 1].Type == TokenType.UNDEF)
      tokenList[tokenList.Count - 1].Type = type;
    else
      if (type == TokenType.END_OF_LINE)
        tokenList.Add(new Token(type, $"{line}"));
      else tokenList.Add(new Token(type));

    return true;
  }

  private bool IsNumber(char character)
  {
    if (!NUMBER_LITERAL.IsMatch(character.ToString())) return false;

    if (tokenList[tokenList.Count - 1].Type == TokenType.KEY)
      UpdateToken(character);
    else
      AddOrUpdateToken(character, TokenType.NUMBER);

    return true;
  }

  private bool IsKey(char character)
  {
    if (KEY_LITERAL.IsMatch(character.ToString()))
    {
      AddOrUpdateToken(character, TokenType.KEY);
      return true;
    }

    return false;
  }

  private void AddOrUpdateToken(char character, TokenType type)
  {
    if (tokenList[tokenList.Count - 1].IsType(type))
      UpdateToken(character);
    else
      tokenList.Add(new Token(type, character.ToString()));
  }

  private void UpdateToken(char character)
  {
    tokenList[tokenList.Count - 1].Value += character;
  }
}
