namespace SimpleBankAPI.Models;

public record TransferResponseModel(AccountModel? Sender, AccountModel? Recipient);