namespace DatabaseProject;

public static class AuxFunctions
{
    public static void PrintLineAndPause(string message)
    {
        Console.WriteLine(message);
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
    
    public static string GetConsoleStr(string? text)
    {
        getStr:
        Console.Write(text);
        text = Console.ReadLine()!;
        if (text is null or "")
        {
            goto getStr;
        }

        return text;
    }
    
    public static int GetConsoleInt(string? text)
    {
        int toReturn;
        Console.Write(text);
        while (!int.TryParse(Console.ReadLine(), out toReturn))
        {
            Console.Write(text);
        }
        return toReturn;
    }
}