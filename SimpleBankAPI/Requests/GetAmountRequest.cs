using Microsoft.AspNetCore.Mvc;

namespace SimpleBankAPI.Requests;

public class GetAmountRequest
{
    public decimal Amount { get; set; }
}