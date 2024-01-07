namespace plp;

public class TokenTable(List<TokenTable.ItemTokenTable> list)
{
  public static TokenTable DefoltTokenTable = new(new(){
    new(TokenType.ADD, '+'),
    new(TokenType.SUB, '-'),
    new(TokenType.EQU, ':'),
    new(TokenType.CALL, '!'),
    new(TokenType.END_OF_LINE, ';'),
    new(TokenType.OPEN_FUNCTION, '{'),
    new(TokenType.CLOSE_FUNCTION, '}'),
    new(TokenType.OPEN_EXPRESSION, '('),
    new(TokenType.CLOSE_EXSPRESSION, ')'),
  });

  private readonly List<ItemTokenTable> list = list;

  public TokenType Select(char value)
  {
    foreach (var item in list)
    {
      if (item.Value == value)
        return item.Type;
    }

    return TokenType.UNDEF;
  }

  public class ItemTokenTable(TokenType type, char value)
  {
    public TokenType Type{get; private set;} = type;

    public char Value{get; private set;} = value;
  }
}
