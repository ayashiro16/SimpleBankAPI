using SimpleBankAPI.Models;

namespace SimpleBankAPI.Interfaces;

public interface IAccountUtils
{
    Task<AccountModel> CreateAccount(string name);
    Task<AccountModel?> DepositFunds(Guid id, decimal amount);
    Task<AccountModel?> WithdrawFunds(Guid id, decimal amount);
    Task<TransferResponseModel> TransferFunds(Guid senderId, Guid recipientId, decimal amount);
}