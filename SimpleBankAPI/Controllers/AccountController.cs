using Microsoft.AspNetCore.Mvc;
using SimpleBankAPI.Models;
using SimpleBankAPI.Interfaces;

namespace SimpleBankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ISavableCollection _context;
        private readonly IAccountUtils _account;
        
        public AccountController(ISavableCollection context, IAccountUtils account)
        {
            _context = context;
            _account = account;
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
            if (account is null)
            {
                return NotFound();
            }
            else
            {
                return account;
            }
        }
        
        /// <summary>
        /// Creates new account and adds it to the database
        /// </summary>
        /// <param name="request">Request form that contains string field "Name"</param>
        /// <returns>ActionResult with AccountModel</returns>
        [HttpPost]
        public async Task<ActionResult<AccountModel>> PostNewAccount([FromBody] CreateAccountRequest request)
        {
            try
            {
                var account = await _account.CreateAccount(request.Name);
                return account;
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
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
            try
            {
                var account = await _account.DepositFunds(id, request.Amount);
                if (account is null)
                {
                    return NotFound();
                }
                
                return account;
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
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
            try
            {
                var account = await _account.WithdrawFunds(id, request.Amount);
                if (account is null)
                {
                    return NotFound();
                }

                return account;
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Creates a transfer that takes funds from sender account and deposits to receiver account
        /// </summary>
        /// <param name="request">Request form that contains two Guid account IDs, "SenderId" and "RecipientId", and a decimal field "Amount"</param>
        /// <returns>ActionResult with Sender and Recipient AccountModels</returns>
        [HttpPost("transfers")]
        public async Task<ActionResult<TransferResponseModel>> PostTransferFunds([FromBody] TransferFundsRequest request)
        {
            try
            {
                var accounts = await _account.TransferFunds(request.SenderId, request.RecipientId, request.Amount);
                if (accounts.Sender is null)
                {
                    return NotFound("Sender account could not be found");
                }

                if (accounts.Recipient is null)
                {
                    return NotFound("Recipient account could not be found");
                }

                return accounts;
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
