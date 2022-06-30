namespace HanamikojiConsoleVersion;

public static class ConsoleWrapper
{
    public static int ConsoleReadLineUntilProperIndexIsNotSelected(int maxValidIndex, Action errorAction)
        => ConsoleReadLineUnitProperCountOfIndiecesAreSelected(maxValidIndex, 1, errorAction).Single();

    public static List<int> ConsoleReadLineUnitProperCountOfIndiecesAreSelected(int maxValidIndex, int countOfIndices, Action errorAction)
    {
        var selectedIndices = new List<int>();

        while (selectedIndices.Count != countOfIndices)
        {
            var userInput = Console.ReadLine();
            var converted = int.TryParse(userInput, out var selectedIndex);
            if (!converted || selectedIndex < 0 || selectedIndex > maxValidIndex || selectedIndices.Contains(selectedIndex))
            {
                errorAction();
            }
            else
            {
                selectedIndices.Add(selectedIndex);
            }
        }

        return selectedIndices;
    }
}
