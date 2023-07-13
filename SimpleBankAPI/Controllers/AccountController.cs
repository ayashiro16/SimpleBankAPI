using Microsoft.AspNetCore.Mvc;
using ISavableCollection = SimpleBankAPI.Interfaces.ISavableCollection;
using AccountModel = SimpleBankAPI.Models.AccountModel;

namespace SimpleBankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ISavableCollection _context;
        
        public AccountController(ISavableCollection context)
        {
            _context = context;
        }
        
        /// <summary>
        /// Retrieves account from database
        /// </summary>
        /// <param name="id">The Guid account ID</param>
        /// <returns>ActionResult with AccountModel</returns>
        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<AccountModel>> GetAccount(Guid id)
        {
            var account = await _context.FindAsync(id);
            return account is null ?  NotFound() : account;
        }
        
        /// <summary>
        /// Creates new account and adds it to the database
        /// </summary>
        /// <param name="request">Request form that contains string field "Name"</param>
        /// <returns>ActionResult with AccountModel</returns>
        [HttpPost]
        public async Task<ActionResult<AccountModel>> PostNewAccount([FromBody] CreateAccountRequest request)
        {
            var newAccount = new AccountModel()
            {
                Name = request.Name, 
                Balance = 0, 
                Id = Guid.NewGuid()
            };
            _context.Add(newAccount);
            await _context.SaveChangesAsync();
            return newAccount;
        }

        /// <summary>
        /// Creates deposit to add funds to an account
        /// </summary>
        /// <param name="id">The Guid account ID</param>
        /// <param name="request">Request form that contains decimal field "Amount"</param>
        /// <returns>ActionResult with AccountModel</returns>
        [HttpPost("{id:Guid}/deposits")]
        public async Task<ActionResult<AccountModel>> PostDepositFunds(Guid id, [FromBody] GetAmountRequest request)
        {
            var account = await _context.FindAsync(id);
            if (account is null || request.Amount < 0)
                return BadRequest();
            account.Balance += request.Amount;
            await _context.SaveChangesAsync();
            return account;
        }
        
        /// <summary>
        /// Creates withdrawal to take from an account
        /// </summary>
        /// <param name="id">The Guid account ID</param>
        /// <param name="request">Request form that contains the decimal field "Amount"</param>
        /// <returns>ActionResult with AccountModel</returns>
        [HttpPost("{id:Guid}/withdrawals")]
        public async Task<ActionResult<AccountModel>> PostWithdrawFunds(Guid id, [FromBody] GetAmountRequest request)
        {
            var account = await _context.FindAsync(id);
            if (account is null || account.Balance < request.Amount || request.Amount < 0)
                return BadRequest();
            account.Balance -= request.Amount;
            await _context.SaveChangesAsync();
            return account;
        }

        /// <summary>
        /// Creates a transfer that takes funds from sender account and deposits to receiver account
        /// </summary>
        /// <param name="request">Request form that contains two Guid account IDs, "SenderId" and "RecipientId", and a decimal field "Amount"</param>
        /// <returns>ActionResult with Sender and Recipient AccountModels</returns>
        [HttpPost("transfers")]
        public async Task<ActionResult<List<AccountModel>>> PostTransferFunds([FromBody] TransferFundsRequest request)
        {
            var sender = await _context.FindAsync(request.SenderId);
            var recipient = await _context.FindAsync(request.RecipientId);
            if (sender is null || recipient is null || sender.Balance < request.Amount || request.Amount < 0)
                return BadRequest();
            sender.Balance -= request.Amount;
            recipient.Balance += request.Amount;
            await _context.SaveChangesAsync();
            return new List<AccountModel>(){sender, recipient};
        }
    }
}
