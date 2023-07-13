using AccountModel = SimpleBankAPI.Models.AccountModel;

namespace SimpleBankAPI.Interfaces;

public interface ISavableCollection
{
    void Add(AccountModel account);
    ValueTask<AccountModel?> FindAsync(Guid id);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}