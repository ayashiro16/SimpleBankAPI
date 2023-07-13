using Microsoft.AspNetCore.Mvc;

namespace SimpleBankAPI.Requests;

public class CreateAccountRequest
{
    public string Name { get; set; }
}