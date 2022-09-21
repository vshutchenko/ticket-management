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
        /// <param name="id">If of the user.</param>
        /// <returns>List of purchases.</returns>
        [HttpGet("user/{id}")]
        [ProducesResponseType(typeof(List<PurchaseModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetByUserId(string id)
        {
            var purchases = _purchaseService.GetByUserId(id).ToList();

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