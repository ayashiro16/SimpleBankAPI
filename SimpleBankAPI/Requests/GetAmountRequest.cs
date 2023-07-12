using Microsoft.AspNetCore.Mvc;

namespace SimpleBankAPI;

[BindProperties]
public class GetAmountRequest
{
    public decimal Amount { get; set; }
}