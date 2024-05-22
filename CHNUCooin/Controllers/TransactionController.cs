using CHNUCooin.Dtos;
using CHNUCooin.Services;
using Microsoft.AspNetCore.Mvc;

namespace CHNUCooin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController(TransactionService transactionService) : ControllerBase
    {
        private readonly TransactionService _transactionService = transactionService;

        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionRequestDto request, CancellationToken cancellationToken)
        {
            await _transactionService.CreateTransaction(request, cancellationToken);
            return Ok();
        }
    }
}
