namespace TddSqlLite;

public class ConsoleWriteLineWrapper : IConsoleWriteLineWrapper
{
    public void Print(string message)
    {
        Console.WriteLine(message);
    }
}