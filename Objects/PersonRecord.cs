using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using DatabaseProject.Enums;
using MySql.Data.MySqlClient;

namespace DatabaseProject.Objects;

public record PersonRecord
{
    public required string CPF;
    public required string Name;
    public required DateTime BirthDay;
    public required UserType UserType;
    public required string? RawData;

    public PersonRecord()
    {
    }
    
    [SetsRequiredMembers]
    public PersonRecord(Person p)
    {
        CPF = p.CPF;
        Name = p.Name;
        BirthDay = p.BirthDay;
        UserType = p.UserType;
        RawData = p.Data is null ? "NULL" : $"'{JsonSerializer.Serialize(p.Data).Replace("\"", "\\\"")}'";
    }
    
    [SetsRequiredMembers]
    public PersonRecord(MySqlDataReader reader)
    {
        CPF = reader.GetString("cpf");
        Name = reader.GetString("name");
        BirthDay = reader.GetDateTime("birthday");
        if (!reader.IsDBNull(4))
        {
            RawData = reader.GetString("data");
        }
    }
}