using Microsoft.AspNetCore.Mvc;

namespace SimpleBankAPI;

[BindProperties]
public class CreateAccountRequest
{
    public string Name { get; set; }
}