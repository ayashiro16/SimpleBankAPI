using SimpleBankAPI.Models;
using IAccountUtils = SimpleBankAPI.Interfaces.IAccountUtils;
using ISavableCollection = SimpleBankAPI.Interfaces.ISavableCollection;

namespace SimpleBankAPI.Utils;

public class AccountUtils: IAccountUtils
{
    private readonly ISavableCollection _context;
    
    public AccountUtils(ISavableCollection context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Create and store an account with the provided name
    /// </summary>
    /// <param name="name">The account holder's name</param>
    /// <returns>The account details of our newly created account</returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<AccountModel> CreateAccount(string name)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name field cannot be empty or white space");
        }
        if (!name.All(x => char.IsWhiteSpace(x) || char.IsLetter(x)))
        {
            throw new ArgumentException("Name cannot contain special characters or numbers");
        }
        var account = new AccountModel()
        {
            Name = name, 
            Balance = 0, 
            Id = Guid.NewGuid()
        };
        _context.Add(account);
        await _context.SaveChangesAsync();

        return account;
    }
    
    /// <summary>
    /// Deposits funds to an account
    /// </summary>
    /// <param name="id">The account ID</param>
    /// <param name="amount">The amount to be deposited</param>
    /// <returns>The account details following the deposit</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public async Task<AccountModel?> DepositFunds(Guid id, decimal amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Cannot give a negative amount");
        }
        var account = await _context.FindAsync(id);
        if (account is null)
        {
            return account;
        }
        account.Balance += amount;
        await _context.SaveChangesAsync();
        
        return account;
    }

    /// <summary>
    /// Withdraws funds from an account
    /// </summary>
    /// <param name="id">The account ID</param>
    /// <param name="amount">The amount to be withdrawn</param>
    /// <returns>The account details following the withdraw</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<AccountModel?> WithdrawFunds(Guid id, decimal amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Cannot give a negative amount");
        }
        var account = await _context.FindAsync(id);
        if (account is null)
        {
            return account;
        }
        if (account.Balance < amount)
        {
            throw new InvalidOperationException("Insufficient funds");
        }
        account.Balance -= amount;
        await _context.SaveChangesAsync();
        
        return account;
    }

    /// <summary>
    /// Transfers funds from sender to recipient
    /// </summary>
    /// <param name="senderId">The account ID of the sender</param>
    /// <param name="recipientId">The account ID of the recipient</param>
    /// <param name="amount">The amount to be transferred</param>
    /// <returns>The account details of both the sender and the recipient following the transfer</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<TransferResponseModel> TransferFunds(Guid senderId, Guid recipientId, decimal amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Cannot give a negative amount");
        }
        var sender = await _context.FindAsync(senderId);
        var recipient = await _context.FindAsync(recipientId);
        if (sender is null || recipient is null)
        {
            return new TransferResponseModel(sender, recipient);
        }
        if (sender.Balance < amount)
        {
            throw new InvalidOperationException("Insufficient funds");
        }
        sender.Balance -= amount;
        recipient.Balance += amount;
        await _context.SaveChangesAsync();
        
        return new TransferResponseModel(sender, recipient);
    }
}