using SimpleBankAPI.Models;

namespace SimpleBankAPI.Interfaces;

public interface IAccountServices
{
    Task<AccountModel> CreateAccount(string name);
    ValueTask<AccountModel?> FindAccount(Guid id);
    Task<AccountModel?> DepositFunds(Guid id, decimal amount);
    Task<AccountModel?> WithdrawFunds(Guid id, decimal amount);
    Task<TransferResponseModel> TransferFunds(Guid senderId, Guid recipientId, decimal amount);
}