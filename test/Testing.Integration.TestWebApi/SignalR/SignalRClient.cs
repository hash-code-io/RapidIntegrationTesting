namespace Testing.Integration.TestWebApi.SignalR;

public interface ISignalRClient
{
    Task SomethingHappened(SignalRModel model);
}