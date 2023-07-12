using Microsoft.EntityFrameworkCore;
using SimpleBankAPI.Models;

namespace SimpleBankAPI;

public class AccountContext : DbContext
{
    public AccountContext(DbContextOptions<AccountContext> options) : base(options)
    {
    }

    public DbSet<AccountModel> Accounts { get; set; }
}