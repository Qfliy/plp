using plp;

namespace SyntaxTree
{
  public abstract class SyntaxNode
  {
    public static readonly SyntaxNode NullReferens = new NumberNode(0);
    public virtual dynamic Evaluate() => 0;
  }

  public class RootNode : SyntaxNode
  {
    private List<SyntaxNode> childs = new List<SyntaxNode>();

    public void AddNode(SyntaxNode node)
    {
      childs.Add(node);
    }

    public override dynamic Evaluate()
    {
      childs.ForEach(el => { el.Evaluate(); });
      return 0;
    }
  }

  public class VariableNode : SyntaxNode
  {
    private SyntaxNode value;

    public VariableNode(SyntaxNode value)
    {
      this.value = value;
    }

    public override dynamic Evaluate()
    {
      return value.Evaluate();
    }
  }

  public class NumberNode : SyntaxNode
  {
    private double number;

    public NumberNode(double number)
    {
      this.number = number;
    }

    public override dynamic Evaluate() => number;
  }

  public class PutsNode : SyntaxNode
  {
    public SyntaxNode left;

    public PutsNode(SyntaxNode left)
    {
      this.left = left;
    }

    public override dynamic Evaluate()
    {
      Console.WriteLine(left.Evaluate());
      return 0;
    }
  }

  public abstract class BinaryNode : SyntaxNode
  {
    public SyntaxNode Left {get; private set;}

    public SyntaxNode Right {get; private set;}

    public BinaryNode(SyntaxNode left, SyntaxNode right)
    {
      Left = left;
      Right = right;
    }

    public override dynamic Evaluate() => 0;

    public static SyntaxNode FactoryBinOperators(SyntaxNode left, SyntaxNode right, TokenType type)
    {
      return type switch {
      TokenType.ADD => new AddNode(left, right),
      TokenType.SUB => new SubNode(left, right),
      _ => BinaryNode.ErrorNotRequireOperator(),
      };
    }

    private static SyntaxNode ErrorNotRequireOperator()
    {
      PlpError.Alert("parsing error \n\t require operator");
      return SyntaxNode.NullReferens;
    }
  }

  public class AddNode : BinaryNode
  {
    public AddNode(SyntaxNode left, SyntaxNode right): base(left, right) {}

    public override dynamic Evaluate() => Left.Evaluate() + Right.Evaluate();
  }

  public class SubNode : BinaryNode
  {
    public SubNode(SyntaxNode left, SyntaxNode right): base(left, right) {}

    public override dynamic Evaluate() => Left.Evaluate() - Right.Evaluate();
  }
}
