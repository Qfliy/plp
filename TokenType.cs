
namespace plp;

public enum TokenType
{
  #region literals
    NUMBER, KEY,
  #endregion
  #region bin-operators
    ADD, SUB, EQU,
  #endregion
  #region operators
    END_OF_LINE, OPEN_FUNCTION, CLOSE_FUNCTION, CALL, OPEN_EXPRESSION, CLOSE_EXSPRESSION,
  #endregion
  UNDEF,
}
