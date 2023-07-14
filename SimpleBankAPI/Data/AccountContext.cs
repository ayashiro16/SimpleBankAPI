using Microsoft.EntityFrameworkCore;
using AccountModel = SimpleBankAPI.Models.AccountModel;
using ISavableCollection = SimpleBankAPI.Interfaces.ISavableCollection;

namespace SimpleBankAPI.Data;

public class AccountContext : DbContext, ISavableCollection
{
    public DbSet<AccountModel> Accounts { private get; init; }
    
    public AccountContext(DbContextOptions<AccountContext> options) : base(options) {}

    public void Add(AccountModel account) => Accounts.Add(account);

    public ValueTask<AccountModel?> FindAsync(Guid id) => Accounts.FindAsync(id);
}