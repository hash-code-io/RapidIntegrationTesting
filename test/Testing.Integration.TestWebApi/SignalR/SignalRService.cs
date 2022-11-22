using Microsoft.AspNetCore.SignalR;

namespace Testing.Integration.TestWebApi.SignalR;

public interface ISignalRService
{
    Task NotifySomethingHappened(SignalRModel model);
}

public class SignalRService : ISignalRService
{
    private readonly IHubContext<SignalRHub, ISignalRClient> _hubContext;

    public SignalRService(IHubContext<SignalRHub, ISignalRClient> hubContext) => _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));

    public Task NotifySomethingHappened(SignalRModel model) => _hubContext.Clients.All.SomethingHappened(model);
}