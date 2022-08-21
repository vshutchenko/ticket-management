using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.PurchaseApi.Models;
using TicketManagement.PurchaseApi.Services.Interfaces;
using TicketManagement.PurchaseApi.Services.Validation;

namespace TicketManagement.PurchaseApi.Controllers
{
    [ApiController]
    [Route("purchases")]
    [Authorize(Roles = "User")]
    [Produces("application/json")]
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;

        public PurchaseController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService ?? throw new ArgumentNullException(nameof(purchaseService));
        }

        /// <summary>
        /// Get purchases by user id.
        /// </summary>
        /// <param name="userId">If of the user.</param>
        /// <returns>List of purchases.</returns>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(List<PurchaseModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetByUserId(string userId)
        {
            try
            {
                var purchases = _purchaseService.GetByUserId(userId).ToList();

                return Ok(purchases);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Purchase seats for the event.
        /// </summary>
        /// <param name="purchaseModel">Purchase to complete.</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Purchase([FromBody] PurchaseModel purchaseModel)
        {
            try
            {
                await _purchaseService.PurchaseSeatAsync(purchaseModel);

                return Ok();
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}