using Microsoft.AspNetCore.Mvc;
using Store_API.Services;

namespace Store_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Test(IMessageService messageService) : ControllerBase
    {
        [HttpPost]
        public IActionResult Notify()
        {
            messageService.SendMessage("This is a test message from the Test controller.");
            return Ok(new { message = "Notification sent successfully." });
        }
    }
}
