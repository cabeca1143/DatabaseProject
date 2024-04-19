using DatabaseProject;
using DatabaseProject.Objects;

internal static class Program
{
    private static Person? User;

    static Program()
    {
        User = null;
    }

    internal static void Main()
    {
        while (true)
        {
            ApplicationLoop();
        }
    }

    private static void ApplicationLoop()
    {
        Login();
        while (User is not null)
        {
            Console.Clear();
            User.DisplayMenu();
            if (!int.TryParse(Console.ReadLine(), out int option))
            {
                AuxFunctions.PrintLineAndPause("Invalid Option!");
            }
            User.OptionSelected(option);
        }
    }

    private static void Login()
    {
        while (User is null)
        {
            Console.Write("Login with your cpf: ");
            string? cpf = Console.ReadLine();
            User = DatabaseManager.Instance.GetPerson(cpf);
            Console.Clear();
            if (User is null)
            {
                Console.WriteLine("User not found! Try Again!");
            }
        }
    }

    public static void Logout()
    {
        User = null;
    }
}