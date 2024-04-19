using DatabaseProject.Enums;

namespace DatabaseProject.Objects;

public class Admin(string? cpf, string? name, DateTime birthDay) : Person(cpf, name, birthDay)
{
    public Admin(PersonRecord record) : this(record.CPF, record.Name, record.BirthDay) { }

    public override UserType UserType => UserType.Admin;

    public override void DisplayMenu()
    {
        Console.WriteLine("""
                1 - Register Student
                2 - Register Teacher
                3 - Register Admin
                4 - Assign Student to Teacher
                5 - Unassign Student from Teacher
                6 - Log Out
                """);
    }

    public override void OptionSelected(int option)
    {
        switch (option)
        {
            case 1:
                RegisterStudent();
                break;
            case 2:
                RegisterTeacher();
                break;
            case 3:
                RegisterAdmin();
                break;
            case 4:
                AssignStudentToTeacher();
                break;
            case 5:
                UnassignStudentFromTeacher();
                break;
            case 6:
                Program.Logout();
                break;
            default:
                AuxFunctions.PrintLineAndPause("Invalid Option!");
                break;
        }
    }

    private static void AssignStudentToTeacher()
    {
        GetStudentCPFandTeacher(out Person teacher, out string? studentCPF);
        teacher.UpdateData(studentCPF);
    }

    private static void UnassignStudentFromTeacher()
    {
        GetStudentCPFandTeacher(out Person teacher, out string? studentCPF);
        teacher.RemoveData(studentCPF);
    }

    private static void RegisterStudent()
    {
        RegisterPerson(UserType.Student);
    }
    private static void RegisterTeacher()
    {
        RegisterPerson(UserType.Teacher);
    }
    private static void RegisterAdmin()
    {
        RegisterPerson(UserType.Admin);
    }

    private static void RegisterPerson(UserType type)
    {
        Console.Clear();

        string? pCpf = AuxFunctions.GetConsoleStr("CPF: ");
        string? pName = AuxFunctions.GetConsoleStr("Name: ");
        DateTime birthDay;
    getBirthday:
        Console.Write("Birthday(dd/mm/yyyy): ");
        try
        {
            birthDay = DateTime.Parse(Console.ReadLine() ?? "");
        }
        catch
        {
            goto getBirthday;
        }

        DatabaseManager.Instance.AddUser(new()
        {
            CPF = pCpf,
            Name = pName,
            UserType = type,
            BirthDay = birthDay,
            RawData = null
        });
    }

    private static void GetStudentCPFandTeacher(out Person teacher, out string? studentCPF)
    {
        do
        {
            teacher = DatabaseManager.Instance.GetPerson(AuxFunctions.GetConsoleStr("Teacher CPF: "))!;
        } while (teacher is null);

        do
        {
            studentCPF = AuxFunctions.GetConsoleStr("Student CPF: ");
        } while (DatabaseManager.Instance.GetPerson(studentCPF) is null);
    }

    #region Unused
    public override void UpdateData(object? value, params string[] keys)
    {
        return;
    }
    public override void RemoveData(string? value)
    {
        return;
    }
    #endregion
}