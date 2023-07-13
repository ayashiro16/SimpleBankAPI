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