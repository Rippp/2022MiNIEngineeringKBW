using HanamikojiConsoleVersion;

namespace MyApp;

public class Program
{
    static void Main(string[] args)
    {
        var p1 = new Player("Krzysztof");
        var p2 = new Player("Adam");
        var referee = new Referee(p1,p2);
        
        referee.BeginRound();

        while(!referee.NextRound())
        {
        }
    }
}
