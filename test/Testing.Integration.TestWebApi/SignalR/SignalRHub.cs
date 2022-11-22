using Microsoft.AspNetCore.SignalR;

namespace Testing.Integration.TestWebApi.SignalR;

public class SignalRHub : Hub<ISignalRClient>
{
}