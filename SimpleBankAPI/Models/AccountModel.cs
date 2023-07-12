namespace SimpleBankAPI.Models;

public class AccountModel
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public decimal Balance { get; set; }
}