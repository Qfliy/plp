namespace plp;

public class PlpError
{
  public static void Alert(string alert)
  {
    Console.WriteLine("Error \n\t {0}", alert);
    Environment.Exit(0);
  }
}
