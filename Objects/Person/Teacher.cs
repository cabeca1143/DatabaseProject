using DatabaseProject.Enums;
using System.Text.Json;

namespace DatabaseProject.Objects;

internal class Teacher(string? cpf, string? name, DateTime birthDay) : Person(cpf, name, birthDay)
{
    public override UserType UserType => UserType.Teacher;

    public Teacher(PersonRecord record) : this(record.CPF, record.Name, record.BirthDay)
    {
        if (record.RawData is not null)
        {
            Data = JsonSerializer.Deserialize<List<string>>(record.RawData);
        }
    }

    public override void UpdateData(object? value, params string[] key)
    {
        if (value is not string str)
        {
            return;
        }

        List<string> data = Data as List<string> ?? [];
        if (!data.Contains(value))
        {
            data.Add(str);
        }

        Data = data;
        DatabaseManager.Instance.UpdateUserData(new(this));
    }

    public override void RemoveData(string? value)
    {
        if (value is null || Data is not List<string?> data)
        {
            return;
        }

        data.Remove(value);
        DatabaseManager.Instance.UpdateUserData(new(this));
    }

    public override void DisplayMenu()
    {
        Console.WriteLine(
                $"""
                 Welcome {Name}({Age})!
                 1 - Check Students
                 2 - Modify Student Grades
                 3 - Unassign Student Subject
                 4 - Logout
                 """);
    }

    public override void OptionSelected(int option)
    {
        switch (option)
        {
            case 1:
                DisplayStudents();
                break;
            case 2:
                ModifyStudentGrade();
                break;
            case 3:
                UnregisterStudentSubject();
                break;
            case 4:
                Program.Logout();
                break;
            default:
                AuxFunctions.PrintLineAndPause("Invalid Option!");
                break;
        }
    }

    private void DisplayStudents()
    {
        Console.Clear();
        List<string> studentCPFs = Data as List<string> ?? [];
        foreach (Person p in DatabaseManager.Instance.GetMultiplePeople(studentCPFs))
        {
            Console.WriteLine(p.Name);
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private void UnregisterStudentSubject()
    {
        Console.Clear();

        Person? student = GetStudent();
        if (student is null)
        {
            return;
        }
        
        string subj = AuxFunctions.GetConsoleStr("Subject: ");
        student.RemoveData(subj);
    }

    private void ModifyStudentGrade()
    {
        Console.Clear();

        Person? student = GetStudent();
        if (student is null)
        {
            return;
        }

        string subj = AuxFunctions.GetConsoleStr("Subject: ");
        int grade = AuxFunctions.GetConsoleInt("Grade: ");
        student.UpdateData(grade, subj);
    }

    private Person? GetStudent()
    {
        string cpf = AuxFunctions.GetConsoleStr("Student CPF: ");
        List<string> students = Data as List<string> ?? [];
        if (!students.Contains(cpf))
        {
            Console.WriteLine("This CPF doesn't belong to any of your students!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return null;
        }

        Person? student = DatabaseManager.Instance.GetPerson(cpf);
        if (student is null)
        {
            Console.WriteLine("This CPF is registered to you, but it's invalid!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return null;
        }

        return student;
    }
}