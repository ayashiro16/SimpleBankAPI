using BindProperties = Microsoft.AspNetCore.Mvc.BindPropertiesAttribute;

namespace SimpleBankAPI.Requests;

public class TransferFundsRequest
{
    public Guid SenderId { get; set; }
    public Guid RecipientId { get; set; }
    public decimal Amount { get; set; }
}