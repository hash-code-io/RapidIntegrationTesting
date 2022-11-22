using Microsoft.AspNetCore.Mvc;
using Testing.Integration.TestWebApi.SignalR;

namespace Testing.Integration.TestWebApi.Controllers;

[ApiController]
[Route("api/v1.0/[controller]")]
public class SignalRController : ControllerBase
{
    public const string Message = "Hello there";
    private readonly ISignalRService _signalRService;

    public SignalRController(ISignalRService signalRService) => _signalRService = signalRService ?? throw new ArgumentNullException(nameof(signalRService));

    [HttpPost]
    public async Task<IActionResult> DoSomethingAndNotifySignalR()
    {
        await _signalRService.NotifySomethingHappened(new SignalRModel(Message));
        return Ok();
    }
}