using Microsoft.AspNetCore.Mvc;
using TicketManagement.Core.Models;
using TicketManagement.PurchaseApi.Models;
using TicketManagement.PurchaseApi.Services.Interfaces;

namespace TicketManagement.PurchaseApi.Controllers
{
    [ApiController]
    [Route("purchases")]
    [AuthorizeRoles(Roles.User)]
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
            var purchases = _purchaseService.GetByUserId(userId).ToList();

            return Ok(purchases);
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
            await _purchaseService.PurchaseSeatAsync(purchaseModel);

            return Ok();
        }
    }
}