using DatabaseProject.Enums;
using System.Text.Json;

namespace DatabaseProject.Objects;

internal class Student(string? cpf, string? name, DateTime birthDay) : Person(cpf, name, birthDay)
{
    public override UserType UserType => UserType.Student;

    public Student(PersonRecord record) : this(record.CPF, record.Name, record.BirthDay)
    {
        if (record.RawData is not null)
        {
            Data = JsonSerializer.Deserialize<Dictionary<string, int>>(record.RawData);
        }
    }
    
    public override void UpdateData(object? value, params string[] key)
    {
        if (value is not int intValue)
        {
            return;
        }

        Dictionary<string, int> data = Data as Dictionary<string, int> ?? [];
        data[key.First()] = intValue;
        Data = data;
        DatabaseManager.Instance.UpdateUserData(new(this));
    }

    public override void RemoveData(string? value)
    {
        if (value is null || Data is not Dictionary<string, int> data)
        {
            return;
        }

        string key = data.Keys.ToList().Find( x => string.Equals(x, value, StringComparison.CurrentCultureIgnoreCase)) ?? value;
        data.Remove(key);
        DatabaseManager.Instance.UpdateUserData(new(this));
    }

    public override void DisplayMenu()
    {
        Console.WriteLine($"""
                Welcome {Name}({Age})!
                1 - Check Grades
                2 - Log Out
                """);
    }

    public override void OptionSelected(int option)
    {
        switch (option)
        {
            case 1:
                DisplayGrades();
                break;
            case 2:
                Program.Logout();
                break;
            default:
                AuxFunctions.PrintLineAndPause("Invalid Option!");
                break;
        }
    }

    private void DisplayGrades()
    {
        if (Data is Dictionary<string, int> data)
        {
            foreach (KeyValuePair<string, int> pair in data)
            {
                Console.WriteLine($"{pair.Key} = {pair.Value}");
            }
        }
        Console.ReadKey();
    }
}