using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleBankAPI.Models;

namespace SimpleBankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AccountContext _context;
        
        public AccountController(AccountContext context)
        {
            _context = context;
        }
        
        // POST: api/Account
        [HttpPost]
        public async Task<ActionResult<AccountModel>> PostNewAccount([FromBody] CreateAccountRequest request)
        {
            var newAccount = new AccountModel()
            {
                Name = request.Name, 
                Balance = 0, 
                Id = Guid.NewGuid()
            };
            _context.Accounts.Add(newAccount);
            await _context.SaveChangesAsync();
            return newAccount;
        }

        // GET: api/Account/5
        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<AccountModel>> GetAccount(Guid id)
        {
            var account = await _context.Accounts.FindAsync(id);
            return account is null ?  NotFound() : account;
        }

        // POST: api/Account/5/deposits
        [HttpPost("{id:Guid}/deposits")]
        public async Task<ActionResult<AccountModel>> PostDepositFunds(Guid id, [FromBody] GetAmountRequest request)
        {
            var account = await _context.Accounts.SingleOrDefaultAsync(x => x.Id == id);
            if (account is null || request.Amount < 0)
                return BadRequest();
            account.Balance += request.Amount;
            await _context.SaveChangesAsync();
            return account;
        }
        
        // POST: api/Account/5/withdrawals
        [HttpPost("{id:Guid}/withdrawals")]
        public async Task<ActionResult<AccountModel>> PostWithdrawFunds(Guid id, [FromBody] GetAmountRequest request)
        {
            var account = await _context.Accounts.SingleOrDefaultAsync(x => x.Id == id);
            if (account is null || account.Balance < request.Amount || request.Amount < 0)
                return BadRequest();
            account.Balance -= request.Amount;
            await _context.SaveChangesAsync();
            return account;
        }

        // POST: api/Account/transfers
        [HttpPost("transfers")]
        public async Task<ActionResult<List<AccountModel>>> PostTransferFunds([FromBody] TransferFundsRequest request)
        {
            var sender = await _context.Accounts.SingleOrDefaultAsync(x => x.Id == request.SenderId);
            var recipient = await _context.Accounts.SingleOrDefaultAsync(x => x.Id == request.RecipientId);
            if (sender is null || recipient is null || sender.Balance < request.Amount || request.Amount < 0)
                return BadRequest();
            sender.Balance -= request.Amount;
            recipient.Balance += request.Amount;
            await _context.SaveChangesAsync();
            return new List<AccountModel>(){sender, recipient};
        }
    }
}
