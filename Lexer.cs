using System.Text.RegularExpressions;

namespace plp;

public class Lexer(string input)
{
  private readonly Regex
    NUMBER_LITERAL = new(@"[0-9]"),
    KEY_LITERAL = new(@"\w"),
    IGNORE_LITERAL = new(@"\s");

  private readonly List<Token> tokenList = [ new(TokenType.UNDEF) ];

  private bool canInComment;

  private uint line = 1;

  public string Input
  {
    set;
    private get;
  } = input;

  public List<Token> Analisis()
  {
    bool ok;

    foreach (char item in Input)
    {
      ok = AnalisisChar(item);
      if (!ok) PlpError.Alert($"lexer error: \n\t '{item}' is not valid character");
    }

    FilterTokenListOnUndefined();
    return tokenList;
  }

  private void FilterTokenListOnUndefined()
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
      if (tokenList[^1].Type != TokenType.UNDEF)
        tokenList.Add(new Token(TokenType.UNDEF));

      if (character == '\n') line++;

      return true;
    }

    return false;
  }

  private bool AnalisisOperators(char character)
  {
    if (character == '%')
      return canInComment = true;

    TokenType token = TokenTable.DefoltTokenTable.Select(character);

    if (token != TokenType.UNDEF)
    {
      TokenListAppend(token);
      return true;
    }

    return false;
  }

  private bool TokenListAppend(TokenType type)
  {
    if (tokenList[^1].Type == TokenType.UNDEF)
      tokenList[^1].Type = type;
    else
      if (type == TokenType.END_OF_LINE)
      tokenList.Add(new Token(type, $"{line}"));
    else tokenList.Add(new Token(type));

    return true;
  }

  private bool IsNumber(char character)
  {
    if (!NUMBER_LITERAL.IsMatch(character.ToString())) return false;

    if (tokenList[^1].Type == TokenType.KEY)
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
    if (tokenList[^1].IsType(type))
      UpdateToken(character);
    else
      tokenList.Add(new Token(type, character.ToString()));
  }

  private void UpdateToken(char character)
  {
    tokenList[^1].Value += character;
  }
}
