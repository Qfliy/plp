
using SyntaxTree;

namespace plp;

class Program
{
  static void Main(string[] args)
  {
    var source = ReadFileAndGetCode(args[0]);
    RunProgram(source);
  }

  static string ReadFileAndGetCode(string path)
  {
    if (!File.Exists(path))
    {
      PlpError.Alert($"File {path} is not Exist");
    }

    return File.ReadAllText(path);
  }

  static void RunProgram(string source)
  {
    Lexer lexer = new Lexer(source);
    List<Token> tokenList = lexer.Analisis();
    Parser parser = new Parser(tokenList);
    RootNode ast = parser.ParseCode();
    ast.Evaluate();
  }
}
