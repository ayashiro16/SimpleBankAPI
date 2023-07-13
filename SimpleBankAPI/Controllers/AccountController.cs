using Microsoft.AspNetCore.Mvc;
using SimpleBankAPI.Models;
using SimpleBankAPI.Interfaces;
using SimpleBankAPI.Requests;

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
        /// <param name="id">The account ID</param>
        /// <returns>The account associated with the provided ID</returns>
        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<AccountModel>> GetAccount(Guid id)
        {
            var account = await _context.FindAsync(id);
            if (account is null)
            {
                return NotFound();
            }
            
            return account;
        }
        
        /// <summary>
        /// Create and store a new account with the provided user's name
        /// </summary>
        /// <param name="request">The string "Name" of the account holder</param>
        /// <returns>The account details of the newly created account</returns>
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
        /// <param name="id">The account ID</param>
        /// <param name="request">The decimal "Amount" to be withdrawn</param>
        /// <returns>The account details of the account following the deposit</returns>
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
        /// <param name="id">The account ID</param>
        /// <param name="request">The decimal "Amount" to be withdrawn</param>
        /// <returns>The account details of the account following the withdrawal</returns>
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
        /// <param name="request">Sender's account ID: "SenderId", Recipient's account ID: "RecipientId", and decimal "Amount" to be transferred</param>
        /// <returns>The account details of both the sender and the recipient following the transfer</returns>
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
