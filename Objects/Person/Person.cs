using DatabaseProject.Enums;
using System.Text.Json;

namespace DatabaseProject.Objects;

//TODO: Add check to verify if CPF is composed of only numbers
public abstract class Person(string? cpf, string? name, DateTime birthDay)
{
    public string CPF { get; } = cpf;
    public string Name { get; set; } = name;
    public DateTime BirthDay = birthDay;
    public object? Data = null;
    public abstract UserType UserType { get; }
    protected Person(PersonRecord record) : this(record.CPF, record.Name, record.BirthDay) { }
    public int Age => (int.Parse($"{DateTime.Now:yyyyMMdd}") - int.Parse($"{BirthDay:yyyyMMdd}")) / 10000;
    public abstract void UpdateData(object? value, params string[] keys);
    public abstract void RemoveData(string? value);
    public abstract void DisplayMenu();
    public abstract void OptionSelected(int option);
}