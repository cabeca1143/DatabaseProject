using DatabaseProject.Enums;
using DatabaseProject.Objects;
using MySql.Data.MySqlClient;
using System.Data;

namespace DatabaseProject;

//WARN: This is a simple project! There isn't much string treatment, leaving the database mostly vulnerable to sql injections
internal class DatabaseManager
{
    //Singleton
    public static DatabaseManager Instance { get; } = new("LOCALHOST", "mydb", "root", "root", "mydb", "users");

    private const short MAX_NAME_SIZE = 255;
    private readonly MySqlConnection Connection;
    private string Schema;
    private string Table;

    private DatabaseManager(string server, string db, string user, string password, string schema, string table)
    {
        Connection = new($"SERVER={server};DATABASE={db};UID={user};PASSWORD={password}");
        Schema = schema;
        Table = table;

        Connection.Open();
        if (Connection.State is not ConnectionState.Open)
        {
            Console.WriteLine("An issue has occurred while connecting to the database!");
        }
    }

    public void AddUser(PersonRecord data)
    {
        if (Connection.State is not ConnectionState.Open)
        {
            return;
        }

        string query = $"""
                        INSERT INTO `{Schema}`.`{Table}`
                        (
                            `cpf`,
                            `type`,
                            `name`,
                            `birthday`
                            `data`
                        )
                        VALUES
                        (
                            "{data.CPF}",
                            {(byte)data.UserType},
                            "{data.Name[..Math.Min(data.Name.Length, MAX_NAME_SIZE)]}",
                            "{data.BirthDay.Year}-{data.BirthDay.Month}-{data.BirthDay.Day}",
                            {data.RawData ?? "NULL"}
                        );
                        """;

        TryExecuteCommand(query);
    }

    //TODO: Make it async?
    public void UpdateUserData(PersonRecord data)
    {
        string query = $"""
                        UPDATE `{Schema}`.`{Table}` SET
                            `type` = {(int)data.UserType},
                            `name` = "{data.Name[..Math.Min(data.Name.Length, MAX_NAME_SIZE)]}",
                            `data` = {data.RawData ?? "NULL"}
                            WHERE (`cpf` = '{data.CPF}');
                        """;

        TryExecuteCommand(query);
    }

    public Person? GetPerson(string? cpf)
    {
        if (cpf is null or "")
        {
            return null;
        }
        string query = $"""SELECT * FROM {Schema}.{Table} WHERE cpf="{cpf}";""";

        using MySqlCommand command = new(query, Connection);
        using MySqlDataReader reader = command.ExecuteReader();
        return reader.Read() ? BuildPersonFromDatabase(reader) : null;
    }

    public IEnumerable<Person> GetMultiplePeople(IEnumerable<string> cpfs)
    {
        if (!cpfs.Any())
        {
            yield break;
        }

        string query = @$"SELECT * FROM {Schema}.{Table} WHERE cpf in (";
        foreach (string cpf in cpfs)
        {
            query += $"{cpf},";
        }
        query = query[..^1] + ')';

        using MySqlCommand command = new(query, Connection);
        using MySqlDataReader reader = command.ExecuteReader();
        do
        {
            if (reader.Read())
            {
                yield return BuildPersonFromDatabase(reader);
            }
        }
        while (reader.NextResult());
    }

    private void TryExecuteCommand(string query)
    {
        using MySqlCommand command = new(query, Connection);
        try
        {
            if (command.ExecuteNonQuery() is not 1)
            {
                //TODO: Log the error that occurred?
                Console.WriteLine($"There was an issue with the query!");
            }
        }
        catch (Exception ex)
        {
#if DEBUG
            AuxFunctions.PrintLineAndPause(ex.Message);
#endif
        }
    }

    private static Person BuildPersonFromDatabase(MySqlDataReader reader)
    {
        return (UserType)reader.GetByte("type") switch
        {
            UserType.Student => new Student(new(reader)),
            UserType.Teacher => new Teacher(new(reader)),
            UserType.Admin => new Admin(new(reader)),
            _ => null!
        };
    }

    #region Ununsed
    public void SwapTable(string schema, string table)
    {
        Schema = schema;
        Table = table;
    }
    #endregion
}